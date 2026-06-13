using System.Text.Json;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// JSON-file-backed registry of installed courses ("{AppData}\InstalledCourses\
/// installed-courses.json"). This is what makes load/unload real: the learner
/// view shows only Enabled rows, unload flips the flag, and nothing is destroyed
/// until an explicit Remove. Mirrors the soft-disable-never-hard-delete contract
/// the user admin service already follows.
/// </summary>
public sealed class InstalledCourseRegistry
{
    private readonly string filePath;
    private readonly SemaphoreSlim gate = new(1, 1);
    private List<InstalledCourse>? cache;

    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    public InstalledCourseRegistry(IAppDataPathProvider pathProvider)
    {
        var dir = Path.Combine(pathProvider.AppDataDirectory, "InstalledCourses");
        Directory.CreateDirectory(dir);
        this.filePath = Path.Combine(dir, "installed-courses.json");
    }

    public async Task<List<InstalledCourse>> GetAllAsync(CancellationToken ct = default)
    {
        await gate.WaitAsync(ct);
        try
        {
            return new List<InstalledCourse>(await LoadAsync(ct));
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<InstalledCourse?> GetByCourseIdAsync(string courseId, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.FirstOrDefault(c => c.CourseId == courseId);
    }

    public async Task<List<InstalledCourse>> GetByKeyAsync(string courseKey, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.Where(c => string.Equals(c.CourseKey, courseKey, StringComparison.Ordinal)).ToList();
    }

    public async Task RecordInstallAsync(InstalledCourse course, CancellationToken ct = default)
    {
        await gate.WaitAsync(ct);
        try
        {
            var all = await LoadAsync(ct);
            all.RemoveAll(c => c.CourseId == course.CourseId);
            all.Add(course);
            await SaveAsync(all, ct);
            Log.Info($"InstalledCourseRegistry: recorded '{course.CourseKey}' v{course.CourseVersion} (course {course.CourseId})");
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>Load (true) or unload (false) a course. Returns false when the course is not registered.</summary>
    public async Task<bool> SetEnabledAsync(string courseId, bool enabled, CancellationToken ct = default)
    {
        await gate.WaitAsync(ct);
        try
        {
            var all = await LoadAsync(ct);
            var row = all.FirstOrDefault(c => c.CourseId == courseId);
            if (row == null) return false;

            row.Enabled = enabled;
            await SaveAsync(all, ct);
            Log.Info($"InstalledCourseRegistry: '{row.CourseKey}' v{row.CourseVersion} {(enabled ? "loaded" : "unloaded")}");
            return true;
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>Drops the registry row (Remove). The caller owns the data cascade.</summary>
    public async Task<InstalledCourse?> RemoveAsync(string courseId, CancellationToken ct = default)
    {
        await gate.WaitAsync(ct);
        try
        {
            var all = await LoadAsync(ct);
            var row = all.FirstOrDefault(c => c.CourseId == courseId);
            if (row == null) return null;

            all.Remove(row);
            await SaveAsync(all, ct);
            return row;
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>True when the course either has no registry row (pre-registry course) or its row is enabled.</summary>
    public async Task<bool> IsEnabledAsync(string courseId, CancellationToken ct = default)
    {
        var row = await GetByCourseIdAsync(courseId, ct);
        return row?.Enabled ?? true;
    }

    /// <summary>
    /// Course ids whose registry row is disabled — what the learning view filters
    /// out so an unloaded course disappears without its data going anywhere.
    /// </summary>
    public async Task<HashSet<string>> GetDisabledCourseIdsAsync(CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.Where(c => !c.Enabled).Select(c => c.CourseId).ToHashSet(StringComparer.Ordinal);
    }

    private async Task<List<InstalledCourse>> LoadAsync(CancellationToken ct)
    {
        if (cache != null) return cache;

        if (!File.Exists(filePath))
        {
            cache = new List<InstalledCourse>();
            return cache;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath, ct);
            cache = JsonSerializer.Deserialize<List<InstalledCourse>>(json) ?? new List<InstalledCourse>();
        }
        catch (Exception ex)
        {
            Log.Error($"InstalledCourseRegistry: failed to read {filePath} - {ex.Message}", ex);
            cache = new List<InstalledCourse>();
        }

        return cache;
    }

    private async Task SaveAsync(List<InstalledCourse> all, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(all, JsonOpts);
        var tempPath = filePath + ".tmp";
        await File.WriteAllTextAsync(tempPath, json, ct);

        if (File.Exists(filePath))
            File.Replace(tempPath, filePath, filePath + ".bak");
        else
            File.Move(tempPath, filePath);

        cache = all;
    }
}
