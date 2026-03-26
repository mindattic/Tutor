using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tutor.Core.Models;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Request for dynamic concept expansion based on a user query.
/// </summary>
public class ConceptExpansionRequest
{
    /// <summary>
    /// The user's query that triggered the expansion.
    /// </summary>
    public string Query { get; set; } = "";

    /// <summary>
    /// The course ID to expand concepts within.
    /// </summary>
    public string CourseId { get; set; } = "";

    /// <summary>
    /// IDs of existing concepts that are contextually relevant.
    /// These help establish relationship anchors for new concepts.
    /// </summary>
    public List<string> ContextConceptIds { get; set; } = [];

    /// <summary>
    /// Maximum number of new concepts to extract.
    /// </summary>
    public int MaxNewConcepts { get; set; } = 10;

    /// <summary>
    /// Minimum confidence threshold for including a concept (0.0 to 1.0).
    /// </summary>
    public float MinConfidence { get; set; } = 0.7f;
}

/// <summary>
/// Result of a dynamic concept expansion operation.
/// </summary>
public class ConceptExpansionResult
{
    /// <summary>
    /// Whether the expansion was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if expansion failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// New concepts that were extracted and added.
    /// </summary>
    public List<Concept> NewConcepts { get; set; } = [];

    /// <summary>
    /// New relationships created (both between new concepts and to existing ones).
    /// </summary>
    public List<ConceptRelationship> NewRelationships { get; set; } = [];

    /// <summary>
    /// Source citations from resources that grounded the expansion.
    /// Maps concept ID to the resource excerpts that support it.
    /// </summary>
    public Dictionary<string, List<string>> SourceCitations { get; set; } = [];

    /// <summary>
    /// IDs of ConceptMaps that were modified.
    /// </summary>
    public List<string> ModifiedConceptMapIds { get; set; } = [];

    /// <summary>
    /// The original query that triggered this expansion.
    /// </summary>
    public string OriginalQuery { get; set; } = "";

    /// <summary>
    /// When the expansion was performed.
    /// </summary>
    public DateTime ExpandedAt { get; set; } = DateTime.UtcNow;

    public static ConceptExpansionResult Failed(string error) => new()
    {
        Success = false,
        ErrorMessage = error
    };

    public static ConceptExpansionResult NoExpansionNeeded() => new()
    {
        Success = true,
        ErrorMessage = null
    };
}

/// <summary>
/// Service for dynamically expanding the concept graph based on user queries.
/// 
/// When a user asks about something related to existing concepts (e.g., "Tell me about 
/// the Weasley family" when Ron Weasley exists), this service:
/// 1. Searches the course resources for relevant content
/// 2. Extracts new concepts from that content (grounded in source material)
/// 3. Links new concepts to existing ones based on relationships found in content
/// 4. Persists the expanded concepts to the appropriate ConceptMap(s)
/// 
/// Key principle: Resources are the source of truth - no concepts are invented.
/// </summary>
public sealed class DynamicConceptExpansionService
{
    private readonly LlmServiceRouter router;
    private readonly VectorStoreService vectorStore;
    private readonly EmbeddingService embeddingService;
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly ConceptMapCollectionService collectionService;

    private const int MaxContentChunks = 8;
    private const int MaxContentLength = 12000;
    private const float MinSearchSimilarity = 0.3f;
    private const int LshThreshold = 64;

