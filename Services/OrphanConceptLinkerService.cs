using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for detecting and automatically linking orphaned concepts in a ConceptMap.
/// Uses AI to infer missing connections between disconnected concept clusters.
/// </summary>
public sealed class OrphanConceptLinkerService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;

    private const string OrphanLinkingPrompt = """
        You are an expert knowledge analyst. Analyze these orphaned concepts and determine how they should connect to the main concept cluster.

        CONTEXT:
        These concepts all come from the same source material but are currently disconnected in the knowledge graph.
        Your task is to identify the most logical connections that would link the orphaned concepts back to the main cluster.

        MAIN CLUSTER CONCEPTS:
        {MAIN_CONCEPTS}

        ORPHANED CONCEPTS (need to be linked):
        {ORPHAN_CONCEPTS}

        SOURCE CONTENT CONTEXT:
        {SOURCE_CONTEXT}

        For each orphaned concept, identify:
        1. Which main cluster concept it should connect to
        2. The type of relationship (prerequisite, related, contains, instanceOf, similarTo, contrastsWith)
        3. The direction of the relationship
        4. A brief justification for the connection

        Return your analysis as JSON:
        {
            "suggestedLinks": [
                {
                    "orphanConceptTerm": "Name of orphaned concept",
                    "mainConceptTerm": "Name of main cluster concept to link to",
                    "relationshipType": "related",
                    "direction": "orphanToMain" or "mainToOrphan",
                    "confidence": 0.85,
                    "justification": "Brief explanation of why these concepts should be connected"
                }
            ],
            "additionalInsights": "Any observations about why these concepts may have been orphaned"
        }

        Guidelines:
        - Each orphaned concept should have at least one link to the main cluster
        - Prefer 'related' type for thematic connections, 'prerequisite' only for clear learning dependencies
        - Higher confidence for connections with clear textual evidence
        - Consider that orphaned concepts might bridge multiple main concepts
        """;

    public OrphanConceptLinkerService(HttpClient http, OpenAIOptions opt)
    {
        this.http = http;
        this.opt = opt;
    }

    /// <summary>
    /// Analyzes a ConceptMap and returns suggested links for orphaned concepts.
    /// </summary>
    public async Task<OrphanLinkingResult> AnalyzeOrphansAsync(
        ConceptMap conceptMap,
        CancellationToken ct = default)
    {
        var connectivity = conceptMap.GetConnectivityInfo();
        
        if (connectivity.IsFullyConnected)
        {
            Log.Info($"OrphanLinker: ConceptMap '{conceptMap.Name}' is fully connected, no orphans to link");
            return new OrphanLinkingResult
            {
                IsFullyConnected = true,
                Message = "All concepts are connected - no orphans detected"
            };
        }

        Log.Info($"OrphanLinker: Found {connectivity.OrphanedComponentCount} orphaned component(s) with {connectivity.OrphanedConceptCount} concepts");

        var mainComponent = conceptMap.GetMainComponent();
        if (mainComponent == null || mainComponent.Size == 0)
        {
            Log.Warn("OrphanLinker: No main component found");
            return new OrphanLinkingResult
            {
                IsFullyConnected = false,
                Message = "No main component found to link orphans to"
            };
        }

        // Prepare concept lists for the AI prompt
        var mainConcepts = mainComponent.Concepts
            .Select(c => $"- {c.Title}: {c.Summary}")
            .Take(50); // Limit to avoid token overflow

        var orphanedComponents = connectivity.OrphanedComponents;
        var orphanConcepts = orphanedComponents
            .SelectMany(c => c.Concepts)
            .Select(c => $"- {c.Title}: {c.Summary}");

        // Get source context (truncated for API limits)
        var sourceContext = conceptMap.SourceContent?.Length > 4000 
            ? conceptMap.SourceContent[..4000] + "..."
            : conceptMap.SourceContent ?? "";

        var prompt = OrphanLinkingPrompt
            .Replace("{MAIN_CONCEPTS}", string.Join("\n", mainConcepts))
            .Replace("{ORPHAN_CONCEPTS}", string.Join("\n", orphanConcepts))
            .Replace("{SOURCE_CONTEXT}", sourceContext);

        try
        {
            var response = await CallAiAsync<OrphanLinkingResponse>(prompt, ct);
            
            if (response?.SuggestedLinks == null || response.SuggestedLinks.Count == 0)
            {
                Log.Warn("OrphanLinker: AI returned no suggested links");
                return new OrphanLinkingResult
                {
                    IsFullyConnected = false,
                    Message = "No connections could be inferred for orphaned concepts"
                };
            }

            // Convert AI suggestions to actual ConceptRelationships
            var conceptLookup = conceptMap.Concepts.ToDictionary(
                c => c.Title.ToLowerInvariant(),
                c => c);

            var suggestedRelationships = new List<SuggestedLink>();

            foreach (var link in response.SuggestedLinks)
            {
                var orphanKey = link.OrphanConceptTerm?.ToLowerInvariant() ?? "";
                var mainKey = link.MainConceptTerm?.ToLowerInvariant() ?? "";

                if (!conceptLookup.TryGetValue(orphanKey, out var orphanConcept) ||
                    !conceptLookup.TryGetValue(mainKey, out var mainConcept))
                {
                    Log.Debug($"OrphanLinker: Could not find concepts for link {link.OrphanConceptTerm} -> {link.MainConceptTerm}");
                    continue;
                }

                var (sourceId, targetId) = link.Direction?.ToLowerInvariant() == "maintoorphan"
                    ? (mainConcept.Id, orphanConcept.Id)
                    : (orphanConcept.Id, mainConcept.Id);

                suggestedRelationships.Add(new SuggestedLink
                {
                    SourceConceptId = sourceId,
                    TargetConceptId = targetId,
                    SourceConceptTitle = conceptLookup.Values.First(c => c.Id == sourceId).Title,
                    TargetConceptTitle = conceptLookup.Values.First(c => c.Id == targetId).Title,
                    RelationType = ParseRelationType(link.RelationshipType),
                    Confidence = link.Confidence,
                    Justification = link.Justification ?? ""
                });
            }

            Log.Info($"OrphanLinker: Generated {suggestedRelationships.Count} suggested links");

            return new OrphanLinkingResult
            {
                IsFullyConnected = false,
                OrphanedComponentCount = connectivity.OrphanedComponentCount,
                OrphanedConceptCount = connectivity.OrphanedConceptCount,
                SuggestedLinks = suggestedRelationships,
                AdditionalInsights = response.AdditionalInsights,
                Message = $"Found {suggestedRelationships.Count} suggested connections for {connectivity.OrphanedConceptCount} orphaned concepts"
            };
        }
        catch (Exception ex)
        {
            Log.Error($"OrphanLinker: Error analyzing orphans - {ex.Message}", ex);
            return new OrphanLinkingResult
            {
                IsFullyConnected = false,
                OrphanedComponentCount = connectivity.OrphanedComponentCount,
                OrphanedConceptCount = connectivity.OrphanedConceptCount,
                Message = $"Error analyzing orphans: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Applies the suggested links to the ConceptMap and returns the new relationships.
    /// </summary>
    public List<ConceptRelationship> ApplySuggestedLinks(
        ConceptMap conceptMap,
        List<SuggestedLink> suggestedLinks,
        float minConfidence = 0.5f)
    {
        var newRelationships = new List<ConceptRelationship>();

        foreach (var link in suggestedLinks.Where(l => l.Confidence >= minConfidence))
        {
            // Check if relationship already exists
            var exists = conceptMap.Relations.Any(r =>
                r.SourceConceptId == link.SourceConceptId &&
                r.TargetConceptId == link.TargetConceptId);

            if (exists)
            {
                Log.Debug($"OrphanLinker: Skipping duplicate relationship {link.SourceConceptTitle} -> {link.TargetConceptTitle}");
                continue;
            }

            var relationship = new ConceptRelationship
            {
                SourceConceptId = link.SourceConceptId,
                TargetConceptId = link.TargetConceptId,
                RelationType = link.RelationType,
                ConfidenceScore = link.Confidence,
                Justification = $"[Auto-linked] {link.Justification}",
                IsVerified = false
            };

            conceptMap.Relations.Add(relationship);
            newRelationships.Add(relationship);

            Log.Debug($"OrphanLinker: Added link {link.SourceConceptTitle} --[{link.RelationType}]--> {link.TargetConceptTitle}");
        }

        Log.Info($"OrphanLinker: Applied {newRelationships.Count} new relationships to ConceptMap");
        return newRelationships;
    }

    /// <summary>
    /// Performs full orphan detection and auto-linking in one operation.
    /// </summary>
    public async Task<OrphanLinkingResult> DetectAndLinkOrphansAsync(
        ConceptMap conceptMap,
        float minConfidence = 0.5f,
        CancellationToken ct = default)
    {
        var result = await AnalyzeOrphansAsync(conceptMap, ct);

        if (result.IsFullyConnected || result.SuggestedLinks.Count == 0)
        {
            return result;
        }

        var appliedRelationships = ApplySuggestedLinks(conceptMap, result.SuggestedLinks, minConfidence);
        result.AppliedLinkCount = appliedRelationships.Count;

        // Verify connectivity after linking
        var newConnectivity = conceptMap.GetConnectivityInfo();
        result.IsFullyConnected = newConnectivity.IsFullyConnected;
        result.RemainingOrphanCount = newConnectivity.OrphanedConceptCount;

        if (!newConnectivity.IsFullyConnected)
        {
            result.Message += $" | {result.RemainingOrphanCount} concepts still orphaned (may need manual review)";
        }
        else
        {
            result.Message = $"Successfully linked all orphans with {appliedRelationships.Count} new connections";
        }

        return result;
    }

    /// <summary>
    /// Performs iterative orphan linking until the graph is fully connected or max iterations reached.
    /// This is more aggressive and will keep trying to connect clusters.
    /// </summary>
    public async Task<OrphanLinkingResult> LinkAllOrphansIterativelyAsync(
        ConceptMap conceptMap,
        int maxIterations = 10,
        float minConfidence = 0.3f,
        CancellationToken ct = default)
    {
        Log.Info($"OrphanLinker: Starting iterative linking for ConceptMap '{conceptMap.Name}'");
        
        var totalApplied = 0;
        var iteration = 0;
        OrphanLinkingResult lastResult = new();

        while (iteration < maxIterations)
        {
            ct.ThrowIfCancellationRequested();
            iteration++;

            var connectivity = conceptMap.GetConnectivityInfo();
            
            if (connectivity.IsFullyConnected)
            {
                Log.Info($"OrphanLinker: Graph is fully connected after {iteration - 1} iteration(s)");
                return new OrphanLinkingResult
                {
                    IsFullyConnected = true,
                    AppliedLinkCount = totalApplied,
                    Message = $"Graph is fully connected! Applied {totalApplied} links over {iteration - 1} iteration(s)"
                };
            }

            Log.Info($"OrphanLinker: Iteration {iteration} - {connectivity.OrphanedConceptCount} orphans in {connectivity.OrphanedComponentCount} clusters");

            // Use aggressive linking with lower confidence threshold
            lastResult = await AnalyzeOrphansAggressiveAsync(conceptMap, iteration, ct);

            if (lastResult.SuggestedLinks.Count == 0)
            {
                Log.Warn($"OrphanLinker: No more suggestions at iteration {iteration}");
                break;
            }

            var applied = ApplySuggestedLinks(conceptMap, lastResult.SuggestedLinks, minConfidence);
            totalApplied += applied.Count;

            if (applied.Count == 0)
            {
                Log.Warn($"OrphanLinker: No links applied at iteration {iteration} (all below confidence threshold)");
                break;
            }

            Log.Info($"OrphanLinker: Applied {applied.Count} links in iteration {iteration}");

            // Small delay to avoid rate limiting
            await Task.Delay(500, ct);
        }

        var finalConnectivity = conceptMap.GetConnectivityInfo();
        return new OrphanLinkingResult
        {
            IsFullyConnected = finalConnectivity.IsFullyConnected,
            OrphanedComponentCount = finalConnectivity.OrphanedComponentCount,
            OrphanedConceptCount = finalConnectivity.OrphanedConceptCount,
            RemainingOrphanCount = finalConnectivity.OrphanedConceptCount,
            AppliedLinkCount = totalApplied,
            Message = finalConnectivity.IsFullyConnected
                ? $"Graph is fully connected! Applied {totalApplied} links over {iteration} iteration(s)"
                : $"Applied {totalApplied} links over {iteration} iteration(s). {finalConnectivity.OrphanedConceptCount} concepts still orphaned."
        };
    }

    /// <summary>
    /// More aggressive orphan analysis that processes in batches and prioritizes hub concepts.
    /// </summary>
    private async Task<OrphanLinkingResult> AnalyzeOrphansAggressiveAsync(
        ConceptMap conceptMap,
        int iteration,
        CancellationToken ct)
    {
        var connectivity = conceptMap.GetConnectivityInfo();
        var mainComponent = conceptMap.GetMainComponent();

        if (mainComponent == null || mainComponent.Size == 0)
        {
            // If no clear main component, find the largest one
            var allComponents = connectivity.OrphanedComponents.Prepend(mainComponent!).Where(c => c != null);
            mainComponent = allComponents.OrderByDescending(c => c?.Size ?? 0).FirstOrDefault();
        }

        if (mainComponent == null)
        {
            return new OrphanLinkingResult { Message = "No components found" };
        }

        // Get hub concepts (most connected) as primary link targets
        var hubConcepts = GetHubConcepts(conceptMap, mainComponent, 30);
        
        // Get orphaned concepts, prioritizing smaller clusters first (easier to link)
        var orphanedClusters = connectivity.OrphanedComponents
            .OrderBy(c => c.Size)
            .ToList();

        // Process a batch of orphans per iteration
        var orphansToProcess = orphanedClusters
            .SelectMany(c => c.Concepts)
            .Take(50) // Process 50 orphans at a time
            .ToList();

        if (orphansToProcess.Count == 0)
        {
            return new OrphanLinkingResult { IsFullyConnected = true };
        }

        var prompt = BuildAggressiveLinkingPrompt(hubConcepts, orphansToProcess, conceptMap.SourceContent, iteration);

        try
        {
            var response = await CallAiAsync<OrphanLinkingResponse>(prompt, ct);

            if (response?.SuggestedLinks == null)
            {
                return new OrphanLinkingResult { Message = "No suggestions from AI" };
            }

            var conceptLookup = conceptMap.Concepts.ToDictionary(
                c => c.Title.ToLowerInvariant(),
                c => c);

            // Also build alias lookup
            foreach (var concept in conceptMap.Concepts)
            {
                foreach (var alias in concept.Aliases)
                {
                    var key = alias.ToLowerInvariant();
                    if (!conceptLookup.ContainsKey(key))
                    {
                        conceptLookup[key] = concept;
                    }
                }
            }

            var suggestedLinks = new List<SuggestedLink>();

            foreach (var link in response.SuggestedLinks)
            {
                var orphanKey = link.OrphanConceptTerm?.ToLowerInvariant() ?? "";
                var mainKey = link.MainConceptTerm?.ToLowerInvariant() ?? "";

                // Try exact match first, then partial match
                if (!conceptLookup.TryGetValue(orphanKey, out var orphanConcept))
                {
                    orphanConcept = conceptMap.Concepts.FirstOrDefault(c => 
                        c.Title.Contains(link.OrphanConceptTerm ?? "", StringComparison.OrdinalIgnoreCase));
                }

                if (!conceptLookup.TryGetValue(mainKey, out var mainConcept))
                {
                    mainConcept = conceptMap.Concepts.FirstOrDefault(c => 
                        c.Title.Contains(link.MainConceptTerm ?? "", StringComparison.OrdinalIgnoreCase));
                }

                if (orphanConcept == null || mainConcept == null)
                {
                    continue;
                }

                var (sourceId, targetId) = link.Direction?.ToLowerInvariant() == "maintoorphan"
                    ? (mainConcept.Id, orphanConcept.Id)
                    : (orphanConcept.Id, mainConcept.Id);

                suggestedLinks.Add(new SuggestedLink
                {
                    SourceConceptId = sourceId,
                    TargetConceptId = targetId,
                    SourceConceptTitle = conceptMap.Concepts.First(c => c.Id == sourceId).Title,
                    TargetConceptTitle = conceptMap.Concepts.First(c => c.Id == targetId).Title,
                    RelationType = ParseRelationType(link.RelationshipType),
                    Confidence = link.Confidence,
                    Justification = link.Justification ?? ""
                });
            }

            return new OrphanLinkingResult
            {
                SuggestedLinks = suggestedLinks,
                OrphanedConceptCount = connectivity.OrphanedConceptCount,
                OrphanedComponentCount = connectivity.OrphanedComponentCount,
                Message = $"Generated {suggestedLinks.Count} suggestions for iteration {iteration}"
            };
        }
        catch (Exception ex)
        {
            Log.Error($"OrphanLinker: Error in aggressive analysis - {ex.Message}", ex);
            return new OrphanLinkingResult { Message = $"Error: {ex.Message}" };
        }
    }

    /// <summary>
    /// Gets the most connected concepts as hub link targets.
    /// </summary>
    private static List<Concept> GetHubConcepts(ConceptMap conceptMap, ConnectedComponent mainComponent, int count)
    {
        var connectionCounts = new Dictionary<string, int>();

        foreach (var rel in conceptMap.Relations)
        {
            connectionCounts.TryGetValue(rel.SourceConceptId, out var sourceCount);
            connectionCounts[rel.SourceConceptId] = sourceCount + 1;

            connectionCounts.TryGetValue(rel.TargetConceptId, out var targetCount);
            connectionCounts[rel.TargetConceptId] = targetCount + 1;
        }

        return mainComponent.Concepts
            .OrderByDescending(c => connectionCounts.GetValueOrDefault(c.Id, 0))
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Builds an aggressive linking prompt optimized for maximum connectivity.
    /// </summary>
    private static string BuildAggressiveLinkingPrompt(
        List<Concept> hubConcepts,
        List<Concept> orphanConcepts,
        string? sourceContent,
        int iteration)
    {
        var hubList = string.Join("\n", hubConcepts.Select(c => 
            $"- {c.Title}: {c.Summary}" + (c.Aliases.Count > 0 ? $" (also: {string.Join(", ", c.Aliases)})" : "")));

        var orphanList = string.Join("\n", orphanConcepts.Select(c => 
            $"- {c.Title}: {c.Summary}"));

        var contextSnippet = sourceContent?.Length > 3000 
            ? sourceContent[..3000] + "..." 
            : sourceContent ?? "";

        return $$"""
            You are connecting orphaned concepts to a knowledge graph. This is iteration {{iteration}} of linking.
            
            GOAL: Create a FULLY CONNECTED graph where every concept can reach every other concept.
            
            HUB CONCEPTS (well-connected, good link targets):
            {{hubList}}
            
            ORPHANED CONCEPTS (MUST be connected to at least one hub):
            {{orphanList}}
            
            SOURCE CONTEXT:
            {{contextSnippet}}
            
            RULES:
            1. EVERY orphaned concept MUST have at least one connection suggested
            2. Link to the most semantically related hub concept
            3. Use "related" relationship type for any thematic connection
            4. Be generous with confidence scores - if there's any reasonable connection, score it 0.5+
            5. Characters should link to other characters they interact with
            6. Objects/items should link to characters who own/use them
            7. Places should link to characters who visit them or events that happen there
            
            Return JSON:
            {
                "suggestedLinks": [
                    {
                        "orphanConceptTerm": "Exact name of orphaned concept",
                        "mainConceptTerm": "Exact name of hub concept to link to",
                        "relationshipType": "related",
                        "direction": "orphanToMain",
                        "confidence": 0.7,
                        "justification": "Why these connect"
                    }
                ]
            }
            
            IMPORTANT: Do not leave any orphan unlinked. Find a connection for EVERY orphaned concept.
            """;
    }

    private async Task<T?> CallAiAsync<T>(string prompt, CancellationToken ct)
    {
        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is missing. Configure it in Settings → Credentials.");

        var payload = new
        {
            model = opt.Model,
            input = "Analyze the orphaned concepts and suggest connections.",
            instructions = prompt
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await http.SendAsync(request, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI API error: {response.StatusCode} - {responseText}");
        }

        // Parse the response using the same pattern as ConceptMapService
        var content = ExtractResponseText(responseText);

        if (string.IsNullOrEmpty(content))
            return default;

        return ParseJsonResponse<T>(content);
    }

    /// <summary>
    /// Extracts the text content from the OpenAI responses API response.
    /// </summary>
    private static string ExtractResponseText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Try to get output array (responses API format)
            if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in output.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out var contentArray) && 
                        contentArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var contentItem in contentArray.EnumerateArray())
                        {
                            if (contentItem.TryGetProperty("text", out var text))
                            {
                                return text.GetString() ?? "";
                            }
                        }
                    }
                }
            }

            // Fallback to choices format (chat completions)
            if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
            {
                var firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var msgContent))
                {
                    return msgContent.GetString() ?? "";
                }
            }

            return "";
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// Parses the JSON response text into the target type.
    /// </summary>
    private static T? ParseJsonResponse<T>(string? responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
            return default;

        try
        {
            // Try to find JSON in the response (may be wrapped in markdown code blocks)
            var jsonText = responseText;
            
            // Remove markdown code blocks if present
            if (jsonText.Contains("```json"))
            {
                var start = jsonText.IndexOf("```json") + 7;
                var end = jsonText.LastIndexOf("```");
                if (end > start)
                {
                    jsonText = jsonText[start..end].Trim();
                }
            }
            else if (jsonText.Contains("```"))
            {
                var start = jsonText.IndexOf("```") + 3;
                var end = jsonText.LastIndexOf("```");
                if (end > start)
                {
                    jsonText = jsonText[start..end].Trim();
                }
            }

            return JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return default;
        }
    }

    private static ConceptRelationType ParseRelationType(string? type)
    {
        return type?.ToLowerInvariant() switch
        {
            "prerequisite" => ConceptRelationType.Prerequisite,
            "related" => ConceptRelationType.Related,
            "contains" => ConceptRelationType.PartOf,
            "instanceof" => ConceptRelationType.Instance,
            "similarto" => ConceptRelationType.Related,
            "contrastswith" => ConceptRelationType.Related,
            _ => ConceptRelationType.Related
        };
    }
}

