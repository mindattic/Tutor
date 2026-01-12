using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Service for managing user learning progress across courses.
/// Persists progress to local storage.
/// </summary>
public sealed class UserProgressService
{
    private const string ProgressKeyPrefix = "USER_PROGRESS_";
    private const string DefaultUserId = "default";

    private readonly Dictionary<string, UserProgress> cachedProgress = [];

    /// <summary>
    /// Event fired when progress is updated.
    /// </summary>
    public event Action<string>? OnProgressUpdated;

    /// <summary>
    /// Gets or creates progress for a course.
    /// </summary>
    public async Task<UserProgress> GetProgressAsync(string courseId, string? userId = null)
    {
        userId ??= DefaultUserId;
        var key = GetCacheKey(courseId, userId);

        if (cachedProgress.TryGetValue(key, out var cached))
            return cached;

        try
        {
            var storageKey = GetStorageKey(courseId, userId);
            var json = await SecureStorage.GetAsync(storageKey);

            if (!string.IsNullOrEmpty(json))
            {
                var progress = JsonSerializer.Deserialize<UserProgress>(json);
                if (progress != null)
                {
                    cachedProgress[key] = progress;
                    return progress;
                }
            }
        }
        catch
        {
            // If loading fails, create new progress
        }

        // Create new progress
        var newProgress = new UserProgress
        {
            UserId = userId,
            CourseId = courseId
        };

        cachedProgress[key] = newProgress;
        await SaveProgressAsync(newProgress);

        return newProgress;
    }

    /// <summary>
    /// Saves progress to storage.
    /// </summary>
    public async Task SaveProgressAsync(UserProgress progress)
    {
        var key = GetCacheKey(progress.CourseId, progress.UserId);
        cachedProgress[key] = progress;

        try
        {
            var storageKey = GetStorageKey(progress.CourseId, progress.UserId);
            var json = JsonSerializer.Serialize(progress);
            await SecureStorage.SetAsync(storageKey, json);
            
            OnProgressUpdated?.Invoke(progress.CourseId);
        }
        catch
        {
            // Storage failure - progress will be in memory only
        }
    }

    /// <summary>
    /// Marks a concept as learned and saves progress.
    /// </summary>
    public async Task MarkConceptLearnedAsync(string courseId, string conceptId, string? userId = null)
    {
        var progress = await GetProgressAsync(courseId, userId);
        progress.MarkConceptLearned(conceptId);
        await SaveProgressAsync(progress);
    }

    /// <summary>
    /// Marks a concept as visited and saves progress.
    /// </summary>
    public async Task MarkConceptVisitedAsync(string courseId, string conceptId, string? userId = null)
    {
        var progress = await GetProgressAsync(courseId, userId);
        progress.MarkConceptVisited(conceptId);
        await SaveProgressAsync(progress);
    }

    /// <summary>
    /// Updates the user's current position and saves progress.
    /// </summary>
    public async Task SetPositionAsync(string courseId, string? chapterId, string? conceptId, string? userId = null)
    {
        var progress = await GetProgressAsync(courseId, userId);
        progress.SetPosition(chapterId, conceptId);
        await SaveProgressAsync(progress);
    }

    /// <summary>
    /// Gets the learned concept IDs for a course.
    /// </summary>
    public async Task<List<string>> GetLearnedConceptsAsync(string courseId, string? userId = null)
    {
        var progress = await GetProgressAsync(courseId, userId);
        return progress.LearnedConceptIds.ToList();
    }

    /// <summary>
    /// Clears all progress for a course.
    /// </summary>
    public async Task ClearProgressAsync(string courseId, string? userId = null)
    {
        userId ??= DefaultUserId;
        var key = GetCacheKey(courseId, userId);
        
        cachedProgress.Remove(key);

        try
        {
            var storageKey = GetStorageKey(courseId, userId);
            SecureStorage.Remove(storageKey);
        }
        catch
        {
            // Ignore storage errors
        }

        OnProgressUpdated?.Invoke(courseId);
    }

    /// <summary>
    /// Gets progress for all courses the user has started.
    /// </summary>
    public async Task<List<UserProgress>> GetAllProgressAsync(string? userId = null)
    {
        // Return cached progress for the user
        userId ??= DefaultUserId;
        return cachedProgress.Values
            .Where(p => p.UserId == userId)
            .ToList();
    }

    private static string GetCacheKey(string courseId, string userId)
    {
        return $"{userId}_{courseId}";
    }

    private static string GetStorageKey(string courseId, string userId)
    {
        return $"{ProgressKeyPrefix}{userId}_{courseId}";
    }
}
