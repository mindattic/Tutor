using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Queue;

/// <summary>
/// Handler for resource upload tasks.
/// Saves a resource to storage and optionally queues formatting and graph building.
/// </summary>
public sealed class ResourceUploadTaskHandler : IBackgroundTaskHandler
{
    public BackgroundTaskType TaskType => BackgroundTaskType.ResourceUpload;

    public async Task<BackgroundTaskResult> ExecuteAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<ResourceUploadPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskResult.Failed("Invalid payload", isTransient: false);
            }

            context.ReportProgress(item, 10, "Preparing resource...");

            var courseService = context.GetService<CourseService>();

            // Create the resource
            var resource = new CourseResource
            {
                Id = payload.ResourceId,
                Title = payload.Title,
                Content = payload.Content,
                Author = payload.Author ?? "",
                Year = payload.Year ?? "",
                Description = payload.Description ?? "",
                FileName = payload.FileName ?? "",
                Type = Enum.TryParse<ResourceType>(payload.ResourceType, out var rt) ? rt : ResourceType.Text
            };

            context.ReportProgress(item, 30, "Saving resource...");

            // Save the resource
            var savedId = await courseService.SaveResourceCoreAsync(resource);
            resource.Id = savedId;
            item.ResourceId = savedId;

            Log.Info($"ResourceUploadTask: Saved resource '{payload.Title}' with ID {savedId}");

            // Add to course if specified
            if (!string.IsNullOrEmpty(payload.CourseId))
            {
                context.ReportProgress(item, 50, "Adding to course...");
                await courseService.AddResourceToCourseAsync(payload.CourseId, savedId);
                Log.Debug($"ResourceUploadTask: Added resource to course {payload.CourseId}");
            }

            context.ReportProgress(item, 70, "Upload complete");

            // Queue follow-up tasks if requested
            if (payload.AutoFormat)
            {
                BackgroundQueueService.EnqueueResourceFormat(savedId, payload.Title);
                Log.Debug($"ResourceUploadTask: Queued format task for '{payload.Title}'");
            }

            // Note: Graph building will be triggered after formatting completes
            // or we could queue it here with a dependency system

            context.ReportProgress(item, 100, "Complete");

            return BackgroundTaskResult.Succeeded(JsonSerializer.Serialize(new { ResourceId = savedId }));
        }
        catch (Exception ex)
        {
            Log.Error($"ResourceUploadTask: Failed - {ex.Message}", ex);
            return BackgroundTaskResult.Failed(ex.Message);
        }
    }

    public Task<BackgroundTaskResult> ResumeAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        // Upload tasks don't have checkpoints - just re-run
        return ExecuteAsync(item, context, ct);
    }

    public BackgroundTaskValidation Validate(BackgroundQueueItem item)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<ResourceUploadPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskValidation.Invalid("Payload is null");
            }

            if (string.IsNullOrWhiteSpace(payload.Title))
            {
                return BackgroundTaskValidation.Invalid("Title is required");
            }

            if (string.IsNullOrWhiteSpace(payload.Content))
            {
                return BackgroundTaskValidation.Invalid("Content is required");
            }

            return BackgroundTaskValidation.Valid();
        }
        catch (JsonException ex)
        {
            return BackgroundTaskValidation.Invalid($"Invalid payload JSON: {ex.Message}");
        }
    }
}
