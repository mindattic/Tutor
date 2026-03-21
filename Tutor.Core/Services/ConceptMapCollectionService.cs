using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Service for managing ConceptMapCollections.
/// 
/// A ConceptMapCollection aggregates multiple ConceptMaps (one per Resource)
/// for a Course. This allows dynamic composition of courses from any subset of resources.
/// </summary>
public sealed class ConceptMapCollectionService
{
    private readonly ConceptMapStorageService conceptMapStorageService;

    // In-memory cache of collections (keyed by collection ID)
    private readonly Dictionary<string, ConceptMapCollection> cache = [];

    /// <summary>
    /// Event fired when a collection is saved.
    /// </summary>
    public event Action<string>? OnCollectionSaved;

    /// <summary>
    /// Gets the current storage path for collections.
    /// </summary>
    private string StoragePath => Path.Combine(DataStorageSettings.GetKnowledgeBasesDirectory(), "collections");

    public ConceptMapCollectionService(ConceptMapStorageService conceptMapStorageService)
    {
        this.conceptMapStorageService = conceptMapStorageService;
        Log.Debug($"ConceptMapCollectionService initialized at: {StoragePath}");
    }

    /// <summary>
    /// Creates a new empty ConceptMapCollection.
    /// </summary>
    public async Task<ConceptMapCollection> CreateAsync(string name, string description = "", CancellationToken ct = default)
    {
        Log.Info($"ConceptMapCollection: Creating new collection '{name}'");
        var collection = new ConceptMapCollection
        {
            Name = name,
            Description = description
        };

        await SaveAsync(collection, ct);
        return collection;
    }

    /// <summary>
    /// Creates a ConceptMapCollection for a course from its resource ConceptMaps.
    /// </summary>
    public async Task<ConceptMapCollection> CreateForCourseAsync(
        string courseId,
        string courseName,
        List<string> conceptMapIds,
        CancellationToken ct = default)
    {
        Log.Info($"ConceptMapCollection: Creating collection for course '{courseName}' with {conceptMapIds.Count} ConceptMaps");
        
        var collection = new ConceptMapCollection
        {
            Id = $"collection_{courseId}", // Use predictable ID based on course
            Name = $"{courseName} Concept Map Collection",
            Description = $"Combined concepts from {conceptMapIds.Count} resource(s)",
            ConceptMapIds = conceptMapIds.ToList()
        };

        await SaveAsync(collection, ct);
        return collection;
    }

