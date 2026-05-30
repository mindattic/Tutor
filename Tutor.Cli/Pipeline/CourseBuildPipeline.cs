using Tutor.Core.Models;
using Tutor.Core.Services;

namespace Tutor.Cli.Pipeline;

/// <summary>
/// How section quizzes are handled when building a course.
/// </summary>
public enum QuizMode
{
    /// <summary>Bake pre-generated questions into every quiz section AND ship the
    /// concept maps so the runtime dynamic fallback stays available. The default.</summary>
    Both,
    /// <summary>Bake pre-generated questions only — equivalent to <see cref="Both"/>
    /// today, kept distinct so the intent ("ship offline-ready baked quizzes") is explicit.</summary>
    Baked,
    /// <summary>Skip pre-generation. Sections keep <c>HasQuiz</c> but no baked questions;
    /// the runtime generates them live from the bundled concept maps.</summary>
    Dynamic,
}

/// <summary>Parsing + behavior helpers for <see cref="QuizMode"/>.</summary>
public static class QuizModes
{
    public static QuizMode Parse(string? value) => (value ?? "both").Trim().ToLowerInvariant() switch
    {
        "" or "both" => QuizMode.Both,
        "baked" => QuizMode.Baked,
        "dynamic" => QuizMode.Dynamic,
        _ => throw new ArgumentException($"Invalid --quiz-mode '{value}'. Use baked, dynamic, or both."),
    };

    /// <summary>True when section quizzes should be pre-generated at build time.</summary>
    public static bool ShouldBake(this QuizMode mode) => mode != QuizMode.Dynamic;
}

/// <summary>
/// Builds a complete course from one or more resources, mirroring the Blazor
/// "Build Course" flow (<c>CourseStructureBuildTaskHandler</c>) rather than the
/// single-resource shortcut: every resource is formatted and turned into a
/// ConceptMap, the per-resource maps are merged into one collection, and a single
/// CourseStructure is generated from that merged map. Quizzes are baked unless
/// <see cref="QuizMode.Dynamic"/> is requested. Progress is printed to stdout.
/// <para/>
/// <see cref="BookImportPipeline"/> is the single-item convenience wrapper over this.
/// </summary>
public sealed class CourseBuildPipeline
{
    private readonly CourseService courseService;
    private readonly ContentFormatterService formatterService;
    private readonly ConceptMapService conceptMapService;
    private readonly ConceptMapStorageService conceptMapStorageService;
    private readonly CourseStructureService courseStructureService;
    private readonly CourseStructureStorageService courseStructureStorageService;
    private readonly QuizGenerationService quizGenerationService;
    private readonly SettingsService settingsService;

    public CourseBuildPipeline(
        CourseService courseService,
        ContentFormatterService formatterService,
        ConceptMapService conceptMapService,
        ConceptMapStorageService conceptMapStorageService,
        CourseStructureService courseStructureService,
        CourseStructureStorageService courseStructureStorageService,
        QuizGenerationService quizGenerationService,
        SettingsService settingsService)
    {
        this.courseService = courseService;
        this.formatterService = formatterService;
        this.conceptMapService = conceptMapService;
        this.conceptMapStorageService = conceptMapStorageService;
        this.courseStructureService = courseStructureService;
        this.courseStructureStorageService = courseStructureStorageService;
        this.quizGenerationService = quizGenerationService;
        this.settingsService = settingsService;
    }

