using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Queue;

/// <summary>
/// Static singleton service for managing background processing tasks.
/// Provides a persistent queue that survives navigation between tabs
/// and can resume after app restarts or rate limiting.
/// </summary>
public static class BackgroundQueueService
{
    private static readonly object initLock = new();
    private static bool isInitialized;
    private static IServiceProvider? serviceProvider;

    // Queue state
    private static BackgroundQueueState state = new();
    private static readonly ConcurrentDictionary<string, BackgroundQueueItem> itemLookup = new();

    // Processing infrastructure
    private static Channel<string>? processingChannel;
    private static CancellationTokenSource? cts;
    private static Task? processingLoopTask;
    private static Task? retryLoopTask;
    private static Task? persistenceLoopTask;

    // Throttling
    private static SemaphoreSlim? taskThrottle;
    private static SemaphoreSlim? aiThrottle;

    // Task handlers
    private static readonly ConcurrentDictionary<BackgroundTaskType, IBackgroundTaskHandler> handlers = new();

    // Events for UI updates
    private static event Action<BackgroundQueueItem>? statusChangedEvent;
    private static event Action<BackgroundQueueItem>? taskCompletedEvent;
    private static event Action<BackgroundQueueItem, string>? taskErrorEvent;
    private static event Action<int>? queueCountChangedEvent;

    /// <summary>
    /// Event fired when a task's status changes.
    /// </summary>
    public static event Action<BackgroundQueueItem>? OnStatusChanged
    {
        add => statusChangedEvent += value;
        remove => statusChangedEvent -= value;
    }

    /// <summary>
    /// Event fired when a task completes successfully.
    /// </summary>
    public static event Action<BackgroundQueueItem>? OnTaskCompleted
    {
        add => taskCompletedEvent += value;
        remove => taskCompletedEvent -= value;
    }

    /// <summary>
    /// Event fired when a task encounters an error.
    /// </summary>
    public static event Action<BackgroundQueueItem, string>? OnTaskError
    {
        add => taskErrorEvent += value;
        remove => taskErrorEvent -= value;
    }

    /// <summary>
    /// Event fired when the number of active tasks changes.
    /// </summary>
    public static event Action<int>? OnQueueCountChanged
    {
        add => queueCountChangedEvent += value;
        remove => queueCountChangedEvent -= value;
    }

    /// <summary>
    /// Gets whether the queue service is initialized.
    /// </summary>
    public static bool IsInitialized => isInitialized;

    /// <summary>
    /// Gets whether the queue is currently paused.
    /// </summary>
    public static bool IsPaused => state.Settings.IsPaused;

    /// <summary>
    /// Gets whether we're currently rate limited.
    /// </summary>
    public static bool IsRateLimited => state.RateLimitState.IsRateLimited && !state.RateLimitState.CanMakeApiCall();

    /// <summary>
    /// Gets the number of active (non-terminal) tasks.
    /// </summary>
    public static int ActiveCount => state.GetActiveItems().Count();

    /// <summary>
    /// Gets the number of tasks currently being processed.
    /// </summary>
    public static int InProgressCount => state.GetInProgressItems().Count();

    /// <summary>
    /// Gets the number of tasks waiting in queue.
    /// </summary>
    public static int PendingCount => state.Items.Count(i => i.Status == BackgroundTaskStatus.Pending);

    /// <summary>
    /// Initializes the background queue service.
    /// Must be called once during app startup.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider provider)
    {
        if (isInitialized) return;

        lock (initLock)
        {
            if (isInitialized) return;

            Log.Info("BackgroundQueueService: Initializing...");

            serviceProvider = provider;

            // Initialize throttles
            var settings = state.Settings;
            taskThrottle = new SemaphoreSlim(settings.MaxConcurrentTasks, settings.MaxConcurrentTasks);
            aiThrottle = new SemaphoreSlim(settings.MaxConcurrentAiCalls, settings.MaxConcurrentAiCalls);

            // Initialize processing channel
            processingChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

            cts = new CancellationTokenSource();

            isInitialized = true;
        }

        // Load persisted state
        await LoadStateAsync();

        // Register built-in handlers
        RegisterBuiltInHandlers();

        // Start background loops
        processingLoopTask = Task.Run(ProcessingLoopAsync);
        retryLoopTask = Task.Run(RetryLoopAsync);
        persistenceLoopTask = Task.Run(PersistenceLoopAsync);

        // Resume pending tasks if configured
        if (state.Settings.AutoResumeOnStartup)
        {
            await ResumePendingTasksAsync();
        }

        Log.Info($"BackgroundQueueService: Initialized with {state.Items.Count} persisted items");
    }

