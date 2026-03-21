using System.Collections.Concurrent;
using System.Threading.Channels;
using Tutor.Core.Models;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;

namespace Tutor.Core.Services;

/// <summary>
/// Background service for processing resources asynchronously.
/// Handles formatting, concept extraction, and knowledge graph building
/// with throttling to prevent memory issues.
/// 
/// NOTE: This service now delegates to BackgroundQueueService for persistent,
/// resumable processing. The legacy methods are preserved for backward compatibility.
/// </summary>
public sealed class ResourceProcessingService : IDisposable
{
    private readonly CourseService courseService;
    private readonly ContentFormatterService formatterService;
    private readonly KnowledgeGraphService graphService;
    private readonly ConceptExtractionService extractionService;
    private readonly ConceptMapService conceptMapService;
    private readonly ConceptMapStorageService conceptMapStorageService;
    private readonly SettingsService settingsService;

    // Throttling - process ONE resource at a time sequentially to avoid API spam
    private readonly SemaphoreSlim aiThrottle = new(1, 1); // Max 1 concurrent AI call
    private readonly SemaphoreSlim processingThrottle = new(1, 1); // Max 1 resource processing at once (sequential)

    // Processing queue
    private readonly Channel<ResourceProcessingTask> processingQueue;
    private readonly ConcurrentDictionary<string, ResourceProcessingStatus> statusTracker = new();
    private readonly CancellationTokenSource cts = new();
    private readonly Task processingLoopTask;

    // Events for UI updates
    public event Action<ResourceProcessingStatus>? OnStatusChanged;
    public event Action<string>? OnProcessingComplete;
    public event Action<string, string>? OnProcessingError;

