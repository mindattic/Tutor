using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Service for persisting and loading CourseStructure data.
/// CourseStructures define the learning path for a Course and reference
/// Concepts from the KnowledgeBase by ID.
/// </summary>
public sealed class CourseStructureStorageService
{
    private const string StructuresDirectoryName = "CourseStructures";
    private readonly IAppDataPathProvider _pathProvider;
    private readonly string storagePath;

    // In-memory cache of loaded structures
    private readonly Dictionary<string, CourseStructure> cache = [];

    /// <summary>
    /// Event fired when a course structure is saved.
    /// </summary>
    public event Action<string>? OnStructureSaved;

    public CourseStructureStorageService(IAppDataPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
        storagePath = Path.Combine(_pathProvider.AppDataDirectory, StructuresDirectoryName);
        Directory.CreateDirectory(storagePath);
    }

    /// <summary>
    /// Saves a course structure to disk.
    /// </summary>
    public async Task SaveAsync(CourseStructure structure, CancellationToken ct = default)
    {
        structure.UpdatedAt = DateTime.UtcNow;

        var filePath = GetFilePath(structure.Id);
        var json = JsonSerializer.Serialize(structure, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        // Write to temp file first, then move (atomic operation)
        var tempPath = filePath + ".tmp";
        await File.WriteAllTextAsync(tempPath, json, ct);

        // Replace existing file atomically
        if (File.Exists(filePath))
        {
            var backupPath = filePath + ".bak";
            File.Move(filePath, backupPath, overwrite: true);
        }
        File.Move(tempPath, filePath, overwrite: true);

        // Update cache
        cache[structure.Id] = structure;

        OnStructureSaved?.Invoke(structure.Id);
    }

    /// <summary>
    /// Loads a course structure by ID.
    /// </summary>
    public async Task<CourseStructure?> LoadAsync(string structureId, CancellationToken ct = default)
    {
        // Check cache first
        if (cache.TryGetValue(structureId, out var cached))
            return cached;

        var filePath = GetFilePath(structureId);

        if (!File.Exists(filePath))
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(filePath, ct);
            var structure = JsonSerializer.Deserialize<CourseStructure>(json);

            if (structure != null)
                cache[structure.Id] = structure;

            return structure;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Loads a course structure by course ID.
    /// </summary>
    public async Task<CourseStructure?> LoadByCourseIdAsync(string courseId, CancellationToken ct = default)
    {
        // Check cache first
        var cachedStructure = cache.Values.FirstOrDefault(s => s.CourseId == courseId);
        if (cachedStructure != null)
            return cachedStructure;

        // Scan directory for matching structure
        if (!Directory.Exists(storagePath))
            return null;

        foreach (var file in Directory.GetFiles(storagePath, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file, ct);
                var structure = JsonSerializer.Deserialize<CourseStructure>(json);

                if (structure?.CourseId == courseId)
                {
                    cache[structure.Id] = structure;
                    return structure;
                }
            }
            catch
            {
                // Skip invalid files
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all course structures.
    /// </summary>
    public async Task<List<CourseStructure>> GetAllAsync(CancellationToken ct = default)
    {
        var result = new List<CourseStructure>();

        if (!Directory.Exists(storagePath))
            return result;

        foreach (var file in Directory.GetFiles(storagePath, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file, ct);
                var structure = JsonSerializer.Deserialize<CourseStructure>(json);

                if (structure != null)
                {
                    cache[structure.Id] = structure;
                    result.Add(structure);
                }
            }
            catch
            {
                // Skip invalid files
            }
        }

        return result.OrderByDescending(s => s.UpdatedAt).ToList();
    }

    /// <summary>
    /// Deletes a course structure.
    /// </summary>
    public Task DeleteAsync(string structureId, CancellationToken ct = default)
    {
        var filePath = GetFilePath(structureId);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        cache.Remove(structureId);

        // Clean up backup file if exists
        var backupPath = filePath + ".bak";
        if (File.Exists(backupPath))
        {
            File.Delete(backupPath);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes the course structure for a course.
    /// </summary>
    public async Task DeleteByCourseIdAsync(string courseId, CancellationToken ct = default)
    {
        var structure = await LoadByCourseIdAsync(courseId, ct);
        if (structure != null)
        {
            await DeleteAsync(structure.Id, ct);
        }
    }

    /// <summary>
    /// Checks if a course structure exists for a course.
    /// </summary>
    public async Task<bool> ExistsForCourseAsync(string courseId, CancellationToken ct = default)
    {
        var structure = await LoadByCourseIdAsync(courseId, ct);
        return structure != null;
    }

    /// <summary>
    /// Gets the status of a course structure for a course.
    /// </summary>
    public async Task<CourseStructureStatus?> GetStatusAsync(string courseId, CancellationToken ct = default)
    {
        var structure = await LoadByCourseIdAsync(courseId, ct);
        return structure?.Status;
    }

    /// <summary>
    /// Clears the in-memory cache.
    /// </summary>
    public void ClearCache()
    {
        cache.Clear();
    }

    private string GetFilePath(string structureId)
    {
        return Path.Combine(storagePath, $"{structureId}.json");
    }
}
