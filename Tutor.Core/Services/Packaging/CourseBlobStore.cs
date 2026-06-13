using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// Retains the verbatim .tutor a course was installed from, at
/// "{AppData}\courses\{key}\{version}.tutor", so a learner can re-share exactly
/// the bundle they received (and a future rollback has the original to hand).
/// </summary>
public sealed class CourseBlobStore
{
    private readonly string rootDirectory;

    public CourseBlobStore(IAppDataPathProvider pathProvider)
    {
        this.rootDirectory = Path.Combine(pathProvider.AppDataDirectory, "courses");
    }

    /// <summary>Copies the bundle into the store and returns the retained blob path.</summary>
    public string Save(string sourceBundlePath, string courseKey, int courseVersion)
    {
        var path = GetBlobPath(courseKey, courseVersion);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.Copy(sourceBundlePath, path, overwrite: true);
        Log.Info($"CourseBlobStore: retained verbatim bundle at {path}");
        return path;
    }

    public string GetBlobPath(string courseKey, int courseVersion) =>
        Path.Combine(rootDirectory, courseKey, $"{courseVersion}.tutor");

    public bool Exists(string courseKey, int courseVersion) => File.Exists(GetBlobPath(courseKey, courseVersion));

    /// <summary>Opens the retained bundle for re-share download; null when not retained.</summary>
    public Stream? Open(string courseKey, int courseVersion)
    {
        var path = GetBlobPath(courseKey, courseVersion);
        return File.Exists(path) ? File.OpenRead(path) : null;
    }

    public void Delete(string courseKey, int courseVersion)
    {
        var path = GetBlobPath(courseKey, courseVersion);
        if (File.Exists(path)) File.Delete(path);

        // Tidy the per-key folder once its last version is gone.
        var dir = Path.GetDirectoryName(path)!;
        if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
            Directory.Delete(dir);
    }
}
