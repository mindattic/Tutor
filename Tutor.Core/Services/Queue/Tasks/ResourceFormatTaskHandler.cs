using System.Net;
using System.Text.Json;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Queue;

/// <summary>
/// Checkpoint data for resumable formatting.
/// Stores progress so formatting can resume after failures, rate limits, or app restarts.
/// </summary>
public class FormatCheckpoint
{
    /// <summary>
    /// The original content chunks to process.
    /// </summary>
    public List<string> Chunks { get; set; } = [];
    
    /// <summary>
    /// Successfully formatted chunks (in order).
    /// </summary>
    public List<string> CompletedChunks { get; set; } = [];
    
    /// <summary>
    /// Total number of chunks to process.
    /// </summary>
    public int TotalChunks { get; set; }
    
    /// <summary>
    /// Index of the last successfully completed chunk.
    /// </summary>
    public int LastCompletedIndex { get; set; } = -1;
    
    /// <summary>
    /// When this checkpoint was last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Handler for resource formatting tasks.
/// Uses AI to format resource content with retry, rate limit handling, and checkpoint/resume support.
/// Saves progress after each chunk so work is never lost on failure.
/// </summary>
public sealed class ResourceFormatTaskHandler : IBackgroundTaskHandler
{
    public BackgroundTaskType TaskType => BackgroundTaskType.ResourceFormat;

    public async Task<BackgroundTaskResult> ExecuteAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        return await FormatWithCheckpointAsync(item, context, null, ct);
    }

    public async Task<BackgroundTaskResult> ResumeAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        FormatCheckpoint? checkpoint = null;

        if (!string.IsNullOrEmpty(item.CheckpointJson))
        {
            try
            {
                checkpoint = JsonSerializer.Deserialize<FormatCheckpoint>(item.CheckpointJson);
                if (checkpoint != null && checkpoint.CompletedChunks.Count > 0)
                {
                    Log.Info($"ResourceFormatTask: Resuming from checkpoint - " +
                        $"Completed {checkpoint.CompletedChunks.Count}/{checkpoint.TotalChunks} chunks " +
                        $"(last saved: {checkpoint.LastUpdatedAt:HH:mm:ss})");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"ResourceFormatTask: Could not parse checkpoint, starting fresh - {ex.Message}");
            }
        }

