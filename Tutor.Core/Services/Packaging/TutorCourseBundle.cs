using System.Text.Json;
using System.Text.Json.Serialization;
using Tutor.Core.Models;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// On-disk manifest for a .tutor bundle (zip-compressed). The archive layout is:
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
/// .tutor extension is just a friendly name on a zip file. A reader accepts any
/// format up to its own <see cref="HostMaxFormatVersion"/>; only formats newer
/// than the build are refused.
/// </summary>
public sealed class BundleManifest
{
    /// <summary>The newest bundle format this build can read and the format it writes.</summary>
    public const int HostMaxFormatVersion = 1;

    public int FormatVersion { get; set; } = HostMaxFormatVersion;
    public string CourseId { get; set; } = "";
    public string CourseName { get; set; } = "";

    /// <summary>
    /// Stable identity that survives re-install (GUIDs are remapped on every import;
    /// the key is what lets the system say "you already have Dracula"). Lowercase slug,
    /// pattern ^[a-z0-9][a-z0-9._-]{0,119}$. Empty on legacy (pre-identity) bundles —
    /// importers derive one from <see cref="CourseName"/>.
    /// </summary>
    public string CourseKey { get; set; } = "";

    /// <summary>Whole-number, forward-only course version (house versioning rule).</summary>
    public int CourseVersion { get; set; } = 1;

    /// <summary>
    /// SHA-256 (lowercase hex) of the bundle payload — every entry except
    /// manifest.json, hashed in canonical (name-sorted) order. Empty on legacy
    /// bundles, which import with an integrity warning instead of a guarantee.
    /// </summary>
    public string Sha256 { get; set; } = "";

    public string ExportedBy { get; set; } = "tutor";
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
    public int ResourceCount { get; set; }
    public int ConceptMapCount { get; set; }
    public int ChunkCount { get; set; }
    public bool IncludesEmbeddings { get; set; } = true;

    // Provenance for shared courses — who made it, under what license, from what source.
    public string? Author { get; set; }
    public string? License { get; set; }
    public string? Description { get; set; }
    public string? SourceAttribution { get; set; }

    /// <summary>
    /// Forward-compat round-trip: fields written by a newer format that this build
    /// doesn't know about land here on read and are written back verbatim, so an
    /// older host never strips data from a newer bundle it re-exports.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Extra { get; set; } = new();
}

/// <summary>
/// Wrapper for the bundled <see cref="ContentChunk"/> entries (vector-store rows)
/// so re-imports can skip the LLM pipeline entirely.
/// </summary>
public sealed class BundleChunkSet
{
    public List<ContentChunk> Chunks { get; set; } = new();
}