    /// <summary>
    /// Registers a task handler for a specific task type.
    /// </summary>
    public static void RegisterHandler(IBackgroundTaskHandler handler)
    {
        handlers[handler.TaskType] = handler;
        Log.Debug($"BackgroundQueueService: Registered handler for {handler.TaskType}");
    }

    /// <summary>
    /// Enqueues a new task for background processing.
    /// </summary>
    public static string Enqueue(BackgroundQueueItem item)
    {
        EnsureInitialized();

        item.Status = BackgroundTaskStatus.Pending;
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        state.Items.Add(item);
        itemLookup[item.Id] = item;

        // Signal the processing loop
        processingChannel!.Writer.TryWrite(item.Id);

        NotifyStatusChanged(item);
        NotifyQueueCountChanged();

        Log.Info($"BackgroundQueueService: Enqueued {item.TaskType} task '{item.DisplayName}' (ID: {item.Id})");

        return item.Id;
    }

    /// <summary>
    /// Enqueues a resource upload task.
    /// </summary>
    public static string EnqueueResourceUpload(
        string resourceId,
        string title,
        string content,
        string? courseId = null,
        bool autoFormat = true,
        bool buildGraph = true)
    {
        var payload = new ResourceUploadPayload
        {
            ResourceId = resourceId,
            Title = title,
            Content = content,
            CourseId = courseId,
            AutoFormat = autoFormat,
            BuildGraph = buildGraph
        };

        var item = new BackgroundQueueItem
        {
            TaskType = BackgroundTaskType.ResourceUpload,
            DisplayName = $"Upload: {title}",
            ResourceId = resourceId,
            CourseId = courseId,
            PayloadJson = JsonSerializer.Serialize(payload),
            Priority = 10 // High priority
        };

        return Enqueue(item);
    }

    /// <summary>
    /// Enqueues a resource format task.
    /// </summary>
    public static string EnqueueResourceFormat(string resourceId, string title)
    {
        var payload = new ResourceFormatPayload
        {
            ResourceId = resourceId,
            Title = title
        };

        var item = new BackgroundQueueItem
        {
            TaskType = BackgroundTaskType.ResourceFormat,
            DisplayName = $"Format: {title}",
            ResourceId = resourceId,
            PayloadJson = JsonSerializer.Serialize(payload),
            Priority = 50
        };

        return Enqueue(item);
    }

    /// <summary>
    /// Enqueues a concept map build task for a single resource.
    /// </summary>
    public static string EnqueueConceptMapBuildForResource(
        string resourceId,
        string resourceTitle,
        bool updateResource = true)
    {
        var payload = new ConceptMapBuildPayload
        {
            Name = $"{resourceTitle} Concept Map",
            ResourceId = resourceId,
            UpdateResource = updateResource
        };

        var item = new BackgroundQueueItem
        {
            TaskType = BackgroundTaskType.ConceptMapBuild,
            DisplayName = $"Build CM: {resourceTitle}",
            ResourceId = resourceId,
            PayloadJson = JsonSerializer.Serialize(payload),
            Priority = 100, // Lower priority than uploads/formats
            MaxRetries = 10 // CM builds can take a long time, allow more retries
        };

        return Enqueue(item);
    }

    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    public static BackgroundQueueItem? GetTask(string taskId)
    {
        return itemLookup.TryGetValue(taskId, out var item) ? item : null;
    }

    /// <summary>
    /// Gets all active (non-terminal) tasks.
    /// </summary>
    public static IReadOnlyList<BackgroundQueueItem> GetActiveTasks()
    {
        return state.GetActiveItems().ToList();
    }

