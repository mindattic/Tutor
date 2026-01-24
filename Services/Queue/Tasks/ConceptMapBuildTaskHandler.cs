using System.Net;
using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services.Queue;

/// <summary>
/// Handler for concept map build tasks.
/// Supports checkpointing and resume for long-running builds.
/// </summary>
public sealed class ConceptMapBuildTaskHandler : IBackgroundTaskHandler
{
    public BackgroundTaskType TaskType => BackgroundTaskType.ConceptMapBuild;

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
        ConceptMapBuildCheckpoint? checkpoint = null;
        
        if (!string.IsNullOrEmpty(item.CheckpointJson))
        {
            try
            {
                checkpoint = JsonSerializer.Deserialize<ConceptMapBuildCheckpoint>(item.CheckpointJson);
                Log.Info($"ConceptMapBuildTask: Resuming from checkpoint - Stage: {checkpoint?.CurrentStage}, Chunks: {checkpoint?.ChunksProcessed}/{checkpoint?.TotalChunks}");
            }
            catch (Exception ex)
            {
                Log.Warn($"ConceptMapBuildTask: Could not parse checkpoint, starting fresh - {ex.Message}");
            }
        }

        return await BuildAsync(item, context, checkpoint, ct);
    }

    private async Task<BackgroundTaskResult> BuildAsync(
        BackgroundQueueItem item,
        BackgroundTaskContext context,
        ConceptMapBuildCheckpoint? checkpoint,
        CancellationToken ct)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<ConceptMapBuildPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskResult.Failed("Invalid payload", isTransient: false);
            }

            checkpoint ??= new ConceptMapBuildCheckpoint();

            var courseService = context.GetService<CourseService>();
            var cmService = context.GetService<ConceptMapService>();
            var conceptMapStorageService = context.GetService<ConceptMapStorageService>();

            context.ReportProgress(item, 5, "Loading resource...");

            // Load the single resource
            CourseResource? resource = null;

            if (!string.IsNullOrEmpty(payload.ResourceId))
            {
                resource = await courseService.GetResourceAsync(payload.ResourceId);
            }

            if (resource == null)
            {
                return BackgroundTaskResult.Failed("Resource not found to build ConceptMap", isTransient: false);
            }

            // Check if we're resuming an existing ConceptMap or creating new
            ConceptMap? conceptMap = null;
            if (!string.IsNullOrEmpty(payload.ConceptMapId))
            {
                conceptMap = await conceptMapStorageService.LoadAsync(payload.ConceptMapId, ct);
            }

            if (conceptMap == null)
            {
                conceptMap = new ConceptMap
                {
                    Id = string.IsNullOrEmpty(payload.ConceptMapId) ? Guid.NewGuid().ToString() : payload.ConceptMapId,
                    Name = payload.Name,
                    Description = $"Concept map generated from: {resource.Title}",
                    ResourceId = resource.Id,
                    Status = ConceptMapStatus.NotStarted
                };
                await conceptMapStorageService.SaveAsync(conceptMap, ct);
            }

            item.ConceptMapId = conceptMap.Id;

            // Stage 1: Prepare content (if not already done)
            if (!checkpoint.ContentCombined)
            {
                context.ReportProgress(item, 10, "Preparing content...");
                
                conceptMap.Status = ConceptMapStatus.PreparingContent;
                conceptMap.SourceContent = cmService.PrepareResourceContent(resource);
                await conceptMapStorageService.SaveAsync(conceptMap, ct);

                checkpoint.ContentCombined = true;
                SaveCheckpoint(item, context, checkpoint);

                Log.Debug($"ConceptMapBuildTask: Prepared content for '{resource.Title}' ({conceptMap.SourceContent?.Length ?? 0} chars)");
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

                    conceptMap.Status = ConceptMapStatus.ExtractingConcepts;
                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    // Extract concepts with progress reporting (use SourceContent)
                    conceptMap.Concepts = await cmService.ExtractConceptsAsync(conceptMap.SourceContent!, conceptMap.Id, ct);
                    
                    context.RateLimitState.RecordSuccess();

                    checkpoint.ConceptsExtracted = true;
                    checkpoint.ExtractedConceptIds = conceptMap.Concepts.Select(c => c.Id).ToList();
                    SaveCheckpoint(item, context, checkpoint);

                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    Log.Info($"ConceptMapBuildTask: Extracted {conceptMap.Concepts.Count} concepts");
                    context.ReportProgress(item, 50, $"Extracted {conceptMap.Concepts.Count} concepts");
                }
                catch (HttpRequestException ex) when (IsRateLimitError(ex))
                {
                    checkpoint.CurrentStage = "ConceptExtraction";
                    Log.Warn($"ConceptMapBuildTask: Rate limited during concept extraction");
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

                    conceptMap.Status = ConceptMapStatus.BuildingRelationships;
                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    conceptMap.Relations = await cmService.BuildRelationshipsAsync(conceptMap.Concepts, ct);
                    
                    context.RateLimitState.RecordSuccess();

                    checkpoint.RelationshipsBuilt = true;
                    SaveCheckpoint(item, context, checkpoint);

                    await conceptMapStorageService.SaveAsync(conceptMap, ct);

                    Log.Info($"ConceptMapBuildTask: Built {conceptMap.Relations.Count} relationships");
                    context.ReportProgress(item, 80, $"Built {conceptMap.Relations.Count} relationships");
                }
                catch (HttpRequestException ex) when (IsRateLimitError(ex))
                {
                    checkpoint.CurrentStage = "RelationshipBuilding";
                    Log.Warn($"ConceptMapBuildTask: Rate limited during relationship building");
                    return BackgroundTaskResult.WithCheckpoint(
                        JsonSerializer.Serialize(checkpoint),
                        "Rate limited during relationship building");
                }
                finally
                {
                    context.AiThrottle.Release();
                }
            }

            // Stage 4: Link orphaned concepts (AI-powered, with checkpoint support)
            if (!checkpoint.OrphansLinked)
            {
                context.ReportProgress(item, 82, "Checking for orphaned concepts...");

                var connectivity = conceptMap.GetConnectivityInfo();
                
                if (!connectivity.IsFullyConnected && connectivity.OrphanedConceptCount > 0)
                {
                    // Load settings and apply resource overrides
                    var settingsService = context.GetService<SettingsService>();
                    var globalMaxIterations = await settingsService.GetOrphanLinkingMaxIterationsAsync();
                    var globalMinConfidence = await settingsService.GetOrphanLinkingMinConfidenceAsync();
                    
                    var effectiveMaxIterations = resource.GetEffectiveMaxIterations(globalMaxIterations);
                    var effectiveMinConfidence = resource.GetEffectiveMinConfidence(globalMinConfidence);

                    Log.Info($"ConceptMapBuildTask: Found {connectivity.OrphanedConceptCount} orphaned concept(s) (maxIter={effectiveMaxIterations}, minConf={effectiveMinConfidence})");
                    context.ReportProgress(item, 83, $"Linking {connectivity.OrphanedConceptCount} orphaned concepts (iterative)...");

                    await context.AiThrottle.WaitAsync(ct);

                    try
                    {
                        if (!context.RateLimitState.CanMakeApiCall())
                        {
                            checkpoint.CurrentStage = "OrphanLinking";
                            return BackgroundTaskResult.WithCheckpoint(
                                JsonSerializer.Serialize(checkpoint),
                                "Rate limited during orphan linking");
                        }

                        var orphanLinker = context.GetService<OrphanConceptLinkerService>();
                        
                        // Use iterative linking with settings
                        var linkResult = await orphanLinker.LinkAllOrphansIterativelyAsync(
                            conceptMap, 
                            maxIterations: effectiveMaxIterations, 
                            minConfidence: effectiveMinConfidence, 
                            ct);

                        context.RateLimitState.RecordSuccess();

                        if (linkResult.AppliedLinkCount > 0)
                        {
                            await conceptMapStorageService.SaveAsync(conceptMap, ct);
                            Log.Info($"ConceptMapBuildTask: Linked {linkResult.AppliedLinkCount} orphaned concepts. " +
                                (linkResult.IsFullyConnected 
                                    ? "Graph is now fully connected!" 
                                    : $"Remaining orphans: {linkResult.RemainingOrphanCount}"));
                        }
                        else
                        {
                            Log.Debug($"ConceptMapBuildTask: No orphan links could be applied");
                        }
                    }
                    catch (HttpRequestException ex) when (IsRateLimitError(ex))
                    {
                        checkpoint.CurrentStage = "OrphanLinking";
                        Log.Warn($"ConceptMapBuildTask: Rate limited during orphan linking");
                        return BackgroundTaskResult.WithCheckpoint(
                            JsonSerializer.Serialize(checkpoint),
                            "Rate limited during orphan linking");
                    }
                    catch (Exception ex)
                    {
                        // Orphan linking is not critical - log and continue
                        Log.Warn($"ConceptMapBuildTask: Orphan linking failed (non-critical) - {ex.Message}");
                    }
                    finally
                    {
                        context.AiThrottle.Release();
                    }
                }
                else
                {
                    Log.Debug($"ConceptMapBuildTask: Graph is fully connected, no orphan linking needed");
                }

                checkpoint.OrphansLinked = true;
                SaveCheckpoint(item, context, checkpoint);
            }

            // Stage 5: Calculate complexity (local, no AI needed)
            if (!checkpoint.ComplexityCalculated)
            {
                context.ReportProgress(item, 90, "Calculating complexity order...");

                conceptMap.Status = ConceptMapStatus.CalculatingComplexity;
                conceptMap.ComplexityOrder = cmService.CalculateComplexityOrder(conceptMap.Concepts, conceptMap.Relations);

                checkpoint.ComplexityCalculated = true;
                await conceptMapStorageService.SaveAsync(conceptMap, ct);

                Log.Debug($"ConceptMapBuildTask: Calculated complexity order for {conceptMap.ComplexityOrder.Count} concepts");
            }


            // Mark as complete
            conceptMap.Status = ConceptMapStatus.Ready;
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
                Log.Info($"ConceptMapBuildTask: Updated resource '{resource.Title}' with ConceptMap ID: {conceptMap.Id}");
            }

            context.ReportProgress(item, 100, "Concept map complete");
            Log.Info($"ConceptMapBuildTask: Completed '{payload.Name}' with {conceptMap.Concepts.Count} concepts and {conceptMap.Relations.Count} relationships");

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
            var checkpoint2 = checkpoint ?? new ConceptMapBuildCheckpoint();
            return BackgroundTaskResult.WithCheckpoint(
                JsonSerializer.Serialize(checkpoint2),
                "Rate limited");
        }
        catch (Exception ex)
        {
            Log.Error($"ConceptMapBuildTask: Failed - {ex.Message}", ex);
            
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
            var payload = JsonSerializer.Deserialize<ConceptMapBuildPayload>(item.PayloadJson);
            if (payload == null)
            {
                return BackgroundTaskValidation.Invalid("Payload is null");
            }

            if (string.IsNullOrWhiteSpace(payload.Name))
            {
                return BackgroundTaskValidation.Invalid("Name is required");
            }

            // Check for single resource
            if (string.IsNullOrEmpty(payload.ResourceId))
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

    private static void SaveCheckpoint(BackgroundQueueItem item, BackgroundTaskContext context, ConceptMapBuildCheckpoint checkpoint)
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
