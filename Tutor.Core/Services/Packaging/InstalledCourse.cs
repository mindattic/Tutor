namespace Tutor.Core.Services.Packaging;

/// <summary>
/// One row in the installed-courses registry: what is installed, from which
/// bundle, when — and whether it is currently loaded. Unload flips
/// <see cref="Enabled"/> to false (the course disappears from the learning view;
/// data and progress are retained); only an explicit Remove cascades a delete.
/// </summary>
public sealed class InstalledCourse
{
    /// <summary>The remapped course GUID actually persisted in storage.</summary>
    public string CourseId { get; set; } = "";

    /// <summary>Stable identity from the bundle manifest (or derived on legacy imports).</summary>
    public string CourseKey { get; set; } = "";

    public int CourseVersion { get; set; } = 1;
    public string Name { get; set; } = "";

    /// <summary>Payload SHA-256 from the manifest; empty for legacy bundles.</summary>
    public string Sha256 { get; set; } = "";

    /// <summary>Where the verbatim .tutor is retained for re-share/rollback; null when not kept.</summary>
    public string? BlobPath { get; set; }

    /// <summary>Soft load/unload switch — unload hides, it doesn't erase.</summary>
    public bool Enabled { get; set; } = true;

    public DateTime InstalledUtc { get; set; }
}
