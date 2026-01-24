using System.Text;
using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for persisting and loading ConceptMap data.
/// 
/// ConceptMaps are INDEPENDENT of Courses. They can be:
/// - Built from Resources
/// - Assigned to one or more Courses
/// - Reused across different Courses
/// </summary>
public sealed class ConceptMapStorageService
{
    // In-memory cache of loaded concept maps (keyed by ConceptMap ID)
    private readonly Dictionary<string, ConceptMap> cache = [];

    /// <summary>
    /// Event fired when a concept map is saved.
    /// </summary>
    public event Action<string>? OnConceptMapSaved;

    /// <summary>
    /// Gets the current storage path (uses DataStorageSettings).
    /// </summary>
    private string StoragePath => DataStorageSettings.GetKnowledgeBasesDirectory();

    public ConceptMapStorageService()
    {
        Log.Debug($"ConceptMapStorageService initialized at: {StoragePath}");
    }

    /// <summary>
    /// Creates a new ConceptMap.
    /// </summary>
    public async Task<ConceptMap> CreateAsync(string name, string description = "", CancellationToken ct = default)
    {
        Log.Info($"ConceptMapStorage: Creating new ConceptMap '{name}'");
        var conceptMap = new ConceptMap
        {
            Name = name,
            Description = description,
            Status = ConceptMapStatus.NotStarted
        };

        await SaveAsync(conceptMap, ct);
        return conceptMap;
    }

