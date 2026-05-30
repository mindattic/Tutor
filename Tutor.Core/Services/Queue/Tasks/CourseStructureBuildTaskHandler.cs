using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Queue;

/// <summary>
/// Handler for course-structure build tasks. Turns a course's aggregated
/// ConceptMapCollection into a learning path (lessons → sections → baked content) and
/// pre-generates section quizzes, mirroring the CLI import pipeline's structure step so
/// courses built inside the app gain the same teachable structure.
///
/// The collection's per-resource ConceptMaps are merged into a single in-memory ConceptMap,
/// persisted under the collection id so the many existing consumers that resolve a course's
/// concepts via <see cref="ConceptMapStorageService.LoadAsync"/> (quiz generation, side nav)
/// finally find a populated map.
/// </summary>
public sealed class CourseStructureBuildTaskHandler : IBackgroundTaskHandler
{
    public BackgroundTaskType TaskType => BackgroundTaskType.CourseStructureBuild;

    public Task<BackgroundTaskResult> ResumeAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        // Structure generation isn't chunk-checkpointed; resuming just rebuilds from scratch.
        return ExecuteAsync(item, context, ct);
    }

    public async Task<BackgroundTaskResult> ExecuteAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<CourseStructureBuildPayload>(item.PayloadJson);
            if (payload == null || string.IsNullOrEmpty(payload.CourseId))
            {
                return BackgroundTaskResult.Failed("Invalid course-structure build payload", isTransient: false);
            }

            var courseService = context.GetService<CourseService>();
            var conceptMapStorage = context.GetService<ConceptMapStorageService>();
            var structureService = context.GetService<CourseStructureService>();
            var structureStorage = context.GetService<CourseStructureStorageService>();
            var quizGenerator = context.GetService<QuizGenerationService>();

            var course = await courseService.GetCourseAsync(payload.CourseId);
            if (course == null)
            {
                return BackgroundTaskResult.Failed("Course not found", isTransient: false);
            }

            context.ReportProgress(item, 5, "Loading concepts...");

            // Merge all of the course's resource ConceptMaps into one map. Persist it under the
            // collection id so concept-id lookups (quiz generation, etc.) resolve to a real map.
            var loaded = await courseService.GetCourseConceptMapCollectionAsync(payload.CourseId, ct);
            var allConcepts = loaded?.GetAllConcepts().ToList() ?? [];
            if (allConcepts.Count == 0)
            {
                return BackgroundTaskResult.Failed(
                    "Course has no ready concepts yet — build the concept maps first.", isTransient: false);
            }

            var collectionId = string.IsNullOrEmpty(payload.ConceptMapCollectionId)
                ? course.ConceptMapCollectionId ?? $"collection_{course.Id}"
                : payload.ConceptMapCollectionId;

            var mergedMap = new ConceptMap
            {
                Id = collectionId,
                Name = string.IsNullOrWhiteSpace(payload.Name) ? course.Name : payload.Name,
                Description = $"Merged concepts for course '{course.Name}'",
                Concepts = allConcepts,
                Relations = loaded!.GetAllRelations().ToList(),
                ComplexityOrder = loaded.GetAllComplexityOrder().ToList(),
                Status = ConceptMapStatus.Ready
            };
            await conceptMapStorage.SaveAsync(mergedMap, ct);

            // Forward the structure service's fine-grained progress (0-100) into the queue,
            // mapped into the 10-70 band we reserve for structure generation.
            void OnStructureProgress(CourseStructureProgress p) =>
                context.ReportProgress(item, 10 + p.Progress * 60 / 100, p.Message);

            structureService.OnProgressChanged += OnStructureProgress;
            CourseStructure structure;
            try
            {
                structure = await structureService.GenerateFromConceptMapAsync(course.Id, mergedMap, ct);
            }
            finally
            {
                structureService.OnProgressChanged -= OnStructureProgress;
            }

            course.CourseStructureId = structure.Id;
            course.UpdatedAt = DateTime.UtcNow;
            await courseService.SaveCourseAsync(course);

            // Pre-generate section quizzes so the lesson/quiz flow has baked questions and never
            // has to spend tokens live. Best-effort per section — a failure here must not fail the
            // whole build (the structure itself is already saved and usable).
            var quizSections = structure.Lessons
                .SelectMany(l => l.GetAllSectionsFlattened())
                .Where(s => s.HasQuiz && s.ConceptIds.Count > 0)
                .ToList();

            for (var i = 0; i < quizSections.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                var section = quizSections[i];
                context.ReportProgress(item, 70 + (i + 1) * 28 / Math.Max(1, quizSections.Count),
                    $"Generating quiz {i + 1}/{quizSections.Count}: {section.Title}");

                try
                {
                    section.PreGeneratedQuestions = await quizGenerator.GenerateAsync(
                        mergedMap.Id,
                        section.ConceptIds,
                        new List<string> { section.Id },
                        Math.Max(1, section.QuizQuestionCount),
                        ct);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Warn($"CourseStructureBuildTask: Quiz gen failed for section '{section.Title}' (non-critical) - {ex.Message}");
                }
            }

            await structureStorage.SaveAsync(structure, ct);

            context.ReportProgress(item, 100, "Course structure ready");
            Log.Info($"CourseStructureBuildTask: Built structure for '{course.Name}' " +
                $"({structure.TotalLessons} lessons, {structure.TotalTopics} topics, {quizSections.Count} quizzes)");

            return BackgroundTaskResult.Succeeded(JsonSerializer.Serialize(new
            {
                CourseId = course.Id,
                StructureId = structure.Id,
                Lessons = structure.TotalLessons,
                Topics = structure.TotalTopics,
                Quizzes = quizSections.Count
            }));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"CourseStructureBuildTask: Failed - {ex.Message}", ex);
            return BackgroundTaskResult.Failed(ex.Message);
        }
    }

    public BackgroundTaskValidation Validate(BackgroundQueueItem item)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<CourseStructureBuildPayload>(item.PayloadJson);
            if (payload == null)
                return BackgroundTaskValidation.Invalid("Payload is null");
            if (string.IsNullOrWhiteSpace(payload.CourseId))
                return BackgroundTaskValidation.Invalid("CourseId is required");
            return BackgroundTaskValidation.Valid();
        }
        catch (JsonException ex)
        {
            return BackgroundTaskValidation.Invalid($"Invalid payload JSON: {ex.Message}");
        }
    }
}
