using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// The hard-remove cascade: deletes a course and every derivative it produced —
/// resources, vector-store chunks, concept maps, course structure, the
/// per-course ConceptMapCollection file, and the merged concept map saved under
/// the collection id (each with its .bak, TUT-T63). This is the explicit "Remove" behind
/// the soft load/unload lifecycle; unloading a course never calls this.
/// Shared verbatim by the CLI delete command and the Blazor course library.
/// </summary>
public sealed class CourseDeleteService
{
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly CourseStructureStorageService structureStorage;
    private readonly VectorStoreService vectorStore;
    private readonly ConceptMapCollectionService collectionService;

    public CourseDeleteService(
        CourseService courseService,
        ConceptMapStorageService conceptMapStorage,
        CourseStructureStorageService structureStorage,
        VectorStoreService vectorStore,
        ConceptMapCollectionService collectionService)
    {
        this.courseService = courseService;
        this.conceptMapStorage = conceptMapStorage;
        this.structureStorage = structureStorage;
        this.vectorStore = vectorStore;
        this.collectionService = collectionService;
    }

    /// <summary>
    /// The collection ids a course can own: its recorded ConceptMapCollectionId and the
    /// <c>collection_&lt;courseId&gt;</c> name the builders and the bundle importer use.
    /// </summary>
    private static List<string> CollectionIdsFor(Tutor.Core.Models.Course course)
    {
        var ids = new List<string> { $"collection_{course.Id}" };
        if (!string.IsNullOrWhiteSpace(course.ConceptMapCollectionId) && !ids.Contains(course.ConceptMapCollectionId))
            ids.Add(course.ConceptMapCollectionId);
        return ids;
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
        var collectionIds = CollectionIdsFor(course);

        return new CourseDeletePlan(
            CourseId: course.Id,
            CourseName: course.Name,
            ResourceCount: resources.Count,
            ConceptMapCount: conceptMapIds.Count,
            HasStructure: structure != null,
            HasConceptMapCollection: collectionIds.Any(collectionService.HasFile),
            HasMergedConceptMap: collectionIds.Any(conceptMapStorage.HasFile));
    }

    /// <summary>
    /// Cascades the delete. Returns the executed plan, or null when the course
    /// does not exist.
    /// </summary>
    public async Task<CourseDeletePlan?> DeleteAsync(string courseId, CancellationToken ct = default)
    {
        var course = await courseService.GetCourseAsync(courseId);
        var plan = await PlanAsync(courseId, ct);
        if (plan == null || course == null) return null;

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

        // The collection file and the merged concept map saved under the collection id
        // (CourseBuildPipeline / CourseStructureBuildTaskHandler), each with its .bak, through
        // the services that own them (TUT-T63).
        foreach (var collectionId in CollectionIdsFor(course))
        {
            try { await collectionService.DeleteAsync(collectionId, ct); }
            catch (Exception ex) { Log.Warn($"CourseDeleteService: could not delete collection {collectionId} - {ex.Message}"); }
            try { await conceptMapStorage.DeleteAsync(collectionId, ct); }
            catch (Exception ex) { Log.Warn($"CourseDeleteService: could not delete merged concept map {collectionId} - {ex.Message}"); }
        }

        await courseService.DeleteCourseAsync(plan.CourseId);

        Log.Info($"CourseDeleteService: deleted course '{plan.CourseName}' ({plan.CourseId}) " +
                 $"with {plan.ResourceCount} resources and {plan.ConceptMapCount} concept maps");
        return plan;
    }
}

/// <summary>What a course delete removes (or would remove, for a dry run).</summary>
public sealed record CourseDeletePlan(
    string CourseId,
    string CourseName,
    int ResourceCount,
    int ConceptMapCount,
    bool HasStructure,
    bool HasConceptMapCollection,
    bool HasMergedConceptMap = false);
