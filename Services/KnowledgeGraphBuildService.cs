using System.Collections.Concurrent;
using System.Threading.Channels;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Background service for building knowledge graphs asynchronously.
/// Supports queuing multiple builds and provides progress updates.
/// Persists across navigation/tab changes.
/// </summary>
public sealed class KnowledgeGraphBuildService : IDisposable
{
    private readonly CourseService courseService;
    private readonly KnowledgeGraphService graphService;

    // Throttling - only one graph build at a time (they're expensive)
    private readonly SemaphoreSlim buildThrottle = new(1, 1);

    // Build queue
    private readonly Channel<GraphBuildTask> buildQueue;
    private readonly ConcurrentDictionary<string, GraphBuildStatus> statusTracker = new();
    private readonly CancellationTokenSource cts = new();
    private readonly Task processingLoopTask;

    // Events for UI updates
    public event Action<GraphBuildStatus>? OnStatusChanged;
    public event Action<string>? OnBuildComplete;
    public event Action<string, string>? OnBuildError;

    public KnowledgeGraphBuildService(
        CourseService courseService,
        KnowledgeGraphService graphService)
    {
        this.courseService = courseService;
        this.graphService = graphService;

        // Unbounded channel for queuing builds
        buildQueue = Channel.CreateUnbounded<GraphBuildTask>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        // Start the background processing loop
        processingLoopTask = Task.Run(ProcessingLoopAsync);
    }

    /// <summary>
    /// Queues a knowledge graph build for a course.
    /// Returns immediately - building happens in background.
    /// </summary>
    public string QueueGraphBuild(string courseId, string courseName, bool rebuild = false)
    {
        // Check if already building this course
        var existingBuild = statusTracker.Values.FirstOrDefault(s => 
            s.CourseId == courseId && 
            s.Stage != GraphBuildStage.Complete && 
            s.Stage != GraphBuildStage.Failed);

        if (existingBuild != null)
        {
            return existingBuild.TaskId; // Return existing task
        }

        var taskId = Guid.NewGuid().ToString();

        var status = new GraphBuildStatus
        {
            TaskId = taskId,
            CourseId = courseId,
            CourseName = courseName,
            Stage = GraphBuildStage.Queued,
            QueuedAt = DateTime.UtcNow,
            IsRebuild = rebuild
        };

        statusTracker[taskId] = status;
        NotifyStatusChanged(status);

        var task = new GraphBuildTask
        {
            TaskId = taskId,
            CourseId = courseId,
            CourseName = courseName,
            Rebuild = rebuild
        };

        buildQueue.Writer.TryWrite(task);

        return taskId;
    }

    /// <summary>
    /// Gets the current status of a build task.
    /// </summary>
    public GraphBuildStatus? GetStatus(string taskId)
    {
        return statusTracker.TryGetValue(taskId, out var status) ? status : null;
    }

    /// <summary>
    /// Gets build status for a specific course.
    /// </summary>
    public GraphBuildStatus? GetStatusForCourse(string courseId)
    {
        return statusTracker.Values.FirstOrDefault(s => 
            s.CourseId == courseId && 
            s.Stage != GraphBuildStage.Complete && 
            s.Stage != GraphBuildStage.Failed);
    }

    /// <summary>
    /// Gets all active build statuses.
    /// </summary>
    public IEnumerable<GraphBuildStatus> GetAllStatuses()
    {
        return statusTracker.Values
            .Where(s => s.Stage != GraphBuildStage.Complete && s.Stage != GraphBuildStage.Failed)
            .OrderBy(s => s.QueuedAt);
    }

    /// <summary>
    /// Gets all statuses including completed (for recent history).
    /// </summary>
    public IEnumerable<GraphBuildStatus> GetRecentStatuses(int maxAge = 300) // 5 minutes
    {
        var cutoff = DateTime.UtcNow.AddSeconds(-maxAge);
        return statusTracker.Values
            .Where(s => s.QueuedAt > cutoff || (s.Stage != GraphBuildStage.Complete && s.Stage != GraphBuildStage.Failed))
            .OrderByDescending(s => s.QueuedAt);
    }

    /// <summary>
    /// Gets count of items in queue or building.
    /// </summary>
    public int GetActiveCount()
    {
        return statusTracker.Values.Count(s => 
            s.Stage != GraphBuildStage.Complete && s.Stage != GraphBuildStage.Failed);
    }

    /// <summary>
    /// Checks if a course has a build in progress or queued.
    /// </summary>
    public bool IsBuildingCourse(string courseId)
    {
        return statusTracker.Values.Any(s => 
            s.CourseId == courseId && 
            s.Stage != GraphBuildStage.Complete && 
            s.Stage != GraphBuildStage.Failed);
    }

    private async Task ProcessingLoopAsync()
    {
        await foreach (var task in buildQueue.Reader.ReadAllAsync(cts.Token))
        {
            await BuildGraphAsync(task);
        }
    }