/// <summary>
/// Result of orphan concept analysis and linking.
/// </summary>
public class OrphanLinkingResult
{
    /// <summary>
    /// Whether the graph is now fully connected.
    /// </summary>
    public bool IsFullyConnected { get; set; }

    /// <summary>
    /// Number of orphaned components found.
    /// </summary>
    public int OrphanedComponentCount { get; set; }

    /// <summary>
    /// Total count of orphaned concepts.
    /// </summary>
    public int OrphanedConceptCount { get; set; }

    /// <summary>
    /// Suggested links to connect orphans to the main cluster.
    /// </summary>
    public List<SuggestedLink> SuggestedLinks { get; set; } = [];

    /// <summary>
    /// Number of links that were applied.
    /// </summary>
    public int AppliedLinkCount { get; set; }

    /// <summary>
    /// Remaining orphan count after linking (if any).
    /// </summary>
    public int RemainingOrphanCount { get; set; }

    /// <summary>
    /// Additional insights from the AI about the orphaned concepts.
    /// </summary>
    public string? AdditionalInsights { get; set; }

    /// <summary>
    /// Human-readable status message.
    /// </summary>
    public string Message { get; set; } = "";
}

/// <summary>
/// A suggested link between an orphaned concept and the main cluster.
/// </summary>
public class SuggestedLink
{
    public string SourceConceptId { get; set; } = "";
    public string TargetConceptId { get; set; } = "";
    public string SourceConceptTitle { get; set; } = "";
    public string TargetConceptTitle { get; set; } = "";
    public ConceptRelationType RelationType { get; set; }
    public float Confidence { get; set; }
    public string Justification { get; set; } = "";
}

/// <summary>
/// AI response for orphan linking analysis.
/// </summary>
internal class OrphanLinkingResponse
{
    [JsonPropertyName("suggestedLinks")]
    public List<OrphanLinkSuggestion> SuggestedLinks { get; set; } = [];

    [JsonPropertyName("additionalInsights")]
    public string? AdditionalInsights { get; set; }
}

/// <summary>
/// Individual link suggestion from AI.
/// </summary>
internal class OrphanLinkSuggestion
{
    [JsonPropertyName("orphanConceptTerm")]
    public string? OrphanConceptTerm { get; set; }

    [JsonPropertyName("mainConceptTerm")]
    public string? MainConceptTerm { get; set; }

    [JsonPropertyName("relationshipType")]
    public string? RelationshipType { get; set; }

    [JsonPropertyName("direction")]
    public string? Direction { get; set; }

    [JsonPropertyName("confidence")]
    public float Confidence { get; set; }

    [JsonPropertyName("justification")]
    public string? Justification { get; set; }
}
