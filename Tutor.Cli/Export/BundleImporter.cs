using System.IO.Compression;
using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services;

namespace Tutor.Cli.Export;

// Inverse of CourseExporter. Reads a .tutorcourse zip, rewrites every ID with a
// fresh GUID (so importing the same bundle twice yields two distinct courses
// instead of overwriting), then persists the course/resources/conceptMaps/
// structure/chunks via the same Tutor.Core services the live app uses. The
// expensive embeddings ride along in chunks.json so re-imports are near-instant.
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

    public async Task<ImportBundleResult> ImportAsync(
        string bundlePath,
        string? overrideCourseName,
        bool allowDuplicate,
        CancellationToken ct = default)
    {
        if (!File.Exists(bundlePath))
            throw new FileNotFoundException($"Bundle not found: {bundlePath}");

        using var archive = ZipFile.OpenRead(bundlePath);

        var manifest = await ReadJsonAsync<BundleManifest>(archive, "manifest.json", ct)
            ?? throw new InvalidOperationException("Bundle is missing manifest.json.");
        if (manifest.FormatVersion != 1)
            throw new InvalidOperationException(
                $"Unsupported bundle format version {manifest.FormatVersion}. This CLI handles version 1.");

        var course = await ReadJsonAsync<Course>(archive, "course.json", ct)
            ?? throw new InvalidOperationException("Bundle is missing course.json.");

        var resources = new List<CourseResource>();
        foreach (var entry in archive.Entries.Where(e =>
                     e.FullName.StartsWith("resources/", StringComparison.Ordinal) &&
                     e.FullName.EndsWith(".json", StringComparison.Ordinal)))
        {
            var r = await ReadJsonEntryAsync<CourseResource>(entry, ct);
            if (r != null) resources.Add(r);
        }

        var conceptMaps = new List<ConceptMap>();
        foreach (var entry in archive.Entries.Where(e =>
                     e.FullName.StartsWith("conceptMaps/", StringComparison.Ordinal) &&
                     e.FullName.EndsWith(".json", StringComparison.Ordinal)))
        {
            var cm = await ReadJsonEntryAsync<ConceptMap>(entry, ct);
            if (cm != null) conceptMaps.Add(cm);
        }

        CourseStructure? structure = null;
        var structureEntry = archive.GetEntry("courseStructure.json");
        if (structureEntry != null)
            structure = await ReadJsonEntryAsync<CourseStructure>(structureEntry, ct);

        var chunkSet = await ReadJsonAsync<BundleChunkSet>(archive, "chunks.json", ct) ?? new BundleChunkSet();

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
            ResourceCount: resources.Count,
            ConceptMapCount: conceptMaps.Count,
            ChunkCount: chunkSet.Chunks.Count,
            HasStructure: structure != null);
    }

    private static async Task<T?> ReadJsonAsync<T>(ZipArchive archive, string entryName, CancellationToken ct)
    {
        var entry = archive.GetEntry(entryName);
        return entry == null ? default : await ReadJsonEntryAsync<T>(entry, ct);
    }

    private static async Task<T?> ReadJsonEntryAsync<T>(ZipArchiveEntry entry, CancellationToken ct)
    {
        using var stream = entry.Open();
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOpts, ct);
    }
}

public sealed record ImportBundleResult(
    Course Course,
    int ResourceCount,
    int ConceptMapCount,
    int ChunkCount,
    bool HasStructure);
