using Tutor.Core.Models;
using Tutor.Core.Services;

namespace Tutor.Cli.Pipeline;

// Synchronous orchestration mirroring ResourceProcessingService.ProcessResourceAsync,
// but with stdout progress instead of UI events. Stages:
//   1. Create Course
//   2. Save raw resource (also persists original to disk for GitHub sync)
//   3. Add resource to course → embeds raw content into the vector store
//   4. Format content with AI → updates FormattedContent
//   5. Build per-resource ConceptMap → links to resource
//   6. Rebuild course ConceptMapCollection
//   7. Generate CourseStructure (lessons → topics → sections → content)
public sealed class BookImportPipeline
{
    private readonly CourseService courseService;
    private readonly ContentFormatterService formatterService;
    private readonly ConceptMapService conceptMapService;
    private readonly ConceptMapStorageService conceptMapStorageService;
    private readonly CourseStructureService courseStructureService;
    private readonly SettingsService settingsService;

    public BookImportPipeline(
        CourseService courseService,
        ContentFormatterService formatterService,
        ConceptMapService conceptMapService,
        ConceptMapStorageService conceptMapStorageService,
        CourseStructureService courseStructureService,
        SettingsService settingsService)
    {
        this.courseService = courseService;
        this.formatterService = formatterService;
        this.conceptMapService = conceptMapService;
        this.conceptMapStorageService = conceptMapStorageService;
        this.courseStructureService = courseStructureService;
        this.settingsService = settingsService;
    }

    public async Task<ImportResult> ImportAsync(BookImportRequest req, CancellationToken ct = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Duplicate-name guard. Phase 1: case-insensitive exact match on course
        // name. Phase 2 (TODO): also compare a SimHash of the first ~50KB of
        // content against existing resources to catch "same book, different
        // filename" while still allowing distinct printings (e.g. The Gunslinger
        // 1982 vs 2003) whose openings diverge.
        if (!req.AllowDuplicate)
        {
            var existing = (await courseService.GetAllCoursesAsync())
                .FirstOrDefault(c => string.Equals(c.Name, req.CourseName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"A course named '{req.CourseName}' already exists (id: {existing.Id}). " +
                    "Pass --allow-duplicate to import anyway, or rename the new course with --course \"...\".");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"[1/7] Creating course '{req.CourseName}'");
        var course = await courseService.CreateCourseAsync(req.CourseName, req.CourseDescription);

        Console.WriteLine($"[2/7] Saving resource '{req.ResourceTitle}' ({req.Content.Length:N0} chars)");
        var resource = new CourseResource
        {
            Title = req.ResourceTitle,
            Author = req.Author,
            Description = req.ResourceDescription,
            Content = req.Content,
            Type = ResourceType.Text,
            FileName = req.FileName,
        };
        var resourceId = await courseService.SaveResourceCoreAsync(resource);
        resource.Id = resourceId;

        Console.WriteLine("[3/7] Adding resource to course (chunk + embed for RAG)");
        await courseService.AddResourceToCourseAsync(course.Id, resourceId);

        Console.WriteLine("[4/7] Formatting content with AI (this is the slow stage)");
        try
        {
            var formatted = await formatterService.FormatContentAsync(
                resource.Content,
                (current, total) => Console.WriteLine($"      formatting section {current}/{total}"),
                ct);
            resource.FormattedContent = formatted;
            await courseService.UpdateResourceContentAsync(resourceId, formatted, isFormatted: true);
        }
        catch (Exception ex)
        {
            // Match ResourceProcessingService: formatting failures are non-fatal warnings.
            Console.WriteLine($"      WARN: formatting failed — {ex.Message} (continuing with raw content)");
        }

        Console.WriteLine("[5/7] Building ConceptMap (extract concepts + relationships)");
        var globalMaxIters = await settingsService.GetOrphanLinkingMaxIterationsAsync();
        var globalMinConf  = await settingsService.GetOrphanLinkingMinConfidenceAsync();
        var maxIters = resource.GetEffectiveMaxIterations(globalMaxIters);
        var minConf  = resource.GetEffectiveMinConfidence(globalMinConf);

        void OnConceptProgress(ConceptMapBuildProgress p) =>
            Console.WriteLine($"      [{p.Progress,3}%] {p.Message}");
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

        Console.WriteLine($"      ConceptMap: {conceptMap.Concepts.Count} concepts, {conceptMap.Relations.Count} relations");

        Console.WriteLine("[6/7] Rebuilding course ConceptMap collection");
        await courseService.RebuildCourseConceptMapCollectionAsync(course, ct);

        Console.WriteLine("[7/7] Generating course structure (lessons → topics → sections → content)");
        void OnStructureProgress(CourseStructureProgress p) =>
            Console.WriteLine($"      [{p.Progress,3}%] {p.Message}");
        courseStructureService.OnProgressChanged += OnStructureProgress;
        CourseStructure structure;
        try
        {
            structure = await courseStructureService.GenerateFromConceptMapAsync(course.Id, conceptMap.Id, ct);
        }
        finally
        {
            courseStructureService.OnProgressChanged -= OnStructureProgress;
        }

        course.CourseStructureId = structure.Id;
        course.UpdatedAt = DateTime.UtcNow;
        await courseService.SaveCourseAsync(course);

        stopwatch.Stop();
        Console.WriteLine();
        Console.WriteLine($"Done in {stopwatch.Elapsed:hh\\:mm\\:ss}.");
        Console.WriteLine($"  Course ID:      {course.Id}");
        Console.WriteLine($"  Resource ID:    {resourceId}");
        Console.WriteLine($"  ConceptMap ID:  {conceptMap.Id}");
        Console.WriteLine($"  Structure ID:   {structure.Id}");
        Console.WriteLine($"  Lessons:        {structure.TotalLessons}");
        Console.WriteLine($"  Topics:         {structure.TotalTopics}");

        return new ImportResult(course, resource, conceptMap, structure, stopwatch.Elapsed);
    }
}

public sealed record BookImportRequest(
    string CourseName,
    string CourseDescription,
    string ResourceTitle,
    string Author,
    string ResourceDescription,
    string FileName,
    string Content,
    bool AllowDuplicate = false);

public sealed record ImportResult(
    Course Course,
    CourseResource Resource,
    ConceptMap ConceptMap,
    CourseStructure Structure,
    TimeSpan Elapsed);
