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
            var conceptMapStorageService = context.GetService<ConceptMapStorageService>();

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
                return BackgroundTaskResult.Failed("Resource not found to build ConceptMap", isTransient: false);
            }

            // Check if we're resuming an existing ConceptMap or creating new
            KnowledgeBase? conceptMap = null;
            if (!string.IsNullOrEmpty(payload.KnowledgeBaseId))
            {
                conceptMap = await conceptMapStorageService.LoadAsync(payload.KnowledgeBaseId, ct);
            }

            if (conceptMap == null)
            {
                conceptMap = new KnowledgeBase
                {
                    Id = string.IsNullOrEmpty(payload.KnowledgeBaseId) ? Guid.NewGuid().ToString() : payload.KnowledgeBaseId,
                    Name = payload.Name,
                    Description = $"Concept map generated from: {resource.Title}",
                    ResourceId = resource.Id,
                    Status = KnowledgeBaseStatus.NotStarted
                };
                await conceptMapStorageService.SaveAsync(conceptMap, ct);
            }

            item.KnowledgeBaseId = conceptMap.Id;

            // Stage 1: Prepare content (if not already done)
            if (!checkpoint.ContentCombined)
            {
                context.ReportProgress(item, 10, "Preparing content...");
                
                conceptMap.Status = KnowledgeBaseStatus.PreparingContent;
                conceptMap.SourceContent = kbService.PrepareResourceContent(resource);
                await conceptMapStorageService.SaveAsync(conceptMap, ct);

                checkpoint.ContentCombined = true;
                SaveCheckpoint(item, context, checkpoint);

                Log.Debug($"KnowledgeBaseBuildTask: Prepared content for '{resource.Title}' ({conceptMap.SourceContent?.Length ?? 0} chars)");
            }

            if (string.IsNullOrWhiteSpace(conceptMap.SourceContent))
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

                    conceptMap.Status = KnowledgeBaseStatus.GeneratingConcepts;
                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    // Extract concepts with progress reporting (use SourceContent)
                    conceptMap.Concepts = await kbService.ExtractConceptsAsync(conceptMap.SourceContent!, conceptMap.Id, ct);
                    
                    context.RateLimitState.RecordSuccess();

                    checkpoint.ConceptsExtracted = true;
                    checkpoint.ExtractedConceptIds = conceptMap.Concepts.Select(c => c.Id).ToList();
                    SaveCheckpoint(item, context, checkpoint);

                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    Log.Info($"KnowledgeBaseBuildTask: Extracted {conceptMap.Concepts.Count} concepts");
                    context.ReportProgress(item, 50, $"Extracted {conceptMap.Concepts.Count} concepts");
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

            if (conceptMap.Concepts == null || conceptMap.Concepts.Count == 0)
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

                    conceptMap.Status = KnowledgeBaseStatus.BuildingRelationships;
                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    conceptMap.Relations = await kbService.BuildRelationshipsAsync(conceptMap.Concepts, ct);
                    
                    context.RateLimitState.RecordSuccess();

                    checkpoint.RelationshipsBuilt = true;
                    SaveCheckpoint(item, context, checkpoint);

                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    Log.Info($"KnowledgeBaseBuildTask: Built {conceptMap.Relations.Count} relationships");
                    context.ReportProgress(item, 80, $"Built {conceptMap.Relations.Count} relationships");
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

                conceptMap.Status = KnowledgeBaseStatus.CalculatingComplexity;
                conceptMap.ComplexityOrder = kbService.CalculateComplexityOrder(conceptMap.Concepts, conceptMap.Relations);

                checkpoint.ComplexityCalculated = true;
                await conceptMapStorageService.SaveAsync(conceptMap, ct);

                Log.Debug($"KnowledgeBaseBuildTask: Calculated complexity order for {conceptMap.ComplexityOrder.Count} concepts");
            }


            // Mark as complete
            conceptMap.Status = KnowledgeBaseStatus.Ready;
            conceptMap.Progress = 100;
            conceptMap.UpdatedAt = DateTime.UtcNow;
            conceptMap.Version++;
            await conceptMapStorageService.SaveAsync(conceptMap, ct);

            // Update the source resource with the ConceptMap ID (new architecture)
            if (payload.UpdateResource && resource != null)
            {
                resource.ConceptMapId = conceptMap.Id;
                resource.ConceptMapStatus = ConceptMapStatus.Ready;
                await courseService.SaveResourceAsync(resource);
                Log.Info($"KnowledgeBaseBuildTask: Updated resource '{resource.Title}' with ConceptMap ID: {conceptMap.Id}");
            }

            context.ReportProgress(item, 100, "Concept map complete");
            Log.Info($"KnowledgeBaseBuildTask: Completed '{payload.Name}' with {conceptMap.Concepts.Count} concepts and {conceptMap.Relations.Count} relationships");

            return BackgroundTaskResult.Succeeded(JsonSerializer.Serialize(new
            {
                ConceptMapId = conceptMap.Id,
                ResourceId = resource?.Id,
                ConceptCount = conceptMap.Concepts.Count,
                RelationCount = conceptMap.Relations.Count
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