    private const string ConceptExpansionPrompt = """
        You are an expert knowledge analyst helping to expand a knowledge graph.
        
        CONTEXT:
        A student asked: "{QUERY}"
        
        The knowledge graph already contains these related concepts:
        {EXISTING_CONCEPTS}
        
        SOURCE MATERIAL (from course resources - this is the ONLY source of truth):
        {SOURCE_CONTENT}
        
        TASK:
        Identify NEW concepts from the source material that are:
        1. Directly relevant to the student's query
        2. Related to the existing concepts but NOT already in the graph
        3. Clearly supported by the source material (no inventing information)
        
        For each new concept, identify:
        - The term/name (canonical form)
        - A concise definition (1-2 sentences from the source)
        - How it relates to existing concepts (prerequisite, related, contains, etc.)
        - The exact text from the source that supports this concept
        
        Return JSON:
        {
            "newConcepts": [
                {
                    "term": "Concept Name",
                    "description": "Definition from source material",
                    "sourceExcerpt": "Exact quote from source that defines/mentions this",
                    "confidence": 0.95,
                    "relatedExistingConcepts": [
                        {
                            "existingConceptTitle": "Ron Weasley",
                            "relationshipType": "related",
                            "relationshipDescription": "sibling relationship"
                        }
                    ]
                }
            ],
            "conceptRelationships": [
                {
                    "sourceTerm": "New Concept A",
                    "targetTerm": "New Concept B",
                    "type": "related",
                    "description": "Brief explanation"
                }
            ]
        }
        
        IMPORTANT:
        - ONLY extract concepts that are clearly present in the source material
        - Every concept MUST have a sourceExcerpt to prove it exists in the content
        - Do NOT invent or assume information not in the sources
        - Focus on entities, characters, terms, and concepts mentioned in the query context
        """;

    public DynamicConceptExpansionService(
        LlmServiceRouter router,
        VectorStoreService vectorStore,
        EmbeddingService embeddingService,
        CourseService courseService,
        ConceptMapStorageService conceptMapStorage,
        ConceptMapCollectionService collectionService)
    {
        this.router = router;
        this.vectorStore = vectorStore;
        this.embeddingService = embeddingService;
        this.courseService = courseService;
        this.conceptMapStorage = conceptMapStorage;
        this.collectionService = collectionService;
    }

