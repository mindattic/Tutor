using System.Collections.Concurrent;
using System.Threading.Channels;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Background service for processing resources asynchronously.
/// Handles formatting, concept extraction, and knowledge graph building
/// with throttling to prevent memory issues.
/// </summary>
public sealed class ResourceProcessingService : IDisposable
{
    private readonly CourseService courseService;
    private readonly ContentFormatterService formatterService;
    private readonly KnowledgeGraphService graphService;
    private readonly ConceptExtractionService extractionService;

    // Throttling - limit concurrent AI operations
    private readonly SemaphoreSlim aiThrottle = new(2, 2); // Max 2 concurrent AI calls
    private readonly SemaphoreSlim processingThrottle = new(3, 3); // Max 3 resources processing at once

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
        ConceptExtractionService extractionService)
    {
        this.courseService = courseService;
        this.formatterService = formatterService;
        this.graphService = graphService;
        this.extractionService = extractionService;

        // Unbounded channel for queuing
        processingQueue = Channel.CreateUnbounded<ResourceProcessingTask>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });

        // Start the background processing loop
        processingLoopTask = Task.Run(ProcessingLoopAsync);
        Log.Info("ResourceProcessingService: Background processing loop started");
    }

    /// <summary>
    /// Queues a resource for async processing (format ? extract ? build graph).
    /// Returns immediately - processing happens in background.
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
        // Acquire processing slot
        await processingThrottle.WaitAsync(cts.Token);
        
        Log.Info($"ResourceProcessing: Starting '{task.Resource.Title}' (Task: {task.TaskId})");

        try
        {
            var status = statusTracker[task.TaskId];

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

            // Stage 2: Format with AI (if enabled)
            if (task.AutoFormat)
            {
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
            }

            // Stage 3: Extract concepts (if building graph)
            if (task.BuildGraph && !string.IsNullOrEmpty(task.CourseId))
            {
                Log.Debug($"ResourceProcessing: Stage 3 - Extracting concepts from '{task.Resource.Title}'");
                UpdateStatus(task.TaskId, ProcessingStage.ExtractingConcepts, 50, "Extracting concepts...");

                await aiThrottle.WaitAsync(cts.Token);
                try
                {
                    await ExtractConceptsAsync(task, status);
                    Log.Info($"ResourceProcessing: Concept extraction complete for '{task.Resource.Title}'");
                }
                finally
                {
                    aiThrottle.Release();
                }

                // Stage 4: Update knowledge graph
                Log.Debug($"ResourceProcessing: Stage 4 - Building graph for '{task.Resource.Title}'");
                UpdateStatus(task.TaskId, ProcessingStage.BuildingGraph, 90, "Updating knowledge graph...");
                await UpdateKnowledgeGraphAsync(task, status);
            }

            // Complete
            UpdateStatus(task.TaskId, ProcessingStage.Complete, 100, "Complete");
            status.CompletedAt = DateTime.UtcNow;
            Log.Info($"ResourceProcessing: Completed '{task.Resource.Title}' in {(status.CompletedAt - status.QueuedAt)?.TotalSeconds:F1}s");
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

    private async Task ExtractConceptsAsync(ResourceProcessingTask task, ResourceProcessingStatus status)
    {
        try
        {
            var content = task.Resource.FormattedContent ?? task.Resource.Content;
            var concepts = await extractionService.ExtractConceptsAsync(content, task.Resource.Id, cts.Token);
            status.ConceptsExtracted = concepts.Count;
            
            UpdateStatus(task.TaskId, ProcessingStage.ExtractingConcepts, 65, 
                $"Extracted {concepts.Count} concepts");
        }
        catch (Exception ex)
        {
            status.Warnings.Add($"Concept extraction warning: {ex.Message}");
        }
    }

    private async Task UpdateKnowledgeGraphAsync(ResourceProcessingTask task, ResourceProcessingStatus status)
    {
        if (string.IsNullOrEmpty(task.CourseId))
            return;

        try
        {
            // Check if graph exists for this course
            var existingGraph = await graphService.GetGraphForCourseAsync(task.CourseId, cts.Token);

            if (existingGraph != null)
            {
                // Add resource to existing graph
                var newConceptCount = await graphService.AddResourceToGraphAsync(
                    existingGraph.Id, 
                    task.Resource, 
                    cts.Token);
                
                status.ConceptsExtracted = newConceptCount;
            }
            else
            {
                // Create new graph with just this resource
                var resources = new List<CourseResource> { task.Resource };
                var course = await courseService.GetCourseAsync(task.CourseId);
                
                await graphService.CreateGraphFromResourcesAsync(
                    course?.Name ?? "Knowledge Graph",
                    task.CourseId,
                    resources,
                    new Progress<GraphBuildProgress>(p =>
                    {
                        UpdateStatus(task.TaskId, ProcessingStage.BuildingGraph, 
                            75 + (int)(p.PercentComplete * 0.15), p.Message);
                    }),
                    cts.Token);
            }
        }
        catch (Exception ex)
        {
            status.Warnings.Add($"Graph building warning: {ex.Message}");
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
        cts.Cancel();
        processingQueue.Writer.Complete();
        aiThrottle.Dispose();
        processingThrottle.Dispose();
        cts.Dispose();
    }
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
    ExtractingConcepts,
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
    public DateTime LastUpdated { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int ConceptsExtracted { get; set; }
    public List<string> Warnings { get; set; } = [];

    public string StageDisplayName => Stage switch
    {
        ProcessingStage.Queued => "Queued",
        ProcessingStage.Saving => "Saving",
        ProcessingStage.Formatting => "Formatting",
        ProcessingStage.ExtractingConcepts => "Extracting Concepts",
        ProcessingStage.BuildingGraph => "Building Graph",
        ProcessingStage.Complete => "Complete",
        ProcessingStage.Failed => "Failed",
        _ => "Unknown"
    };
}
