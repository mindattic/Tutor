using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Service for discovering and scoring relationships between concepts.
/// Uses multi-signal correlation combining:
/// - Semantic similarity (embedding cosine distance)
/// - LSH approximate matching for fast candidate generation
/// - SimHash lexical similarity
/// - Co-occurrence patterns
/// - LLM-based prerequisite inference
/// </summary>
public sealed class ConceptCorrelationService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    private readonly EmbeddingService embeddingService;
    private readonly LSHService lshService;
    private readonly SimHashService simHashService;

    // Thresholds for relationship detection
    private const float MinSemanticSimilarity = 0.4f;
    private const int MaxLshDistance = 80;
    private const int MaxLexicalDistance = 20;
    private const float MinRelationshipConfidence = 0.3f;

    // Weights for combined scoring
    private const float SemanticWeight = 0.50f;
    private const float LshWeight = 0.25f;
    private const float LexicalWeight = 0.15f;
    private const float CoOccurrenceWeight = 0.10f;

    private const string PrerequisiteAnalysisPrompt = """
        You are an expert educator analyzing the prerequisite relationships between concepts.
        
        Given the following pairs of concepts, determine which ones have prerequisite relationships.
        A prerequisite relationship means: to understand Concept B, you must first understand Concept A.
        
        For each pair, return:
        - "prerequisite": true if A is a prerequisite for B
        - "reverse": true if B is a prerequisite for A (instead)
        - "related": true if they are related but neither is a strict prerequisite
        - "confidence": 0.0-1.0 indicating how confident you are
        - "justification": brief explanation
        
        Return as JSON array:
        {
            "relationships": [
                {
                    "conceptA": "...",
                    "conceptB": "...",
                    "prerequisite": true,
                    "reverse": false,
                    "related": true,
                    "confidence": 0.85,
                    "justification": "A defines the foundation that B builds upon"
                }
            ]
        }
        """;

    public ConceptCorrelationService(
        HttpClient http,
        OpenAIOptions opt,
        EmbeddingService embeddingService,
        LSHService lshService,
        SimHashService simHashService)
    {
        this.http = http;
        this.opt = opt;
        this.embeddingService = embeddingService;
        this.lshService = lshService;
        this.simHashService = simHashService;
    }

    /// <summary>
    /// Finds all related concept pairs using multi-signal correlation.
    /// Returns candidate pairs sorted by relationship strength.
    /// </summary>
    public List<ConceptPairScore> FindRelatedPairs(
        List<ConceptNode> concepts,
        int maxPairsPerConcept = 10)
    {
        var pairs = new List<ConceptPairScore>();

        for (int i = 0; i < concepts.Count; i++)
        {
            var conceptA = concepts[i];
            var candidates = new List<ConceptPairScore>();

            for (int j = 0; j < concepts.Count; j++)
            {
                if (i == j) continue;

                var conceptB = concepts[j];
                var score = ComputePairScore(conceptA, conceptB);

                if (score.CombinedScore >= MinRelationshipConfidence)
                {
                    candidates.Add(score);
                }
            }

            // Take top candidates for this concept
            pairs.AddRange(candidates
                .OrderByDescending(p => p.CombinedScore)
                .Take(maxPairsPerConcept));
        }

        // Deduplicate (A,B) and (B,A) keeping the higher scored one
        var uniquePairs = new Dictionary<string, ConceptPairScore>();
        foreach (var pair in pairs)
        {
            var key = string.Compare(pair.ConceptAId, pair.ConceptBId) < 0
                ? $"{pair.ConceptAId}|{pair.ConceptBId}"
                : $"{pair.ConceptBId}|{pair.ConceptAId}";

            if (!uniquePairs.TryGetValue(key, out var existing) || pair.CombinedScore > existing.CombinedScore)
            {
                uniquePairs[key] = pair;
            }
        }

        return uniquePairs.Values.OrderByDescending(p => p.CombinedScore).ToList();
    }

    /// <summary>
    /// Computes the correlation score between two concepts using all available signals.
    /// </summary>
    public ConceptPairScore ComputePairScore(ConceptNode conceptA, ConceptNode conceptB)
    {
        // Semantic similarity (cosine of embeddings)
        var semanticSimilarity = conceptA.Embedding.Length > 0 && conceptB.Embedding.Length > 0
            ? EmbeddingService.CosineSimilarity(conceptA.Embedding, conceptB.Embedding)
            : 0f;

        // LSH distance (Hamming distance of semantic signatures)
        var lshDistance = conceptA.SemanticSignature.Length > 0 && conceptB.SemanticSignature.Length > 0
            ? LSHService.HammingDistance(conceptA.SemanticSignature, conceptB.SemanticSignature)
            : 256; // Max distance if no signatures

        // Lexical distance (Hamming distance of SimHash signatures)
        var lexicalDistance = SimHashService.HammingDistance(
            conceptA.LexicalSignature, 
            conceptB.LexicalSignature);

        // Co-occurrence (shared source resources)
        var sharedResources = conceptA.SourceResourceIds
            .Intersect(conceptB.SourceResourceIds)
            .Count();

        // Compute normalized scores
        var normalizedLsh = 1f - Math.Clamp(lshDistance / 128f, 0f, 1f);
        var normalizedLexical = 1f - Math.Clamp(lexicalDistance / 32f, 0f, 1f);
        var normalizedCoOccurrence = Math.Clamp(sharedResources / 5f, 0f, 1f);

        // Combined weighted score
        var combinedScore =
            (semanticSimilarity * SemanticWeight) +
            (normalizedLsh * LshWeight) +
            (normalizedLexical * LexicalWeight) +
            (normalizedCoOccurrence * CoOccurrenceWeight);

        return new ConceptPairScore
        {
            ConceptAId = conceptA.Id,
            ConceptATerm = conceptA.Term,
            ConceptBId = conceptB.Id,
            ConceptBTerm = conceptB.Term,
            SemanticSimilarity = semanticSimilarity,
            LshDistance = lshDistance,
            LexicalDistance = lexicalDistance,
            CoOccurrenceCount = sharedResources,
            CombinedScore = combinedScore
        };
    }

    /// <summary>
    /// Uses LLM to determine prerequisite relationships from concept pairs.
    /// </summary>
    public async Task<List<ConceptRelationship>> InferPrerequisitesAsync(
        List<ConceptPairScore> pairs,
        Dictionary<string, ConceptNode> conceptLookup,
        CancellationToken ct = default)
    {
        if (pairs.Count == 0)
            return [];

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            return BuildRelationshipsWithoutLlm(pairs, conceptLookup);

        // Prepare pairs for LLM analysis (batch for efficiency)
        var relationships = new List<ConceptRelationship>();
        const int batchSize = 20;

        for (int i = 0; i < pairs.Count; i += batchSize)
        {
            ct.ThrowIfCancellationRequested();

            var batch = pairs.Skip(i).Take(batchSize).ToList();
            var batchRelationships = await AnalyzeBatchAsync(batch, conceptLookup, apiKey, ct);
            relationships.AddRange(batchRelationships);

            // Rate limit between batches
            if (i + batchSize < pairs.Count)
            {
                await Task.Delay(300, ct);
            }
        }

        return relationships;
    }

    /// <summary>
    /// Builds relationships using only similarity scores (no LLM).
    /// Useful as fallback or for quick initial relationships.
    /// </summary>
    public List<ConceptRelationship> BuildRelationshipsWithoutLlm(
        List<ConceptPairScore> pairs,
        Dictionary<string, ConceptNode> conceptLookup)
    {
        var relationships = new List<ConceptRelationship>();

        foreach (var pair in pairs)
        {
            if (!conceptLookup.TryGetValue(pair.ConceptAId, out var conceptA) ||
                !conceptLookup.TryGetValue(pair.ConceptBId, out var conceptB))
                continue;

            // Use heuristics to determine direction:
            // - Shorter description often = more foundational
            // - Fewer characters in term = often more basic
            // - Higher confidence concepts are more likely foundational

            var aScore = conceptA.Description.Length + conceptA.Term.Length * 2 + (1 - conceptA.ConfidenceScore) * 100;
            var bScore = conceptB.Description.Length + conceptB.Term.Length * 2 + (1 - conceptB.ConfidenceScore) * 100;

            string sourceId, targetId;
            if (aScore < bScore)
            {
                // A is likely more foundational
                sourceId = pair.ConceptAId;
                targetId = pair.ConceptBId;
            }
            else
            {
                // B is likely more foundational
                sourceId = pair.ConceptBId;
                targetId = pair.ConceptAId;
            }

            var relType = pair.SemanticSimilarity >= 0.7f
                ? ConceptRelationType.Prerequisite
                : ConceptRelationType.Related;

            relationships.Add(new ConceptRelationship
            {
                SourceConceptId = sourceId,
                TargetConceptId = targetId,
                RelationType = relType,
                ConfidenceScore = pair.CombinedScore,
                SemanticSimilarity = pair.SemanticSimilarity,
                SemanticDistance = pair.LshDistance,
                LexicalDistance = pair.LexicalDistance,
                CoOccurrenceCount = pair.CoOccurrenceCount,
                IsVerified = false
            });
        }

        return relationships;
    }

    private async Task<List<ConceptRelationship>> AnalyzeBatchAsync(
        List<ConceptPairScore> batch,
        Dictionary<string, ConceptNode> conceptLookup,
        string apiKey,
        CancellationToken ct)
    {
        var relationships = new List<ConceptRelationship>();

        try
        {
            // Format pairs for LLM
            var pairDescriptions = new StringBuilder();
            foreach (var pair in batch)
            {
                if (!conceptLookup.TryGetValue(pair.ConceptAId, out var a) ||
                    !conceptLookup.TryGetValue(pair.ConceptBId, out var b))
                    continue;

                pairDescriptions.AppendLine($"Concept A: {a.Term} - {a.Description}");
                pairDescriptions.AppendLine($"Concept B: {b.Term} - {b.Description}");
                pairDescriptions.AppendLine();
            }

            var payload = new
            {
                model = opt.Model,
                input = pairDescriptions.ToString(),
                instructions = PrerequisiteAnalysisPrompt
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var resp = await http.SendAsync(req, ct);
            var json = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                // Fall back to heuristic-based relationships
                return BuildRelationshipsWithoutLlm(batch, conceptLookup);
            }

            var responseText = ExtractResponseText(json);
            var analyzed = ParsePrerequisiteAnalysis(responseText, batch, conceptLookup);
            relationships.AddRange(analyzed);
        }
        catch
        {
            // Fall back to heuristic-based relationships
            return BuildRelationshipsWithoutLlm(batch, conceptLookup);
        }

        return relationships;
    }

    private static string ExtractResponseText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("output_text", out var outText) && outText.ValueKind == JsonValueKind.String)
                return outText.GetString() ?? "";

            if (doc.RootElement.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in output.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var c in content.EnumerateArray())
                        {
                            if (c.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                                return textProp.GetString() ?? "";
                        }
                    }
                }
            }
        }
        catch { }

        return json;
    }

    private static List<ConceptRelationship> ParsePrerequisiteAnalysis(
        string responseText,
        List<ConceptPairScore> originalPairs,
        Dictionary<string, ConceptNode> conceptLookup)
    {
        var relationships = new List<ConceptRelationship>();

        try
        {
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');
            if (jsonStart < 0 || jsonEnd < jsonStart)
                return relationships;

            var jsonText = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
            using var doc = JsonDocument.Parse(jsonText);

            if (!doc.RootElement.TryGetProperty("relationships", out var relArray) ||
                relArray.ValueKind != JsonValueKind.Array)
                return relationships;

            // Build lookup from term to concept
            var termLookup = conceptLookup.Values
                .ToDictionary(c => c.Term.ToLowerInvariant(), c => c, StringComparer.OrdinalIgnoreCase);

            foreach (var item in relArray.EnumerateArray())
            {
                var termA = item.TryGetProperty("conceptA", out var a) ? a.GetString() ?? "" : "";
                var termB = item.TryGetProperty("conceptB", out var b) ? b.GetString() ?? "" : "";
                var isPrereq = item.TryGetProperty("prerequisite", out var p) && p.GetBoolean();
                var isReverse = item.TryGetProperty("reverse", out var r) && r.GetBoolean();
                var isRelated = item.TryGetProperty("related", out var rel) && rel.GetBoolean();
                var confidence = item.TryGetProperty("confidence", out var conf) ? (float)conf.GetDouble() : 0.5f;
                var justification = item.TryGetProperty("justification", out var j) ? j.GetString() ?? "" : "";

                // Find the matching concepts
                if (!termLookup.TryGetValue(termA.ToLowerInvariant(), out var conceptA) ||
                    !termLookup.TryGetValue(termB.ToLowerInvariant(), out var conceptB))
                    continue;

                // Find the original pair score
                var pairScore = originalPairs.FirstOrDefault(ps =>
                    (ps.ConceptAId == conceptA.Id && ps.ConceptBId == conceptB.Id) ||
                    (ps.ConceptAId == conceptB.Id && ps.ConceptBId == conceptA.Id));

                string sourceId, targetId;
                ConceptRelationType relType;

                if (isPrereq && !isReverse)
                {
                    sourceId = conceptA.Id;
                    targetId = conceptB.Id;
                    relType = ConceptRelationType.Prerequisite;
                }
                else if (isReverse)
                {
                    sourceId = conceptB.Id;
                    targetId = conceptA.Id;
                    relType = ConceptRelationType.Prerequisite;
                }
                else if (isRelated)
                {
                    sourceId = conceptA.Id;
                    targetId = conceptB.Id;
                    relType = ConceptRelationType.Related;
                }
                else
                {
                    continue; // Skip pairs with no relationship
                }

                relationships.Add(new ConceptRelationship
                {
                    SourceConceptId = sourceId,
                    TargetConceptId = targetId,
                    RelationType = relType,
                    ConfidenceScore = confidence,
                    SemanticSimilarity = pairScore?.SemanticSimilarity ?? 0,
                    SemanticDistance = pairScore?.LshDistance ?? 256,
                    LexicalDistance = pairScore?.LexicalDistance ?? 64,
                    CoOccurrenceCount = pairScore?.CoOccurrenceCount ?? 0,
                    Justification = justification,
                    IsVerified = true // LLM analyzed
                });
            }
        }
        catch { }

        return relationships;
    }

    /// <summary>
    /// Detects potential cycles in the graph and removes the weakest edge.
    /// </summary>
    public static void RemoveCycles(KnowledgeGraph graph)
    {
        while (true)
        {
            var cycle = FindCycle(graph);
            if (cycle == null || cycle.Count == 0)
                break;

            // Find the weakest edge in the cycle
            ConceptRelationship? weakest = null;
            var lowestConfidence = float.MaxValue;

            for (int i = 0; i < cycle.Count; i++)
            {
                var fromId = cycle[i];
                var toId = cycle[(i + 1) % cycle.Count];

                var edge = graph.Relationships.Values.FirstOrDefault(r =>
                    r.SourceConceptId == fromId && r.TargetConceptId == toId &&
                    r.RelationType == ConceptRelationType.Prerequisite);

                if (edge != null && edge.ConfidenceScore < lowestConfidence)
                {
                    weakest = edge;
                    lowestConfidence = edge.ConfidenceScore;
                }
            }

            if (weakest != null)
            {
                // Demote to Related instead of removing
                weakest.RelationType = ConceptRelationType.Related;
                graph.RebuildAdjacencyLists();
            }
            else
            {
                break;
            }
        }
    }

    private static List<string>? FindCycle(KnowledgeGraph graph)
    {
        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();
        var parent = new Dictionary<string, string?>();

        foreach (var nodeId in graph.Nodes.Keys)
        {
            if (!visited.Contains(nodeId))
            {
                var cycle = DfsCycle(nodeId, visited, recursionStack, parent, graph);
                if (cycle != null)
                    return cycle;
            }
        }

        return null;
    }

    private static List<string>? DfsCycle(
        string nodeId,
        HashSet<string> visited,
        HashSet<string> stack,
        Dictionary<string, string?> parent,
        KnowledgeGraph graph)
    {
        visited.Add(nodeId);
        stack.Add(nodeId);

        if (graph.ForwardEdges.TryGetValue(nodeId, out var neighbors))
        {
            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    parent[neighbor] = nodeId;
                    var cycle = DfsCycle(neighbor, visited, stack, parent, graph);
                    if (cycle != null)
                        return cycle;
                }
                else if (stack.Contains(neighbor))
                {
                    // Found cycle - reconstruct it
                    var cycleNodes = new List<string> { neighbor };
                    var current = nodeId;
                    while (current != neighbor)
                    {
                        cycleNodes.Add(current);
                        current = parent.TryGetValue(current, out var p) ? p! : neighbor;
                    }
                    cycleNodes.Reverse();
                    return cycleNodes;
                }
            }
        }

        stack.Remove(nodeId);
        return null;
    }
}

/// <summary>
/// Represents the correlation score between two concepts.
/// </summary>
public class ConceptPairScore
{
    public string ConceptAId { get; set; } = "";
    public string ConceptATerm { get; set; } = "";
    public string ConceptBId { get; set; } = "";
    public string ConceptBTerm { get; set; } = "";
    public float SemanticSimilarity { get; set; }
    public int LshDistance { get; set; }
    public int LexicalDistance { get; set; }
    public int CoOccurrenceCount { get; set; }
    public float CombinedScore { get; set; }
}
