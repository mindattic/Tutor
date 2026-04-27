using System.IO.Compression;
using System.Text.Json;
using Tutor.Core.Services;

namespace Tutor.Cli.Export;

public sealed class CourseExporter
{
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly CourseStructureStorageService structureStorage;
    private readonly VectorStoreService vectorStore;

    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    public CourseExporter(
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

    public async Task<BundleManifest> ExportAsync(string courseId, string outputPath, CancellationToken ct = default)
    {
        var course = await courseService.GetCourseAsync(courseId)
            ?? throw new InvalidOperationException($"Course '{courseId}' not found.");

        var resources = await courseService.GetCourseResourcesAsync(course.Id);

        var conceptMapIds = resources
            .Where(r => !string.IsNullOrEmpty(r.ConceptMapId))
            .Select(r => r.ConceptMapId!)
            .Distinct()
            .ToList();

        var manifest = new BundleManifest
        {
            CourseId = course.Id,
            CourseName = course.Name,
            ResourceCount = resources.Count,
            ConceptMapCount = conceptMapIds.Count,
        };

        // .tutorcourse is just a zip file with a recognisable extension; the
        // BundleManifest.FormatVersion is the source of truth for compatibility.
        var dir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        if (File.Exists(outputPath)) File.Delete(outputPath);

        using (var zipStream = File.Create(outputPath))
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            await WriteJsonEntryAsync(archive, "course.json", course, ct);

            var structure = await structureStorage.LoadByCourseIdAsync(course.Id, ct);
            if (structure != null)
                await WriteJsonEntryAsync(archive, "courseStructure.json", structure, ct);

            foreach (var resource in resources)
            {
                await WriteJsonEntryAsync(archive, $"resources/{resource.Id}.json", resource, ct);

                if (!string.IsNullOrEmpty(resource.Content))
                    await WriteTextEntryAsync(archive, $"resources/{resource.Id}.original.txt", resource.Content, ct);

                if (!string.IsNullOrEmpty(resource.FormattedContent))
                    await WriteTextEntryAsync(archive, $"resources/{resource.Id}.formatted.md", resource.FormattedContent!, ct);
            }

            foreach (var cmId in conceptMapIds)
            {
                var cm = await conceptMapStorage.LoadAsync(cmId, ct);
                if (cm != null)
                    await WriteJsonEntryAsync(archive, $"conceptMaps/{cm.Id}.json", cm, ct);
            }

            // Bundle the embeddings/chunks for this course. This is the expensive
            // bit to recompute, so re-imports become near-instant when included.
            var allChunks = new List<Tutor.Core.Models.ContentChunk>();
            foreach (var resource in resources)
            {
                var chunks = await vectorStore.GetChunksForResourceAsync(resource.Id);
                allChunks.AddRange(chunks);
            }
            manifest.ChunkCount = allChunks.Count;
            await WriteJsonEntryAsync(archive, "chunks.json", new BundleChunkSet { Chunks = allChunks }, ct);

            // Manifest written last so reading it doesn't require the rest to be present.
            await WriteJsonEntryAsync(archive, "manifest.json", manifest, ct);
        }

        return manifest;
    }

    private static async Task WriteJsonEntryAsync<T>(ZipArchive archive, string entryName, T value, CancellationToken ct)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        using var stream = entry.Open();
        await JsonSerializer.SerializeAsync(stream, value, JsonOpts, ct);
    }

    private static async Task WriteTextEntryAsync(ZipArchive archive, string entryName, string text, CancellationToken ct)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        using var stream = entry.Open();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(text.AsMemory(), ct);
    }
}