    public ResourceProcessingService(
        CourseService courseService,
        ContentFormatterService formatterService,
        KnowledgeGraphService graphService,
        ConceptExtractionService extractionService,
        ConceptMapService conceptMapService,
        ConceptMapStorageService conceptMapStorageService,
        SettingsService settingsService)
    {
        this.courseService = courseService;
        this.formatterService = formatterService;
        this.graphService = graphService;
        this.extractionService = extractionService;
        this.conceptMapService = conceptMapService;
        this.conceptMapStorageService = conceptMapStorageService;
        this.settingsService = settingsService;

        // Unbounded channel for queuing
        processingQueue = Channel.CreateUnbounded<ResourceProcessingTask>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });

        // Start the background processing loop
        processingLoopTask = Task.Run(ProcessingLoopAsync);
        Log.Info("ResourceProcessingService: Background processing loop started");
        
        // Subscribe to BackgroundQueueService events for bridging
        BackgroundQueueService.OnStatusChanged += OnBackgroundQueueStatusChanged;
        BackgroundQueueService.OnTaskCompleted += OnBackgroundQueueTaskCompleted;
        BackgroundQueueService.OnTaskError += OnBackgroundQueueTaskError;
    }

    #region New BackgroundQueueService Integration

    /// <summary>
    /// Queues a resource for processing using the new persistent background queue.
    /// This method supports resume capability and rate limit handling.
    /// </summary>
    public string QueueResourceForPersistentProcessing(
        CourseResource resource,
        string? courseId,
        bool autoFormat = true,
        bool buildGraph = true)
    {
        Log.Info($"ResourceProcessing: Queuing '{resource.Title}' via persistent queue (format={autoFormat}, graph={buildGraph})");

        // Use the static BackgroundQueueService for persistent processing
        var taskId = BackgroundQueueService.EnqueueResourceUpload(
            resource.Id,
            resource.Title,
            resource.Content,
            courseId,
            autoFormat,
            buildGraph);

        return taskId;
    }

    /// <summary>
    /// Gets all active tasks from the persistent background queue.
    /// </summary>
    public IReadOnlyList<BackgroundQueueItem> GetPersistentQueueTasks()
    {
        return BackgroundQueueService.GetActiveTasks();
    }

    /// <summary>
    /// Gets the count of active tasks in the persistent queue.
    /// </summary>
    public int GetPersistentQueueCount()
    {
        return BackgroundQueueService.ActiveCount;
    }

    /// <summary>
    /// Checks if the persistent queue is rate limited.
    /// </summary>
    public bool IsPersistentQueueRateLimited()
    {
        return BackgroundQueueService.IsRateLimited;
    }

    private void OnBackgroundQueueStatusChanged(BackgroundQueueItem item)
    {
        // Bridge to legacy status tracking if needed
        if (!string.IsNullOrEmpty(item.ResourceId))
        {
            var legacyStatus = new ResourceProcessingStatus
            {
                TaskId = item.Id,
                ResourceId = item.ResourceId,
                ResourceTitle = item.DisplayName,
                CourseId = item.CourseId,
                Stage = MapToLegacyStage(item.Status),
                Progress = item.Progress,
                CurrentOperation = item.CurrentOperation,
                QueuedAt = item.CreatedAt,
                LastUpdated = item.UpdatedAt
            };

            statusTracker[item.Id] = legacyStatus;
            NotifyStatusChanged(legacyStatus);
        }
    }

    private void OnBackgroundQueueTaskCompleted(BackgroundQueueItem item)
    {
        if (!string.IsNullOrEmpty(item.ResourceId))
        {
            OnProcessingComplete?.Invoke(item.Id);
        }
    }

    private void OnBackgroundQueueTaskError(BackgroundQueueItem item, string error)
    {
        if (!string.IsNullOrEmpty(item.ResourceId))
        {
            OnProcessingError?.Invoke(item.Id, error);
        }
    }

    private static ProcessingStage MapToLegacyStage(BackgroundTaskStatus status)
    {
        return status switch
        {
            BackgroundTaskStatus.Pending => ProcessingStage.Queued,
            BackgroundTaskStatus.InProgress => ProcessingStage.Formatting,
            BackgroundTaskStatus.Completed => ProcessingStage.Complete,
            BackgroundTaskStatus.Failed => ProcessingStage.Failed,
            BackgroundTaskStatus.Cancelled => ProcessingStage.Failed,
            BackgroundTaskStatus.RateLimited => ProcessingStage.Queued,
            BackgroundTaskStatus.PermanentlyFailed => ProcessingStage.Failed,
            _ => ProcessingStage.Queued
        };
    }

    #endregion

    #region Legacy Implementation (preserved for backward compatibility)

    /// <summary>
    /// Queues a resource for async processing (format ? extract ? build graph).
    /// Returns immediately - processing happens in background.
    /// 
    /// NOTE: Consider using QueueResourceForPersistentProcessing for better
    /// resume capability and rate limit handling.
    /// </summary>
    public string QueueResourceForProcessing(
        CourseResource resource,
        string? courseId,
        bool autoFormat = true,
        bool buildGraph = true)
    {
        var taskId = Guid.NewGuid().ToString();
        
        Log.Info($"ResourceProcessing: Queuing '{resource.Title}' (format={autoFormat}, graph={buildGraph})");

        var status = new ResourceProcessingStatus
        {
            TaskId = taskId,
            ResourceId = resource.Id,
            ResourceTitle = resource.Title,
            CourseId = courseId,
            Stage = ProcessingStage.Queued,
            QueuedAt = DateTime.UtcNow
        };

        statusTracker[taskId] = status;
        NotifyStatusChanged(status);

        var task = new ResourceProcessingTask
        {
            TaskId = taskId,
            Resource = resource,
            CourseId = courseId,
            AutoFormat = autoFormat,
            BuildGraph = buildGraph
        };

        // Queue for processing
        processingQueue.Writer.TryWrite(task);
        Log.Debug($"ResourceProcessing: Task {taskId} queued for '{resource.Title}'");

        return taskId;
    }



    /// <summary>
    /// Gets the current status of a processing task.
    /// </summary>
    public ResourceProcessingStatus? GetStatus(string taskId)
    {
        return statusTracker.TryGetValue(taskId, out var status) ? status : null;
    }

    /// <summary>
    /// Gets all active processing statuses.
    /// </summary>
    public IEnumerable<ResourceProcessingStatus> GetAllStatuses()
    {
        return statusTracker.Values
            .Where(s => s.Stage != ProcessingStage.Complete && s.Stage != ProcessingStage.Failed)
            .OrderBy(s => s.QueuedAt);
    }

    /// <summary>
    /// Gets all processing statuses including completed ones (for time display).
    /// </summary>
    public IEnumerable<ResourceProcessingStatus> GetAllStatusesIncludingCompleted()
    {
        return statusTracker.Values.OrderBy(s => s.QueuedAt);
    }

    /// <summary>
    /// Gets the cumulative processing time across all active and recent tasks.
    /// </summary>
    public TimeSpan GetCumulativeProcessingTime()
    {
        return statusTracker.Values
            .Where(s => s.StartedAt.HasValue)
            .Aggregate(TimeSpan.Zero, (total, s) => total + s.ElapsedTime);
    }

    /// <summary>
    /// Gets the formatted cumulative processing time (HH:MM:SS).
    /// </summary>
    public string GetCumulativeProcessingTimeFormatted()
    {
        return GetCumulativeProcessingTime().ToString(@"hh\:mm\:ss");
    }

    /// <summary>
    /// Gets count of items in queue or processing.
    /// </summary>
    public int GetActiveCount()
    {
        return statusTracker.Values.Count(s => 
            s.Stage != ProcessingStage.Complete && s.Stage != ProcessingStage.Failed);
    }

    private async Task ProcessingLoopAsync()
    {
        await foreach (var task in processingQueue.Reader.ReadAllAsync(cts.Token))
        {
            // Don't await - let multiple tasks process concurrently (up to throttle limit)
            _ = ProcessResourceAsync(task);
        }
    }

    private async Task ProcessResourceAsync(ResourceProcessingTask task)
    {
        // Acquire processing slot - ensures sequential processing
        await processingThrottle.WaitAsync(cts.Token);
        
        Log.Info($"ResourceProcessing: Starting '{task.Resource.Title}' (Task: {task.TaskId})");

        try
        {
            var status = statusTracker[task.TaskId];
            status.StartedAt = DateTime.UtcNow;

            // Stage 1: Save resource
            Log.Debug($"ResourceProcessing: Stage 1 - Saving resource '{task.Resource.Title}'");
            UpdateStatus(task.TaskId, ProcessingStage.Saving, 0, "Saving resource...");
            
            var savedId = await courseService.SaveResourceCoreAsync(task.Resource);
            task.Resource.Id = savedId;
            status.ResourceId = savedId;
            Log.Debug($"ResourceProcessing: Resource saved with ID {savedId}");

            // Add to course if specified
            if (!string.IsNullOrEmpty(task.CourseId))
            {
                await courseService.AddResourceToCourseAsync(task.CourseId, savedId);
                Log.Debug($"ResourceProcessing: Resource added to course {task.CourseId}");
            }

            // Stage 2: Format with AI (always enabled now)
            Log.Debug($"ResourceProcessing: Stage 2 - Formatting '{task.Resource.Title}'");
            UpdateStatus(task.TaskId, ProcessingStage.Formatting, 10, "Formatting with AI...");

            await aiThrottle.WaitAsync(cts.Token);
            try
            {
                await FormatResourceAsync(task, status);
                Log.Info($"ResourceProcessing: Formatting complete for '{task.Resource.Title}'");
            }
            finally
            {
                aiThrottle.Release();
            }
            
            // Small delay between AI calls to avoid rate limiting
            await Task.Delay(500, cts.Token);

            // Stage 3: Build Concept Map (always enabled now)
            Log.Debug($"ResourceProcessing: Stage 3 - Building Concept Map for '{task.Resource.Title}'");
            UpdateStatus(task.TaskId, ProcessingStage.BuildingGraph, 50, "Building Concept Map...");

            await aiThrottle.WaitAsync(cts.Token);
            try
            {
                await BuildKnowledgeBaseAsync(task, status);
                Log.Info($"ResourceProcessing: Concept Map complete for '{task.Resource.Title}'");
            }
            finally
            {
                aiThrottle.Release();
            }

            // Mark resource as fully processed
            task.Resource.IsProcessed = true;
            await courseService.SaveResourceAsync(task.Resource);

            // Complete
            UpdateStatus(task.TaskId, ProcessingStage.Complete, 100, "Complete");
            status.CompletedAt = DateTime.UtcNow;
            Log.Info($"ResourceProcessing: Completed '{task.Resource.Title}' in {status.ElapsedTimeFormatted}");
            OnProcessingComplete?.Invoke(task.TaskId);
        }
        catch (OperationCanceledException)
        {
            Log.Warn($"ResourceProcessing: Cancelled '{task.Resource.Title}'");
            UpdateStatus(task.TaskId, ProcessingStage.Failed, 0, "Cancelled");
        }
        catch (Exception ex)
        {
            Log.Error($"ResourceProcessing: Failed '{task.Resource.Title}' - {ex.Message}", ex);
            UpdateStatus(task.TaskId, ProcessingStage.Failed, 0, $"Error: {ex.Message}");
            OnProcessingError?.Invoke(task.TaskId, ex.Message);
        }
        finally
        {
            processingThrottle.Release();
        }
    }

    private async Task FormatResourceAsync(ResourceProcessingTask task, ResourceProcessingStatus status)
    {
        try
        {
            var content = task.Resource.Content;
            
            UpdateStatus(task.TaskId, ProcessingStage.Formatting, 15, "Formatting content with AI...");

            var formattedContent = await formatterService.FormatContentAsync(
                content, 
                (current, total) =>
                {
                    var progress = 10 + (int)((double)current / total * 35);
                    UpdateStatus(task.TaskId, ProcessingStage.Formatting, progress, 
                        $"Formatting section {current} of {total}...");
                },
                cts.Token);

            // Save formatted version
            task.Resource.FormattedContent = formattedContent;
            await courseService.UpdateResourceContentAsync(task.Resource.Id, formattedContent, isFormatted: true);
        }
        catch (Exception ex)
        {
            // Log but don't fail - formatting is optional
            status.Warnings.Add($"Formatting warning: {ex.Message}");
        }
    }

    private async Task BuildKnowledgeBaseAsync(ResourceProcessingTask task, ResourceProcessingStatus status)
    {
        try
        {
            UpdateStatus(task.TaskId, ProcessingStage.BuildingGraph, 55, "Building Concept Map...");

            // Get global settings and apply resource overrides
            var globalMaxIterations = await settingsService.GetOrphanLinkingMaxIterationsAsync();
            var globalMinConfidence = await settingsService.GetOrphanLinkingMinConfidenceAsync();
            
            var effectiveMaxIterations = task.Resource.GetEffectiveMaxIterations(globalMaxIterations);
            var effectiveMinConfidence = task.Resource.GetEffectiveMinConfidence(globalMinConfidence);

            // Subscribe to progress updates from the CM service
            void OnProgress(ConceptMapBuildProgress progress)
            {
                var adjustedProgress = 50 + (int)(progress.Progress * 0.45);
                UpdateStatus(task.TaskId, ProcessingStage.BuildingGraph, adjustedProgress, progress.Message);
            }


            conceptMapService.OnProgressChanged += OnProgress;
            try
            {
                // Build the ConceptMap for this resource with settings
                var cm = await conceptMapService.BuildFromResourceAsync(
                    task.Resource, 
                    effectiveMaxIterations, 
                    effectiveMinConfidence, 
                    cts.Token);

                // Link the ConceptMap to the resource
                task.Resource.ConceptMapId = cm.Id;
                task.Resource.ConceptMapStatus = cm.Status;
                status.ConceptsExtracted = cm.Concepts.Count;

                UpdateStatus(task.TaskId, ProcessingStage.BuildingGraph, 95, 
                    $"Concept Map ready: {cm.Concepts.Count} concepts, {cm.Relations.Count} relationships");
            }
            finally
            {
                conceptMapService.OnProgressChanged -= OnProgress;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Concept Map build failed: {ex.Message}", ex);
            status.Warnings.Add($"Concept Map warning: {ex.Message}");
            throw; // Re-throw to fail the resource processing
        }
    }

    private void UpdateStatus(string taskId, ProcessingStage stage, int progress, string message)
    {
        if (statusTracker.TryGetValue(taskId, out var status))
        {
            status.Stage = stage;
            status.Progress = progress;
            status.CurrentOperation = message;
            status.LastUpdated = DateTime.UtcNow;
            NotifyStatusChanged(status);
        }
    }

    private void NotifyStatusChanged(ResourceProcessingStatus status)
    {
        try
        {
            OnStatusChanged?.Invoke(status);
        }
        catch
        {
            // Ignore UI callback errors
        }
    }

    /// <summary>
    /// Clears completed/failed tasks from the tracker.
    /// </summary>
    public void ClearCompletedTasks()
    {
        var toRemove = statusTracker
            .Where(kvp => kvp.Value.Stage == ProcessingStage.Complete || kvp.Value.Stage == ProcessingStage.Failed)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in toRemove)
        {
            statusTracker.TryRemove(key, out _);
        }
    }

    public void Dispose()
    {
        // Unsubscribe from BackgroundQueueService events
        BackgroundQueueService.OnStatusChanged -= OnBackgroundQueueStatusChanged;
        BackgroundQueueService.OnTaskCompleted -= OnBackgroundQueueTaskCompleted;
        BackgroundQueueService.OnTaskError -= OnBackgroundQueueTaskError;
        
        cts.Cancel();
        processingQueue.Writer.Complete();
        aiThrottle.Dispose();
        processingThrottle.Dispose();
        cts.Dispose();
    }

    #endregion
}