        return await FormatWithCheckpointAsync(item, context, checkpoint, ct);
    }

    private async Task<BackgroundTaskResult> FormatWithCheckpointAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        FormatCheckpoint? checkpoint,
        CancellationToken ct)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<ResourceFormatPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskResult.Failed("Invalid payload", isTransient: false);
            }

            context.ReportProgress(item, 5, "Loading resource...");

            var courseService = context.GetService<CourseService>();
            var formatterService = context.GetService<ContentFormatterService>();

            // Load the resource
            var resource = await courseService.GetResourceAsync(payload.ResourceId);
            if (resource == null)
            {
                return BackgroundTaskResult.Failed($"Resource {payload.ResourceId} not found", isTransient: false);
            }

            // Skip if already fully formatted (and no checkpoint means we're not resuming partial work)
            if (!string.IsNullOrEmpty(resource.FormattedContent) && checkpoint == null)
            {
                Log.Debug($"ResourceFormatTask: Resource '{payload.Title}' already formatted, skipping");
                return BackgroundTaskResult.Succeeded();
            }

            var content = resource.Content;
            if (string.IsNullOrWhiteSpace(content))
            {
                return BackgroundTaskResult.Failed("Resource has no content to format", isTransient: false);
            }

            // Initialize or restore checkpoint
            checkpoint ??= new FormatCheckpoint();

            // Build chunks if not already done (first run or corrupted checkpoint)
            if (checkpoint.Chunks.Count == 0)
            {
                context.ReportProgress(item, 8, "Splitting content into chunks...");
                checkpoint.Chunks = formatterService.SplitIntoChunks(content);
                checkpoint.TotalChunks = checkpoint.Chunks.Count;
                checkpoint.CompletedChunks = [];
                checkpoint.LastCompletedIndex = -1;
                SaveCheckpoint(item, checkpoint);
                Log.Info($"ResourceFormatTask: Split '{payload.Title}' into {checkpoint.TotalChunks} chunks for processing");
            }
            else
            {
                Log.Info($"ResourceFormatTask: Resuming '{payload.Title}' - {checkpoint.CompletedChunks.Count}/{checkpoint.TotalChunks} chunks already complete");
            }

            // Calculate starting point - start from the chunk AFTER the last completed one
            // This ensures we never try to use a potentially corrupted partial result
            var startIndex = checkpoint.LastCompletedIndex + 1;

            if (startIndex > 0)
            {
                context.ReportProgress(item, 10, $"Resuming from chunk {startIndex + 1}/{checkpoint.TotalChunks}...");
            }
            else
            {
                context.ReportProgress(item, 10, $"Formatting {checkpoint.TotalChunks} chunks...");
            }

            // Process remaining chunks one at a time
            for (int i = startIndex; i < checkpoint.Chunks.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                // Acquire AI throttle for each chunk
                await context.AiThrottle.WaitAsync(ct);

                try
                {
                    // Check rate limit state before each call
                    if (!context.RateLimitState.CanMakeApiCall())
                    {
                        var waitTime = context.RateLimitState.RetryAfter?.Subtract(DateTime.UtcNow);
                        Log.Warn($"ResourceFormatTask: Rate limited at chunk {i + 1}/{checkpoint.TotalChunks}");
                        
                        // Return with checkpoint - progress is saved, will resume later
                        return BackgroundTaskResult.WithCheckpoint(
                            JsonSerializer.Serialize(checkpoint),
                            $"Rate limited at chunk {i + 1}/{checkpoint.TotalChunks}");
                    }

                    var progress = 10 + (int)((double)(i + 1) / checkpoint.TotalChunks * 80);
                    context.ReportProgress(item, progress, $"Formatting chunk {i + 1}/{checkpoint.TotalChunks}...");

                    // Format this single chunk
                    var formattedChunk = await formatterService.FormatSingleChunkAsync(checkpoint.Chunks[i], ct);
                    
                    // Success! Add to completed chunks
                    checkpoint.CompletedChunks.Add(formattedChunk);
                    checkpoint.LastCompletedIndex = i;
                    checkpoint.LastUpdatedAt = DateTime.UtcNow;
                    
                    context.RateLimitState.RecordSuccess();

                    // IMPORTANT: Save checkpoint after EACH successful chunk
                    // This ensures we never lose more than one chunk of work
                    SaveCheckpoint(item, checkpoint);

                    Log.Debug($"ResourceFormatTask: Completed chunk {i + 1}/{checkpoint.TotalChunks}");

                    // Small delay between chunks to be nice to the API
                    if (i < checkpoint.Chunks.Count - 1)
                    {
                        await Task.Delay(300, ct);
                    }
                }
                catch (HttpRequestException ex) when (IsRateLimitError(ex))
                {
                    Log.Warn($"ResourceFormatTask: Rate limited at chunk {i + 1}/{checkpoint.TotalChunks} - {ex.Message}");
                    
                    // Don't include the failed chunk - it will be retried on resume
                    return BackgroundTaskResult.WithCheckpoint(
                        JsonSerializer.Serialize(checkpoint),
                        $"Rate limited at chunk {i + 1}/{checkpoint.TotalChunks}");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Log.Error($"ResourceFormatTask: Failed at chunk {i + 1}/{checkpoint.TotalChunks} - {ex.Message}");
                    
                    // Save checkpoint and return with error - can be resumed
                    return BackgroundTaskResult.WithCheckpoint(
                        JsonSerializer.Serialize(checkpoint),
                        $"Failed at chunk {i + 1}: {ex.Message}");
                }
                finally
                {
                    context.AiThrottle.Release();
                }
            }

            context.ReportProgress(item, 92, "Combining formatted chunks...");

            // All chunks completed - combine them
            var formattedContent = string.Join("\n\n", checkpoint.CompletedChunks);

            context.ReportProgress(item, 95, "Saving formatted content...");

            // Save the final formatted content
            resource.FormattedContent = formattedContent;
            await courseService.UpdateResourceContentAsync(payload.ResourceId, formattedContent, isFormatted: true);

            Log.Info($"ResourceFormatTask: Completed formatting for '{payload.Title}' ({checkpoint.TotalChunks} chunks, {formattedContent.Length} chars)");

            // Queue ConceptMap build for this resource
            if (!resource.HasConceptMap)
            {
                context.ReportProgress(item, 98, "Queuing concept map build...");
                BackgroundQueueService.EnqueueConceptMapBuildForResource(
                    payload.ResourceId,
                    payload.Title,
                    updateResource: true);
                Log.Info($"ResourceFormatTask: Queued ConceptMap build for '{payload.Title}'");
            }

            context.ReportProgress(item, 100, "Complete");
            
            // Clear checkpoint on success (no longer needed)
            item.CheckpointJson = null;
            
            return BackgroundTaskResult.Succeeded();
        }
        catch (OperationCanceledException)
        {
            // User cancelled - save checkpoint so they can resume
            if (checkpoint != null && checkpoint.CompletedChunks.Count > 0)
            {
                SaveCheckpoint(item, checkpoint);
                Log.Info($"ResourceFormatTask: Cancelled with {checkpoint.CompletedChunks.Count}/{checkpoint.TotalChunks} chunks complete - can be resumed");
            }
            throw;
        }
        catch (HttpRequestException ex) when (IsRateLimitError(ex))
        {
            if (checkpoint != null && checkpoint.CompletedChunks.Count > 0)
            {
                return BackgroundTaskResult.WithCheckpoint(
                    JsonSerializer.Serialize(checkpoint),
                    "Rate limited");
            }
            return BackgroundTaskResult.RateLimited(ExtractRetryAfter(ex));
        }
        catch (Exception ex)
        {
            Log.Error($"ResourceFormatTask: Failed - {ex.Message}", ex);

            // Save checkpoint so work isn't lost
            if (checkpoint != null && checkpoint.CompletedChunks.Count > 0)
            {
                return BackgroundTaskResult.WithCheckpoint(
                    JsonSerializer.Serialize(checkpoint),
                    ex.Message);
            }

            return BackgroundTaskResult.Failed(ex.Message);
        }
    }

    private static void SaveCheckpoint(BackgroundQueueItem item, FormatCheckpoint checkpoint)
    {
        item.CheckpointJson = JsonSerializer.Serialize(checkpoint);
        item.UpdatedAt = DateTime.UtcNow;
    }

    public BackgroundTaskValidation Validate(BackgroundQueueItem item)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<ResourceFormatPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskValidation.Invalid("Payload is null");
            }

            if (string.IsNullOrWhiteSpace(payload.ResourceId))
            {
                return BackgroundTaskValidation.Invalid("ResourceId is required");
            }

            return BackgroundTaskValidation.Valid();
        }
        catch (JsonException ex)
        {
            return BackgroundTaskValidation.Invalid($"Invalid payload JSON: {ex.Message}");
        }
    }

    private static bool IsRateLimitError(HttpRequestException ex)
    {
        return ex.StatusCode == HttpStatusCode.TooManyRequests ||
               ex.Message.Contains("429") ||
               ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase);
    }

    private static int? ExtractRetryAfter(HttpRequestException ex)
    {
        if (ex.Message.Contains("retry", StringComparison.OrdinalIgnoreCase))
        {
            return 60;
        }
        return null;
    }
}