    private async Task BuildGraphAsync(GraphBuildTask task)
    {
        await buildThrottle.WaitAsync(cts.Token);

        try
        {
            var status = statusTracker[task.TaskId];

            // Stage 1: Load course and resources
            UpdateStatus(task.TaskId, GraphBuildStage.LoadingResources, 5, "Loading course resources...");
            
            var course = await courseService.GetCourseAsync(task.CourseId);
            if (course == null)
            {
                throw new InvalidOperationException($"Course {task.CourseId} not found");
            }

            var resources = await courseService.GetCourseResourcesAsync(task.CourseId);
            if (resources.Count == 0)
            {
                throw new InvalidOperationException("Course has no resources to analyze");
            }

            status.TotalResources = resources.Count;
            UpdateStatus(task.TaskId, GraphBuildStage.LoadingResources, 10, 
                $"Found {resources.Count} resources");

            // Stage 2: Check for existing graph (if not rebuilding)
            if (!task.Rebuild)
            {
                var existingGraph = await graphService.GetGraphForCourseAsync(task.CourseId, cts.Token);
                if (existingGraph != null)
                {
                    // Graph exists - update status and complete
                    status.ConceptsExtracted = existingGraph.Nodes.Count;
                    status.RelationshipsFound = existingGraph.Relationships.Count;
                    UpdateStatus(task.TaskId, GraphBuildStage.Complete, 100, 
                        $"Graph already exists: {existingGraph.Nodes.Count} concepts");
                    status.CompletedAt = DateTime.UtcNow;
                    OnBuildComplete?.Invoke(task.TaskId);
                    return;
                }
            }

            // Stage 3: Build the graph
            UpdateStatus(task.TaskId, GraphBuildStage.ExtractingConcepts, 15, "Extracting concepts...");

            var progress = new Progress<GraphBuildProgress>(p =>
            {
                var stage = p.Message.Contains("relationship", StringComparison.OrdinalIgnoreCase) 
                    ? GraphBuildStage.FindingRelationships
                    : p.Message.Contains("embedding", StringComparison.OrdinalIgnoreCase)
                        ? GraphBuildStage.GeneratingEmbeddings
                        : GraphBuildStage.ExtractingConcepts;

                var baseProgress = stage switch
                {
                    GraphBuildStage.ExtractingConcepts => 15,
                    GraphBuildStage.GeneratingEmbeddings => 50,
                    GraphBuildStage.FindingRelationships => 70,
                    _ => 15
                };

                var stageProgress = baseProgress + (int)(p.PercentComplete * 0.2);
                
                status.CurrentResource = p.Detail ?? "";
                UpdateStatus(task.TaskId, stage, Math.Min(stageProgress, 90), p.Message);
            });

            var graph = await graphService.CreateGraphFromResourcesAsync(
                task.CourseName,
                task.CourseId,
                resources,
                progress,
                cts.Token);

            status.ConceptsExtracted = graph.Nodes.Count;
            status.RelationshipsFound = graph.Relationships.Count;

            // Complete
            UpdateStatus(task.TaskId, GraphBuildStage.Complete, 100, 
                $"Complete: {graph.Nodes.Count} concepts, {graph.Relationships.Count} relationships");
            status.CompletedAt = DateTime.UtcNow;
            OnBuildComplete?.Invoke(task.TaskId);
        }
        catch (OperationCanceledException)
        {
            UpdateStatus(task.TaskId, GraphBuildStage.Failed, 0, "Cancelled");
        }
        catch (Exception ex)
        {
            UpdateStatus(task.TaskId, GraphBuildStage.Failed, 0, $"Error: {ex.Message}");
            OnBuildError?.Invoke(task.TaskId, ex.Message);
        }
        finally
        {
            buildThrottle.Release();
        }
    }

    private void UpdateStatus(string taskId, GraphBuildStage stage, int progress, string message)
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

    private void NotifyStatusChanged(GraphBuildStatus status)
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
            .Where(kvp => kvp.Value.Stage == GraphBuildStage.Complete || kvp.Value.Stage == GraphBuildStage.Failed)
            .Where(kvp => kvp.Value.CompletedAt.HasValue && kvp.Value.CompletedAt.Value < DateTime.UtcNow.AddMinutes(-5))
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
        buildQueue.Writer.Complete();
        buildThrottle.Dispose();
        cts.Dispose();
    }
}

/// <summary>
/// Internal task representation for the build queue.
/// </summary>
internal class GraphBuildTask
{
    public string TaskId { get; set; } = "";
    public string CourseId { get; set; } = "";
    public string CourseName { get; set; } = "";
    public bool Rebuild { get; set; }
}

/// <summary>
/// Build stages for a knowledge graph.
/// </summary>
public enum GraphBuildStage
{
    Queued,
    LoadingResources,
    ExtractingConcepts,
    GeneratingEmbeddings,
    FindingRelationships,
    BuildingGraph,
    Complete,
    Failed
}

/// <summary>
/// Status tracking for a graph build.
/// </summary>
public class GraphBuildStatus
{
    public string TaskId { get; set; } = "";
    public string CourseId { get; set; } = "";
    public string CourseName { get; set; } = "";
    public GraphBuildStage Stage { get; set; } = GraphBuildStage.Queued;
    public int Progress { get; set; }
    public string CurrentOperation { get; set; } = "";
    public string CurrentResource { get; set; } = "";
    public DateTime QueuedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalResources { get; set; }
    public int ConceptsExtracted { get; set; }
    public int RelationshipsFound { get; set; }
    public bool IsRebuild { get; set; }

    public string StageDisplayName => Stage switch
    {
        GraphBuildStage.Queued => "Queued",
        GraphBuildStage.LoadingResources => "Loading Resources",
        GraphBuildStage.ExtractingConcepts => "Extracting Concepts",
        GraphBuildStage.GeneratingEmbeddings => "Generating Embeddings",
        GraphBuildStage.FindingRelationships => "Finding Relationships",
        GraphBuildStage.BuildingGraph => "Building Graph",
        GraphBuildStage.Complete => "Complete",
        GraphBuildStage.Failed => "Failed",
        _ => "Unknown"
    };

    public bool IsActive => Stage != GraphBuildStage.Complete && Stage != GraphBuildStage.Failed;
}
