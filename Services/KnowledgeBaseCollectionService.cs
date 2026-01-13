using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for managing KnowledgeBaseCollections.
/// 
/// A KnowledgeBaseCollection aggregates multiple KnowledgeBases (one per Resource)
/// for a Course. This allows dynamic composition of courses from any subset of resources.
/// </summary>
public sealed class KnowledgeBaseCollectionService
{
    private readonly KnowledgeBaseStorageService kbStorageService;

    // In-memory cache of collections (keyed by collection ID)
    private readonly Dictionary<string, KnowledgeBaseCollection> cache = [];

    /// <summary>
    /// Event fired when a collection is saved.
    /// </summary>
    public event Action<string>? OnCollectionSaved;

    /// <summary>
    /// Gets the current storage path for collections.
    /// </summary>
    private string StoragePath => Path.Combine(DataStorageSettings.GetKnowledgeBasesDirectory(), "collections");

    public KnowledgeBaseCollectionService(KnowledgeBaseStorageService kbStorageService)
    {
        this.kbStorageService = kbStorageService;
        Log.Debug($"KnowledgeBaseCollectionService initialized at: {StoragePath}");
    }

    /// <summary>
    /// Creates a new empty KnowledgeBaseCollection.
    /// </summary>
    public async Task<KnowledgeBaseCollection> CreateAsync(string name, string description = "", CancellationToken ct = default)
    {
        Log.Info($"KbCollection: Creating new collection '{name}'");
        var collection = new KnowledgeBaseCollection
        {
            Name = name,
            Description = description
        };

        await SaveAsync(collection, ct);
        return collection;
    }

    /// <summary>
    /// Creates a KnowledgeBaseCollection for a course from its resource KnowledgeBases.
    /// </summary>
    public async Task<KnowledgeBaseCollection> CreateForCourseAsync(
        string courseId,
        string courseName,
        List<string> knowledgeBaseIds,
        CancellationToken ct = default)
    {
        Log.Info($"KbCollection: Creating collection for course '{courseName}' with {knowledgeBaseIds.Count} KBs");
        
        var collection = new KnowledgeBaseCollection
        {
            Id = $"collection_{courseId}", // Use predictable ID based on course
            Name = $"{courseName} Knowledge Collection",
            Description = $"Combined knowledge from {knowledgeBaseIds.Count} resource(s)",
            KnowledgeBaseIds = knowledgeBaseIds.ToList()
        };

        await SaveAsync(collection, ct);
        return collection;
    }

