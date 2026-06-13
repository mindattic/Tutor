using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Tutor.Core.Models;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// Packs a course (resources, concept maps, structure, and vector-store chunks)
/// into a single shareable .tutor zip bundle. Embeddings ride along so matching
/// imports skip the LLM pipeline entirely. The manifest carries a stable
/// CourseKey + whole-number CourseVersion (identity), a payload SHA-256
/// (integrity), and provenance fields for the person receiving a shared course.
/// </summary>
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

    /// <summary>
    /// Writes a .tutor bundle for <paramref name="courseId"/> to
    /// <paramref name="outputPath"/>, overwriting any existing file. Returns the
    /// manifest describing what was written. Throws if the course is not found.
    /// </summary>
    public async Task<BundleManifest> ExportAsync(
        string courseId,
        string outputPath,
        BundleExportOptions? options = null,
        CancellationToken ct = default)
    {
        var course = await courseService.GetCourseAsync(courseId)
            ?? throw new InvalidOperationException($"Course '{courseId}' not found.");

        var resources = await courseService.GetCourseResourcesAsync(course.Id);

        var conceptMapIds = resources
            .Where(r => !string.IsNullOrEmpty(r.ConceptMapId))
            .Select(r => r.ConceptMapId!)
            .Distinct()
            .ToList();

        options ??= new BundleExportOptions();

        var manifest = new BundleManifest
        {
            CourseId = course.Id,
            CourseName = course.Name,
            CourseKey = string.IsNullOrEmpty(options.CourseKey) ? DeriveCourseKey(course.Name) : options.CourseKey,
            CourseVersion = options.CourseVersion,
            ResourceCount = resources.Count,
            ConceptMapCount = conceptMapIds.Count,
            Author = options.Author,
            License = options.License,
            Description = options.Description,
            SourceAttribution = options.SourceAttribution,
        };

        if (!CourseManifestValidator.CourseKeyPattern.IsMatch(manifest.CourseKey))
            throw new InvalidOperationException(
                $"Course key '{manifest.CourseKey}' is not a valid slug ({CourseManifestValidator.CourseKeyPattern}).");
        if (manifest.CourseVersion < 1)
            throw new InvalidOperationException("Course versions are whole numbers starting at 1.");

        // Payload entries are buffered as bytes so the canonical SHA-256 can be
        // computed over exactly what lands in the zip (everything but the manifest).
        var payload = new List<(string Name, byte[] Content)>
        {
            ("course.json", ToJsonBytes(course)),
        };

        var structure = await structureStorage.LoadByCourseIdAsync(course.Id, ct);
        if (structure != null)
            payload.Add(("courseStructure.json", ToJsonBytes(structure)));

        foreach (var resource in resources)
        {
            payload.Add(($"resources/{resource.Id}.json", ToJsonBytes(resource)));

            if (!string.IsNullOrEmpty(resource.Content))
                payload.Add(($"resources/{resource.Id}.original.txt", Encoding.UTF8.GetBytes(resource.Content)));

            if (!string.IsNullOrEmpty(resource.FormattedContent))
                payload.Add(($"resources/{resource.Id}.formatted.md", Encoding.UTF8.GetBytes(resource.FormattedContent!)));
        }

        foreach (var cmId in conceptMapIds)
        {
            var cm = await conceptMapStorage.LoadAsync(cmId, ct);
            if (cm != null)
                payload.Add(($"conceptMaps/{cm.Id}.json", ToJsonBytes(cm)));
        }

        // Bundle the embeddings/chunks for this course. This is the expensive
        // bit to recompute, so re-imports become near-instant when included.
        var allChunks = new List<ContentChunk>();
        foreach (var resource in resources)
        {
            var chunks = await vectorStore.GetChunksForResourceAsync(resource.Id);
            allChunks.AddRange(chunks);
        }
        manifest.ChunkCount = allChunks.Count;
        payload.Add(("chunks.json", ToJsonBytes(new BundleChunkSet { Chunks = allChunks })));

        manifest.Sha256 = BundleArchiveSafety.ComputePayloadSha256(payload);

        var dir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        if (File.Exists(outputPath)) File.Delete(outputPath);

        using (var zipStream = File.Create(outputPath))
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            foreach (var (name, content) in payload)
                await WriteEntryAsync(archive, name, content, ct);

            // Manifest written last so reading it doesn't require the rest to be present.
            await WriteEntryAsync(archive, "manifest.json", ToJsonBytes(manifest), ct);
        }

        return manifest;
    }

    /// <summary>
    /// Derives a stable course key slug from a display name: lowercased, runs of
    /// disallowed characters collapsed to single hyphens ("Dracula" → "dracula",
    /// "A Tale of Two Cities" → "a-tale-of-two-cities").
    /// </summary>
    public static string DeriveCourseKey(string courseName)
    {
        var sb = new StringBuilder(courseName.Length);
        foreach (var c in courseName.Trim().ToLowerInvariant())
        {
            if (c is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '.' or '_' or '-')
                sb.Append(c);
            else if (sb.Length > 0 && sb[^1] != '-')
                sb.Append('-');
        }

        var key = sb.ToString().Trim('-', '.', '_');
        if (key.Length == 0) key = "course";
        if (key.Length > 120) key = key[..120].TrimEnd('-', '.', '_');
        return key;
    }

    private static byte[] ToJsonBytes<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value, JsonOpts);

    private static async Task WriteEntryAsync(ZipArchive archive, string entryName, byte[] content, CancellationToken ct)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        using var stream = entry.Open();
        await stream.WriteAsync(content, ct);
    }
}

/// <summary>
/// Identity + provenance choices for an export. When <see cref="CourseKey"/> is
/// empty one is derived from the course name; the version follows the house
/// whole-number rule.
/// </summary>
public sealed class BundleExportOptions
{
    public string CourseKey { get; set; } = "";
    public int CourseVersion { get; set; } = 1;
    public string? Author { get; set; }
    public string? License { get; set; }
    public string? Description { get; set; }
    public string? SourceAttribution { get; set; }
}
