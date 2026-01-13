using System.Text.Json.Serialization;

namespace Tutor.Services.Queue;

/// <summary>
/// Types of background tasks that can be queued.
/// </summary>
public enum BackgroundTaskType
{
    /// <summary>
    /// Upload and save a resource.
    /// </summary>
    ResourceUpload,

    /// <summary>
    /// Format a resource's content using AI.
    /// </summary>
    ResourceFormat,

    /// <summary>
    /// Generate a knowledge base from resources.
    /// </summary>
    KnowledgeBaseBuild,

    /// <summary>
    /// Extract concepts from a resource.
    /// </summary>
    ConceptExtraction,

    /// <summary>
    /// Build knowledge graph relationships.
    /// </summary>
    GraphBuild
}

/// <summary>
/// Status of a queued background task.
/// </summary>
public enum BackgroundTaskStatus
{
    /// <summary>
    /// Task is waiting in queue.
    /// </summary>
    Pending,

    /// <summary>
    /// Task is currently being processed.
    /// </summary>
    InProgress,

    /// <summary>
    /// Task completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Task failed but can be retried.
    /// </summary>
    Failed,

    /// <summary>
    /// Task was cancelled.
    /// </summary>
    Cancelled,

    /// <summary>
    /// Task is paused waiting for rate limit to clear.
    /// </summary>
    RateLimited,

    /// <summary>
    /// Task exceeded max retries and permanently failed.
    /// </summary>
    PermanentlyFailed
}

/// <summary>
/// Represents a background task item in the processing queue.
/// Designed to be serializable for persistence and resume capability.
/// </summary>
public class BackgroundQueueItem
{
    /// <summary>
    /// Unique identifier for this queue item.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Type of background task.
    /// </summary>
    public BackgroundTaskType TaskType { get; set; }

    /// <summary>
    /// Current status of the task.
    /// </summary>
    public BackgroundTaskStatus Status { get; set; } = BackgroundTaskStatus.Pending;

    /// <summary>
    /// Priority level (lower = higher priority).
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// When the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the task was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the task started processing (null if not started).
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When the task completed (null if not complete).
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// Current operation description.
    /// </summary>
    public string CurrentOperation { get; set; } = "";

    /// <summary>
    /// Number of retry attempts so far.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Maximum number of retry attempts allowed.
    /// </summary>
    public int MaxRetries { get; set; } = 5;

    /// <summary>
    /// When to attempt the next retry (for rate limiting).
    /// </summary>
    public DateTime? NextRetryAt { get; set; }

    /// <summary>
    /// Last error message encountered.
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// All error messages encountered during processing.
    /// </summary>
    public List<string> ErrorHistory { get; set; } = [];

    /// <summary>
    /// JSON-serialized task-specific payload.
    /// </summary>
    public string PayloadJson { get; set; } = "{}";

    /// <summary>
    /// JSON-serialized checkpoint data for resume capability.
    /// </summary>
    public string? CheckpointJson { get; set; }

    /// <summary>
    /// Display-friendly name for the task.
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// Associated resource ID (if applicable).
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// Associated course ID (if applicable).
    /// </summary>
    public string? CourseId { get; set; }

    /// <summary>
    /// Associated knowledge base ID (if applicable).
    /// </summary>
    public string? KnowledgeBaseId { get; set; }

    /// <summary>
    /// Gets the status display name.
    /// </summary>
    [JsonIgnore]
    public string StatusDisplayName => Status switch
    {
        BackgroundTaskStatus.Pending => "Pending",
        BackgroundTaskStatus.InProgress => "Processing",
        BackgroundTaskStatus.Completed => "Completed",
        BackgroundTaskStatus.Failed => $"Failed (retry {RetryCount}/{MaxRetries})",
        BackgroundTaskStatus.Cancelled => "Cancelled",
        BackgroundTaskStatus.RateLimited => "Rate Limited",
        BackgroundTaskStatus.PermanentlyFailed => "Failed",
        _ => "Unknown"
    };