    /// <summary>
    /// Builds the course end-to-end. Throws <see cref="InvalidOperationException"/>
    /// when a same-named course already exists and <see cref="CourseBuildRequest.AllowDuplicate"/>
    /// is false, or when no concepts could be extracted from any resource.
    /// </summary>
    public async Task<CourseBuildResult> BuildAsync(CourseBuildRequest req, CancellationToken ct = default)
    {
        if (req.Items.Count == 0)
            throw new InvalidOperationException("A course needs at least one item.");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Duplicate-name guard. Case-insensitive exact match on course name.
        if (!req.AllowDuplicate)
        {
            var existing = (await courseService.GetAllCoursesAsync())
                .FirstOrDefault(c => string.Equals(c.Name, req.CourseName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"A course named '{req.CourseName}' already exists (id: {existing.Id}). " +
                    "Pass --allow-duplicate to import anyway, or rename the new course.");
            }
        }

        var globalMaxIters = await settingsService.GetOrphanLinkingMaxIterationsAsync();
        var globalMinConf = await settingsService.GetOrphanLinkingMinConfidenceAsync();

        Console.WriteLine();
        Console.WriteLine($"Creating course '{req.CourseName}' with {req.Items.Count} item(s) [quiz-mode: {req.QuizMode.ToString().ToLowerInvariant()}]");
        var course = await courseService.CreateCourseAsync(req.CourseName, req.CourseDescription);

        var resources = new List<CourseResource>();
        for (var i = 0; i < req.Items.Count; i++)
        {
            var item = req.Items[i];
            var label = $"item {i + 1}/{req.Items.Count}";
            Console.WriteLine();
            Console.WriteLine($"[{label}] '{item.Title}' ({item.Content.Length:N0} chars)");

            var resource = new CourseResource
            {
                Title = item.Title,
                Author = item.Author,
                Description = item.Description,
                Content = item.Content,
                Type = ResourceType.Text,
                FileName = item.FileName,
            };
            var resourceId = await courseService.SaveResourceCoreAsync(resource);
            resource.Id = resourceId;

            Console.WriteLine($"[{label}] adding to course (chunk + embed for RAG)");
            try
            {
                await courseService.AddResourceToCourseAsync(course.Id, resourceId);
            }
            catch (Exception ex)
            {
                // The resource is already attached to the course before embedding runs,
                // so a RAG-embedding failure (e.g. a dead OpenAI key) must not abort the
                // build — structure and both quiz types come from the concept maps via
                // the chat LLM, not the vector store. The course just ships without RAG
                // vectors; reindex later once a working embedding key is available.
                Console.WriteLine($"        WARN: RAG embedding failed — {ex.Message}");
                Console.WriteLine("        (continuing; lessons, structure, and quizzes don't require embeddings)");
            }

            Console.WriteLine($"[{label}] formatting content with AI");
            try
            {
                var formatted = await formatterService.FormatContentAsync(
                    resource.Content,
                    (current, total) => Console.WriteLine($"        formatting section {current}/{total}"),
                    ct);
                resource.FormattedContent = formatted;
                await courseService.UpdateResourceContentAsync(resourceId, formatted, isFormatted: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"        WARN: formatting failed — {ex.Message} (continuing with raw content)");
            }

            Console.WriteLine($"[{label}] building ConceptMap");
            var maxIters = resource.GetEffectiveMaxIterations(globalMaxIters);
            var minConf = resource.GetEffectiveMinConfidence(globalMinConf);
            void OnConceptProgress(ConceptMapBuildProgress p) =>
                Console.WriteLine($"        [{p.Progress,3}%] {p.Message}");
            conceptMapService.OnProgressChanged += OnConceptProgress;
            ConceptMap conceptMap;
            try
            {
                conceptMap = await conceptMapService.BuildFromResourceAsync(resource, maxIters, minConf, ct);
            }
            finally
            {
                conceptMapService.OnProgressChanged -= OnConceptProgress;
            }

            resource.ConceptMapId = conceptMap.Id;
            resource.ConceptMapStatus = conceptMap.Status;
            resource.IsProcessed = true;
            await courseService.SaveResourceAsync(resource);
            resources.Add(resource);

            Console.WriteLine($"        ConceptMap: {conceptMap.Concepts.Count} concepts, {conceptMap.Relations.Count} relations");
        }

        // --- Finalize once across all resources ---
        Console.WriteLine();
        Console.WriteLine("Rebuilding course ConceptMap collection");
        await courseService.RebuildCourseConceptMapCollectionAsync(course, ct);

        // Merge the collection's per-resource maps into one ConceptMap persisted under
        // the collection id, exactly as CourseStructureBuildTaskHandler does, so quiz
        // generation and the side nav resolve a populated map.
        var loaded = await courseService.GetCourseConceptMapCollectionAsync(course.Id, ct);
        var allConcepts = loaded?.GetAllConcepts().ToList() ?? [];
        if (allConcepts.Count == 0)
            throw new InvalidOperationException("No concepts were extracted from any resource — cannot build a structure.");

        var collectionId = course.ConceptMapCollectionId ?? $"collection_{course.Id}";
        var mergedMap = new ConceptMap
        {
            Id = collectionId,
            Name = course.Name,
            Description = $"Merged concepts for course '{course.Name}'",
            Concepts = allConcepts,
            Relations = loaded!.GetAllRelations().ToList(),
            ComplexityOrder = loaded.GetAllComplexityOrder().ToList(),
            Status = ConceptMapStatus.Ready,
        };
        await conceptMapStorageService.SaveAsync(mergedMap, ct);

        Console.WriteLine($"Generating course structure from {allConcepts.Count} merged concepts (lessons → topics → sections → content)");
        void OnStructureProgress(CourseStructureProgress p) =>
            Console.WriteLine($"        [{p.Progress,3}%] {p.Message}");
        courseStructureService.OnProgressChanged += OnStructureProgress;
        CourseStructure structure;
        try
        {
            structure = await courseStructureService.GenerateFromConceptMapAsync(course.Id, mergedMap, ct);
        }
        finally
        {
            courseStructureService.OnProgressChanged -= OnStructureProgress;
        }

        course.CourseStructureId = structure.Id;
        course.UpdatedAt = DateTime.UtcNow;
        await courseService.SaveCourseAsync(course);

        var bakedQuizCount = 0;
        if (req.QuizMode.ShouldBake())
        {
            var quizSections = structure.Lessons
                .SelectMany(l => l.GetAllSectionsFlattened())
                .Where(s => s.HasQuiz && s.ConceptIds.Count > 0)
                .ToList();
            Console.WriteLine($"Pre-generating {quizSections.Count} section quizzes (baked into the bundle)");
            for (var i = 0; i < quizSections.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                var section = quizSections[i];
                var questions = await quizGenerationService.GenerateAsync(
                    mergedMap.Id,
                    section.ConceptIds,
                    new List<string> { section.Id },
                    Math.Max(1, section.QuizQuestionCount),
                    ct);
                section.PreGeneratedQuestions = questions;
                bakedQuizCount += questions.Count;
                Console.WriteLine($"        [{i + 1}/{quizSections.Count}] {section.Number} {section.Title} → {questions.Count} questions");
            }
        }
        else
        {
            Console.WriteLine("Skipping quiz pre-generation (--quiz-mode dynamic); quizzes generate at runtime from the bundled concept maps.");
        }
        await courseStructureStorageService.SaveAsync(structure, ct);

        stopwatch.Stop();
        Console.WriteLine();
        Console.WriteLine($"Done in {stopwatch.Elapsed:hh\\:mm\\:ss}.");
        Console.WriteLine($"  Course ID:     {course.Id}");
        Console.WriteLine($"  Resources:     {resources.Count}");
        Console.WriteLine($"  Structure ID:  {structure.Id}");
        Console.WriteLine($"  Lessons:       {structure.TotalLessons}");
        Console.WriteLine($"  Topics:        {structure.TotalTopics}");
        Console.WriteLine($"  Baked quizzes: {(req.QuizMode.ShouldBake() ? bakedQuizCount.ToString() : "0 (dynamic)")}");

        return new CourseBuildResult(course, resources, structure, bakedQuizCount, stopwatch.Elapsed);
    }
}

/// <summary>One resource to fold into a course.</summary>
public sealed record CourseItem(
    string Title,
    string Author,
    string Description,
    string FileName,
    string Content);

/// <summary>Inputs for <see cref="CourseBuildPipeline.BuildAsync"/>.</summary>
public sealed record CourseBuildRequest(
    string CourseName,
    string CourseDescription,
    IReadOnlyList<CourseItem> Items,
    QuizMode QuizMode = QuizMode.Both,
    bool AllowDuplicate = false);

/// <summary>Output of <see cref="CourseBuildPipeline.BuildAsync"/>.</summary>
public sealed record CourseBuildResult(
    Course Course,
    IReadOnlyList<CourseResource> Resources,
    CourseStructure Structure,
    int BakedQuizCount,
    TimeSpan Elapsed);