    /// <summary>
    /// Saves a concept map to disk.
    /// </summary>
    public async Task SaveAsync(ConceptMap conceptMap, CancellationToken ct = default)
    {
        conceptMap.UpdatedAt = DateTime.UtcNow;

        var filePath = GetFilePath(conceptMap.Id);
        Log.Debug($"ConceptMapStorage: Saving ConceptMap '{conceptMap.Name}' (ID: {conceptMap.Id}) to {filePath}");
        
        var json = JsonSerializer.Serialize(conceptMap, new JsonSerializerOptions
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

            // Update cache
            cache[conceptMap.Id] = conceptMap;
            Log.Debug($"ConceptMapStorage: ConceptMap '{conceptMap.Name}' saved successfully ({json.Length} bytes)");

            // Also save a human-readable markdown export
            await SaveHumanReadableExportAsync(conceptMap, ct);

            OnConceptMapSaved?.Invoke(conceptMap.Id);
        }
        catch (Exception ex)
        {
            Log.Error($"ConceptMapStorage: Failed to save ConceptMap '{conceptMap.Name}' - {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// Loads a concept map by ID.
    /// </summary>
    public async Task<ConceptMap?> LoadAsync(string conceptMapId, CancellationToken ct = default)
    {
        // Check cache first
        if (cache.TryGetValue(conceptMapId, out var cached))
        {
            Log.Trace($"ConceptMapStorage: Loading ConceptMap {conceptMapId} from cache");
            return cached;
        }

        var filePath = GetFilePath(conceptMapId);
        
        if (!File.Exists(filePath))
        {
            Log.Warn($"ConceptMapStorage: ConceptMap file not found: {filePath}");
            return null;
        }

        try
        {
            Log.Debug($"ConceptMapStorage: Loading ConceptMap {conceptMapId} from disk");
            var json = await File.ReadAllTextAsync(filePath, ct);
            var conceptMap = JsonSerializer.Deserialize<ConceptMap>(json);
            
            if (conceptMap != null)
            {
                cache[conceptMap.Id] = conceptMap;
                Log.Debug($"ConceptMapStorage: Loaded ConceptMap '{conceptMap.Name}' ({conceptMap.Concepts.Count} concepts)");
            }
            
            return conceptMap;
        }
        catch (Exception ex)
        {
            Log.Error($"ConceptMapStorage: Failed to load ConceptMap {conceptMapId} - {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// Gets all concept maps.
    /// </summary>
    public async Task<List<ConceptMap>> GetAllAsync(CancellationToken ct = default)
    {
        var result = new List<ConceptMap>();

        if (!Directory.Exists(StoragePath))
            return result;

        foreach (var file in Directory.GetFiles(StoragePath, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file, ct);
                var conceptMap = JsonSerializer.Deserialize<ConceptMap>(json);
                
                if (conceptMap != null)
                {
                    cache[conceptMap.Id] = conceptMap;
                    result.Add(conceptMap);
                }
            }
            catch
            {
                // Skip invalid files
            }
        }

        return result.OrderByDescending(cm => cm.UpdatedAt).ToList();
    }

    /// <summary>
    /// Finds the ConceptMap built from a specific resource.
    /// Returns null if no ConceptMap exists for that resource.
    /// </summary>
    public async Task<ConceptMap?> FindByResourceIdAsync(
        string resourceId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(resourceId))
            return null;

        var all = await GetAllAsync(ct);
        
        // Find by ResourceId property
        return all.FirstOrDefault(cm => cm.ResourceId == resourceId);
    }

    /// <summary>
    /// Gets ConceptMaps for multiple resources.
    /// Returns a dictionary mapping ResourceId to ConceptMap.
    /// </summary>
    public async Task<Dictionary<string, ConceptMap>> GetForResourcesAsync(
        List<string> resourceIds,
        CancellationToken ct = default)
    {
        var result = new Dictionary<string, ConceptMap>();
        var all = await GetAllAsync(ct);

        foreach (var conceptMap in all)
        {
            // Check ResourceId property
            if (!string.IsNullOrEmpty(conceptMap.ResourceId) && resourceIds.Contains(conceptMap.ResourceId))
            {
                result[conceptMap.ResourceId] = conceptMap;
            }
        }

        return result;
    }

    /// <summary>
    /// Deletes a concept map.
    /// </summary>
    public Task DeleteAsync(string conceptMapId, CancellationToken ct = default)
    {
        var filePath = GetFilePath(conceptMapId);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Remove from cache
        cache.Remove(conceptMapId);

        // Clean up backup file if exists
        var backupPath = filePath + ".bak";
        if (File.Exists(backupPath))
        {
            File.Delete(backupPath);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a concept map exists.
    /// </summary>
    public async Task<bool> ExistsAsync(string conceptMapId, CancellationToken ct = default)
    {
        if (cache.ContainsKey(conceptMapId))
            return true;

        var conceptMap = await LoadAsync(conceptMapId, ct);
        return conceptMap != null;
    }

    /// <summary>
    /// Gets the status of a concept map.
    /// </summary>
    public async Task<ConceptMapStatus?> GetStatusAsync(string conceptMapId, CancellationToken ct = default)
    {
        var conceptMap = await LoadAsync(conceptMapId, ct);
        return conceptMap?.Status;
    }

    /// <summary>
    /// Gets summary information about all concept maps.
    /// </summary>
    public async Task<List<ConceptMapSummary>> GetSummariesAsync(CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        
        return all.Select(cm => new ConceptMapSummary
        {
            Id = cm.Id,
            Name = cm.Name,
            Status = cm.Status,
            ConceptCount = cm.TotalConcepts,
            RelationCount = cm.TotalRelations,
            ResourceId = cm.ResourceId,
            Version = cm.Version,
            UpdatedAt = cm.UpdatedAt
        }).ToList();
    }

    /// <summary>
    /// Clears the in-memory cache.
    /// </summary>
    public void ClearCache()
    {
        cache.Clear();
    }

    /// <summary>
    /// Saves a human-readable markdown export of the ConceptMap to the Resources folder.
    /// This allows verification and analysis of the extracted concepts.
    /// </summary>
    private async Task SaveHumanReadableExportAsync(ConceptMap conceptMap, CancellationToken ct = default)
    {
        // Only export if ConceptMap is ready and has concepts
        if (conceptMap.Status != ConceptMapStatus.Ready || conceptMap.Concepts.Count == 0)
            return;

        try
        {
            StorageSettings.Load();
            var cmDir = Path.Combine(StorageSettings.GetResolvedDirectory(), "ConceptMaps");
            Directory.CreateDirectory(cmDir);

            var safeFileName = SanitizeFileName(conceptMap.Name);
            var filePath = Path.Combine(cmDir, $"{safeFileName}_concept-map.md");

            var sb = new StringBuilder();
            
            // Frontmatter
            sb.AppendLine("---");
            sb.AppendLine($"title: \"{conceptMap.Name}\"");
            sb.AppendLine($"id: {conceptMap.Id}");
            sb.AppendLine($"resource_id: {conceptMap.ResourceId}");
            sb.AppendLine($"status: {conceptMap.Status}");
            sb.AppendLine($"concepts: {conceptMap.TotalConcepts}");
            sb.AppendLine($"relations: {conceptMap.TotalRelations}");
            sb.AppendLine($"created: {conceptMap.CreatedAt:O}");
            sb.AppendLine($"updated: {conceptMap.UpdatedAt:O}");
            sb.AppendLine($"version: {conceptMap.Version}");
            sb.AppendLine("---");
            sb.AppendLine();

            // Summary
            sb.AppendLine($"# {conceptMap.Name} - Concept Map");
            sb.AppendLine();
            if (!string.IsNullOrEmpty(conceptMap.Description))
            {
                sb.AppendLine(conceptMap.Description);
                sb.AppendLine();
            }
            sb.AppendLine($"**Total Concepts (Nodes):** {conceptMap.TotalConcepts}");
            sb.AppendLine($"**Total Relations (Edges):** {conceptMap.TotalRelations}");
            sb.AppendLine();

            // Concepts organized by complexity
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## Concepts");
            sb.AppendLine();

            // Group concepts by complexity level if available
            var conceptsByLevel = conceptMap.ComplexityOrder.Count > 0
                ? conceptMap.GetConceptsByComplexity().ToList()
                : conceptMap.Concepts.OrderBy(c => c.Title).ToList();

            var currentLevel = -1;
            foreach (var concept in conceptsByLevel)
            {
                var complexity = conceptMap.GetComplexity(concept.Id);
                if (complexity != null && complexity.Level != currentLevel)
                {
                    currentLevel = complexity.Level;
                    sb.AppendLine($"### Level {currentLevel} - {GetLevelDescription(currentLevel)}");
                    sb.AppendLine();
                }

                sb.AppendLine($"#### {concept.Title}");
                sb.AppendLine();
                
                if (!string.IsNullOrEmpty(concept.Summary))
                {
                    sb.AppendLine(concept.Summary);
                    sb.AppendLine();
                }

                if (!string.IsNullOrEmpty(concept.Content))
                {
                    sb.AppendLine("**Details:**");
                    sb.AppendLine(concept.Content);
                    sb.AppendLine();
                }

                if (concept.Aliases.Count > 0)
                {
                    sb.AppendLine($"**Also known as:** {string.Join(", ", concept.Aliases)}");
                    sb.AppendLine();
                }

                if (concept.Tags.Count > 0)
                {
                    sb.AppendLine($"**Tags:** {string.Join(", ", concept.Tags)}");
                    sb.AppendLine();
                }

                // Show prerequisites
                var prereqs = conceptMap.GetPrerequisites(concept.Id).ToList();
                if (prereqs.Count > 0)
                {
                    sb.AppendLine($"**Prerequisites:** {string.Join(", ", prereqs.Select(p => p.Title))}");
                    sb.AppendLine();
                }

                // Show related concepts
                var related = conceptMap.GetRelatedConcepts(concept.Id).ToList();
                if (related.Count > 0)
                {
                    sb.AppendLine($"**Related:** {string.Join(", ", related.Select(r => r.Title))}");
                    sb.AppendLine();
                }

                sb.AppendLine("---");
                sb.AppendLine();
            }

            // Relations summary
            if (conceptMap.Relations.Count > 0)
            {
                sb.AppendLine("## Relationships");
                sb.AppendLine();
                sb.AppendLine("| Source | Relationship | Target |");
                sb.AppendLine("|--------|--------------|--------|");
                
                foreach (var rel in conceptMap.Relations.OrderBy(r => r.RelationType).ThenBy(r => conceptMap.GetConcept(r.SourceConceptId)?.Title))
                {
                    var source = conceptMap.GetConcept(rel.SourceConceptId)?.Title ?? rel.SourceConceptId;
                    var target = conceptMap.GetConcept(rel.TargetConceptId)?.Title ?? rel.TargetConceptId;
                    var relType = FormatRelationType(rel.RelationType);
                    sb.AppendLine($"| {source} | {relType} | {target} |");
                }
                sb.AppendLine();
            }

            // Complexity order summary
            if (conceptMap.ComplexityOrder.Count > 0)
            {
                sb.AppendLine("## Learning Path (by Complexity)");
                sb.AppendLine();
                
                var grouped = conceptMap.ComplexityOrder
                    .GroupBy(c => c.Level)
                    .OrderBy(g => g.Key);
                
                foreach (var group in grouped)
                {
                    sb.AppendLine($"### Level {group.Key}");
                    sb.AppendLine();
                    foreach (var item in group.OrderBy(i => i.PrerequisiteCount))
                    {
                        var concept = conceptMap.GetConcept(item.ConceptId);
                        if (concept != null)
                        {
                            sb.AppendLine($"- **{concept.Title}** (prereqs: {item.PrerequisiteCount}, dependents: {item.DependentCount})");
                        }
                    }
                    sb.AppendLine();
                }
            }

            await File.WriteAllTextAsync(filePath, sb.ToString(), ct);
            Log.Debug($"ConceptMapStorage: Saved human-readable export to {filePath}");
        }
        catch (Exception ex)
        {
            Log.Warn($"ConceptMapStorage: Failed to save human-readable export - {ex.Message}");
            // Don't fail the main save operation
        }
    }

    private static string GetLevelDescription(int level) => level switch
    {
        0 => "Foundational",
        1 => "Basic",
        2 => "Intermediate",
        3 => "Advanced",
        4 => "Expert",
        _ => level > 4 ? "Specialized" : "Unknown"
    };

    private static string FormatRelationType(ConceptRelationType type) => type switch
    {
        ConceptRelationType.Prerequisite => "→ requires",
        ConceptRelationType.Related => "↔ related to",
        ConceptRelationType.Contains => "⊃ contains",
        ConceptRelationType.InstanceOf => "∈ instance of",
        ConceptRelationType.SimilarTo => "≈ similar to",
        ConceptRelationType.ContrastsWith => "≠ contrasts with",
        _ => type.ToString()
    };

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return sanitized.ToLowerInvariant().Replace(' ', '-');
    }

    private string GetFilePath(string conceptMapId)
    {
        return Path.Combine(StoragePath, $"{conceptMapId}.json");
    }
}

/// <summary>
/// Summary information about a concept map.
/// </summary>
public class ConceptMapSummary
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public ConceptMapStatus Status { get; set; }
    public int ConceptCount { get; set; }
    public int RelationCount { get; set; }
    
    /// <summary>
    /// ID of the source resource (1:1 relationship).
    /// </summary>
    public string ResourceId { get; set; } = "";
    
    /// <summary>
    /// Name of the source resource for display purposes.
    /// </summary>
    public string? ResourceName { get; set; }
    
    public int Version { get; set; }
    public DateTime UpdatedAt { get; set; }
}
