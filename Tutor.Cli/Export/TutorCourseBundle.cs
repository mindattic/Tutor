using Tutor.Core.Models;

namespace Tutor.Cli.Export;

/// <summary>
/// On-disk manifest for a .tutorcourse bundle (zip-compressed). The archive layout is:
/// <code>
/// manifest.json
/// course.json
/// courseStructure.json
/// resources/{resourceId}.json
/// resources/{resourceId}.original.txt        (only if Content is non-empty)
/// resources/{resourceId}.formatted.md        (only if FormattedContent is non-empty)
/// conceptMaps/{conceptMapId}.json
/// chunks.json                                (vector store entries for this course)
/// </code>
/// <see cref="FormatVersion"/> is the source of truth for compatibility — the
/// .tutorcourse extension is just a friendly name on a zip file.
/// </summary>
public sealed class BundleManifest
{
    public int FormatVersion { get; set; } = 1;
    public string CourseId { get; set; } = "";
    public string CourseName { get; set; } = "";
    public string ExportedBy { get; set; } = "tutor-cli";
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
    public int ResourceCount { get; set; }
    public int ConceptMapCount { get; set; }
    public int ChunkCount { get; set; }
    public bool IncludesEmbeddings { get; set; } = true;
}

/// <summary>
/// Wrapper for the bundled <see cref="ContentChunk"/> entries (vector-store rows)
/// so re-imports can skip re-embedding the content.
/// </summary>
public sealed class BundleChunkSet
{
    public List<ContentChunk> Chunks { get; set; } = new();
}