    /// <summary>
    /// Expands the concept graph based on a user query.
    /// Searches resources for relevant content and extracts new concepts grounded in that content.
    /// </summary>
    public async Task<ConceptExpansionResult> ExpandConceptsAsync(
        ConceptExpansionRequest request,
        CancellationToken ct = default)
    {
        Log.Info($"DynamicExpansion: Starting expansion for query: '{request.Query}' in course {request.CourseId}");

        try
        {
            // Step 1: Get the course and its resources
            var course = await courseService.GetCourseAsync(request.CourseId);
            if (course == null)
            {
                return ConceptExpansionResult.Failed($"Course not found: {request.CourseId}");
            }

            // Step 2: Load existing concepts for context
            var existingConcepts = await LoadExistingConceptsAsync(course, request.ContextConceptIds, ct);
            if (existingConcepts.Count == 0)
            {
                Log.Warn("DynamicExpansion: No existing concepts found for context");
            }

            // Step 3: Search resources for relevant content
            var relevantContent = await SearchResourcesForExpansionAsync(
                request.Query,
                course,
                ct);

            if (string.IsNullOrWhiteSpace(relevantContent))
            {
                Log.Info("DynamicExpansion: No relevant content found in resources");
                return ConceptExpansionResult.NoExpansionNeeded();
            }

            // Step 4: Extract new concepts using AI (grounded in source content)
            var expansionResponse = await ExtractNewConceptsAsync(
                request.Query,
                existingConcepts,
                relevantContent,
                ct);

            if (expansionResponse == null || expansionResponse.NewConcepts.Count == 0)
            {
                Log.Info("DynamicExpansion: No new concepts extracted");
                return ConceptExpansionResult.NoExpansionNeeded();
            }

            // Step 5: Filter by confidence and validate against source
            var validConcepts = expansionResponse.NewConcepts
                .Where(c => c.Confidence >= request.MinConfidence)
                .Where(c => !string.IsNullOrWhiteSpace(c.SourceExcerpt))
                .Take(request.MaxNewConcepts)
                .ToList();

            if (validConcepts.Count == 0)
            {
                Log.Info("DynamicExpansion: No concepts met confidence threshold");
                return ConceptExpansionResult.NoExpansionNeeded();
            }

            // Step 6: Create Concept objects and relationships
            var result = await CreateAndPersistConceptsAsync(
                course,
                existingConcepts,
                validConcepts,
                expansionResponse.ConceptRelationships ?? [],
                request.Query,
                ct);

            Log.Info($"DynamicExpansion: Successfully added {result.NewConcepts.Count} concepts and {result.NewRelationships.Count} relationships");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error($"DynamicExpansion: Failed - {ex.Message}", ex);
            return ConceptExpansionResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Loads existing concepts from the course's ConceptMapCollection.
    /// </summary>
    private async Task<List<Concept>> LoadExistingConceptsAsync(
        Course course,
        List<string> contextConceptIds,
        CancellationToken ct)
    {
        var allConcepts = new List<Concept>();

        // Load resources for the course via CourseService
        var resources = await courseService.GetCourseResourcesAsync(course.Id);

        // Load all ConceptMaps for resources that have them
        foreach (var resource in resources.Where(r => r.HasConceptMap))
        {
            var conceptMap = await conceptMapStorage.LoadAsync(resource.ConceptMapId!, ct);
            if (conceptMap?.Concepts != null)
            {
                allConcepts.AddRange(conceptMap.Concepts);
            }
        }

        // If specific context concepts are requested, prioritize those
        if (contextConceptIds.Count > 0)
        {
            var contextSet = contextConceptIds.ToHashSet();
            var contextConcepts = allConcepts.Where(c => contextSet.Contains(c.Id)).ToList();
            var otherConcepts = allConcepts.Where(c => !contextSet.Contains(c.Id)).ToList();
            
            // Return context concepts first, then others (limited)
            return [.. contextConcepts, .. otherConcepts.Take(50)];
        }


        return allConcepts.Take(100).ToList(); // Limit to avoid token overflow
    }

    /// <summary>
    /// Searches course resources for content relevant to the query.
    /// </summary>
    private async Task<string> SearchResourcesForExpansionAsync(
        string query,
        Course course,
        CancellationToken ct)
    {
        var relevantChunks = new List<ContentChunk>();

        // Search using vector store if the course has embedded content
        if (!string.IsNullOrEmpty(course.Id))
        {
            var chunks = await vectorStore.SearchAsync(
                query,
                course.Id,
                embeddingService,
                MaxContentChunks,
                MinSearchSimilarity,
                LshThreshold,
                ct);

            relevantChunks.AddRange(chunks);
        }

        // If no chunks found via vector search, fall back to keyword search in resources
        if (relevantChunks.Count == 0)
        {
            Log.Debug("DynamicExpansion: Vector search found no results, trying keyword fallback");
            var resources = await courseService.GetCourseResourcesAsync(course.Id);
            var keywordContent = SearchResourcesByKeyword(query, resources);
            if (!string.IsNullOrWhiteSpace(keywordContent))
            {
                return TruncateContent(keywordContent);
            }
        }

        // Combine chunk content
        if (relevantChunks.Count > 0)
        {
            var combined = new StringBuilder();
            foreach (var chunk in relevantChunks.OrderByDescending(c => c.ChunkIndex))
            {
                combined.AppendLine($"[Source: {chunk.ResourceId}]");
                combined.AppendLine(chunk.Content);
                combined.AppendLine("---");
            }
            return TruncateContent(combined.ToString());
        }

        return "";
    }

    /// <summary>
    /// Simple keyword search fallback when vector search fails.
    /// </summary>
    private static string SearchResourcesByKeyword(string query, List<CourseResource> resources)
    {
        var keywords = query.ToLowerInvariant()
            .Split([' ', ',', '.', '?', '!'], StringSplitOptions.RemoveEmptyEntries)
            .Where(k => k.Length > 3)
            .ToHashSet();

        var relevantParagraphs = new List<string>();

        foreach (var resource in resources)
        {
            var content = resource.GetEffectiveContent();
            if (string.IsNullOrWhiteSpace(content)) continue;

            // Split into paragraphs and find those containing keywords
            var paragraphs = content.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var para in paragraphs)
            {
                var paraLower = para.ToLowerInvariant();
                if (keywords.Any(k => paraLower.Contains(k, StringComparison.Ordinal)))
                {
                    relevantParagraphs.Add($"[{resource.Title}]\n{para}");
                }
            }
        }

        return string.Join("\n\n---\n\n", relevantParagraphs.Take(10));
    }

    /// <summary>
    /// Truncates content to fit within token limits.
    /// </summary>
    private static string TruncateContent(string content)
    {
        if (content.Length <= MaxContentLength)
            return content;

        return content[..MaxContentLength] + "\n\n[Content truncated...]";
    }

    /// <summary>
    /// Uses AI to extract new concepts from the source content.
    /// </summary>
    private async Task<ConceptExpansionResponse?> ExtractNewConceptsAsync(
        string query,
        List<Concept> existingConcepts,
        string sourceContent,
        CancellationToken ct)
    {
        // Format existing concepts for the prompt
        var existingConceptsText = existingConcepts.Count > 0
            ? string.Join("\n", existingConcepts.Take(30).Select(c => $"- {c.Title}: {c.Summary}"))
            : "(No existing concepts)";

        var prompt = ConceptExpansionPrompt
            .Replace("{QUERY}", query)
            .Replace("{EXISTING_CONCEPTS}", existingConceptsText)
            .Replace("{SOURCE_CONTENT}", sourceContent);

        var messages = new[] { new ChatMessage("user", prompt, prompt) };
        var reply = await router.GetReplyAsync(messages, "Return only valid JSON. Do not include any explanatory text outside the JSON structure.", ct);

        // Parse the response
        var responseText = reply.Text;
        return ParseExpansionResponse(responseText);
    }

    /// <summary>
    /// Parses the AI response into a structured expansion response.
    /// </summary>
    private static ConceptExpansionResponse? ParseExpansionResponse(string responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
            return null;

        try
        {
            // Try to find JSON in the response (it might be wrapped in markdown code blocks)
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonText = responseText[jsonStart..(jsonEnd + 1)];
                return JsonSerializer.Deserialize<ConceptExpansionResponse>(jsonText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }
        catch (Exception ex)
        {
            Log.Warn($"DynamicExpansion: Failed to parse expansion response - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Creates Concept objects and persists them to the appropriate ConceptMaps.
    /// </summary>
    private async Task<ConceptExpansionResult> CreateAndPersistConceptsAsync(
        Course course,
        List<Concept> existingConcepts,
        List<ExtractedNewConcept> newConceptDtos,
        List<ExtractedConceptRelationship> newRelationshipDtos,
        string originalQuery,
        CancellationToken ct)
    {
        var result = new ConceptExpansionResult
        {
            Success = true,
            OriginalQuery = originalQuery
        };

        // Build lookup for existing concepts
        var existingLookup = existingConcepts.ToDictionary(
            c => c.Title.ToLowerInvariant(),
            c => c);

        // Find the primary ConceptMap to add new concepts to
        // Use the first resource that has a ConceptMap, or create a dynamic expansion map
        var targetConceptMap = await GetOrCreateExpansionConceptMapAsync(course, ct);
        if (targetConceptMap == null)
        {
            return ConceptExpansionResult.Failed("No ConceptMap available for expansion");
        }

        // Create Concept objects
        var newConcepts = new List<Concept>();
        var newConceptLookup = new Dictionary<string, Concept>();

        foreach (var dto in newConceptDtos)
        {
            // Skip if concept already exists
            if (existingLookup.ContainsKey(dto.Term.ToLowerInvariant()))
            {
                Log.Debug($"DynamicExpansion: Skipping '{dto.Term}' - already exists");
                continue;
            }

            var concept = new Concept
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Term,
                Summary = dto.Description,
                ConceptMapId = targetConceptMap.Id,
                ConfidenceScore = dto.Confidence,
                Tags = ["dynamic-expansion", "query-generated"],
                IsDynamicallyExpanded = true,
                ExpansionQuery = originalQuery,
                SourceExcerpt = dto.SourceExcerpt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            newConcepts.Add(concept);
            newConceptLookup[dto.Term.ToLowerInvariant()] = concept;

            // Store source citation
            if (!string.IsNullOrWhiteSpace(dto.SourceExcerpt))
            {
                result.SourceCitations[concept.Id] = [dto.SourceExcerpt];
            }

            // Create relationships to existing concepts
            if (dto.RelatedExistingConcepts != null)
            {
                foreach (var relDto in dto.RelatedExistingConcepts)
                {
                    if (existingLookup.TryGetValue(relDto.ExistingConceptTitle.ToLowerInvariant(), out var existingConcept))
                    {
                        var relationship = new ConceptRelationship
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceConceptId = concept.Id,
                            TargetConceptId = existingConcept.Id,
                            RelationType = ParseRelationType(relDto.RelationshipType),
                            Justification = relDto.RelationshipDescription ?? "",
                            ConfidenceScore = dto.Confidence
                        };
                        result.NewRelationships.Add(relationship);
                    }
                }
            }
        }

        // Create relationships between new concepts
        foreach (var relDto in newRelationshipDtos)
        {
            var sourceKey = relDto.SourceTerm?.ToLowerInvariant() ?? "";
            var targetKey = relDto.TargetTerm?.ToLowerInvariant() ?? "";

            Concept? sourceConcept = null;
            Concept? targetConcept = null;

            // Try new concepts first, then existing
            if (!newConceptLookup.TryGetValue(sourceKey, out sourceConcept))
                existingLookup.TryGetValue(sourceKey, out sourceConcept);

            if (!newConceptLookup.TryGetValue(targetKey, out targetConcept))
                existingLookup.TryGetValue(targetKey, out targetConcept);

            if (sourceConcept != null && targetConcept != null)
            {
                var relationship = new ConceptRelationship
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceConceptId = sourceConcept.Id,
                    TargetConceptId = targetConcept.Id,
                    RelationType = ParseRelationType(relDto.Type),
                    Justification = relDto.Description ?? "",
                    ConfidenceScore = 0.8f
                };
                result.NewRelationships.Add(relationship);
            }
        }

        // Add concepts and relationships to the ConceptMap
        if (newConcepts.Count > 0)
        {
            targetConceptMap.Concepts.AddRange(newConcepts);
            targetConceptMap.Relations.AddRange(result.NewRelationships);
            targetConceptMap.UpdatedAt = DateTime.UtcNow;
            targetConceptMap.Version++;

            await conceptMapStorage.SaveAsync(targetConceptMap, ct);

            result.ModifiedConceptMapIds.Add(targetConceptMap.Id);
            result.NewConcepts = newConcepts;

            Log.Info($"DynamicExpansion: Persisted {newConcepts.Count} concepts and {result.NewRelationships.Count} relationships to ConceptMap {targetConceptMap.Id}");
        }

        return result;
    }

    /// <summary>
    /// Gets an existing ConceptMap or creates a dedicated expansion ConceptMap for dynamic concepts.
    /// </summary>
    private async Task<ConceptMap?> GetOrCreateExpansionConceptMapAsync(Course course, CancellationToken ct)
    {
        // First, try to find an existing resource with a ConceptMap
        var resources = await courseService.GetCourseResourcesAsync(course.Id);
        foreach (var resource in resources.Where(r => r.HasConceptMap))
        {
            var cm = await conceptMapStorage.LoadAsync(resource.ConceptMapId!, ct);
            if (cm != null && cm.Status == ConceptMapStatus.Ready)
            {
                return cm;
            }
        }

        // If no existing ConceptMap, look for a dedicated expansion map
        var expansionMapId = $"expansion-{course.Id}";
        var expansionMap = await conceptMapStorage.LoadAsync(expansionMapId, ct);

        if (expansionMap != null)
            return expansionMap;

        // Create a new expansion ConceptMap
        expansionMap = new ConceptMap
        {
            Id = expansionMapId,
            Name = $"{course.Name} - Dynamic Expansions",
            Description = "Dynamically generated concepts from user queries",
            Status = ConceptMapStatus.Ready,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await conceptMapStorage.SaveAsync(expansionMap, ct);
        Log.Info($"DynamicExpansion: Created expansion ConceptMap {expansionMapId}");

        return expansionMap;
    }

    /// <summary>
    /// Parses a relationship type string to the enum.
    /// </summary>
    private static ConceptRelationType ParseRelationType(string? type)
    {
        return type?.ToLowerInvariant() switch
        {
            "prerequisite" => ConceptRelationType.Prerequisite,
            "contains" => ConceptRelationType.Contains,
            "instanceof" => ConceptRelationType.InstanceOf,
            "similarto" => ConceptRelationType.SimilarTo,
            "contrastswith" => ConceptRelationType.ContrastsWith,
            _ => ConceptRelationType.Related
        };
    }

    /// <summary>
    /// Checks if a query might benefit from concept expansion.
    /// Call this before the full expansion to avoid unnecessary processing.
    /// </summary>
    public async Task<bool> ShouldExpandForQueryAsync(
        string query,
        string courseId,
        CancellationToken ct = default)
    {
        // Simple heuristics to detect expansion-worthy queries
        var expansionIndicators = new[]
        {
            "who is", "what is", "tell me about", "explain", "describe",
            "family", "related to", "connection", "relationship",
            "more about", "other", "similar", "like"
        };

        var queryLower = query.ToLowerInvariant();
        
        // Check if query contains expansion indicators
        if (!expansionIndicators.Any(i => queryLower.Contains(i)))
            return false;

        // Check if the query mentions something not in the current concepts
        var course = await courseService.GetCourseAsync(courseId);
        if (course == null) return false;

        var resources = await courseService.GetCourseResourcesAsync(courseId);
        var existingTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var resource in resources.Where(r => r.HasConceptMap))
        {
            var cm = await conceptMapStorage.LoadAsync(resource.ConceptMapId!, ct);
            if (cm?.Concepts != null)
            {
                foreach (var concept in cm.Concepts)
                {
                    existingTitles.Add(concept.Title);
                    foreach (var alias in concept.Aliases)
                    {
                        existingTitles.Add(alias);
                    }
                }
            }
        }

        // Extract potential entity names from query (simple approach)
        var words = query.Split([' ', ',', '.', '?', '!'], StringSplitOptions.RemoveEmptyEntries);
        var capitalizedWords = words.Where(w => w.Length > 1 && char.IsUpper(w[0])).ToList();

        // If query mentions capitalized words not in concepts, might need expansion
        return capitalizedWords.Any(w => !existingTitles.Contains(w));
    }
}

#region Response DTOs

/// <summary>
/// AI response for concept expansion.
/// </summary>
internal class ConceptExpansionResponse
{
    [JsonPropertyName("newConcepts")]
    public List<ExtractedNewConcept> NewConcepts { get; set; } = [];

    [JsonPropertyName("conceptRelationships")]
    public List<ExtractedConceptRelationship>? ConceptRelationships { get; set; }
}

/// <summary>
/// A newly extracted concept from the AI.
/// </summary>
internal class ExtractedNewConcept
{
    [JsonPropertyName("term")]
    public string Term { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("sourceExcerpt")]
    public string SourceExcerpt { get; set; } = "";

    [JsonPropertyName("confidence")]
    public float Confidence { get; set; }

    [JsonPropertyName("relatedExistingConcepts")]
    public List<ExistingConceptRelation>? RelatedExistingConcepts { get; set; }
}

/// <summary>
/// Relationship to an existing concept.
/// </summary>
internal class ExistingConceptRelation
{
    [JsonPropertyName("existingConceptTitle")]
    public string ExistingConceptTitle { get; set; } = "";

    [JsonPropertyName("relationshipType")]
    public string RelationshipType { get; set; } = "related";

    [JsonPropertyName("relationshipDescription")]
    public string? RelationshipDescription { get; set; }
}

/// <summary>
/// Relationship between concepts (new to new or new to existing).
/// </summary>
internal class ExtractedConceptRelationship
{
    [JsonPropertyName("sourceTerm")]
    public string? SourceTerm { get; set; }

    [JsonPropertyName("targetTerm")]
    public string? TargetTerm { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

#endregion
