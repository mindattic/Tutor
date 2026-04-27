using Tutor.Core.Models;

namespace Tutor.Cli.Export;

// On-disk format for a .tutorcourse bundle (zip-compressed).
// Layout inside the archive:
//   manifest.json
//   course.json
//   courseStructure.json
//   resources/{resourceId}.json
//   resources/{resourceId}.original.txt        (only if Content is non-empty)
//   resources/{resourceId}.formatted.md        (only if FormattedContent is non-empty)
//   conceptMaps/{conceptMapId}.json
//   chunks.json                                (vector store entries for this course)
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

public sealed class BundleChunkSet
{
    public List<ContentChunk> Chunks { get; set; } = new();
}