    /// <summary>
    /// Indicates if the task can be retried.
    /// </summary>
    [JsonIgnore]
    public bool CanRetry => Status == BackgroundTaskStatus.Failed && RetryCount < MaxRetries;

    /// <summary>
    /// Indicates if the task is in a terminal state.
    /// </summary>
    [JsonIgnore]
    public bool IsTerminal => Status is BackgroundTaskStatus.Completed
        or BackgroundTaskStatus.Cancelled
        or BackgroundTaskStatus.PermanentlyFailed;

    /// <summary>
    /// Indicates if the task is ready to be processed.
    /// </summary>
    [JsonIgnore]
    public bool IsReady => Status == BackgroundTaskStatus.Pending
        || (Status == BackgroundTaskStatus.Failed && CanRetry && (NextRetryAt == null || DateTime.UtcNow >= NextRetryAt))
        || (Status == BackgroundTaskStatus.RateLimited && NextRetryAt != null && DateTime.UtcNow >= NextRetryAt);
}

/// <summary>
/// Payload for resource upload tasks.
/// </summary>
public class ResourceUploadPayload
{
    public string ResourceId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? CourseId { get; set; }
    public string ResourceType { get; set; } = "Text";
    public string? FileName { get; set; }
    public string? Author { get; set; }
    public string? Year { get; set; }
    public string? Description { get; set; }
    public bool AutoFormat { get; set; } = true;
    public bool BuildGraph { get; set; } = true;
}

/// <summary>
/// Payload for resource format tasks.
/// </summary>
public class ResourceFormatPayload
{
    public string ResourceId { get; set; } = "";
    public string Title { get; set; } = "";
}

/// <summary>
/// Payload for knowledge base build tasks.
/// Supports building a KnowledgeBase from a single Resource (new architecture).
/// </summary>
public class KnowledgeBaseBuildPayload
{
    /// <summary>
    /// ID of the KnowledgeBase being built (for resumption).
    /// </summary>
    public string KnowledgeBaseId { get; set; } = "";

    /// <summary>
    /// Name for the KnowledgeBase.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// ID of the single Resource to build KB from (new architecture).
    /// When set, ResourceIds is ignored.
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// IDs of Resources to build KB from.
    /// Maintained for backward compatibility. New code should use ResourceId.
    /// </summary>
    [Obsolete("Use ResourceId instead for single resource KB builds.")]
    public List<string> ResourceIds { get; set; } = [];

    /// <summary>
    /// Whether to update the CourseResource with the KnowledgeBase ID after build.
    /// </summary>
    public bool UpdateResource { get; set; } = true;

    /// <summary>
    /// Gets the effective resource ID (prefers single ResourceId over list).
    /// </summary>
    public string? GetEffectiveResourceId()
    {
        if (!string.IsNullOrEmpty(ResourceId))
            return ResourceId;

#pragma warning disable CS0618
        return ResourceIds.FirstOrDefault();
#pragma warning restore CS0618
    }
}

/// <summary>
/// Checkpoint data for resumable knowledge base builds.
/// </summary>
public class KnowledgeBaseBuildCheckpoint
{
    /// <summary>
    /// Current build stage.
    /// </summary>
    public string CurrentStage { get; set; } = "";

    /// <summary>
    /// Number of chunks processed so far.
    /// </summary>
    public int ChunksProcessed { get; set; }

    /// <summary>
    /// Total number of chunks to process.
    /// </summary>
    public int TotalChunks { get; set; }

    /// <summary>
    /// IDs of concepts extracted so far.
    /// </summary>
    public List<string> ExtractedConceptIds { get; set; } = [];

    /// <summary>
    /// Whether combined content has been saved.
    /// </summary>
    public bool ContentCombined { get; set; }

    /// <summary>
    /// Whether concepts have been extracted.
    /// </summary>
    public bool ConceptsExtracted { get; set; }

    /// <summary>
    /// Whether relationships have been built.
    /// </summary>
    public bool RelationshipsBuilt { get; set; }

    /// <summary>
    /// Whether complexity has been calculated.
    /// </summary>
    public bool ComplexityCalculated { get; set; }
}
