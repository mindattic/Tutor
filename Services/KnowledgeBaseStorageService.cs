using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for persisting and loading KnowledgeBase data.
/// 
/// KnowledgeBases are INDEPENDENT of Courses. They can be:
/// - Built from Resources
/// - Assigned to one or more Courses
/// - Reused across different Courses
/// </summary>
public sealed class KnowledgeBaseStorageService
{
    private const string KnowledgeBasesDirectoryName = "KnowledgeBases";
    private readonly string storagePath;

    // In-memory cache of loaded knowledge bases (keyed by KB ID)
    private readonly Dictionary<string, KnowledgeBase> cache = [];

    /// <summary>
    /// Event fired when a knowledge base is saved.
    /// </summary>
    public event Action<string>? OnKnowledgeBaseSaved;

    public KnowledgeBaseStorageService()
    {
        storagePath = Path.Combine(FileSystem.AppDataDirectory, KnowledgeBasesDirectoryName);
        Directory.CreateDirectory(storagePath);
        Log.Debug($"KnowledgeBaseStorageService initialized at: {storagePath}");
    }

    /// <summary>
    /// Creates a new KnowledgeBase.
    /// </summary>
    public async Task<KnowledgeBase> CreateAsync(string name, string description = "", CancellationToken ct = default)
    {
        Log.Info($"KbStorage: Creating new KnowledgeBase '{name}'");
        var kb = new KnowledgeBase
        {
            Name = name,
            Description = description,
            Status = KnowledgeBaseStatus.NotStarted
        };

        await SaveAsync(kb, ct);
        return kb;
    }

    /// <summary>
    /// Saves a knowledge base to disk.
    /// </summary>
    public async Task SaveAsync(KnowledgeBase kb, CancellationToken ct = default)
    {
        kb.UpdatedAt = DateTime.UtcNow;

        var filePath = GetFilePath(kb.Id);
        Log.Debug($"KbStorage: Saving KB '{kb.Name}' (ID: {kb.Id}) to {filePath}");
        
        var json = JsonSerializer.Serialize(kb, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        try
        {
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

            // Update cache (keyed by KB ID)
            cache[kb.Id] = kb;
            Log.Debug($"KbStorage: KB '{kb.Name}' saved successfully ({json.Length} bytes)");

            OnKnowledgeBaseSaved?.Invoke(kb.Id);
        }
        catch (Exception ex)
        {
            Log.Error($"KbStorage: Failed to save KB '{kb.Name}' - {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// Loads a knowledge base by ID.
    /// </summary>
    public async Task<KnowledgeBase?> LoadAsync(string knowledgeBaseId, CancellationToken ct = default)
    {
        // Check cache first
        if (cache.TryGetValue(knowledgeBaseId, out var cached))
        {
            Log.Trace($"KbStorage: Loading KB {knowledgeBaseId} from cache");
            return cached;
        }

        var filePath = GetFilePath(knowledgeBaseId);
        
        if (!File.Exists(filePath))
        {
            Log.Warn($"KbStorage: KB file not found: {filePath}");
            return null;
        }

        try
        {
            Log.Debug($"KbStorage: Loading KB {knowledgeBaseId} from disk");
            var json = await File.ReadAllTextAsync(filePath, ct);
            var kb = JsonSerializer.Deserialize<KnowledgeBase>(json);
            
            if (kb != null)
            {
                cache[kb.Id] = kb;
                Log.Debug($"KbStorage: Loaded KB '{kb.Name}' ({kb.Concepts.Count} concepts)");
            }
            
            
            return kb;
        }
        catch (Exception ex)
        {
            Log.Error($"KbStorage: Failed to load KB {knowledgeBaseId} - {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// Gets all knowledge bases.
    /// </summary>
    public async Task<List<KnowledgeBase>> GetAllAsync(CancellationToken ct = default)
    {
        var result = new List<KnowledgeBase>();

        if (!Directory.Exists(storagePath))
            return result;

        foreach (var file in Directory.GetFiles(storagePath, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file, ct);
                var kb = JsonSerializer.Deserialize<KnowledgeBase>(json);
                
                if (kb != null)
                {
                    cache[kb.Id] = kb;
                    result.Add(kb);
                }
            }
            catch
            {
                // Skip invalid files
            }
        }

        return result.OrderByDescending(kb => kb.UpdatedAt).ToList();
    }

    /// <summary>
    /// Finds knowledge bases that were built from specific resources.
    /// </summary>
    public async Task<List<KnowledgeBase>> FindByResourceIdsAsync(
        List<string> resourceIds, 
        CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.Where(kb => 
            kb.ResourceIds.Any(r => resourceIds.Contains(r))).ToList();
    }

    /// <summary>
    /// Deletes a knowledge base.
    /// </summary>
    public Task DeleteAsync(string knowledgeBaseId, CancellationToken ct = default)
    {
        var filePath = GetFilePath(knowledgeBaseId);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Remove from cache
        cache.Remove(knowledgeBaseId);

        // Clean up backup file if exists
        var backupPath = filePath + ".bak";
        if (File.Exists(backupPath))
        {
            File.Delete(backupPath);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a knowledge base exists.
    /// </summary>
    public async Task<bool> ExistsAsync(string knowledgeBaseId, CancellationToken ct = default)
    {
        if (cache.ContainsKey(knowledgeBaseId))
            return true;

        var kb = await LoadAsync(knowledgeBaseId, ct);
        return kb != null;
    }

    /// <summary>
    /// Gets the status of a knowledge base.
    /// </summary>
    public async Task<KnowledgeBaseStatus?> GetStatusAsync(string knowledgeBaseId, CancellationToken ct = default)
    {
        var kb = await LoadAsync(knowledgeBaseId, ct);
        return kb?.Status;
    }

    /// <summary>
    /// Gets summary information about all knowledge bases.
    /// </summary>
    public async Task<List<KnowledgeBaseSummary>> GetSummariesAsync(CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        
        return all.Select(kb => new KnowledgeBaseSummary
        {
            Id = kb.Id,
            Name = kb.Name,
            Status = kb.Status,
            ConceptCount = kb.TotalConcepts,
            RelationCount = kb.TotalRelations,
            ResourceCount = kb.ResourceIds.Count,
            Version = kb.Version,
            UpdatedAt = kb.UpdatedAt
        }).ToList();
    }

    /// <summary>
    /// Clears the in-memory cache.
    /// </summary>
    public void ClearCache()
    {
        cache.Clear();
    }

    private string GetFilePath(string knowledgeBaseId)
    {
        return Path.Combine(storagePath, $"{knowledgeBaseId}.json");
    }
}

/// <summary>
/// Summary information about a knowledge base.
/// </summary>
public class KnowledgeBaseSummary
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public KnowledgeBaseStatus Status { get; set; }
    public int ConceptCount { get; set; }
    public int RelationCount { get; set; }
    public int ResourceCount { get; set; }
    public int Version { get; set; }
    public DateTime UpdatedAt { get; set; }
}