    /// <summary>
    /// Saves a collection to disk.
    /// </summary>
    public async Task SaveAsync(KnowledgeBaseCollection collection, CancellationToken ct = default)
    {
        collection.UpdatedAt = DateTime.UtcNow;

        var filePath = GetFilePath(collection.Id);
        Log.Debug($"KbCollection: Saving collection '{collection.Name}' (ID: {collection.Id}) to {filePath}");

        var json = JsonSerializer.Serialize(collection, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

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
            cache[collection.Id] = collection;
            Log.Debug($"KbCollection: Collection '{collection.Name}' saved successfully ({json.Length} bytes)");

            OnCollectionSaved?.Invoke(collection.Id);
        }
        catch (Exception ex)
        {
            Log.Error($"KbCollection: Failed to save collection '{collection.Name}' - {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// Loads a collection by ID.
    /// </summary>
    public async Task<KnowledgeBaseCollection?> LoadAsync(string collectionId, CancellationToken ct = default)
    {
        // Check cache first
        if (cache.TryGetValue(collectionId, out var cached))
        {
            Log.Debug($"KbCollection: Returning cached collection '{cached.Name}'");
            return cached;
        }

        var filePath = GetFilePath(collectionId);
        if (!File.Exists(filePath))
        {
            Log.Debug($"KbCollection: Collection file not found: {filePath}");
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath, ct);
            var collection = JsonSerializer.Deserialize<KnowledgeBaseCollection>(json);

            if (collection != null)
            {
                cache[collection.Id] = collection;
                Log.Debug($"KbCollection: Loaded collection '{collection.Name}'");
            }

            return collection;
        }
        catch (Exception ex)
        {
            Log.Error($"KbCollection: Failed to load collection {collectionId} - {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// Loads a collection and all its KnowledgeBases.
    /// </summary>
    public async Task<LoadedKnowledgeBaseCollection?> LoadWithKnowledgeBasesAsync(
        string collectionId,
        CancellationToken ct = default)
    {
        var collection = await LoadAsync(collectionId, ct);
        if (collection == null)
            return null;

        var knowledgeBases = new List<KnowledgeBase>();
        foreach (var kbId in collection.KnowledgeBaseIds)
        {
            var kb = await kbStorageService.LoadAsync(kbId, ct);
            if (kb != null)
            {
                knowledgeBases.Add(kb);
            }
            else
            {
                Log.Warn($"KbCollection: KnowledgeBase {kbId} not found for collection '{collection.Name}'");
            }
        }

        Log.Debug($"KbCollection: Loaded {knowledgeBases.Count}/{collection.KnowledgeBaseIds.Count} KBs for '{collection.Name}'");
        return new LoadedKnowledgeBaseCollection(collection, knowledgeBases);
    }

    /// <summary>
    /// Adds a KnowledgeBase to a collection.
    /// </summary>
    public async Task<bool> AddKnowledgeBaseAsync(
        string collectionId,
        string knowledgeBaseId,
        CancellationToken ct = default)
    {
        var collection = await LoadAsync(collectionId, ct);
        if (collection == null)
        {
            Log.Warn($"KbCollection: Cannot add KB to non-existent collection {collectionId}");
            return false;
        }

        if (collection.AddKnowledgeBase(knowledgeBaseId))
        {
            await SaveAsync(collection, ct);
            Log.Info($"KbCollection: Added KB {knowledgeBaseId} to collection '{collection.Name}'");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes a KnowledgeBase from a collection.
    /// </summary>
    public async Task<bool> RemoveKnowledgeBaseAsync(
        string collectionId,
        string knowledgeBaseId,
        CancellationToken ct = default)
    {
        var collection = await LoadAsync(collectionId, ct);
        if (collection == null)
            return false;

        if (collection.RemoveKnowledgeBase(knowledgeBaseId))
        {
            await SaveAsync(collection, ct);
            Log.Info($"KbCollection: Removed KB {knowledgeBaseId} from collection '{collection.Name}'");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Rebuilds a collection from a course's resources.
    /// This loads each resource's KnowledgeBase ID and creates/updates the collection.
    /// Also verifies KBs exist on disk to handle stale resource status.
    /// </summary>
    public async Task<KnowledgeBaseCollection> RebuildForCourseAsync(
        Course course,
        List<CourseResource> resources,
        CancellationToken ct = default)
    {
        Log.Info($"KbCollection: Rebuilding collection for course '{course.Name}'");

        // Gather KnowledgeBase IDs from resources, verifying they exist on disk
        var kbIds = new List<string>();
        foreach (var resource in resources)
        {
            if (string.IsNullOrEmpty(resource.KnowledgeBaseId))
                continue;
                
            // Verify the KB exists and is ready
            var kb = await kbStorageService.LoadAsync(resource.KnowledgeBaseId, ct);
            if (kb != null && kb.Status == KnowledgeBaseStatus.Ready)
            {
                kbIds.Add(resource.KnowledgeBaseId);
                
                // Fix up stale resource status if needed
                if (resource.KnowledgeBaseStatus != KnowledgeBaseStatus.Ready)
                {
                    resource.KnowledgeBaseStatus = KnowledgeBaseStatus.Ready;
                    Log.Debug($"KbCollection: Fixed stale KB status for resource '{resource.Title}'");
                }
            }
        }

        // Try to load existing collection or create new
        var collectionId = $"collection_{course.Id}";
        var collection = await LoadAsync(collectionId, ct);

        if (collection == null)
        {
            collection = new KnowledgeBaseCollection
            {
                Id = collectionId,
                Name = $"{course.Name} Knowledge Collection",
                Description = $"Combined knowledge from {kbIds.Count} resource(s)"
            };
        }

        // Update the collection's KnowledgeBase IDs
        collection.KnowledgeBaseIds = kbIds;
        collection.Description = $"Combined knowledge from {kbIds.Count} resource(s)";
        collection.UpdatedAt = DateTime.UtcNow;

        await SaveAsync(collection, ct);

        Log.Info($"KbCollection: Collection for '{course.Name}' now has {kbIds.Count} KBs");
        return collection;
    }

    /// <summary>
    /// Gets all collections.
    /// </summary>
    public async Task<List<KnowledgeBaseCollection>> GetAllAsync(CancellationToken ct = default)
    {
        var collections = new List<KnowledgeBaseCollection>();

        if (!Directory.Exists(StoragePath))
            return collections;

        var files = Directory.GetFiles(StoragePath, "*.json");
        foreach (var file in files)
        {
            var id = Path.GetFileNameWithoutExtension(file);
            var collection = await LoadAsync(id, ct);
            if (collection != null)
            {
                collections.Add(collection);
            }
        }

        Log.Debug($"KbCollection: Found {collections.Count} collections");
        return collections;
    }

    /// <summary>
    /// Deletes a collection (does not delete the underlying KnowledgeBases).
    /// </summary>
    public async Task DeleteAsync(string collectionId, CancellationToken ct = default)
    {
        var filePath = GetFilePath(collectionId);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            cache.Remove(collectionId);
            Log.Info($"KbCollection: Deleted collection {collectionId}");
        }
        await Task.CompletedTask;
    }

    private string GetFilePath(string collectionId)
    {
        // Ensure directory exists
        if (!Directory.Exists(StoragePath))
        {
            Directory.CreateDirectory(StoragePath);
        }

        return Path.Combine(StoragePath, $"{collectionId}.json");
    }
}
