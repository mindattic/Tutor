using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for building a ConceptMap from Resources.
/// 
/// The ConceptMap is an interconnected map of concepts that:
/// - Is INDEPENDENT of any Course
/// - Contains all Concepts and their relationships
/// - Can be assigned to one or more Courses
/// 
/// Generation pipeline:
/// 1. Prepare Resource content
/// 2. Extract atomic Concepts from the content (parallel with throttling)
/// 3. Build Concept relationships (prerequisites, related, etc.)
/// 4. Calculate complexity ordering for progressive learning
/// </summary>
public sealed class ConceptMapService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    private readonly ConceptMapStorageService storageService;

    // Throttling settings for parallel API calls
    private const int MaxConcurrentApiCalls = 3;  // Max parallel requests to avoid rate limiting
    private const int ChunkDelayMs = 200;         // Small delay between starting chunks
    private const int MaxChunkSize = 8000;        // Smaller chunks for better API reliability
    private const int MaxConceptsPerRelationshipChunk = 30;  // Max concepts per relationship extraction call

    private const string ConceptExtractionPrompt = """
        You are an expert knowledge analyst. Analyze the following educational content and extract atomic concepts.
        
        An atomic concept is the smallest teachable unit of knowledge - a single term, idea, or fact that can be:
        - Defined in 1-2 sentences
        - Taught independently (though it may have prerequisites)
        - Tested for understanding
        
        For each concept, identify:
        1. The term or name (canonical form)
        2. A concise definition/summary (1-2 sentences)
        3. Any alternative names or synonyms
        4. Tags for categorization (subject area, type, etc.)
        5. Terms that appear to be prerequisites (concepts needed first)
        6. Related concepts mentioned in the same context
        
        Return your analysis as JSON:
        {
            "concepts": [
                {
                    "term": "Concept Name",
                    "description": "Clear, concise definition (1-2 sentences)",
                    "aliases": ["Alternative Name 1"],
                    "tags": ["category", "subject"],
                    "potentialPrerequisites": ["Foundational Concept"],
                    "relatedTerms": ["Related Concept"],
                    "confidence": 0.95
                }
            ]
        }
        
        Guidelines:
        - Focus on domain-specific terminology and key concepts
        - Each concept should be meaningful for learning the subject
        - Avoid trivial or overly common terms
        - Be thorough but maintain quality over quantity
        - Ensure descriptions are self-contained and understandable
        """;

    private const string RelationshipExtractionPrompt = """
        You are an expert knowledge analyst. Given a list of concepts from an educational domain,
        identify the relationships between them.
        
        Types of relationships:
        - prerequisite: Concept A must be understood before Concept B
        - related: Concepts are related but neither is a prerequisite
        - contains: Concept A is a broader category containing Concept B
        - instanceOf: Concept A is a specific instance of Concept B
        - similarTo: Concepts are similar or easily confused
        - contrastsWith: Concepts are opposites or contrasting ideas
        
        Given these concepts:
        {CONCEPTS}
        
        Return your analysis as JSON:
        {
            "relationships": [
                {
                    "sourceConceptTerm": "Concept A",
                    "targetConceptTerm": "Concept B",
                    "type": "prerequisite",
                    "strength": 0.9,
                    "description": "Brief explanation of the relationship"
                }
            ]
        }
        
        Guidelines:
        - Focus on the most important relationships
        - Prerequisite relationships should form a logical learning order
        - Avoid circular prerequisites
        - Include relationships that help learners understand connections
        """;

    /// <summary>
    /// Event fired when build progress changes.
    /// </summary>
    public event Action<ConceptMapBuildProgress>? OnProgressChanged;

    public ConceptMapService(
        HttpClient http,
        OpenAIOptions opt,
        ConceptMapStorageService storageService)
    {
        this.http = http;
        this.opt = opt;
        this.storageService = storageService;
    }


    /// <summary>
    /// Builds a ConceptMap from a list of Resources.
    /// This method is maintained for backward compatibility.
    /// New code should use BuildFromResourceAsync for single resource ConceptMap generation.
    /// </summary>
    [Obsolete("Use BuildFromResourceAsync instead for single resource ConceptMap generation.")]
    public async Task<ConceptMap> BuildFromResourcesAsync(
        string name,
        List<CourseResource> resources,
        CancellationToken ct = default)
    {
        Log.Info($"Starting Concept Map build: '{name}' with {resources.Count} resource(s)");
        
        if (resources.Count == 0)
        {
            Log.Error("Concept Map build failed: No resources provided");
            throw new InvalidOperationException("At least one resource is required to build a ConceptMap.");
        }

        // Create new ConceptMap
        var cm = new ConceptMap
        {
            Name = name,
            Description = $"Concept map generated from {resources.Count} resource(s)",
            ResourceId = resources.First().Id,
            Status = ConceptMapStatus.NotStarted
        };
        
        Log.Debug($"Created ConceptMap with ID: {cm.Id}");

        try
        {
            // Step 1: Combine resources
            Log.Info("Step 1/4: Combining resources...");
            ReportProgress(cm, ConceptMapStatus.PreparingContent, 5, "Combining resources...");
            cm.Status = ConceptMapStatus.PreparingContent;
            cm.SourceContent = CombineResources(resources);
            await storageService.SaveAsync(cm, ct);
            
            Log.Debug($"Combined content size: {cm.SourceContent?.Length ?? 0} characters");

            if (string.IsNullOrWhiteSpace(cm.SourceContent))
            {
                Log.Error("Concept Map build failed: No content found in resources");
                throw new InvalidOperationException("No content found in resources.");
            }

            // Step 2: Extract concepts
            Log.Info("Step 2/4: Extracting concepts via AI...");
            ReportProgress(cm, ConceptMapStatus.ExtractingConcepts, 20, "Extracting concepts...");
            cm.Status = ConceptMapStatus.ExtractingConcepts;
            cm.Concepts = await ExtractConceptsAsync(cm.SourceContent, cm.Id, ct);
            await storageService.SaveAsync(cm, ct);

            if (cm.Concepts.Count == 0)
            {
                Log.Error("Concept Map build failed: No concepts could be extracted");
                throw new InvalidOperationException("No concepts could be extracted from the content.");
            }
            
            Log.Info($"Successfully extracted {cm.Concepts.Count} concepts");

            ReportProgress(cm, ConceptMapStatus.ExtractingConcepts, 40, 
                $"Extracted {cm.Concepts.Count} concepts");

            // Step 3: Build relationships
            Log.Info("Step 3/4: Building concept relationships via AI...");
            ReportProgress(cm, ConceptMapStatus.BuildingRelationships, 50, "Building concept relationships...");
            cm.Status = ConceptMapStatus.BuildingRelationships;
            cm.Relations = await BuildRelationshipsAsync(cm.Concepts, ct);
            await storageService.SaveAsync(cm, ct);
            
            Log.Info($"Built {cm.Relations.Count} relationships between concepts");

            ReportProgress(cm, ConceptMapStatus.BuildingRelationships, 70,
                $"Built {cm.Relations.Count} relationships");

            // Step 4: Calculate complexity ordering
            Log.Info("Step 4/4: Calculating complexity order...");
            ReportProgress(cm, ConceptMapStatus.CalculatingComplexity, 80, "Calculating complexity order...");
            cm.Status = ConceptMapStatus.CalculatingComplexity;
            cm.ComplexityOrder = CalculateComplexityOrder(cm.Concepts, cm.Relations);
            await storageService.SaveAsync(cm, ct);

            // Mark as ready
            cm.Status = ConceptMapStatus.Ready;
            cm.Progress = 100;
            cm.UpdatedAt = DateTime.UtcNow;
            cm.Version++;
            await storageService.SaveAsync(cm, ct);

            ReportProgress(cm, ConceptMapStatus.Ready, 100,
                $"ConceptMap ready with {cm.Concepts.Count} concepts and {cm.Relations.Count} relationships");
            
            Log.Info($"Concept Map '{name}' build completed successfully: {cm.Concepts.Count} concepts, {cm.Relations.Count} relationships");

            return cm;
        }
        catch (OperationCanceledException)
        {
            Log.Warn($"Concept Map build cancelled: '{name}'");
            cm.Status = ConceptMapStatus.Failed;
            cm.ErrorMessage = "Build was cancelled";
            // Use CancellationToken.None to save even after cancellation
            try
            {
                await storageService.SaveAsync(cm, CancellationToken.None);
            }
            catch (Exception saveEx)
            {
                Log.Error($"Failed to save cancelled CM state: {saveEx.Message}", saveEx);
            }
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"Concept Map build failed: '{name}' - {ex.Message}", ex);
            cm.Status = ConceptMapStatus.Failed;
            cm.ErrorMessage = ex.Message;
            // Use CancellationToken.None to ensure we can save error state
            try
            {
                await storageService.SaveAsync(cm, CancellationToken.None);
            }
            catch (Exception saveEx)
            {
                Log.Error($"Failed to save failed CM state: {saveEx.Message}", saveEx);
            }

            ReportProgress(cm, ConceptMapStatus.Failed, cm.Progress,
                "Build failed", ex.Message);

            throw;
        }
    }

    /// <summary>
    /// Builds a ConceptMap from a single Resource.
    /// This is the primary method for the new architecture where each Resource
    /// generates its own independent ConceptMap.
    /// </summary>
    public async Task<ConceptMap> BuildFromResourceAsync(
        CourseResource resource,
        CancellationToken ct = default)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        var name = $"{resource.Title} Concept Map";
        Log.Info($"Starting Concept Map build for resource: '{resource.Title}'");

        // Create new ConceptMap linked to this single resource
        var cm = new ConceptMap
        {
            Name = name,
            Description = $"Concept map generated from: {resource.Title}",
            ResourceId = resource.Id,
            Status = ConceptMapStatus.NotStarted
        };

        Log.Debug($"Created ConceptMap with ID: {cm.Id} for Resource: {resource.Id}");

        try
        {
            // Step 1: Prepare content
            Log.Info("Step 1/4: Preparing content...");
            ReportProgress(cm, ConceptMapStatus.PreparingContent, 5, "Preparing content...");
            cm.Status = ConceptMapStatus.PreparingContent;
            cm.SourceContent = PrepareResourceContent(resource);
            await storageService.SaveAsync(cm, ct);

            Log.Debug($"Content size: {cm.SourceContent?.Length ?? 0} characters");

            if (string.IsNullOrWhiteSpace(cm.SourceContent))
            {
                Log.Error("Concept Map build failed: No content found in resource");
                throw new InvalidOperationException("No content found in resource.");
            }

            // Step 2: Extract concepts
            Log.Info("Step 2/4: Extracting concepts via AI...");
            ReportProgress(cm, ConceptMapStatus.ExtractingConcepts, 20, "Extracting concepts...");
            cm.Status = ConceptMapStatus.ExtractingConcepts;
            cm.Concepts = await ExtractConceptsAsync(cm.SourceContent, cm.Id, ct);
            await storageService.SaveAsync(cm, ct);

            if (cm.Concepts.Count == 0)
            {
                Log.Error("Concept Map build failed: No concepts could be extracted");
                throw new InvalidOperationException("No concepts could be extracted from the content.");
            }

            Log.Info($"Successfully extracted {cm.Concepts.Count} concepts");
            ReportProgress(cm, ConceptMapStatus.ExtractingConcepts, 40,
                $"Extracted {cm.Concepts.Count} concepts");

            // Step 3: Build relationships
            Log.Info("Step 3/4: Building concept relationships via AI...");
            ReportProgress(cm, ConceptMapStatus.BuildingRelationships, 50, "Building concept relationships...");
            cm.Status = ConceptMapStatus.BuildingRelationships;
            cm.Relations = await BuildRelationshipsAsync(cm.Concepts, ct);
            await storageService.SaveAsync(cm, ct);

            Log.Info($"Built {cm.Relations.Count} relationships between concepts");
            ReportProgress(cm, ConceptMapStatus.BuildingRelationships, 70,
                $"Built {cm.Relations.Count} relationships");

            // Step 4: Calculate complexity ordering
            Log.Info("Step 4/4: Calculating complexity order...");
            ReportProgress(cm, ConceptMapStatus.CalculatingComplexity, 80, "Calculating complexity order...");
            cm.Status = ConceptMapStatus.CalculatingComplexity;
            cm.ComplexityOrder = CalculateComplexityOrder(cm.Concepts, cm.Relations);
            await storageService.SaveAsync(cm, ct);

            // Mark as ready
            cm.Status = ConceptMapStatus.Ready;
            cm.Progress = 100;
            cm.UpdatedAt = DateTime.UtcNow;
            cm.Version++;
            await storageService.SaveAsync(cm, ct);

            ReportProgress(cm, ConceptMapStatus.Ready, 100,
                $"ConceptMap ready with {cm.Concepts.Count} concepts and {cm.Relations.Count} relationships");

            Log.Info($"Concept Map for '{resource.Title}' build completed: {cm.Concepts.Count} concepts, {cm.Relations.Count} relationships");

            return cm;
        }
        catch (OperationCanceledException)
        {
            Log.Warn($"Concept Map build cancelled for resource: '{resource.Title}'");
            cm.Status = ConceptMapStatus.Failed;
            cm.ErrorMessage = "Build was cancelled";
            try
            {
                await storageService.SaveAsync(cm, CancellationToken.None);
            }
            catch (Exception saveEx)
            {
                Log.Error($"Failed to save cancelled CM state: {saveEx.Message}", saveEx);
            }
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"Concept Map build failed for resource: '{resource.Title}' - {ex.Message}", ex);
            cm.Status = ConceptMapStatus.Failed;
            cm.ErrorMessage = ex.Message;
            try
            {
                await storageService.SaveAsync(cm, CancellationToken.None);
            }
            catch (Exception saveEx)
            {
                Log.Error($"Failed to save failed CM state: {saveEx.Message}", saveEx);
            }

            ReportProgress(cm, ConceptMapStatus.Failed, cm.Progress, "Build failed", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Prepares content from a single resource for processing.
    /// </summary>
    public string PrepareResourceContent(CourseResource resource)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"# {resource.Title}");
        if (!string.IsNullOrEmpty(resource.Author))
            sb.AppendLine($"Author: {resource.Author}");
        if (!string.IsNullOrEmpty(resource.Year))
            sb.AppendLine($"Year: {resource.Year}");
        sb.AppendLine();
        sb.AppendLine(resource.GetEffectiveContent());

        return sb.ToString();
    }

    /// <summary>
    /// Combines multiple Resources into a single text for analysis.
    /// </summary>
    public string CombineResources(List<CourseResource> resources)
    {
        var sb = new StringBuilder();
        
        foreach (var resource in resources.OrderBy(r => r.Title))
        {
            sb.AppendLine($"# {resource.Title}");
            if (!string.IsNullOrEmpty(resource.Author))
                sb.AppendLine($"Author: {resource.Author}");
            sb.AppendLine();
            sb.AppendLine(resource.GetEffectiveContent());
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Extracts atomic Concepts from combined content using AI.
    /// Uses parallel processing with SemaphoreSlim throttling for better performance.
    /// </summary>
    public async Task<List<Concept>> ExtractConceptsAsync(
        string combinedContent,
        string conceptMapId,
        CancellationToken ct = default)
    {
        Log.Debug($"ExtractConceptsAsync: Starting concept extraction for CM {conceptMapId}");
        
        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("OpenAI API key is missing - cannot extract concepts");
            throw new InvalidOperationException("OpenAI API key is missing.");
        }

        // Split content into chunks to avoid token limits
        var chunks = SplitIntoChunks(combinedContent, maxChunkSize: MaxChunkSize);
        Log.Info($"ExtractConceptsAsync: Split content into {chunks.Count} chunk(s) for parallel processing");

        if (chunks.Count == 0)
        {
            Log.Warn("ExtractConceptsAsync: No chunks to process");
            return [];
        }

        // Use SemaphoreSlim to throttle concurrent API calls
        using var semaphore = new SemaphoreSlim(MaxConcurrentApiCalls, MaxConcurrentApiCalls);
        var allConcepts = new System.Collections.Concurrent.ConcurrentBag<Concept>();
        var processedCount = 0;
        var errors = new System.Collections.Concurrent.ConcurrentBag<(int Index, string Error)>();

        // Create tasks for all chunks
        var tasks = chunks.Select(async (chunk, index) =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                // Small staggered delay to avoid burst requests
                if (index > 0)
                {
                    await Task.Delay(index * ChunkDelayMs, ct);
                }

                ct.ThrowIfCancellationRequested();
                
                Log.Debug($"Processing chunk {index + 1}/{chunks.Count} ({chunk.Length} chars)");

                var response = await CallAiAsync<ConceptExtractionResponse>(
                    ConceptExtractionPrompt,
                    chunk,
                    ct);

                if (response?.Concepts != null && response.Concepts.Count > 0)
                {
                    var conceptsFromChunk = response.Concepts
                        .Where(c => !string.IsNullOrWhiteSpace(c.Term))
                        .Select(c => new Concept
                        {
                            Title = c.Term.Trim(),
                            Summary = c.Description?.Trim() ?? "",
                            Aliases = c.Aliases ?? [],
                            Tags = c.Tags ?? [],
                            ConceptMapId = conceptMapId,
                            SourceResourceIds = ["combined"],
                            ConfidenceScore = c.Confidence
                        })
                        .ToList();

                    foreach (var concept in conceptsFromChunk)
                    {
                        allConcepts.Add(concept);
                    }
                    
                    var completed = Interlocked.Increment(ref processedCount);
                    Log.Debug($"Chunk {index + 1}: Extracted {conceptsFromChunk.Count} concepts (completed {completed}/{chunks.Count})");
                    
                    // Report progress
                    var progress = (int)(20 + (completed * 20.0 / chunks.Count)); // 20-40% range
                    ReportProgress(null!, ConceptMapStatus.ExtractingConcepts, progress,
                        $"Processed {completed}/{chunks.Count} chunks ({allConcepts.Count} concepts found)");
                }
                else
                {
                    Interlocked.Increment(ref processedCount);
                    Log.Warn($"Chunk {index + 1}/{chunks.Count}: No concepts extracted - response was null or empty");
                }
            }
            catch (OperationCanceledException)
            {
                throw; // Let cancellation propagate
            }
            catch (Exception ex)
            {
                errors.Add((index + 1, ex.Message));
                Log.Error($"Error extracting concepts from chunk {index + 1}: {ex.Message}", ex);
                // Don't throw - continue processing other chunks
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        // Wait for all tasks to complete
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            Log.Warn("Concept extraction cancelled by user");
            throw;
        }

        // Log any errors that occurred
        if (!errors.IsEmpty)
        {
            Log.Warn($"ExtractConceptsAsync: {errors.Count} chunk(s) failed to process");
            foreach (var (idx, err) in errors.OrderBy(e => e.Index))
            {
                Log.Debug($"  Chunk {idx}: {err}");
            }
        }

        var conceptList = allConcepts.ToList();
        Log.Info($"ExtractConceptsAsync: Extracted {conceptList.Count} total concepts from {chunks.Count} chunks");

        // If we couldn't extract any concepts, throw an error with more details
        if (conceptList.Count == 0 && chunks.Count > 0)
        {
            var errorDetail = errors.IsEmpty 
                ? "The AI may not have returned data in the expected JSON format."
                : $"Errors occurred: {string.Join("; ", errors.Take(3).Select(e => e.Error))}";
            
            Log.Error($"Failed to extract any concepts from {chunks.Count} chunk(s). {errorDetail}");
            throw new InvalidOperationException(
                $"Failed to extract concepts from {chunks.Count} content chunk(s). {errorDetail} " +
                "Ensure your content has meaningful educational material.");
        }

        // Merge duplicate concepts
        return MergeDuplicateConcepts(conceptList);
    }


    /// <summary>
    /// Builds relationships between Concepts using AI analysis.
    /// Uses chunking and parallel processing for large concept sets.
    /// </summary>
    public async Task<List<ConceptRelationship>> BuildRelationshipsAsync(
        List<Concept> concepts,
        CancellationToken ct = default)
    {
        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is missing.");

        if (concepts.Count == 0)
            return [];

        Log.Info($"BuildRelationshipsAsync: Processing {concepts.Count} concepts");

        // Build concept lookup for converting responses
        var conceptLookup = concepts.ToDictionary(
            c => c.Title.ToLowerInvariant(),
            c => c.Id);

        // For small sets, process in one call
        if (concepts.Count <= MaxConceptsPerRelationshipChunk)
        {
            Log.Debug($"BuildRelationshipsAsync: Small set, processing in single call");
            return await ExtractRelationshipsForChunkAsync(concepts, conceptLookup, ct);
        }

        // For large sets, chunk the concepts and process in parallel
        var conceptChunks = ChunkList(concepts, MaxConceptsPerRelationshipChunk);
        Log.Info($"BuildRelationshipsAsync: Split into {conceptChunks.Count} chunks for parallel processing");

        using var semaphore = new SemaphoreSlim(MaxConcurrentApiCalls, MaxConcurrentApiCalls);
        var allRelationships = new System.Collections.Concurrent.ConcurrentBag<ConceptRelationship>();
        var processedCount = 0;

        var tasks = conceptChunks.Select(async (chunk, index) =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                if (index > 0)
                {
                    await Task.Delay(index * ChunkDelayMs, ct);
                }

                ct.ThrowIfCancellationRequested();

                Log.Debug($"Processing relationship chunk {index + 1}/{conceptChunks.Count} ({chunk.Count} concepts)");

                var chunkRelationships = await ExtractRelationshipsForChunkAsync(chunk, conceptLookup, ct);

                foreach (var rel in chunkRelationships)
                {
                    allRelationships.Add(rel);
                }

                var completed = Interlocked.Increment(ref processedCount);
                Log.Debug($"Relationship chunk {index + 1}: Found {chunkRelationships.Count} relationships (completed {completed}/{conceptChunks.Count})");

                // Report progress (50-70% range)
                var progress = (int)(50 + (completed * 20.0 / conceptChunks.Count));
                ReportProgress(null!, ConceptMapStatus.BuildingRelationships, progress,
                    $"Processed {completed}/{conceptChunks.Count} chunks ({allRelationships.Count} relationships found)");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"Error processing relationship chunk {index + 1}: {ex.Message}", ex);
                // Continue with other chunks
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            Log.Warn("Relationship extraction cancelled by user");
            throw;
        }

        var result = allRelationships.ToList();
        Log.Info($"BuildRelationshipsAsync: Extracted {result.Count} total relationships");
        
        // Deduplicate relationships (same source-target-type)
        return DeduplicateRelationships(result);
    }

    /// <summary>
    /// Extracts relationships for a chunk of concepts.
    /// </summary>
    private async Task<List<ConceptRelationship>> ExtractRelationshipsForChunkAsync(
        List<Concept> concepts,
        Dictionary<string, string> conceptLookup,
        CancellationToken ct)
    {
        var conceptList = string.Join("\n", concepts.Select(c =>
            $"- {c.Title}: {c.Summary}"));

        var prompt = RelationshipExtractionPrompt.Replace("{CONCEPTS}", conceptList);

        var response = await CallAiAsync<RelationshipExtractionResponse>(
            prompt,
            "",
            ct);

        if (response?.Relationships == null || response.Relationships.Count == 0)
            return [];

        var relationships = new List<ConceptRelationship>();

        foreach (var rel in response.Relationships)
        {
            var sourceKey = rel.SourceConceptTerm?.ToLowerInvariant() ?? "";
            var targetKey = rel.TargetConceptTerm?.ToLowerInvariant() ?? "";

            if (!conceptLookup.TryGetValue(sourceKey, out var sourceId) ||
                !conceptLookup.TryGetValue(targetKey, out var targetId))
                continue;

            relationships.Add(new ConceptRelationship
            {
                SourceConceptId = sourceId,
                TargetConceptId = targetId,
                RelationType = ParseRelationType(rel.Type),
                ConfidenceScore = rel.Strength,
                Justification = rel.Description ?? ""
            });
        }

        return relationships;
    }

    /// <summary>
    /// Removes duplicate relationships (same source, target, and type).
    /// </summary>
    private static List<ConceptRelationship> DeduplicateRelationships(List<ConceptRelationship> relationships)
    {
        var seen = new HashSet<string>();
        var result = new List<ConceptRelationship>();

        foreach (var rel in relationships.OrderByDescending(r => r.ConfidenceScore))
        {
            var key = $"{rel.SourceConceptId}|{rel.TargetConceptId}|{rel.RelationType}";
            if (seen.Add(key))
            {
                result.Add(rel);
            }
        }

        return result;
    }

    /// <summary>
    /// Splits a list into chunks of the specified size.
    /// </summary>
    private static List<List<T>> ChunkList<T>(List<T> source, int chunkSize)
    {
        var chunks = new List<List<T>>();
        for (var i = 0; i < source.Count; i += chunkSize)
        {
            chunks.Add(source.Skip(i).Take(chunkSize).ToList());
        }
        return chunks;
    }

    /// <summary>
    /// Calculates complexity ordering for Concepts based on their relationships.
    /// Concepts with no prerequisites are foundational (level 0).
    /// </summary>
    public List<ConceptComplexity> CalculateComplexityOrder(
        List<Concept> concepts,
        List<ConceptRelationship> relations)
    {
        var result = new List<ConceptComplexity>();

        // Build adjacency lists for prerequisite relationships
        var prerequisites = new Dictionary<string, HashSet<string>>();
        var dependents = new Dictionary<string, HashSet<string>>();

        foreach (var concept in concepts)
        {
            prerequisites[concept.Id] = [];
            dependents[concept.Id] = [];
        }

        foreach (var rel in relations.Where(r => r.RelationType == ConceptRelationType.Prerequisite))
        {
            if (prerequisites.ContainsKey(rel.TargetConceptId) && 
                dependents.ContainsKey(rel.SourceConceptId))
            {
                prerequisites[rel.TargetConceptId].Add(rel.SourceConceptId);
                dependents[rel.SourceConceptId].Add(rel.TargetConceptId);
            }
        }

        // Calculate levels using topological sort approach
        var levels = new Dictionary<string, int>();
        var visited = new HashSet<string>();

        int CalculateLevel(string conceptId)
        {
            if (levels.TryGetValue(conceptId, out var existing))
                return existing;

            if (visited.Contains(conceptId))
                return 0; // Break cycles

            visited.Add(conceptId);

            var prereqs = prerequisites.GetValueOrDefault(conceptId) ?? [];
            var level = prereqs.Count == 0 
                ? 0 
                : prereqs.Max(p => CalculateLevel(p)) + 1;

            levels[conceptId] = level;
            return level;
        }

        foreach (var concept in concepts)
        {
            visited.Clear();
            var level = CalculateLevel(concept.Id);
            var prereqCount = prerequisites.GetValueOrDefault(concept.Id)?.Count ?? 0;
            var depCount = dependents.GetValueOrDefault(concept.Id)?.Count ?? 0;

            // Calculate centrality as ratio of connections to total possible
            var totalConnections = prereqCount + depCount;
            var maxConnections = (concepts.Count - 1) * 2;
            var centrality = maxConnections > 0 
                ? (float)totalConnections / maxConnections 
                : 0f;

            result.Add(new ConceptComplexity
            {
                ConceptId = concept.Id,
                Level = level,
                PrerequisiteCount = prereqCount,
                DependentCount = depCount,
                Centrality = centrality
            });
        }

        return result.OrderBy(c => c.Level).ThenBy(c => c.PrerequisiteCount).ToList();
    }

    // Helper methods

    /// <summary>
    /// Call OpenAI API with automatic retry for rate limiting.
    /// </summary>
    private async Task<T?> CallAiAsync<T>(string systemPrompt, string content, CancellationToken ct)
    {
        return await CallAiWithRetryAsync<T>(systemPrompt, content, ct, maxRetries: 5);
    }

    /// <summary>
    /// Call OpenAI API with retry logic for rate limiting and transient errors.
    /// </summary>
    private async Task<T?> CallAiWithRetryAsync<T>(
        string systemPrompt, 
        string content, 
        CancellationToken ct,
        int maxRetries = 5)
    {
        var baseDelayMs = 2000; // Start with 2 seconds
        Exception? lastException = null;
        
        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await CallAiCoreAsync<T>(systemPrompt, content, ct);
            }
            catch (InvalidOperationException ex) when (
                ex.Message.Contains("429") || 
                ex.Message.Contains("Rate") ||
                ex.Message.Contains("overloaded") ||
                ex.Message.Contains("502") ||
                ex.Message.Contains("503"))
            {
                lastException = ex;
                
                if (attempt == maxRetries)
                {
                    Log.Error($"ConceptMapService: API call failed after {maxRetries + 1} attempts");
                    throw;
                }
                
                // Exponential backoff: 2s, 4s, 8s, 16s, 32s
                var delayMs = baseDelayMs * (int)Math.Pow(2, attempt);
                
                // Try to extract wait time from error message
                var waitMatch = System.Text.RegularExpressions.Regex.Match(ex.Message, @"try again in (\d+\.?\d*)s");
                if (waitMatch.Success && double.TryParse(waitMatch.Groups[1].Value, 
                    System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    out var waitSeconds))
                {
                    delayMs = (int)(waitSeconds * 1000) + 500; // Add 500ms buffer
                }
                
                Log.Warn($"ConceptMapService: Rate limited or server error, waiting {delayMs}ms before retry {attempt + 1}/{maxRetries}");
                await Task.Delay(delayMs, ct);
            }
        }
        
        // Should never reach here, but compiler needs it
        throw lastException ?? new InvalidOperationException("Unexpected retry loop exit");
    }

    /// <summary>
    /// Core API call implementation (no retry logic).
    /// </summary>
    private async Task<T?> CallAiCoreAsync<T>(string systemPrompt, string content, CancellationToken ct)
    {
        var apiKey = await opt.GetApiKeyAsync();
        
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("CallAiAsync: OpenAI API key is missing");
            throw new InvalidOperationException("OpenAI API key is missing. Configure it in Settings ? Credentials.");
        }
        
        var contentPreview = content.Length > 100 ? content[..100] + "..." : content;
        Log.Info($"CallAiAsync: Starting API request to OpenAI (model: {opt.Model}, content: {content.Length} chars)");
        Log.Trace($"CallAiAsync: Content preview: {contentPreview}");

        var input = string.IsNullOrWhiteSpace(content)
            ? "Please analyze and respond."
            : $"Content to analyze:\n\n{content}";

        var payload = new
        {
            model = opt.Model,
            input = input,
            instructions = systemPrompt
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            Log.Trace($"CallAiAsync: Sending HTTP request...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            // Log all non-200 responses
            if (statusCode != 200)
            {
                Log.Error($"OpenAI API non-200 response: HTTP {statusCode} {resp.ReasonPhrase}");
                Log.Debug($"OpenAI error response body: {(json.Length > 500 ? json[..500] + "..." : json)}");
                
                // Provide specific error messages based on status code
                var errorMessage = statusCode switch
                {
                    400 => $"Bad Request - Invalid API parameters: {json}",
                    401 => "Unauthorized - Invalid or missing API key",
                    403 => "Forbidden - API key doesn't have access to this resource",
                    404 => "Not Found - Invalid API endpoint or model",
                    429 => "Rate Limited - Too many requests, please wait and try again",
                    500 => "OpenAI Server Error - Please try again later",
                    502 => "Bad Gateway - OpenAI service temporarily unavailable",
                    503 => "Service Unavailable - OpenAI is overloaded, try again later",
                    _ => $"OpenAI API error: {json}"
                };
                
                throw new InvalidOperationException($"OpenAI HTTP {statusCode}: {errorMessage}");
            }
            
            Log.Debug($"OpenAI API response received: HTTP {statusCode} ({json.Length} chars)");
            Log.Trace($"OpenAI response preview: {(json.Length > 200 ? json[..200] + "..." : json)}");

            var responseText = ExtractResponseText(json);
            
            if (string.IsNullOrWhiteSpace(responseText))
            {
                Log.Warn("OpenAI response text extraction returned empty string");
            }
            else
            {
                Log.Trace($"Extracted response text: {(responseText.Length > 200 ? responseText[..200] + "..." : responseText)}");
            }
            
            var result = ParseJsonResponse<T>(responseText);
            
            if (result == null)
            {
                Log.Warn($"Failed to parse OpenAI response to {typeof(T).Name}");
                Log.Debug($"Unparseable response: {(responseText.Length > 500 ? responseText[..500] + "..." : responseText)}");
            }
            else
            {
                Log.Debug($"Successfully parsed response to {typeof(T).Name}");
            }
            
            return result;
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"HTTP request to OpenAI failed: {ex.Message} (Status: {ex.StatusCode})", ex);
            throw new InvalidOperationException($"Network error calling OpenAI: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            Log.Error($"OpenAI API request timed out after waiting", ex);
            throw new InvalidOperationException("OpenAI API request timed out - the server took too long to respond", ex);
        }
        catch (TaskCanceledException)
        {
            Log.Debug("OpenAI API request was cancelled by user");
            throw;
        }
        catch (JsonException ex)
        {
            Log.Error($"Failed to parse OpenAI response JSON: {ex.Message}", ex);
            throw new InvalidOperationException($"Invalid JSON response from OpenAI: {ex.Message}", ex);
        }
    }

    private static string ExtractResponseText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("output_text", out var outText) &&
                outText.ValueKind == JsonValueKind.String)
            {
                return outText.GetString() ?? "";
            }

            if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0)
            {
                var first = choices[0];
                if (first.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var msgContent))
                {
                    return msgContent.GetString() ?? "";
                }
            }

            if (doc.RootElement.TryGetProperty("output", out var output))
            {
                if (output.ValueKind == JsonValueKind.Array && output.GetArrayLength() > 0)
                {
                    foreach (var item in output.EnumerateArray())
                    {
                        if (item.TryGetProperty("content", out var contentArr) &&
                            contentArr.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var c in contentArr.EnumerateArray())
                            {
                                if (c.TryGetProperty("text", out var txt))
                                    return txt.GetString() ?? "";
                            }
                        }
                    }
                }
            }


            return json;
        }
        catch
        {
            return json;
        }
    }

    private static T? ParseJsonResponse<T>(string text)
    {
        Log.Trace($"ParseJsonResponse: Received text length: {text?.Length ?? 0}");
        
        if (string.IsNullOrEmpty(text))
        {
            Log.Warn("ParseJsonResponse: Received empty response text");
            return default;
        }
        
        var jsonStart = text.IndexOf('{');
        var jsonEnd = text.LastIndexOf('}');

        if (jsonStart >= 0 && jsonEnd > jsonStart)
        {
            var jsonText = text[jsonStart..(jsonEnd + 1)];
            try
            {
                var result = JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                Log.Debug($"Successfully parsed JSON response to {typeof(T).Name}");
                return result;
            }
            catch (JsonException ex)
            {
                Log.Warn($"Failed to parse extracted JSON to {typeof(T).Name}: {ex.Message}");
            }
        }
        else
        {
            Log.Warn($"No valid JSON object found in response (jsonStart={jsonStart}, jsonEnd={jsonEnd})");
        }

        try
        {
            return JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            Log.Error($"Failed to parse raw response to {typeof(T).Name}: {ex.Message}");
            return default;
        }
    }

    private static List<string> SplitIntoChunks(string content, int maxChunkSize)
    {
        var chunks = new List<string>();

        if (string.IsNullOrWhiteSpace(content))
            return chunks;

        // First try to split by paragraphs
        var paragraphs = content.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);

        var currentChunk = new StringBuilder();
        foreach (var para in paragraphs)
        {
            // If a single paragraph exceeds maxChunkSize, we need to split it further
            if (para.Length > maxChunkSize)
            {
                // Flush current chunk first if not empty
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }
                
                // Split the large paragraph by sentences or force-split
                var subChunks = SplitLargeParagraph(para, maxChunkSize);
                chunks.AddRange(subChunks);
                continue;
            }

            if (currentChunk.Length + para.Length > maxChunkSize && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();
            }

            currentChunk.AppendLine(para);
            currentChunk.AppendLine();
        }

        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());

        // Filter out tiny chunks that are just whitespace or too small to be useful
        return chunks.Where(c => c.Trim().Length > 50).ToList();
    }

    /// <summary>
    /// Split a large paragraph that exceeds maxChunkSize by sentences or force-splitting.
    /// </summary>
    private static List<string> SplitLargeParagraph(string paragraph, int maxChunkSize)
    {
        var chunks = new List<string>();
        
        // Try to split by sentences first
        var sentences = System.Text.RegularExpressions.Regex.Split(paragraph, @"(?<=[.!?])\s+");
        
        var currentChunk = new StringBuilder();
        foreach (var sentence in sentences)
        {
            // If a single sentence is too long, force-split it
            if (sentence.Length > maxChunkSize)
            {
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }
                
                // Force split by character count
                for (int i = 0; i < sentence.Length; i += maxChunkSize)
                {
                    var length = Math.Min(maxChunkSize, sentence.Length - i);
                    chunks.Add(sentence.Substring(i, length));
                }
                continue;
            }
            
            if (currentChunk.Length + sentence.Length > maxChunkSize && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();
            }
            
            currentChunk.Append(sentence);
            currentChunk.Append(' ');
        }
        
        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());
        
        return chunks;
    }

    private static List<Concept> MergeDuplicateConcepts(List<Concept> concepts)
    {
        var merged = new Dictionary<string, Concept>(StringComparer.OrdinalIgnoreCase);

        foreach (var concept in concepts)
        {
            var key = concept.Title.ToLowerInvariant().Trim();

            if (merged.TryGetValue(key, out var existing))
            {
                // Merge: keep longer description, combine aliases and tags
                if (concept.Summary.Length > existing.Summary.Length)
                    existing.Summary = concept.Summary;

                existing.Aliases = existing.Aliases.Union(concept.Aliases, StringComparer.OrdinalIgnoreCase).ToList();
                existing.Tags = existing.Tags.Union(concept.Tags, StringComparer.OrdinalIgnoreCase).ToList();
                existing.ConfidenceScore = Math.Max(existing.ConfidenceScore, concept.ConfidenceScore);
            }
            else
            {
                merged[key] = concept;
            }
        }

        return merged.Values.ToList();
    }

    private static ConceptRelationType ParseRelationType(string? type)
    {
        return type?.ToLowerInvariant() switch
        {
            "prerequisite" => ConceptRelationType.Prerequisite,
            "related" => ConceptRelationType.Related,
            "contains" => ConceptRelationType.Contains,
            "instanceof" => ConceptRelationType.InstanceOf,
            "similarto" => ConceptRelationType.SimilarTo,
            "contrastswith" => ConceptRelationType.ContrastsWith,
            _ => ConceptRelationType.Related
        };
    }

    private void ReportProgress(
        ConceptMap? cm,
        ConceptMapStatus status,
        int progress,
        string message,
        string? errorMessage = null)
    {
        if (cm is not null)
        {
            cm.Progress = progress;
        }

        OnProgressChanged?.Invoke(new ConceptMapBuildProgress
        {
            Status = status,
            Progress = progress,
            Message = message,
            ConceptsExtracted = cm?.Concepts.Count ?? 0,
            ErrorMessage = errorMessage
        });
    }
}

// DTOs for AI responses

internal class ConceptExtractionResponse
{
    public List<ExtractedConceptDto> Concepts { get; set; } = [];
}

internal class ExtractedConceptDto
{
    public string Term { get; set; } = "";
    public string? Description { get; set; }
    public List<string>? Aliases { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? PotentialPrerequisites { get; set; }
    public List<string>? RelatedTerms { get; set; }
    public float Confidence { get; set; } = 0.8f;
}

internal class RelationshipExtractionResponse
{
    public List<RelationshipDto> Relationships { get; set; } = [];
}

internal class RelationshipDto
{
    public string? SourceConceptTerm { get; set; }
    public string? TargetConceptTerm { get; set; }
    public string? Type { get; set; }
    public float Strength { get; set; } = 0.5f;
    public string? Description { get; set; }
}
