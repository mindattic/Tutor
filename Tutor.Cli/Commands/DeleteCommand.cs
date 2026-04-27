using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

// CourseService.DeleteCourseAsync only removes the row from COURSES_DATA — it
// leaves resources, embeddings, concept maps, structures, and the per-course
// ConceptMapCollection file dangling. This command cascades the cleanup so a
// deleted course doesn't bloat the local data store.
public sealed class DeleteCommand
{
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly CourseStructureStorageService structureStorage;
    private readonly VectorStoreService vectorStore;

    public DeleteCommand(
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

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine("Usage: tutor delete <course-id> [--dry-run]");
            return 64;
        }

        var courseId = positionals[0];
        var dryRun = options.ContainsKey("dry-run");

        var course = await courseService.GetCourseAsync(courseId);
        if (course == null)
        {
            Console.Error.WriteLine($"Course '{courseId}' not found.");
            return 1;
        }

        Console.WriteLine($"{(dryRun ? "[dry-run] Would delete" : "Deleting")} course '{course.Name}' ({course.Id})");

        var resources = await courseService.GetCourseResourcesAsync(course.Id);
        var conceptMapIds = resources
            .Where(r => !string.IsNullOrEmpty(r.ConceptMapId))
            .Select(r => r.ConceptMapId!)
            .Distinct()
            .ToList();

        var structure = await structureStorage.LoadByCourseIdAsync(course.Id, ct);
        var collectionFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "ConceptMaps", "collections", $"collection_{course.Id}.json");

        Console.WriteLine($"  resources:                {resources.Count}");
        Console.WriteLine($"  concept maps:             {conceptMapIds.Count}");
        Console.WriteLine($"  course structure:         {(structure != null ? "yes" : "no")}");
        Console.WriteLine($"  concept map collection:   {(File.Exists(collectionFile) ? "yes" : "no")}");

        if (dryRun) return 0;

        // Resources: removes chunks (via DeleteResourceAsync's vector store cleanup
        // path is missing — handle chunks explicitly to be safe) and the resource row.
        foreach (var resource in resources)
        {
            await vectorStore.RemoveChunksForResourceAsync(resource.Id);
            await courseService.DeleteResourceAsync(resource.Id);
        }

        // Belt-and-braces: also strip any chunks scoped to the courseId itself.
        await vectorStore.RemoveChunksForCurriculumAsync(course.Id);

        foreach (var cmId in conceptMapIds)
        {
            await conceptMapStorage.DeleteAsync(cmId, ct);
        }

        if (structure != null)
        {
            await structureStorage.DeleteByCourseIdAsync(course.Id, ct);
        }

        if (File.Exists(collectionFile))
        {
            try { File.Delete(collectionFile); } catch { /* best effort */ }
        }

        await courseService.DeleteCourseAsync(course.Id);

        Console.WriteLine("Done.");
        return 0;
    }
}
