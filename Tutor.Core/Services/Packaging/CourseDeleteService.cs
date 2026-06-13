using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// The hard-remove cascade: deletes a course and every derivative it produced —
/// resources, vector-store chunks, concept maps, course structure, and the
/// per-course ConceptMapCollection file. This is the explicit "Remove" behind
/// the soft load/unload lifecycle; unloading a course never calls this.
/// Shared verbatim by the CLI delete command and the Blazor course library.
/// </summary>
public sealed class CourseDeleteService
{
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly CourseStructureStorageService structureStorage;
    private readonly VectorStoreService vectorStore;

    public CourseDeleteService(
        CourseService courseService,
        ConceptMapStorageService conceptMapStorage,
        CourseStructureStorageService structureStorage,
        VectorStoreService vectorStore)
    {
        this.courseService = courseService;
        this.conceptMapStorage = conceptMapStorage;
        this.structureStorage = structureStorage;
        this.vectorStore = vectorStore;
    }

    /// <summary>
    /// Describes what a delete would remove without removing it (the CLI's --dry-run).
    /// Returns null when the course does not exist.
    /// </summary>
    public async Task<CourseDeletePlan?> PlanAsync(string courseId, CancellationToken ct = default)
    {
        var course = await courseService.GetCourseAsync(courseId);
        if (course == null) return null;

        var resources = await courseService.GetCourseResourcesAsync(course.Id);
        var conceptMapIds = resources
            .Where(r => !string.IsNullOrEmpty(r.ConceptMapId))
            .Select(r => r.ConceptMapId!)
            .Distinct()
            .ToList();

        var structure = await structureStorage.LoadByCourseIdAsync(course.Id, ct);

        return new CourseDeletePlan(
            CourseId: course.Id,
            CourseName: course.Name,
            ResourceCount: resources.Count,
            ConceptMapCount: conceptMapIds.Count,
            HasStructure: structure != null,
            HasConceptMapCollection: File.Exists(GetCollectionFilePath(course.Id)));
    }

    /// <summary>
    /// Cascades the delete. Returns the executed plan, or null when the course
    /// does not exist.
    /// </summary>
    public async Task<CourseDeletePlan?> DeleteAsync(string courseId, CancellationToken ct = default)
    {
        var plan = await PlanAsync(courseId, ct);
        if (plan == null) return null;

        var resources = await courseService.GetCourseResourcesAsync(plan.CourseId);
        var conceptMapIds = resources
            .Where(r => !string.IsNullOrEmpty(r.ConceptMapId))
            .Select(r => r.ConceptMapId!)
            .Distinct()
            .ToList();

        foreach (var resource in resources)
        {
            await vectorStore.RemoveChunksForResourceAsync(resource.Id);
            await courseService.DeleteResourceAsync(resource.Id);
        }

        // Belt-and-braces: also strip any chunks scoped to the courseId itself.
        await vectorStore.RemoveChunksForCurriculumAsync(plan.CourseId);

        foreach (var cmId in conceptMapIds)
        {
            await conceptMapStorage.DeleteAsync(cmId, ct);
        }

        if (plan.HasStructure)
        {
            await structureStorage.DeleteByCourseIdAsync(plan.CourseId, ct);
        }

        var collectionFile = GetCollectionFilePath(plan.CourseId);
        if (File.Exists(collectionFile))
        {
            try { File.Delete(collectionFile); }
            catch (Exception ex) { Log.Warn($"CourseDeleteService: could not delete {collectionFile} - {ex.Message}"); }
        }

        await courseService.DeleteCourseAsync(plan.CourseId);

        Log.Info($"CourseDeleteService: deleted course '{plan.CourseName}' ({plan.CourseId}) " +
                 $"with {plan.ResourceCount} resources and {plan.ConceptMapCount} concept maps");
        return plan;
    }

    private static string GetCollectionFilePath(string courseId) =>
        Path.Combine(DataStorageSettings.GetKnowledgeBasesDirectory(), "collections", $"collection_{courseId}.json");
}

/// <summary>What a course delete removes (or would remove, for a dry run).</summary>
public sealed record CourseDeletePlan(
    string CourseId,
    string CourseName,
    int ResourceCount,
    int ConceptMapCount,
    bool HasStructure,
    bool HasConceptMapCollection);
