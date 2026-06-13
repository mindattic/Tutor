using System.IO.Compression;
using System.Text.Json;
using Tutor.Core.Models;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// Inverse of <see cref="CourseExporter"/>. Reads a .tutor zip, validates it
/// (manifest shape, format gate, payload SHA-256, entry-path safety), rewrites
/// every cross-entity ID with a fresh GUID (so importing the same bundle twice
/// yields two distinct courses instead of overwriting), then persists the
/// course/resources/conceptMaps/structure/chunks via the same Tutor.Core
/// services the live app uses. The expensive embeddings ride along in
/// chunks.json so re-imports are near-instant.
/// </summary>
public sealed class BundleImporter
{
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly CourseStructureStorageService structureStorage;
    private readonly VectorStoreService vectorStore;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BundleImporter(
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
    /// Reads the manifest and validates the bundle without importing anything:
    /// the IO-free validation result plus the (possibly legacy) manifest with its
    /// effective CourseKey back-filled. This is the "look before you install"
    /// step the UI shows a user before asking for confirmation.
    /// </summary>
    public async Task<BundleInspection> InspectAsync(string bundlePath, CancellationToken ct = default)
    {
        if (!File.Exists(bundlePath))
            throw new FileNotFoundException($"Bundle not found: {bundlePath}");

        using var archive = ZipFile.OpenRead(bundlePath);
        var entries = await ReadAllEntriesAsync(archive, ct);

        BundleManifest? manifest = null;
        if (entries.TryGetValue("manifest.json", out var manifestBytes))
            manifest = DeserializeBytes<BundleManifest>(manifestBytes);

        var payload = entries
            .Where(e => e.Key != "manifest.json")
            .Select(e => (e.Key, e.Value))
            .ToList();
        var computedSha = BundleArchiveSafety.ComputePayloadSha256(payload);

        var validation = CourseManifestValidator.Validate(
            manifest, entries.Keys.ToList(), BundleManifest.HostMaxFormatVersion, computedSha);

        if (manifest != null && string.IsNullOrEmpty(manifest.CourseKey))
            manifest.CourseKey = CourseExporter.DeriveCourseKey(manifest.CourseName);

        return new BundleInspection(manifest, validation, computedSha);
    }

    /// <summary>
    /// Reads <paramref name="bundlePath"/>, validates it, rewrites the IDs, and
    /// persists the imported course. Pass <paramref name="overrideCourseName"/> to
    /// rename on import; set <paramref name="allowDuplicate"/> when intentionally
    /// re-importing a name that already exists.
    /// </summary>
    public async Task<ImportBundleResult> ImportAsync(
        string bundlePath,
        string? overrideCourseName,
        bool allowDuplicate,
        CancellationToken ct = default)
    {
        if (!File.Exists(bundlePath))
            throw new FileNotFoundException($"Bundle not found: {bundlePath}");

        using var archive = ZipFile.OpenRead(bundlePath);
        var entries = await ReadAllEntriesAsync(archive, ct);

        BundleManifest? manifest = null;
        if (entries.TryGetValue("manifest.json", out var manifestBytes))
            manifest = DeserializeBytes<BundleManifest>(manifestBytes);

        var payloadEntries = entries
            .Where(e => e.Key != "manifest.json")
            .Select(e => (e.Key, e.Value))
            .ToList();
        var computedSha = BundleArchiveSafety.ComputePayloadSha256(payloadEntries);

        var validation = CourseManifestValidator.Validate(
            manifest, entries.Keys.ToList(), BundleManifest.HostMaxFormatVersion, computedSha);
        if (!validation.IsValid)
            throw new InvalidOperationException(
                "Bundle failed validation: " + string.Join("; ", validation.Errors));

        // Validation guarantees the manifest and course.json exist past this point.
        var courseKey = string.IsNullOrEmpty(manifest!.CourseKey)
            ? CourseExporter.DeriveCourseKey(manifest.CourseName)
            : manifest.CourseKey;

        var course = Deserialize<Course>(entries, "course.json")
            ?? throw new InvalidOperationException("Bundle course.json is unreadable.");

        var resources = new List<CourseResource>();
        foreach (var name in entries.Keys.Where(n =>
                     n.StartsWith("resources/", StringComparison.Ordinal) &&
                     n.EndsWith(".json", StringComparison.Ordinal)))
        {
            var r = Deserialize<CourseResource>(entries, name);
            if (r != null) resources.Add(r);
        }

        var conceptMaps = new List<ConceptMap>();
        foreach (var name in entries.Keys.Where(n =>
                     n.StartsWith("conceptMaps/", StringComparison.Ordinal) &&
                     n.EndsWith(".json", StringComparison.Ordinal)))
        {
            var cm = Deserialize<ConceptMap>(entries, name);
            if (cm != null) conceptMaps.Add(cm);
        }

        var structure = Deserialize<CourseStructure>(entries, "courseStructure.json");
        var chunkSet = Deserialize<BundleChunkSet>(entries, "chunks.json") ?? new BundleChunkSet();

        var newCourseName = string.IsNullOrWhiteSpace(overrideCourseName) ? course.Name : overrideCourseName;

        if (!allowDuplicate)
        {
            var existing = (await courseService.GetAllCoursesAsync())
                .FirstOrDefault(c => string.Equals(c.Name, newCourseName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                throw new InvalidOperationException(
                    $"A course named '{newCourseName}' already exists (id: {existing.Id}). " +
                    "Pass --allow-duplicate to import anyway, or rename with --course \"...\".");
        }

        // Build ID remaps. Concept-level IDs (inside a ConceptMap.Concepts list)
        // and Lesson/Topic/Section IDs are scoped within their parent and are
        // preserved as-is — only cross-entity IDs need rewriting.
        var newCourseId = Guid.NewGuid().ToString();
        var newStructureId = Guid.NewGuid().ToString();
        var resourceIdMap = resources.ToDictionary(r => r.Id, _ => Guid.NewGuid().ToString());
        var conceptMapIdMap = conceptMaps.ToDictionary(cm => cm.Id, _ => Guid.NewGuid().ToString());

        // --- Rewrite course ---
        course.Id = newCourseId;
        course.Name = newCourseName;
        course.ResourceIds = course.ResourceIds
            .Select(id => resourceIdMap.TryGetValue(id, out var nid) ? nid : id)
            .ToList();
        course.ConceptMapCollectionId = $"collection_{newCourseId}";
        course.CourseStructureId = structure != null ? newStructureId : null;
        course.UpdatedAt = DateTime.UtcNow;

        // --- Rewrite resources ---
        foreach (var r in resources)
        {
            r.Id = resourceIdMap[r.Id];
            if (!string.IsNullOrEmpty(r.ConceptMapId)
                && conceptMapIdMap.TryGetValue(r.ConceptMapId, out var newCmId))
            {
                r.ConceptMapId = newCmId;
            }
            r.UpdatedAt = DateTime.UtcNow;
        }

        // --- Rewrite concept maps ---
        foreach (var cm in conceptMaps)
        {
            cm.Id = conceptMapIdMap[cm.Id];
            if (!string.IsNullOrEmpty(cm.ResourceId)
                && resourceIdMap.TryGetValue(cm.ResourceId, out var newRid))
            {
                cm.ResourceId = newRid;
            }
            foreach (var concept in cm.Concepts)
            {
                concept.ConceptMapId = cm.Id;
                concept.SourceResourceIds = concept.SourceResourceIds
                    .Select(id => resourceIdMap.TryGetValue(id, out var nid) ? nid : id)
                    .ToList();
            }
            cm.UpdatedAt = DateTime.UtcNow;
        }

        // --- Rewrite structure ---
        if (structure != null)
        {
            structure.Id = newStructureId;
            structure.CourseId = newCourseId;
            if (!string.IsNullOrEmpty(structure.KnowledgeBaseId)
                && conceptMapIdMap.TryGetValue(structure.KnowledgeBaseId, out var newKb))
            {
                structure.KnowledgeBaseId = newKb;
            }
            foreach (var lesson in structure.Lessons)
            {
                lesson.CourseStructureId = newStructureId;
            }
            structure.UpdatedAt = DateTime.UtcNow;
        }

        // --- Rewrite chunks ---
        foreach (var chunk in chunkSet.Chunks)
        {
            if (resourceIdMap.TryGetValue(chunk.ResourceId, out var newRid))
                chunk.ResourceId = newRid;
            chunk.CurriculumId = newCourseId;
        }

        // --- Persist via existing services (same paths Blazor uses) ---
        foreach (var r in resources)
            await courseService.SaveResourceAsync(r);

        foreach (var cm in conceptMaps)
            await conceptMapStorage.SaveAsync(cm, ct);

        await courseService.SaveCourseAsync(course);

        if (chunkSet.Chunks.Count > 0)
        {
            foreach (var group in chunkSet.Chunks.GroupBy(c => c.ResourceId))
            {
                await vectorStore.StoreChunksAsync(group.Key, newCourseId, group.ToList());
            }
        }

        if (structure != null)
            await structureStorage.SaveAsync(structure, ct);

        // Rebuild the per-course ConceptMapCollection file from the imported
        // resources so the UI's collection-loading path works without surprises.
        await courseService.RebuildCourseConceptMapCollectionAsync(course, ct);

        return new ImportBundleResult(
            Course: course,
            CourseKey: courseKey,
            CourseVersion: manifest.CourseVersion,
            Sha256: manifest.Sha256,
            ResourceCount: resources.Count,
            ConceptMapCount: conceptMaps.Count,
            ChunkCount: chunkSet.Chunks.Count,
            HasStructure: structure != null,
            Warnings: validation.Warnings);
    }

    /// <summary>
    /// Buffers every archive entry as bytes (screening names through the zip-slip
    /// guard happens in validation) so integrity hashing and deserialization read
    /// each entry exactly once.
    /// </summary>
    private static async Task<Dictionary<string, byte[]>> ReadAllEntriesAsync(ZipArchive archive, CancellationToken ct)
    {
        var entries = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        foreach (var entry in archive.Entries)
        {
            if (entry.FullName.EndsWith("/", StringComparison.Ordinal)) continue; // directory marker

            using var stream = entry.Open();
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, ct);
            entries[entry.FullName] = buffer.ToArray();
        }
        return entries;
    }

    private static T? Deserialize<T>(Dictionary<string, byte[]> entries, string name) where T : class =>
        entries.TryGetValue(name, out var bytes) ? DeserializeBytes<T>(bytes) : null;

    private static T? DeserializeBytes<T>(byte[] bytes) where T : class
    {
        // Tolerate a UTF-8 BOM — external tools that touched an entry often add one.
        var span = bytes.AsSpan();
        if (span.Length >= 3 && span[0] == 0xEF && span[1] == 0xBB && span[2] == 0xBF)
            span = span[3..];
        return JsonSerializer.Deserialize<T>(span, JsonOpts);
    }
}

/// <summary>
/// Pre-install look at a bundle: its manifest (CourseKey back-filled for legacy
/// bundles), the validation outcome, and the payload hash that was computed.
/// </summary>
public sealed record BundleInspection(
    BundleManifest? Manifest,
    ValidationResult Validation,
    string ComputedSha256);

/// <summary>
/// Summary returned by <see cref="BundleImporter.ImportAsync"/> describing the
/// rebuilt course, its stable identity, the number of entities restored, and
/// whether a course structure rode along.
/// </summary>
public sealed record ImportBundleResult(
    Course Course,
    string CourseKey,
    int CourseVersion,
    string Sha256,
    int ResourceCount,
    int ConceptMapCount,
    int ChunkCount,
    bool HasStructure,
    List<ValidationIssue> Warnings);
