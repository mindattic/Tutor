using System.IO.Compression;
using System.Text.Json;

namespace Tutor.Cli.Pipeline;

/// <summary>
/// The <c>manifest.json</c> that describes a course-source package — a directory or
/// <c>.zip</c> containing this manifest plus the source files it lists. Per-item
/// metadata is optional; anything omitted falls back to what the parser detects.
/// </summary>
public sealed class CourseManifest
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string? Icon { get; set; }
    public List<string> Tags { get; set; } = new();

    /// <summary>Optional default quiz mode (baked|dynamic|both). A CLI <c>--quiz-mode</c> overrides it.</summary>
    public string? QuizMode { get; set; }

    public List<CourseManifestItem> Items { get; set; } = new();
}

/// <summary>One source file in a <see cref="CourseManifest"/>. <see cref="File"/> is
/// relative to the package root.</summary>
public sealed class CourseManifestItem
{
    public string File { get; set; } = "";
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// An opened course-source package. Accepts either a directory containing
/// <c>manifest.json</c> or a <c>.zip</c>/<c>.tutorpkg</c> archive (extracted to a temp
/// dir for the lifetime of this object). Dispose to clean up any temp extraction.
/// </summary>
public sealed class CoursePackage : IDisposable
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public CourseManifest Manifest { get; }
    public string BaseDir { get; }
    private readonly string? tempDir;

    private CoursePackage(CourseManifest manifest, string baseDir, string? tempDir)
    {
        this.Manifest = manifest;
        this.BaseDir = baseDir;
        this.tempDir = tempDir;
    }

    /// <summary>
    /// Opens a package at <paramref name="path"/> (a directory or a zip archive) and
    /// reads its <c>manifest.json</c>. Throws on a missing path or missing/empty manifest.
    /// </summary>
    public static CoursePackage Open(string path)
    {
        string baseDir;
        string? tempDir = null;

        if (Directory.Exists(path))
        {
            baseDir = path;
        }
        else if (File.Exists(path))
        {
            tempDir = Path.Combine(Path.GetTempPath(), "tutor-pkg-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            ZipFile.ExtractToDirectory(path, tempDir);
            // Some zips wrap everything in a single top-level folder; descend into it
            // when that folder (not the archive root) holds the manifest.
            baseDir = ResolveManifestRoot(tempDir);
        }
        else
        {
            throw new FileNotFoundException($"Course package not found: {path}");
        }

        var manifestPath = Path.Combine(baseDir, "manifest.json");
        if (!File.Exists(manifestPath))
        {
            CleanupTemp(tempDir);
            throw new FileNotFoundException($"manifest.json not found in package '{path}'.");
        }

        var manifest = JsonSerializer.Deserialize<CourseManifest>(File.ReadAllText(manifestPath), JsonOpts);
        if (manifest == null || manifest.Items.Count == 0)
        {
            CleanupTemp(tempDir);
            throw new InvalidOperationException("manifest.json is empty or lists no items.");
        }

        return new CoursePackage(manifest, baseDir, tempDir);
    }

    /// <summary>Resolves an item's <c>File</c> to an absolute path; throws if it's missing.</summary>
    public string ResolveItemPath(CourseManifestItem item)
    {
        var full = Path.GetFullPath(Path.Combine(BaseDir, item.File));
        if (!File.Exists(full))
            throw new FileNotFoundException($"Item file not found in package: {item.File}");
        return full;
    }

    private static string ResolveManifestRoot(string dir)
    {
        if (File.Exists(Path.Combine(dir, "manifest.json")))
            return dir;
        var subdirs = Directory.GetDirectories(dir);
        if (subdirs.Length == 1 && File.Exists(Path.Combine(subdirs[0], "manifest.json")))
            return subdirs[0];
        return dir;
    }

    private static void CleanupTemp(string? tempDir)
    {
        if (tempDir != null && Directory.Exists(tempDir))
        {
            try { Directory.Delete(tempDir, recursive: true); } catch { /* best effort */ }
        }
    }

    public void Dispose() => CleanupTemp(tempDir);
}
