using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Service for extracting core concepts from course resources using LLM analysis.
/// Identifies key terms, their definitions, and potential relationships.
/// </summary>
public sealed class ConceptExtractionService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    private readonly EmbeddingService embeddingService;
    private readonly LSHService lshService;
    private readonly SimHashService simHashService;

    private const string ExtractionPrompt = """
        You are an expert knowledge analyst. Analyze the following educational content and extract core concepts.
        
        For each concept, identify:
        1. The term or name (canonical form)
        2. A concise definition/description (1-2 sentences)
        3. Any alternative names or synonyms
        4. Related concepts mentioned in the same context
        5. Whether this concept seems to require understanding of other concepts first (prerequisites)
        
        Return your analysis as a JSON array with this structure:
        {
            "concepts": [
                {
                    "term": "Concept Name",
                    "description": "Clear, concise definition",
                    "aliases": ["Alternative Name 1", "Synonym"],
                    "relatedTerms": ["Related Concept 1", "Related Concept 2"],
                    "potentialPrerequisites": ["Foundational Concept"],
                    "confidence": 0.95
                }
            ]
        }
        
        Focus on:
        - Key terminology and domain-specific vocabulary
        - People, places, events (for history/narrative content)
        - Technical concepts and processes
        - Hierarchical relationships (categories ? subcategories ? instances)
        
        Be thorough but avoid trivial or common terms. Each concept should be meaningful for learning the subject.
        """;

    public ConceptExtractionService(
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
    /// Extracts concepts from a single piece of content.
    /// </summary>
    public async Task<List<ExtractedConcept>> ExtractConceptsAsync(
        string content, 
        string resourceId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(content))
            return [];

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is missing.");

        // Truncate content if too long (leaving room for prompt)
        var maxContentLength = 12000;
        var truncatedContent = content.Length > maxContentLength 
            ? content[..maxContentLength] + "\n\n[Content truncated...]" 
            : content;

        var payload = new
        {
            model = opt.Model,
            input = $"Content to analyze:\n\n{truncatedContent}",
            instructions = ExtractionPrompt
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var resp = await http.SendAsync(req, ct);
        var json = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI error {(int)resp.StatusCode}: {json}");
        }

        var responseText = ExtractResponseText(json);
        var concepts = ParseExtractedConcepts(responseText, resourceId);

        return concepts;
    }

    /// <summary>
    /// Extracts concepts from multiple content chunks and merges results.
    /// </summary>
    public async Task<List<ExtractedConcept>> ExtractConceptsFromChunksAsync(
        List<ContentChunk> chunks,
        string resourceId,
        CancellationToken ct = default)
    {
        var allConcepts = new List<ExtractedConcept>();

        // Process chunks in batches to avoid rate limiting
        const int batchSize = 5;
        for (int i = 0; i < chunks.Count; i += batchSize)
        {
            ct.ThrowIfCancellationRequested();

            var batch = chunks.Skip(i).Take(batchSize);
            var combinedContent = string.Join("\n\n---\n\n", batch.Select(c => c.Content));

            try
            {
                var concepts = await ExtractConceptsAsync(combinedContent, resourceId, ct);
                allConcepts.AddRange(concepts);
            }
            catch (Exception)
            {
                // Continue with other batches on error
            }

            // Small delay between batches
            if (i + batchSize < chunks.Count)
            {
                await Task.Delay(500, ct);
            }
        }

        // Merge duplicate concepts
        return MergeConcepts(allConcepts);
    }

    /// <summary>
    /// Converts extracted concepts to ConceptNodes with embeddings and signatures.
    /// </summary>
    public async Task<List<ConceptNode>> CreateConceptNodesAsync(
        List<ExtractedConcept> extractedConcepts,
        CancellationToken ct = default)
    {
        if (extractedConcepts.Count == 0)
            return [];

        // Generate embeddings for all concepts in batch
        var embeddingTexts = extractedConcepts.Select(c => $"{c.Term}. {c.Description}").ToList();
        var embeddings = await embeddingService.GetEmbeddingsAsync(embeddingTexts, ct);

        var nodes = new List<ConceptNode>();
        
        for (int i = 0; i < extractedConcepts.Count; i++)
        {
            var extracted = extractedConcepts[i];
            var embedding = i < embeddings.Count ? embeddings[i] : [];

            var node = new ConceptNode
            {
                Term = extracted.Term,
                Description = extracted.Description,
                Aliases = extracted.Aliases,
                Embedding = embedding,
                SemanticSignature = embedding.Length > 0 ? lshService.GetSignature(embedding) : [],
                LexicalSignature = simHashService.GetSignature64($"{extracted.Term} {extracted.Description}"),
                SourceResourceIds = [extracted.SourceResourceId],
                ConfidenceScore = extracted.Confidence,
                Tags = extracted.RelatedTerms // Store related terms as tags initially
            };

            nodes.Add(node);
        }

        return nodes;
    }

    /// <summary>
    /// Merges duplicate concepts by term, combining their metadata.
    /// </summary>
    private static List<ExtractedConcept> MergeConcepts(List<ExtractedConcept> concepts)
    {
        var merged = new Dictionary<string, ExtractedConcept>(StringComparer.OrdinalIgnoreCase);

        foreach (var concept in concepts)
        {
            var key = concept.Term.ToLowerInvariant().Trim();

            if (merged.TryGetValue(key, out var existing))
            {
                // Merge aliases
                var allAliases = existing.Aliases.Union(concept.Aliases, StringComparer.OrdinalIgnoreCase).ToList();
                
                // Merge related terms
                var allRelated = existing.RelatedTerms.Union(concept.RelatedTerms, StringComparer.OrdinalIgnoreCase).ToList();
                
                // Merge prerequisites
                var allPrereqs = existing.PotentialPrerequisites.Union(concept.PotentialPrerequisites, StringComparer.OrdinalIgnoreCase).ToList();
                
                // Use higher confidence and longer description
                var newConfidence = Math.Max(existing.Confidence, concept.Confidence);
                var newDescription = concept.Description.Length > existing.Description.Length 
                    ? concept.Description 
                    : existing.Description;

                merged[key] = existing with
                {
                    Description = newDescription,
                    Aliases = allAliases,
                    RelatedTerms = allRelated,
                    PotentialPrerequisites = allPrereqs,
                    Confidence = newConfidence
                };
            }
            else
            {
                merged[key] = concept;
            }
        }

        return merged.Values.ToList();
    }

    private static string ExtractResponseText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            
            if (doc.RootElement.TryGetProperty("output_text", out var outText) && outText.ValueKind == JsonValueKind.String)
            {
                return outText.GetString() ?? "";
            }

            if (doc.RootElement.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in output.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var c in content.EnumerateArray())
                        {
                            if (c.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                            {
                                return textProp.GetString() ?? "";
                            }
                        }
                    }
                }
            }
        }
        catch { }

        return json;
    }

    private static List<ExtractedConcept> ParseExtractedConcepts(string responseText, string resourceId)
    {
        var concepts = new List<ExtractedConcept>();

        try
        {
            // Find JSON in the response (may be wrapped in markdown code blocks)
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');
            
            if (jsonStart < 0 || jsonEnd < jsonStart)
                return concepts;

            var jsonText = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);

            using var doc = JsonDocument.Parse(jsonText);

            if (doc.RootElement.TryGetProperty("concepts", out var conceptsArray) && 
                conceptsArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in conceptsArray.EnumerateArray())
                {
                    var term = DecodeHtmlEntities(item.TryGetProperty("term", out var t) ? t.GetString() ?? "" : "");
                    var description = DecodeHtmlEntities(item.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "");
                    
                    if (string.IsNullOrWhiteSpace(term))
                        continue;

                    var aliases = new List<string>();
                    if (item.TryGetProperty("aliases", out var aliasArray) && aliasArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var alias in aliasArray.EnumerateArray())
                        {
                            if (alias.ValueKind == JsonValueKind.String)
                                aliases.Add(DecodeHtmlEntities(alias.GetString() ?? ""));
                        }
                    }

                    var relatedTerms = new List<string>();
                    if (item.TryGetProperty("relatedTerms", out var relatedArray) && relatedArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var related in relatedArray.EnumerateArray())
                        {
                            if (related.ValueKind == JsonValueKind.String)
                                relatedTerms.Add(DecodeHtmlEntities(related.GetString() ?? ""));
                        }
                    }

                    var prerequisites = new List<string>();
                    if (item.TryGetProperty("potentialPrerequisites", out var prereqArray) && prereqArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var prereq in prereqArray.EnumerateArray())
                        {
                            if (prereq.ValueKind == JsonValueKind.String)
                                prerequisites.Add(DecodeHtmlEntities(prereq.GetString() ?? ""));
                        }
                    }

                    var confidence = item.TryGetProperty("confidence", out var c) && c.ValueKind == JsonValueKind.Number
                        ? (float)c.GetDouble()
                        : 0.8f;

                    concepts.Add(new ExtractedConcept
                    {
                        Term = term.Trim(),
                        Description = description.Trim(),
                        Aliases = aliases.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()).ToList(),
                        RelatedTerms = relatedTerms.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).ToList(),
                        PotentialPrerequisites = prerequisites.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToList(),
                        Confidence = Math.Clamp(confidence, 0f, 1f),
                        SourceResourceId = resourceId
                    });
                }
            }
        }
        catch
        {
            // Return empty list on parse failure
        }

        return concepts;
    }

    /// <summary>
    /// Decodes HTML entities in a string to their actual characters.
    /// Handles common entities like &amp;quot; ? ", &amp;amp; ? &amp;, &amp;lt; ? &lt;, &amp;gt; ? &gt;, &amp;apos; ? ', 
    /// numeric entities like &amp;#39; ? ', &amp;#x27; ? ', and all other standard HTML entities.
    /// </summary>
    private static string DecodeHtmlEntities(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Use WebUtility.HtmlDecode which handles all standard HTML entities including:
        // - Named entities: &quot; &amp; &lt; &gt; &nbsp; &copy; etc.
        // - Decimal numeric entities: &#39; &#169; etc.
        // - Hexadecimal numeric entities: &#x27; &#xA9; etc.
        return WebUtility.HtmlDecode(text);
    }
}

/// <summary>
/// Represents a concept as extracted from content before it's added to the graph.
/// </summary>
public record ExtractedConcept
{
    public string Term { get; init; } = "";
    public string Description { get; init; } = "";
    public List<string> Aliases { get; init; } = [];
    public List<string> RelatedTerms { get; init; } = [];
    public List<string> PotentialPrerequisites { get; init; } = [];
    public float Confidence { get; init; } = 0.8f;
    public string SourceResourceId { get; init; } = "";
}
