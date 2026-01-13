using System.Net;
using System.Text.Json;
using Tutor.Services.Logging;

namespace Tutor.Services.Queue;

/// <summary>
/// Handler for resource formatting tasks.
/// Uses AI to format resource content with retry and rate limit handling.
/// </summary>
public sealed class ResourceFormatTaskHandler : IBackgroundTaskHandler
{
    public BackgroundTaskType TaskType => BackgroundTaskType.ResourceFormat;

    public async Task<BackgroundTaskResult> ExecuteAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
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

            // Skip if already formatted
            if (!string.IsNullOrEmpty(resource.FormattedContent))
            {
                Log.Debug($"ResourceFormatTask: Resource '{payload.Title}' already formatted, skipping");
                return BackgroundTaskResult.Succeeded();
            }

            var content = resource.Content;
            if (string.IsNullOrWhiteSpace(content))
            {
                return BackgroundTaskResult.Failed("Resource has no content to format", isTransient: false);
            }

            context.ReportProgress(item, 10, "Acquiring AI slot...");

            // Throttle AI calls
            await context.AiThrottle.WaitAsync(ct);

            try
            {
                // Check rate limit state
                if (!context.RateLimitState.CanMakeApiCall())
                {
                    var waitTime = context.RateLimitState.RetryAfter?.Subtract(DateTime.UtcNow);
                    return BackgroundTaskResult.RateLimited((int?)waitTime?.TotalSeconds ?? 60);
                }

                context.ReportProgress(item, 15, "Formatting with AI...");

                string formattedContent;
                try
                {
                    formattedContent = await formatterService.FormatContentAsync(
                        content,
                        (current, total) =>
                        {
                            var progress = 15 + (int)((double)current / total * 70);
                            context.ReportProgress(item, progress, $"Formatting chunk {current}/{total}...");
                        },
                        ct);
                }
                catch (HttpRequestException ex) when (IsRateLimitError(ex))
                {
                    Log.Warn($"ResourceFormatTask: Rate limited - {ex.Message}");
                    return BackgroundTaskResult.RateLimited(ExtractRetryAfter(ex));
                }

                context.ReportProgress(item, 90, "Saving formatted content...");

                // Save the formatted content
                resource.FormattedContent = formattedContent;
                await courseService.UpdateResourceContentAsync(payload.ResourceId, formattedContent, isFormatted: true);

                context.RateLimitState.RecordSuccess();

                context.ReportProgress(item, 95, "Formatting complete");
                Log.Info($"ResourceFormatTask: Completed formatting for '{payload.Title}'");

                // Queue KB build for this resource (new architecture: 1 resource = 1 KB)
                if (!resource.HasKnowledgeBase)
                {
                    context.ReportProgress(item, 98, "Queuing knowledge base build...");
                    BackgroundQueueService.EnqueueKnowledgeBaseBuildForResource(
                        payload.ResourceId,
                        payload.Title,
                        updateResource: true);
                    Log.Info($"ResourceFormatTask: Queued KB build for '{payload.Title}'");
                }

                context.ReportProgress(item, 100, "Complete");
                return BackgroundTaskResult.Succeeded();
            }
            finally
            {
                context.AiThrottle.Release();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException ex) when (IsRateLimitError(ex))
        {
            return BackgroundTaskResult.RateLimited(ExtractRetryAfter(ex));
        }
        catch (Exception ex)
        {
            Log.Error($"ResourceFormatTask: Failed - {ex.Message}", ex);
            return BackgroundTaskResult.Failed(ex.Message);
        }
    }

    public Task<BackgroundTaskResult> ResumeAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        // Format tasks don't have checkpoints - just re-run
        // The content will be re-formatted from scratch
        return ExecuteAsync(item, context, ct);
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
        // Try to extract retry-after from the exception message
        // This is a fallback - ideally we'd get this from response headers
        if (ex.Message.Contains("retry", StringComparison.OrdinalIgnoreCase))
        {
            // Default to 60 seconds for rate limiting
            return 60;
        }
        return null;
    }
}
