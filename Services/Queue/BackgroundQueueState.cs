namespace Tutor.Services.Queue;

/// <summary>
/// Represents the persistent state of the background queue.
/// This is serialized to disk for resume capability across app restarts.
/// </summary>
public class BackgroundQueueState
{
    /// <summary>
    /// Version of the queue state format (for future migrations).
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// When the queue state was last saved.
    /// </summary>
    public DateTime LastSavedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// All queue items (pending, in-progress, failed, completed).
    /// </summary>
    public List<BackgroundQueueItem> Items { get; set; } = [];

    /// <summary>
    /// Settings for the queue.
    /// </summary>
    public BackgroundQueueSettings Settings { get; set; } = new();

    /// <summary>
    /// Global rate limit tracking.
    /// </summary>
    public RateLimitState RateLimitState { get; set; } = new();

    /// <summary>
    /// Gets items that are pending or ready to retry.
    /// </summary>
    public IEnumerable<BackgroundQueueItem> GetReadyItems()
    {
        var now = DateTime.UtcNow;
        return Items
            .Where(i => i.IsReady)
            .OrderBy(i => i.Priority)
            .ThenBy(i => i.CreatedAt);
    }

    /// <summary>
    /// Gets items currently in progress.
    /// </summary>
    public IEnumerable<BackgroundQueueItem> GetInProgressItems()
    {
        return Items.Where(i => i.Status == BackgroundTaskStatus.InProgress);
    }

    /// <summary>
    /// Gets items that are rate limited but will be ready soon.
    /// </summary>
    public IEnumerable<BackgroundQueueItem> GetRateLimitedItems()
    {
        return Items.Where(i => i.Status == BackgroundTaskStatus.RateLimited);
    }

    /// <summary>
    /// Gets completed items (for history display).
    /// </summary>
    public IEnumerable<BackgroundQueueItem> GetCompletedItems()
    {
        return Items.Where(i => i.Status == BackgroundTaskStatus.Completed)
            .OrderByDescending(i => i.CompletedAt);
    }

    /// <summary>
    /// Gets failed items that cannot be retried.
    /// </summary>
    public IEnumerable<BackgroundQueueItem> GetPermanentlyFailedItems()
    {
        return Items.Where(i => i.Status == BackgroundTaskStatus.PermanentlyFailed)
            .OrderByDescending(i => i.UpdatedAt);
    }

    /// <summary>
    /// Gets all active (non-terminal) items.
    /// </summary>
    public IEnumerable<BackgroundQueueItem> GetActiveItems()
    {
        return Items.Where(i => !i.IsTerminal)
            .OrderBy(i => i.Priority)
            .ThenBy(i => i.CreatedAt);
    }

    /// <summary>
    /// Removes old completed and failed items to prevent unbounded growth.
    /// </summary>
    public void PruneHistory(int maxCompletedItems = 50, int maxFailedItems = 20, TimeSpan? maxAge = null)
    {
        maxAge ??= TimeSpan.FromDays(7);
        var cutoff = DateTime.UtcNow - maxAge.Value;

        // Remove old completed items
        var completedToRemove = Items
            .Where(i => i.Status == BackgroundTaskStatus.Completed)
            .OrderByDescending(i => i.CompletedAt)
            .Skip(maxCompletedItems)
            .Union(Items.Where(i => i.Status == BackgroundTaskStatus.Completed && i.CompletedAt < cutoff))
            .Select(i => i.Id)
            .ToHashSet();

        // Remove old failed items
        var failedToRemove = Items
            .Where(i => i.Status == BackgroundTaskStatus.PermanentlyFailed)
            .OrderByDescending(i => i.UpdatedAt)
            .Skip(maxFailedItems)
            .Union(Items.Where(i => i.Status == BackgroundTaskStatus.PermanentlyFailed && i.UpdatedAt < cutoff))
            .Select(i => i.Id)
            .ToHashSet();

        Items.RemoveAll(i => completedToRemove.Contains(i.Id) || failedToRemove.Contains(i.Id));
    }
}

/// <summary>
/// Settings for the background queue.
/// </summary>
public class BackgroundQueueSettings
{
    /// <summary>
    /// Maximum concurrent tasks for general processing.
    /// </summary>
    public int MaxConcurrentTasks { get; set; } = 3;

    /// <summary>
    /// Maximum concurrent AI API calls.
    /// </summary>
    public int MaxConcurrentAiCalls { get; set; } = 2;

    /// <summary>
    /// Default max retries for failed tasks.
    /// </summary>
    public int DefaultMaxRetries { get; set; } = 5;

    /// <summary>
    /// Base delay in milliseconds for exponential backoff.
    /// </summary>
    public int BaseRetryDelayMs { get; set; } = 2000;

    /// <summary>
    /// Maximum delay in milliseconds for exponential backoff.
    /// </summary>
    public int MaxRetryDelayMs { get; set; } = 300000; // 5 minutes

    /// <summary>
    /// Delay in milliseconds when rate limited (429 response).
    /// </summary>
    public int RateLimitDelayMs { get; set; } = 60000; // 1 minute

    /// <summary>
    /// How often to save queue state (in milliseconds).
    /// </summary>
    public int StateSaveIntervalMs { get; set; } = 5000;

    /// <summary>
    /// Whether to auto-resume pending tasks on startup.
    /// </summary>
    public bool AutoResumeOnStartup { get; set; } = true;

    /// <summary>
    /// Whether the queue is currently paused.
    /// </summary>
    public bool IsPaused { get; set; } = false;
}

/// <summary>
/// Tracks global rate limit state for OpenAI API.
/// </summary>
public class RateLimitState
{
    /// <summary>
    /// Whether we're currently in a rate-limited state.
    /// </summary>
    public bool IsRateLimited { get; set; }

    /// <summary>
    /// When the rate limit was detected.
    /// </summary>
    public DateTime? RateLimitedAt { get; set; }

    /// <summary>
    /// When we can retry after rate limiting.
    /// </summary>
    public DateTime? RetryAfter { get; set; }

    /// <summary>
    /// Number of consecutive rate limit errors.
    /// </summary>
    public int ConsecutiveRateLimitErrors { get; set; }

    /// <summary>
    /// Remaining requests per minute (from API headers).
    /// </summary>
    public int? RemainingRequestsPerMinute { get; set; }

    /// <summary>
    /// Remaining tokens per minute (from API headers).
    /// </summary>
    public int? RemainingTokensPerMinute { get; set; }

    /// <summary>
    /// Checks if we can make API calls now.
    /// </summary>
    public bool CanMakeApiCall()
    {
        if (!IsRateLimited) return true;
        if (RetryAfter == null) return true;
        return DateTime.UtcNow >= RetryAfter;
    }

    /// <summary>
    /// Records a rate limit error and calculates backoff.
    /// </summary>
    public void RecordRateLimitError(int? retryAfterSeconds = null)
    {
        IsRateLimited = true;
        RateLimitedAt = DateTime.UtcNow;
        ConsecutiveRateLimitErrors++;

        // Use provided retry-after or calculate exponential backoff
        var backoffSeconds = retryAfterSeconds
            ?? (int)Math.Min(60 * Math.Pow(2, ConsecutiveRateLimitErrors - 1), 300); // Max 5 minutes

        RetryAfter = DateTime.UtcNow.AddSeconds(backoffSeconds);
    }

    /// <summary>
    /// Records a successful API call, resetting rate limit state.
    /// </summary>
    public void RecordSuccess()
    {
        IsRateLimited = false;
        ConsecutiveRateLimitErrors = 0;
        RetryAfter = null;
    }
}