    /// <summary>
    /// Saves a collection to disk.
    /// </summary>
    public async Task SaveAsync(ConceptMapCollection collection, CancellationToken ct = default)
    {
        collection.UpdatedAt = DateTime.UtcNow;

        var filePath = GetFilePath(collection.Id);
        Log.Debug($"ConceptMapCollection: Saving collection '{collection.Name}' (ID: {collection.Id}) to {filePath}");

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
            Log.Debug($"ConceptMapCollection: Collection '{collection.Name}' saved successfully ({json.Length} bytes)");

            OnCollectionSaved?.Invoke(collection.Id);
        }
        catch (Exception ex)
        {
            Log.Error($"ConceptMapCollection: Failed to save collection '{collection.Name}' - {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// Loads a collection by ID.
    /// </summary>
    public async Task<ConceptMapCollection?> LoadAsync(string collectionId, CancellationToken ct = default)
    {
        // Check cache first
        if (cache.TryGetValue(collectionId, out var cached))
        {
            Log.Debug($"ConceptMapCollection: Returning cached collection '{cached.Name}'");
            return cached;
        }

        var filePath = GetFilePath(collectionId);
        if (!File.Exists(filePath))
        {
            Log.Debug($"ConceptMapCollection: Collection file not found: {filePath}");
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath, ct);
            var collection = JsonSerializer.Deserialize<ConceptMapCollection>(json);

            if (collection != null)
            {
                cache[collection.Id] = collection;
                Log.Debug($"ConceptMapCollection: Loaded collection '{collection.Name}'");
            }

            return collection;
        }
        catch (Exception ex)
        {
            Log.Error($"ConceptMapCollection: Failed to load collection {collectionId} - {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// Loads a collection and all its ConceptMaps.
    /// </summary>
    public async Task<LoadedConceptMapCollection?> LoadWithConceptMapsAsync(
        string collectionId,
        CancellationToken ct = default)
    {
        var collection = await LoadAsync(collectionId, ct);
        if (collection == null)
            return null;

        var conceptMaps = new List<ConceptMap>();
        foreach (var cmId in collection.ConceptMapIds)
        {
            var cm = await conceptMapStorageService.LoadAsync(cmId, ct);
            if (cm != null)
            {
                conceptMaps.Add(cm);
            }
            else
            {
                Log.Warn($"ConceptMapCollection: ConceptMap {cmId} not found for collection '{collection.Name}'");
            }
        }

        Log.Debug($"ConceptMapCollection: Loaded {conceptMaps.Count}/{collection.ConceptMapIds.Count} ConceptMaps for '{collection.Name}'");
        return new LoadedConceptMapCollection(collection, conceptMaps);
    }

    /// <summary>
    /// Adds a ConceptMap to a collection.
    /// </summary>
    public async Task<bool> AddConceptMapAsync(
        string collectionId,
        string conceptMapId,
        CancellationToken ct = default)
    {
        var collection = await LoadAsync(collectionId, ct);
        if (collection == null)
        {
            Log.Warn($"ConceptMapCollection: Cannot add ConceptMap to non-existent collection {collectionId}");
            return false;
        }

        if (collection.AddConceptMap(conceptMapId))
        {
            await SaveAsync(collection, ct);
            Log.Info($"ConceptMapCollection: Added ConceptMap {conceptMapId} to collection '{collection.Name}'");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes a ConceptMap from a collection.
    /// </summary>
    public async Task<bool> RemoveConceptMapAsync(
        string collectionId,
        string conceptMapId,
        CancellationToken ct = default)
    {
        var collection = await LoadAsync(collectionId, ct);
        if (collection == null)
            return false;

        if (collection.RemoveConceptMap(conceptMapId))
        {
            await SaveAsync(collection, ct);
            Log.Info($"ConceptMapCollection: Removed ConceptMap {conceptMapId} from collection '{collection.Name}'");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Rebuilds a collection from a course's resources.
    /// This loads each resource's ConceptMap ID and creates/updates the collection.
    /// Also verifies ConceptMaps exist on disk to handle stale resource status.
    /// </summary>
    public async Task<ConceptMapCollection> RebuildForCourseAsync(
        Course course,
        List<CourseResource> resources,
        CancellationToken ct = default)
    {
        Log.Info($"ConceptMapCollection: Rebuilding collection for course '{course.Name}'");

        // Gather ConceptMap IDs from resources, verifying they exist on disk
        var conceptMapIds = new List<string>();
        foreach (var resource in resources)
        {
            if (string.IsNullOrEmpty(resource.ConceptMapId))
                continue;
                
            // Verify the ConceptMap exists and is ready
            var cm = await conceptMapStorageService.LoadAsync(resource.ConceptMapId, ct);
            if (cm != null && cm.Status == ConceptMapStatus.Ready)
            {
                conceptMapIds.Add(resource.ConceptMapId);
                
                // Fix up stale resource status if needed
                if (resource.ConceptMapStatus != ConceptMapStatus.Ready)
                {
                    resource.ConceptMapStatus = ConceptMapStatus.Ready;
                    Log.Debug($"ConceptMapCollection: Fixed stale status for resource '{resource.Title}'");
                }
            }
        }

        // Try to load existing collection or create new
        var collectionId = $"collection_{course.Id}";
        var collection = await LoadAsync(collectionId, ct);

        if (collection == null)
        {
            collection = new ConceptMapCollection
            {
                Id = collectionId,
                Name = $"{course.Name} Concept Map Collection",
                Description = $"Combined concepts from {conceptMapIds.Count} resource(s)"
            };
        }

        // Update the collection's ConceptMap IDs
        collection.ConceptMapIds = conceptMapIds;
        collection.Description = $"Combined concepts from {conceptMapIds.Count} resource(s)";
        collection.UpdatedAt = DateTime.UtcNow;

        await SaveAsync(collection, ct);

        Log.Info($"ConceptMapCollection: Collection for '{course.Name}' now has {conceptMapIds.Count} ConceptMaps");
        return collection;
    }

    /// <summary>
    /// Gets all collections.
    /// </summary>
    public async Task<List<ConceptMapCollection>> GetAllAsync(CancellationToken ct = default)
    {
        var collections = new List<ConceptMapCollection>();

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

        Log.Debug($"ConceptMapCollection: Found {collections.Count} collections");
        return collections;
    }

    /// <summary>
    /// Deletes a collection (does not delete the underlying ConceptMaps).
    /// </summary>
    public async Task DeleteAsync(string collectionId, CancellationToken ct = default)
    {
        var filePath = GetFilePath(collectionId);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            cache.Remove(collectionId);
            Log.Info($"ConceptMapCollection: Deleted collection {collectionId}");
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
