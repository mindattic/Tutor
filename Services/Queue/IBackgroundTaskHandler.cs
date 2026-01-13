namespace Tutor.Services.Queue;

/// <summary>
/// Interface for background task handlers.
/// Each task type has a corresponding handler that implements this interface.
/// </summary>
public interface IBackgroundTaskHandler
{
    /// <summary>
    /// The task type this handler processes.
    /// </summary>
    BackgroundTaskType TaskType { get; }

    /// <summary>
    /// Executes the task.
    /// </summary>
    /// <param name="item">The queue item to process.</param>
    /// <param name="context">Shared context for task execution.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the task completed successfully, false if it failed.</returns>
    Task<BackgroundTaskResult> ExecuteAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct);

    /// <summary>
    /// Resumes a previously interrupted task from a checkpoint.
    /// </summary>
    /// <param name="item">The queue item to resume.</param>
    /// <param name="context">Shared context for task execution.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the task completed successfully, false if it failed.</returns>
    Task<BackgroundTaskResult> ResumeAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct);

    /// <summary>
    /// Validates that a task can be executed with its current payload.
    /// </summary>
    /// <param name="item">The queue item to validate.</param>
    /// <returns>Validation result with any error messages.</returns>
    BackgroundTaskValidation Validate(BackgroundQueueItem item);
}

/// <summary>
/// Result of a background task execution.
/// </summary>
public class BackgroundTaskResult
{
    /// <summary>
    /// Whether the task completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the task failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether the error is a rate limit error (429).
    /// </summary>
    public bool IsRateLimitError { get; set; }

    /// <summary>
    /// Suggested retry-after in seconds (from API headers).
    /// </summary>
    public int? RetryAfterSeconds { get; set; }

    /// <summary>
    /// Whether the error is transient and can be retried.
    /// </summary>
    public bool IsTransientError { get; set; }

    /// <summary>
    /// Whether the error is permanent and should not be retried.
    /// </summary>
    public bool IsPermanentError { get; set; }

    /// <summary>
    /// Updated checkpoint data for resume capability.
    /// </summary>
    public string? CheckpointJson { get; set; }

    /// <summary>
    /// Any output data from the task.
    /// </summary>
    public string? OutputJson { get; set; }

    public static BackgroundTaskResult Succeeded(string? outputJson = null) => new()
    {
        Success = true,
        OutputJson = outputJson
    };

    public static BackgroundTaskResult Failed(string errorMessage, bool isTransient = true) => new()
    {
        Success = false,
        ErrorMessage = errorMessage,
        IsTransientError = isTransient,
        IsPermanentError = !isTransient
    };

    public static BackgroundTaskResult RateLimited(int? retryAfterSeconds = null) => new()
    {
        Success = false,
        ErrorMessage = "Rate limited by API",
        IsRateLimitError = true,
        IsTransientError = true,
        RetryAfterSeconds = retryAfterSeconds
    };

    public static BackgroundTaskResult WithCheckpoint(string checkpointJson, string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage,
        IsTransientError = true,
        CheckpointJson = checkpointJson
    };
}

/// <summary>
/// Validation result for a background task.
/// </summary>
public class BackgroundTaskValidation
{
    /// <summary>
    /// Whether the task is valid and can be executed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Error messages if the task is invalid.
    /// </summary>
    public List<string> Errors { get; set; } = [];

    public static BackgroundTaskValidation Valid() => new() { IsValid = true };

    public static BackgroundTaskValidation Invalid(params string[] errors) => new()
    {
        IsValid = false,
        Errors = [.. errors]
    };
}

/// <summary>
/// Shared context for background task execution.
/// Provides access to services and cross-cutting concerns.
/// </summary>
public class BackgroundTaskContext
{
    /// <summary>
    /// The service provider for resolving dependencies.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Semaphore for throttling AI API calls.
    /// </summary>
    public SemaphoreSlim AiThrottle { get; }

    /// <summary>
    /// Callback to report progress updates.
    /// </summary>
    public Action<BackgroundQueueItem>? OnProgressUpdate { get; set; }

    /// <summary>
    /// Callback to save checkpoint data.
    /// </summary>
    public Action<BackgroundQueueItem, string>? OnCheckpoint { get; set; }

    /// <summary>
    /// Current rate limit state.
    /// </summary>
    public RateLimitState RateLimitState { get; }

    public BackgroundTaskContext(
        IServiceProvider serviceProvider,
        SemaphoreSlim aiThrottle,
        RateLimitState rateLimitState)
    {
        ServiceProvider = serviceProvider;
        AiThrottle = aiThrottle;
        RateLimitState = rateLimitState;
    }

    /// <summary>
    /// Gets a required service from the service provider.
    /// </summary>
    public T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Tries to get a service from the service provider.
    /// </summary>
    public T? TryGetService<T>() where T : class
    {
        return ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Reports progress for a task.
    /// </summary>
    public void ReportProgress(BackgroundQueueItem item, int progress, string operation)
    {
        item.Progress = progress;
        item.CurrentOperation = operation;
        item.UpdatedAt = DateTime.UtcNow;
        OnProgressUpdate?.Invoke(item);
    }

    /// <summary>
    /// Saves a checkpoint for resume capability.
    /// </summary>
    public void SaveCheckpoint(BackgroundQueueItem item, string checkpointJson)
    {
        item.CheckpointJson = checkpointJson;
        item.UpdatedAt = DateTime.UtcNow;
        OnCheckpoint?.Invoke(item, checkpointJson);
    }
}