/// <summary>
/// Internal task representation for the processing queue.
/// </summary>
internal class ResourceProcessingTask
{
    public string TaskId { get; set; } = "";
    public CourseResource Resource { get; set; } = new();
    public string? CourseId { get; set; }
    public bool AutoFormat { get; set; } = true;
    public bool BuildGraph { get; set; } = true;
}

/// <summary>
/// Processing stages for a resource.
/// </summary>
public enum ProcessingStage
{
    Queued,
    Saving,
    Formatting,
    BuildingGraph,
    Complete,
    Failed
}

/// <summary>
/// Status tracking for a resource being processed.
/// </summary>
public class ResourceProcessingStatus
{
    public string TaskId { get; set; } = "";
    public string ResourceId { get; set; } = "";
    public string ResourceTitle { get; set; } = "";
    public string? CourseId { get; set; }
    public ProcessingStage Stage { get; set; } = ProcessingStage.Queued;
    public int Progress { get; set; }
    public string CurrentOperation { get; set; } = "";
    public DateTime QueuedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int ConceptsExtracted { get; set; }
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Gets the elapsed processing time for this resource.
    /// </summary>
    public TimeSpan ElapsedTime
    {
        get
        {
            if (StartedAt == null) return TimeSpan.Zero;
            var endTime = CompletedAt ?? DateTime.UtcNow;
            return endTime - StartedAt.Value;
        }
    }

    /// <summary>
    /// Gets the formatted elapsed time string (HH:MM:SS).
    /// </summary>
    public string ElapsedTimeFormatted => ElapsedTime.ToString(@"hh\:mm\:ss");

    public string StageDisplayName => Stage switch
    {
        ProcessingStage.Queued => "Queued",
        ProcessingStage.Saving => "Saving",
        ProcessingStage.Formatting => "AI Formatting",
        ProcessingStage.BuildingGraph => "Building Concept Map",
        ProcessingStage.Complete => "Complete",
        ProcessingStage.Failed => "Failed",
        _ => "Unknown"
    };
}
