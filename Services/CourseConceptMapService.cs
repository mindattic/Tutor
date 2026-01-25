using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for managing course-specific merged ConceptMaps.
/// 
/// Each course has its own MergedCourseConceptMap that is:
/// - Built by COPYING concepts from resource ConceptMaps (not referencing them)
/// - Deduplicated independently (original resource maps are never modified)
/// - Stored separately per course
/// 
/// This allows resources to be shared across courses while each course
/// maintains its own deduplicated concept graph.
/// </summary>
public sealed class CourseConceptMapService
{
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly CourseService courseService;
    private readonly ConceptMergeService mergeService;

    private static readonly string MergedMapsDirectory = Path.Combine(
        DataStorageSettings.GetKnowledgeBasesDirectory(), "merged");

    public CourseConceptMapService(
        ConceptMapStorageService conceptMapStorage,
        CourseService courseService,
        ConceptMergeService mergeService)
    {
        this.conceptMapStorage = conceptMapStorage;
        this.courseService = courseService;
        this.mergeService = mergeService;

        // Ensure directory exists
        Directory.CreateDirectory(MergedMapsDirectory);
    }

    /// <summary>
    /// Gets or creates the merged ConceptMap for a course.
    /// If it doesn't exist or is stale, builds it from the resource ConceptMaps.
    /// </summary>
    public async Task<MergedCourseConceptMap?> GetOrCreateMergedMapAsync(string courseId)
    {
        // Try to load existing
        var existing = await LoadMergedMapAsync(courseId);
        
        if (existing != null && existing.Status == MergedMapStatus.Ready)
        {
            // Check if it needs rebuild (source maps changed)
            var needsRebuild = await CheckNeedsRebuildAsync(courseId, existing);
            if (!needsRebuild)
            {
                return existing;
            }
            
            Log.Info($"[CourseConceptMapService] Merged map for course {courseId} needs rebuild");
        }

        // Build a new merged map
        return await BuildMergedMapAsync(courseId);
    }

    /// <summary>
    /// Builds the merged ConceptMap for a course from its resource ConceptMaps.
    /// This creates COPIES of all concepts - the originals are never modified.
    /// </summary>
    public async Task<MergedCourseConceptMap?> BuildMergedMapAsync(string courseId)
    {
        Log.Info($"[CourseConceptMapService] Building merged map for course {courseId}");

        var conceptMapIds = await courseService.GetResourceConceptMapIdsAsync(courseId);
        if (conceptMapIds.Count == 0)
        {
            Log.Debug($"[CourseConceptMapService] No ConceptMaps found for course {courseId}");
            return null;
        }

        // Load all source ConceptMaps
        var sourceMaps = new List<ConceptMap>();
        foreach (var cmId in conceptMapIds)
        {
            var cm = await conceptMapStorage.LoadAsync(cmId);
            if (cm != null && cm.Status == ConceptMapStatus.Ready)
            {
                sourceMaps.Add(cm);
            }
        }

        if (sourceMaps.Count == 0)
        {
            Log.Debug($"[CourseConceptMapService] No ready ConceptMaps found for course {courseId}");
            return null;
        }

        // Create the merged map with DEEP COPIES of all concepts
        var mergedMap = new MergedCourseConceptMap
        {
            Id = $"merged-{courseId}",
            CourseId = courseId,
            Status = MergedMapStatus.Building
        };

        mergedMap.InitializeFromConceptMaps(sourceMaps);

        Log.Info($"[CourseConceptMapService] Created merged map with {mergedMap.TotalConcepts} concepts from {sourceMaps.Count} sources");

        // Auto-merge exact duplicates
        var mergeResult = mergeService.AutoMergeExactDuplicatesInMergedMap(mergedMap);
        if (mergeResult.MergedCount > 0)
        {
            Log.Info($"[CourseConceptMapService] Auto-merged {mergeResult.MergedCount} exact duplicates");
        }

        mergedMap.Status = MergedMapStatus.Ready;

        // Save the merged map
        await SaveMergedMapAsync(mergedMap);

        return mergedMap;
    }