    /// <summary>
    /// Gets all tasks in the queue.
    /// </summary>
    public static IReadOnlyList<BackgroundQueueItem> GetAllTasks()
    {
        return state.Items.OrderByDescending(i => i.CreatedAt).ToList();
    }

    /// <summary>
    /// Gets completed tasks for history display.
    /// </summary>
    public static IReadOnlyList<BackgroundQueueItem> GetCompletedTasks(int limit = 50)
    {
        return state.GetCompletedItems().Take(limit).ToList();
    }

    /// <summary>
    /// Cancels a pending or in-progress task.
    /// </summary>
    public static bool CancelTask(string taskId)
    {
        if (!itemLookup.TryGetValue(taskId, out var item))
            return false;

        if (item.IsTerminal)
            return false;

        item.Status = BackgroundTaskStatus.Cancelled;
        item.UpdatedAt = DateTime.UtcNow;

        NotifyStatusChanged(item);
        NotifyQueueCountChanged();

        Log.Info($"BackgroundQueueService: Cancelled task {taskId}");
        return true;
    }

    /// <summary>
    /// Retries a failed task.
    /// </summary>
    public static bool RetryTask(string taskId)
    {
        if (!itemLookup.TryGetValue(taskId, out var item))
            return false;

        if (item.Status != BackgroundTaskStatus.Failed && 
            item.Status != BackgroundTaskStatus.PermanentlyFailed)
            return false;

        item.Status = BackgroundTaskStatus.Pending;
        item.RetryCount = 0;
        item.NextRetryAt = null;
        item.LastError = null;
        item.UpdatedAt = DateTime.UtcNow;

        // Signal for processing
        processingChannel!.Writer.TryWrite(item.Id);

        NotifyStatusChanged(item);
        NotifyQueueCountChanged();

        Log.Info($"BackgroundQueueService: Retrying task {taskId}");
        return true;
    }

    /// <summary>
    /// Pauses queue processing.
    /// </summary>
    public static void Pause()
    {
        state.Settings.IsPaused = true;
        Log.Info("BackgroundQueueService: Paused");
    }

    /// <summary>
    /// Resumes queue processing.
    /// </summary>
    public static void Resume()
    {
        state.Settings.IsPaused = false;

        // Signal all pending items
        foreach (var item in state.GetReadyItems())
        {
            processingChannel!.Writer.TryWrite(item.Id);
        }

        Log.Info("BackgroundQueueService: Resumed");
    }

    /// <summary>
    /// Clears completed and cancelled tasks from history.
    /// </summary>
    public static void ClearHistory()
    {
        state.Items.RemoveAll(i => i.Status == BackgroundTaskStatus.Completed || 
                                   i.Status == BackgroundTaskStatus.Cancelled);
        
        // Rebuild lookup
        itemLookup.Clear();
        foreach (var item in state.Items)
        {
            itemLookup[item.Id] = item;
        }

        NotifyQueueCountChanged();
        Log.Info("BackgroundQueueService: Cleared history");
    }

