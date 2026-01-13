using System.Net;
using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services.Queue;

/// <summary>
/// Handler for knowledge base build tasks.
/// Supports checkpointing and resume for long-running builds.
/// </summary>
public sealed class KnowledgeBaseBuildTaskHandler : IBackgroundTaskHandler
{
    public BackgroundTaskType TaskType => BackgroundTaskType.KnowledgeBaseBuild;

    public async Task<BackgroundTaskResult> ExecuteAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        return await BuildAsync(item, context, null, ct);
    }

    public async Task<BackgroundTaskResult> ResumeAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        CancellationToken ct)
    {
        KnowledgeBaseBuildCheckpoint? checkpoint = null;
        
        if (!string.IsNullOrEmpty(item.CheckpointJson))
        {
            try
            {
                checkpoint = JsonSerializer.Deserialize<KnowledgeBaseBuildCheckpoint>(item.CheckpointJson);
                Log.Info($"KnowledgeBaseBuildTask: Resuming from checkpoint - Stage: {checkpoint?.CurrentStage}, Chunks: {checkpoint?.ChunksProcessed}/{checkpoint?.TotalChunks}");
            }
            catch (Exception ex)
            {
                Log.Warn($"KnowledgeBaseBuildTask: Could not parse checkpoint, starting fresh - {ex.Message}");
            }
        }

        return await BuildAsync(item, context, checkpoint, ct);
    }

    private async Task<BackgroundTaskResult> BuildAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        KnowledgeBaseBuildCheckpoint? checkpoint,
        CancellationToken ct)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<KnowledgeBaseBuildPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskResult.Failed("Invalid payload", isTransient: false);
            }

            checkpoint ??= new KnowledgeBaseBuildCheckpoint();

            var courseService = context.GetService<CourseService>();
            var kbService = context.GetService<KnowledgeBaseService>();
            var kbStorageService = context.GetService<KnowledgeBaseStorageService>();

            context.ReportProgress(item, 5, "Loading resource...");

            // Load the single resource (new architecture) or fall back to legacy multiple resources
            var effectiveResourceId = payload.GetEffectiveResourceId();
            CourseResource? resource = null;

            if (!string.IsNullOrEmpty(effectiveResourceId))
            {
                resource = await courseService.GetResourceAsync(effectiveResourceId);
            }

            if (resource == null)
            {
                return BackgroundTaskResult.Failed("Resource not found to build knowledge base", isTransient: false);
            }

            // Check if we're resuming an existing KB or creating new
            KnowledgeBase? kb = null;
            if (!string.IsNullOrEmpty(payload.KnowledgeBaseId))
            {
                kb = await kbStorageService.LoadAsync(payload.KnowledgeBaseId, ct);
            }

            if (kb == null)
            {
                kb = new KnowledgeBase
                {
                    Id = string.IsNullOrEmpty(payload.KnowledgeBaseId) ? Guid.NewGuid().ToString() : payload.KnowledgeBaseId,
                    Name = payload.Name,
                    Description = $"Knowledge base generated from: {resource.Title}",
                    ResourceId = resource.Id,
                    Status = KnowledgeBaseStatus.NotStarted
                };
                await kbStorageService.SaveAsync(kb, ct);
            }

            item.KnowledgeBaseId = kb.Id;

            // Stage 1: Prepare content (if not already done)
            if (!checkpoint.ContentCombined)
            {
                context.ReportProgress(item, 10, "Preparing content...");
                
                kb.Status = KnowledgeBaseStatus.PreparingContent;
                kb.SourceContent = kbService.PrepareResourceContent(resource);
                await kbStorageService.SaveAsync(kb, ct);

                checkpoint.ContentCombined = true;
                SaveCheckpoint(item, context, checkpoint);

                Log.Debug($"KnowledgeBaseBuildTask: Prepared content for '{resource.Title}' ({kb.SourceContent?.Length ?? 0} chars)");
            }

            if (string.IsNullOrWhiteSpace(kb.SourceContent))
            {
                return BackgroundTaskResult.Failed("No content found in resource", isTransient: false);
            }

            // Stage 2: Extract concepts (with checkpoint support)
            if (!checkpoint.ConceptsExtracted)
            {
                context.ReportProgress(item, 20, "Extracting concepts...");

                // Throttle AI calls
                await context.AiThrottle.WaitAsync(ct);

                try
                {
                    // Check rate limit
                    if (!context.RateLimitState.CanMakeApiCall())
                    {
                        var waitTime = context.RateLimitState.RetryAfter?.Subtract(DateTime.UtcNow);
                        checkpoint.CurrentStage = "ConceptExtraction";
                        return BackgroundTaskResult.WithCheckpoint(
                            JsonSerializer.Serialize(checkpoint),
                            "Rate limited during concept extraction");
                    }

                    kb.Status = KnowledgeBaseStatus.GeneratingConcepts;
                    await kbStorageService.SaveAsync(kb, ct);

                    // Extract concepts with progress reporting (use SourceContent)
                    kb.Concepts = await kbService.ExtractConceptsAsync(kb.SourceContent!, kb.Id, ct);
                    
                    context.RateLimitState.RecordSuccess();

                    checkpoint.ConceptsExtracted = true;
                    checkpoint.ExtractedConceptIds = kb.Concepts.Select(c => c.Id).ToList();
                    SaveCheckpoint(item, context, checkpoint);

                    await kbStorageService.SaveAsync(kb, ct);

                    Log.Info($"KnowledgeBaseBuildTask: Extracted {kb.Concepts.Count} concepts");
                    context.ReportProgress(item, 50, $"Extracted {kb.Concepts.Count} concepts");
                }
                catch (HttpRequestException ex) when (IsRateLimitError(ex))
                {
                    checkpoint.CurrentStage = "ConceptExtraction";
                    Log.Warn($"KnowledgeBaseBuildTask: Rate limited during concept extraction");
                    return BackgroundTaskResult.WithCheckpoint(
                        JsonSerializer.Serialize(checkpoint),
                        "Rate limited during concept extraction");
                }
                finally
                {
                    context.AiThrottle.Release();
                }
            }

            if (kb.Concepts == null || kb.Concepts.Count == 0)
            {
                return BackgroundTaskResult.Failed("No concepts could be extracted", isTransient: false);
            }

            // Stage 3: Build relationships (with checkpoint support)
            if (!checkpoint.RelationshipsBuilt)
            {
                context.ReportProgress(item, 55, "Building relationships...");

                await context.AiThrottle.WaitAsync(ct);

                try
                {
                    if (!context.RateLimitState.CanMakeApiCall())
                    {
                        checkpoint.CurrentStage = "RelationshipBuilding";
                        return BackgroundTaskResult.WithCheckpoint(
                            JsonSerializer.Serialize(checkpoint),
                            "Rate limited during relationship building");
                    }

                    kb.Status = KnowledgeBaseStatus.BuildingRelationships;
                    await kbStorageService.SaveAsync(kb, ct);

                    kb.Relations = await kbService.BuildRelationshipsAsync(kb.Concepts, ct);
                    
                    context.RateLimitState.RecordSuccess();

                    checkpoint.RelationshipsBuilt = true;
                    SaveCheckpoint(item, context, checkpoint);

                    await kbStorageService.SaveAsync(kb, ct);

                    Log.Info($"KnowledgeBaseBuildTask: Built {kb.Relations.Count} relationships");
                    context.ReportProgress(item, 80, $"Built {kb.Relations.Count} relationships");
                }
                catch (HttpRequestException ex) when (IsRateLimitError(ex))
                {
                    checkpoint.CurrentStage = "RelationshipBuilding";
                    Log.Warn($"KnowledgeBaseBuildTask: Rate limited during relationship building");
                    return BackgroundTaskResult.WithCheckpoint(
                        JsonSerializer.Serialize(checkpoint),
                        "Rate limited during relationship building");
                }
                finally
                {
                    context.AiThrottle.Release();
                }
            }

            // Stage 4: Calculate complexity (local, no AI needed)
            if (!checkpoint.ComplexityCalculated)
            {
                context.ReportProgress(item, 85, "Calculating complexity order...");

                kb.Status = KnowledgeBaseStatus.CalculatingComplexity;
                kb.ComplexityOrder = kbService.CalculateComplexityOrder(kb.Concepts, kb.Relations);

                checkpoint.ComplexityCalculated = true;
                await kbStorageService.SaveAsync(kb, ct);

                Log.Debug($"KnowledgeBaseBuildTask: Calculated complexity order for {kb.ComplexityOrder.Count} concepts");
            }

            // Mark as complete
            kb.Status = KnowledgeBaseStatus.Ready;
            kb.Progress = 100;
            kb.UpdatedAt = DateTime.UtcNow;
            kb.Version++;
            await kbStorageService.SaveAsync(kb, ct);

            // Update the source resource with the KnowledgeBase ID (new architecture)
            if (payload.UpdateResource && resource != null)
            {
                resource.KnowledgeBaseId = kb.Id;
                resource.KnowledgeBaseStatus = KnowledgeBaseStatus.Ready;
                await courseService.SaveResourceAsync(resource);
                Log.Info($"KnowledgeBaseBuildTask: Updated resource '{resource.Title}' with KB ID: {kb.Id}");
            }

            context.ReportProgress(item, 100, "Knowledge base complete");
            Log.Info($"KnowledgeBaseBuildTask: Completed '{payload.Name}' with {kb.Concepts.Count} concepts and {kb.Relations.Count} relationships");

            return BackgroundTaskResult.Succeeded(JsonSerializer.Serialize(new
            {
                KnowledgeBaseId = kb.Id,
                ResourceId = resource?.Id,
                ConceptCount = kb.Concepts.Count,
                RelationCount = kb.Relations.Count
            }));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException ex) when (IsRateLimitError(ex))
        {
            var checkpoint2 = checkpoint ?? new KnowledgeBaseBuildCheckpoint();
            return BackgroundTaskResult.WithCheckpoint(
                JsonSerializer.Serialize(checkpoint2),
                "Rate limited");
        }
        catch (Exception ex)
        {
            Log.Error($"KnowledgeBaseBuildTask: Failed - {ex.Message}", ex);
            
            // Save current progress in checkpoint for resume
            if (checkpoint != null)
            {
                return BackgroundTaskResult.WithCheckpoint(
                    JsonSerializer.Serialize(checkpoint),
                    ex.Message);
            }
            
            return BackgroundTaskResult.Failed(ex.Message);
        }
    }

    public BackgroundTaskValidation Validate(BackgroundQueueItem item)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<KnowledgeBaseBuildPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskValidation.Invalid("Payload is null");
            }

            if (string.IsNullOrWhiteSpace(payload.Name))
            {
                return BackgroundTaskValidation.Invalid("Name is required");
            }

            // Check for single resource (new architecture) or legacy multiple resources
            var effectiveResourceId = payload.GetEffectiveResourceId();
            if (string.IsNullOrEmpty(effectiveResourceId))
            {
                return BackgroundTaskValidation.Invalid("A resource ID is required");
            }

            return BackgroundTaskValidation.Valid();
        }
        catch (JsonException ex)
        {
            return BackgroundTaskValidation.Invalid($"Invalid payload JSON: {ex.Message}");
        }
    }

    private static void SaveCheckpoint(BackgroundQueueItem item, BackgroundTaskContext context, KnowledgeBaseBuildCheckpoint checkpoint)
    {
        context.SaveCheckpoint(item, JsonSerializer.Serialize(checkpoint));
    }

    private static bool IsRateLimitError(HttpRequestException ex)
    {
        return ex.StatusCode == HttpStatusCode.TooManyRequests ||
               ex.Message.Contains("429") ||
               ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase);
    }
}