    /// <summary>
    /// Rebuilds the merged map for a course (forces rebuild even if current is valid).
    /// </summary>
    public async Task<MergedCourseConceptMap?> RebuildMergedMapAsync(string courseId)
    {
        // Delete existing
        await DeleteMergedMapAsync(courseId);
        
        // Build fresh
        return await BuildMergedMapAsync(courseId);
    }

    /// <summary>
    /// Loads the merged ConceptMap for a course.
    /// </summary>
    public async Task<MergedCourseConceptMap?> LoadMergedMapAsync(string courseId)
    {
        var filePath = GetMergedMapPath(courseId);
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<MergedCourseConceptMap>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Log.Warn($"[CourseConceptMapService] Failed to load merged map for course {courseId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Saves the merged ConceptMap for a course.
    /// </summary>
    public async Task SaveMergedMapAsync(MergedCourseConceptMap mergedMap)
    {
        var filePath = GetMergedMapPath(mergedMap.CourseId);
        
        try
        {
            var json = JsonSerializer.Serialize(mergedMap, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filePath, json);
            
            Log.Debug($"[CourseConceptMapService] Saved merged map for course {mergedMap.CourseId}");
        }
        catch (Exception ex)
        {
            Log.Error($"[CourseConceptMapService] Failed to save merged map: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deletes the merged ConceptMap for a course.
    /// </summary>
    public async Task DeleteMergedMapAsync(string courseId)
    {
        var filePath = GetMergedMapPath(courseId);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Log.Debug($"[CourseConceptMapService] Deleted merged map for course {courseId}");
            }
            catch (Exception ex)
            {
                Log.Warn($"[CourseConceptMapService] Failed to delete merged map: {ex.Message}");
            }
        }
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Marks the merged map as needing rebuild (e.g., when resources change).
    /// </summary>
    public async Task InvalidateMergedMapAsync(string courseId)
    {
        var mergedMap = await LoadMergedMapAsync(courseId);
        if (mergedMap != null)
        {
            mergedMap.Status = MergedMapStatus.NeedsRebuild;
            await SaveMergedMapAsync(mergedMap);
        }
    }

    /// <summary>
    /// Finds duplicates in the course's merged map.
    /// </summary>
    public async Task<DuplicateDetectionResult?> FindDuplicatesAsync(string courseId)
    {
        var mergedMap = await GetOrCreateMergedMapAsync(courseId);
        if (mergedMap == null)
            return null;

        return mergeService.FindDuplicatesInMergedMap(mergedMap);
    }

    /// <summary>
    /// Merges approved concept pairs in the course's merged map.
    /// This only affects the merged map, NOT the original resource ConceptMaps.
    /// </summary>
    public async Task<MergeResult> MergeApprovedPairsAsync(string courseId, List<ApprovedMerge> approvedMerges)
    {
        var mergedMap = await GetOrCreateMergedMapAsync(courseId);
        if (mergedMap == null)
        {
            return new MergeResult { ErrorMessage = "No merged map found for course" };
        }

        var result = mergeService.MergeInMergedMap(mergedMap, approvedMerges);
        
        // Save the updated merged map
        await SaveMergedMapAsync(mergedMap);

        return result;
    }

    /// <summary>
    /// Checks if the merged map needs to be rebuilt because source maps changed.
    /// </summary>
    private async Task<bool> CheckNeedsRebuildAsync(string courseId, MergedCourseConceptMap existing)
    {
        var currentMapIds = await courseService.GetResourceConceptMapIdsAsync(courseId);
        
        // Check if the resource set changed
        var existingSet = existing.SourceConceptMapIds.ToHashSet();
        var currentSet = currentMapIds.ToHashSet();

        if (!existingSet.SetEquals(currentSet))
        {
            return true;
        }

        // Could also check if any source map's UpdatedAt is newer than merged map's UpdatedAt
        // For now, just check the resource set

        return existing.Status == MergedMapStatus.NeedsRebuild;
    }

    private string GetMergedMapPath(string courseId)
    {
        return Path.Combine(MergedMapsDirectory, $"{courseId}.json");
    }
}