    /// <summary>
    /// Forces a save of the queue state to disk.
    /// </summary>
    public static async Task SaveStateAsync()
    {
        if (serviceProvider == null) return;

        try
        {
            var storage = serviceProvider.GetService<BackgroundQueueStorageService>();
            if (storage != null)
            {
                state.LastSavedAt = DateTime.UtcNow;
                await storage.SaveAsync(state);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueService: Failed to save state - {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Shuts down the queue service gracefully.
    /// </summary>
    public static async Task ShutdownAsync()
    {
        if (!isInitialized) return;

        Log.Info("BackgroundQueueService: Shutting down...");

        // Cancel processing
        cts?.Cancel();

        // Wait for loops to finish
        if (processingLoopTask != null)
            await Task.WhenAny(processingLoopTask, Task.Delay(5000));

        // Save final state
        await SaveStateAsync();

        // Cleanup
        taskThrottle?.Dispose();
        aiThrottle?.Dispose();
        cts?.Dispose();

        isInitialized = false;
        Log.Info("BackgroundQueueService: Shutdown complete");
    }

    #region Private Methods

    private static void EnsureInitialized()
    {
        if (!isInitialized)
            throw new InvalidOperationException("BackgroundQueueService must be initialized before use. Call InitializeAsync first.");
    }

    private static async Task LoadStateAsync()
    {
        if (serviceProvider == null) return;

        try
        {
            var storage = serviceProvider.GetService<BackgroundQueueStorageService>();
            if (storage != null)
            {
                var loadedState = await storage.LoadAsync();
                if (loadedState != null)
                {
                    state = loadedState;
                    
                    // Rebuild lookup
                    itemLookup.Clear();
                    foreach (var item in state.Items)
                    {
                        itemLookup[item.Id] = item;
                    }

                    // Reset any in-progress items to pending (they were interrupted)
                    foreach (var item in state.Items.Where(i => i.Status == BackgroundTaskStatus.InProgress))
                    {
                        item.Status = BackgroundTaskStatus.Pending;
                        item.UpdatedAt = DateTime.UtcNow;
                    }

                    Log.Info($"BackgroundQueueService: Loaded {state.Items.Count} items from storage");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueService: Failed to load state - {ex.Message}", ex);
        }
    }

    private static void RegisterBuiltInHandlers()
    {
        if (serviceProvider == null) return;

        // Register handlers from DI container
        var handlerTypes = new[]
        {
            typeof(ResourceUploadTaskHandler),
            typeof(ResourceFormatTaskHandler),
            typeof(ConceptMapBuildTaskHandler)
        };

        foreach (var handlerType in handlerTypes)
        {
            try
            {
                if (serviceProvider.GetService(handlerType) is IBackgroundTaskHandler handler)
                {
                    RegisterHandler(handler);
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"BackgroundQueueService: Could not register handler {handlerType.Name} - {ex.Message}");
            }
        }
    }

    private static async Task ResumePendingTasksAsync()
    {
        var pendingItems = state.GetReadyItems().ToList();
        Log.Info($"BackgroundQueueService: Resuming {pendingItems.Count} pending tasks");

        foreach (var item in pendingItems)
        {
            processingChannel!.Writer.TryWrite(item.Id);
        }
    }

    private static async Task ProcessingLoopAsync()
    {
        Log.Debug("BackgroundQueueService: Processing loop started");

        try
        {
            await foreach (var taskId in processingChannel!.Reader.ReadAllAsync(cts!.Token))
            {
                if (state.Settings.IsPaused)
                {
                    // Re-queue for later
                    await Task.Delay(1000, cts.Token);
                    processingChannel.Writer.TryWrite(taskId);
                    continue;
                }

                // Check rate limit
                if (!state.RateLimitState.CanMakeApiCall())
                {
                    // Re-queue for later
                    var delay = state.RateLimitState.RetryAfter?.Subtract(DateTime.UtcNow) ?? TimeSpan.FromSeconds(30);
                    Log.Debug($"BackgroundQueueService: Rate limited, waiting {delay.TotalSeconds:F0}s");
                    await Task.Delay(delay, cts.Token);
                    processingChannel.Writer.TryWrite(taskId);
                    continue;
                }

                // Don't await - allow concurrent processing up to throttle limit
                _ = ProcessTaskAsync(taskId);
            }
        }
        catch (OperationCanceledException)
        {
            Log.Debug("BackgroundQueueService: Processing loop cancelled");
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueService: Processing loop error - {ex.Message}", ex);
        }
    }

    private static async Task ProcessTaskAsync(string taskId)
    {
        if (!itemLookup.TryGetValue(taskId, out var item))
            return;

        // Skip if already processed or cancelled
        if (item.IsTerminal || item.Status == BackgroundTaskStatus.InProgress)
            return;

        // Acquire processing slot
        await taskThrottle!.WaitAsync(cts!.Token);

        try
        {
            // Double-check status after acquiring lock
            if (item.IsTerminal || item.Status == BackgroundTaskStatus.InProgress)
                return;

            // Check if we need to wait for retry
            if (item.NextRetryAt != null && DateTime.UtcNow < item.NextRetryAt)
            {
                // Re-queue for later
                var delay = item.NextRetryAt.Value - DateTime.UtcNow;
                await Task.Delay(delay, cts.Token);
                processingChannel!.Writer.TryWrite(taskId);
                return;
            }

            // Get handler
            if (!handlers.TryGetValue(item.TaskType, out var handler))
            {
                Log.Error($"BackgroundQueueService: No handler for task type {item.TaskType}");
                item.Status = BackgroundTaskStatus.PermanentlyFailed;
                item.LastError = $"No handler registered for task type {item.TaskType}";
                NotifyStatusChanged(item);
                return;
            }

            // Validate task
            var validation = handler.Validate(item);
            if (!validation.IsValid)
            {
                Log.Error($"BackgroundQueueService: Task {taskId} validation failed: {string.Join(", ", validation.Errors)}");
                item.Status = BackgroundTaskStatus.PermanentlyFailed;
                item.LastError = string.Join("; ", validation.Errors);
                NotifyStatusChanged(item);
                return;
            }

            // Update status
            item.Status = BackgroundTaskStatus.InProgress;
            item.StartedAt ??= DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            NotifyStatusChanged(item);

            Log.Info($"BackgroundQueueService: Processing {item.TaskType} task '{item.DisplayName}'");

            // Create context
            var context = new BackgroundTaskContext(serviceProvider!, aiThrottle!, state.RateLimitState)
            {
                OnProgressUpdate = NotifyStatusChanged,
                OnCheckpoint = (i, c) =>
                {
                    i.CheckpointJson = c;
                    i.UpdatedAt = DateTime.UtcNow;
                }
            };

            // Execute or resume
            BackgroundTaskResult result;
            if (!string.IsNullOrEmpty(item.CheckpointJson))
            {
                result = await handler.ResumeAsync(item, context, cts.Token);
            }
            else
            {
                result = await handler.ExecuteAsync(item, context, cts.Token);
            }

            // Handle result
            await HandleTaskResultAsync(item, result);
        }
        catch (OperationCanceledException)
        {
            item.Status = BackgroundTaskStatus.Pending;
            item.UpdatedAt = DateTime.UtcNow;
            Log.Debug($"BackgroundQueueService: Task {taskId} cancelled");
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueService: Task {taskId} threw exception - {ex.Message}", ex);
            await HandleTaskResultAsync(item, BackgroundTaskResult.Failed(ex.Message));
        }
        finally
        {
            taskThrottle.Release();
        }
    }

    private static async Task HandleTaskResultAsync(BackgroundQueueItem item, BackgroundTaskResult result)
    {
        item.UpdatedAt = DateTime.UtcNow;

        if (result.Success)
        {
            item.Status = BackgroundTaskStatus.Completed;
            item.Progress = 100;
            item.CompletedAt = DateTime.UtcNow;
            item.CheckpointJson = null; // Clear checkpoint on success

            state.RateLimitState.RecordSuccess();

            NotifyStatusChanged(item);
            NotifyTaskCompleted(item);
            NotifyQueueCountChanged();

            Log.Info($"BackgroundQueueService: Task '{item.DisplayName}' completed successfully");
        }
        else if (result.IsRateLimitError)
        {
            state.RateLimitState.RecordRateLimitError(result.RetryAfterSeconds);

            item.Status = BackgroundTaskStatus.RateLimited;
            item.RetryCount++;
            item.NextRetryAt = state.RateLimitState.RetryAfter;
            item.LastError = result.ErrorMessage;
            item.ErrorHistory.Add($"[{DateTime.UtcNow:HH:mm:ss}] {result.ErrorMessage}");

            if (!string.IsNullOrEmpty(result.CheckpointJson))
            {
                item.CheckpointJson = result.CheckpointJson;
            }

            NotifyStatusChanged(item);
            NotifyTaskError(item, result.ErrorMessage ?? "Rate limited");

            Log.Warn($"BackgroundQueueService: Task '{item.DisplayName}' rate limited, retry after {item.NextRetryAt}");

            // Re-queue for retry
            processingChannel!.Writer.TryWrite(item.Id);
        }
        else if (result.IsTransientError && item.CanRetry)
        {
            item.Status = BackgroundTaskStatus.Failed;
            item.RetryCount++;
            item.LastError = result.ErrorMessage;
            item.ErrorHistory.Add($"[{DateTime.UtcNow:HH:mm:ss}] {result.ErrorMessage}");

            // Exponential backoff
            var delay = Math.Min(
                state.Settings.BaseRetryDelayMs * Math.Pow(2, item.RetryCount - 1),
                state.Settings.MaxRetryDelayMs);
            item.NextRetryAt = DateTime.UtcNow.AddMilliseconds(delay);

            if (!string.IsNullOrEmpty(result.CheckpointJson))
            {
                item.CheckpointJson = result.CheckpointJson;
            }

            NotifyStatusChanged(item);
            NotifyTaskError(item, result.ErrorMessage ?? "Failed");

            Log.Warn($"BackgroundQueueService: Task '{item.DisplayName}' failed (attempt {item.RetryCount}/{item.MaxRetries}), retry at {item.NextRetryAt}");

            // Re-queue for retry
            processingChannel!.Writer.TryWrite(item.Id);
        }
        else
        {
            item.Status = BackgroundTaskStatus.PermanentlyFailed;
            item.LastError = result.ErrorMessage;
            item.ErrorHistory.Add($"[{DateTime.UtcNow:HH:mm:ss}] PERMANENT: {result.ErrorMessage}");

            NotifyStatusChanged(item);
            NotifyTaskError(item, result.ErrorMessage ?? "Permanently failed");
            NotifyQueueCountChanged();

            Log.Error($"BackgroundQueueService: Task '{item.DisplayName}' permanently failed: {result.ErrorMessage}");
        }
    }

    private static async Task RetryLoopAsync()
    {
        Log.Debug("BackgroundQueueService: Retry loop started");

        try
        {
            while (!cts!.Token.IsCancellationRequested)
            {
                await Task.Delay(5000, cts.Token); // Check every 5 seconds

                // Find items ready for retry
                var now = DateTime.UtcNow;
                var readyForRetry = state.Items
                    .Where(i => (i.Status == BackgroundTaskStatus.Failed || i.Status == BackgroundTaskStatus.RateLimited)
                                && i.CanRetry
                                && i.NextRetryAt != null
                                && now >= i.NextRetryAt)
                    .ToList();

                foreach (var item in readyForRetry)
                {
                    item.Status = BackgroundTaskStatus.Pending;
                    processingChannel!.Writer.TryWrite(item.Id);
                    Log.Debug($"BackgroundQueueService: Re-queued task '{item.DisplayName}' for retry");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log.Debug("BackgroundQueueService: Retry loop cancelled");
        }
    }

    private static async Task PersistenceLoopAsync()
    {
        Log.Debug("BackgroundQueueService: Persistence loop started");

        try
        {
            while (!cts!.Token.IsCancellationRequested)
            {
                await Task.Delay(state.Settings.StateSaveIntervalMs, cts.Token);
                await SaveStateAsync();
            }
        }
        catch (OperationCanceledException)
        {
            Log.Debug("BackgroundQueueService: Persistence loop cancelled");
        }
    }

    private static void NotifyStatusChanged(BackgroundQueueItem item)
    {
        try
        {
            statusChangedEvent?.Invoke(item);
        }
        catch (Exception ex)
        {
            Log.Warn($"BackgroundQueueService: Error in status changed handler - {ex.Message}");
        }
    }

    private static void NotifyTaskCompleted(BackgroundQueueItem item)
    {
        try
        {
            taskCompletedEvent?.Invoke(item);
        }
        catch (Exception ex)
        {
            Log.Warn($"BackgroundQueueService: Error in task completed handler - {ex.Message}");
        }
    }

    private static void NotifyTaskError(BackgroundQueueItem item, string error)
    {
        try
        {
            taskErrorEvent?.Invoke(item, error);
        }
        catch (Exception ex)
        {
            Log.Warn($"BackgroundQueueService: Error in task error handler - {ex.Message}");
        }
    }

    private static void NotifyQueueCountChanged()
    {
        try
        {
            queueCountChangedEvent?.Invoke(ActiveCount);
        }
        catch (Exception ex)
        {
            Log.Warn($"BackgroundQueueService: Error in queue count handler - {ex.Message}");
        }
    }

    #endregion
}
