using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for detecting and merging duplicate/similar concepts within a ConceptMapCollection.
/// 
/// Supports three types of merging:
/// 1. Exact duplicates - same title (case-insensitive)
/// 2. Semantic similarity - similar meaning using embeddings and SimHash
/// 3. AI-assisted disambiguation - for uncertain matches
/// </summary>
public sealed class ConceptMergeService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    private readonly EmbeddingService embeddingService;
    private readonly SimHashService simHashService;
    private readonly ConceptMapStorageService conceptMapStorage;

    /// <summary>
    /// Threshold for SimHash similarity (0-64 hamming distance, lower = more similar).
    /// Matches with distance below this are considered potential duplicates.
    /// </summary>
    private const int MaxHammingDistanceForSimilar = 15;

    /// <summary>
    /// Threshold for cosine similarity (0-1, higher = more similar).
    /// Matches above this are considered potential duplicates.
    /// </summary>
    private const float MinCosineSimilarityForSimilar = 0.85f;

    /// <summary>
    /// Threshold for automatic merging (highly confident matches).
    /// </summary>
    private const float AutoMergeConfidenceThreshold = 0.95f;

    private const string SimilarityAnalysisPrompt = """
        You are an expert at identifying when two terms refer to the same concept.
        
        Given pairs of concepts, determine if they refer to the same thing.
        Consider:
        - Abbreviated vs full names (e.g., "Harry" vs "Harry Potter")
        - Formal vs informal names (e.g., "Hogwarts" vs "Hogwarts School of Witchcraft and Wizardry")
        - Acronyms (e.g., "AI" vs "Artificial Intelligence")
        - Alternate spellings or transliterations
        
        For each pair, return:
        - "same": true if they refer to the same concept
        - "confidence": 0.0-1.0 indicating certainty
        - "canonical": the preferred/most complete name to use
        - "reason": brief explanation
        
        Return as JSON:
        {
            "pairs": [
                {
                    "conceptA": "...",
                    "conceptB": "...",
                    "same": true,
                    "confidence": 0.95,
                    "canonical": "...",
                    "reason": "..."
                }
            ]
        }
        """;

    public ConceptMergeService(
        HttpClient http,
        OpenAIOptions opt,
        EmbeddingService embeddingService,
        SimHashService simHashService,
        ConceptMapStorageService conceptMapStorage)
    {
        this.http = http;
        this.opt = opt;
        this.embeddingService = embeddingService;
        this.simHashService = simHashService;
        this.conceptMapStorage = conceptMapStorage;
    }

    /// <summary>
    /// Finds all duplicate concepts within a ConceptMap.
    /// </summary>
    public DuplicateDetectionResult FindDuplicates(ConceptMap conceptMap)
    {
        var result = new DuplicateDetectionResult();
        var concepts = conceptMap.Concepts;

        if (concepts.Count < 2)
            return result;

        // Group by normalized title for exact duplicates
        var titleGroups = concepts
            .GroupBy(c => NormalizeTitle(c.Title))
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in titleGroups)
        {
            result.ExactDuplicates.Add(new DuplicateGroup
            {
                MatchType = DuplicateMatchType.Exact,
                Concepts = group.ToList(),
                Confidence = 1.0f,
                SuggestedCanonical = SelectCanonicalConcept(group.ToList())
            });
        }

        // Find similar concepts using SimHash
        var checkedPairs = new HashSet<string>();
        foreach (var conceptA in concepts)
        {
            var sigA = simHashService.GetSignature64($"{conceptA.Title} {conceptA.Summary}");

            foreach (var conceptB in concepts)
            {
                if (conceptA.Id == conceptB.Id)
                    continue;

                var pairKey = GetPairKey(conceptA.Id, conceptB.Id);
                if (checkedPairs.Contains(pairKey))
                    continue;
                checkedPairs.Add(pairKey);

                // Skip if already in an exact duplicate group
                if (NormalizeTitle(conceptA.Title) == NormalizeTitle(conceptB.Title))
                    continue;

                var sigB = simHashService.GetSignature64($"{conceptB.Title} {conceptB.Summary}");
                var distance = SimHashService.HammingDistance(sigA, sigB);

                if (distance <= MaxHammingDistanceForSimilar)
                {
                    // Convert hamming distance to similarity score (0-64 -> 1.0-0.0)
                    var similarity = 1.0f - (distance / 64.0f);

                    result.SimilarConcepts.Add(new SimilarConceptPair
                    {
                        ConceptA = conceptA,
                        ConceptB = conceptB,
                        SimilarityScore = similarity,
                        HammingDistance = distance,
                        NeedsUserConfirmation = similarity < AutoMergeConfidenceThreshold
                    });
                }
            }
        }

        result.TotalDuplicateGroups = result.ExactDuplicates.Count;
        result.TotalSimilarPairs = result.SimilarConcepts.Count;

        return result;
    }

    /// <summary>
    /// Finds duplicates across all ConceptMaps in a collection.
    /// </summary>
    public DuplicateDetectionResult FindDuplicatesInCollection(LoadedConceptMapCollection collection)
    {
        var result = new DuplicateDetectionResult();
        var allConcepts = collection.GetAllConcepts().ToList();

        if (allConcepts.Count < 2)
            return result;

        // Group by normalized title for exact duplicates
        var titleGroups = allConcepts
            .GroupBy(c => NormalizeTitle(c.Title))
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in titleGroups)
        {
            result.ExactDuplicates.Add(new DuplicateGroup
            {
                MatchType = DuplicateMatchType.Exact,
                Concepts = group.ToList(),
                Confidence = 1.0f,
                SuggestedCanonical = SelectCanonicalConcept(group.ToList())
            });
        }

        // Find similar concepts using SimHash
        var checkedPairs = new HashSet<string>();
        foreach (var conceptA in allConcepts)
        {
            var sigA = simHashService.GetSignature64($"{conceptA.Title} {conceptA.Summary}");

            foreach (var conceptB in allConcepts)
            {
                if (conceptA.Id == conceptB.Id)
                    continue;

                var pairKey = GetPairKey(conceptA.Id, conceptB.Id);
                if (checkedPairs.Contains(pairKey))
                    continue;
                checkedPairs.Add(pairKey);

                // Skip if already in an exact duplicate group
                if (NormalizeTitle(conceptA.Title) == NormalizeTitle(conceptB.Title))
                    continue;

                var sigB = simHashService.GetSignature64($"{conceptB.Title} {conceptB.Summary}");
                var distance = SimHashService.HammingDistance(sigA, sigB);

                if (distance <= MaxHammingDistanceForSimilar)
                {
                    var similarity = 1.0f - (distance / 64.0f);

                    result.SimilarConcepts.Add(new SimilarConceptPair
                    {
                        ConceptA = conceptA,
                        ConceptB = conceptB,
                        SimilarityScore = similarity,
                        HammingDistance = distance,
                        NeedsUserConfirmation = true // Always confirm cross-map merges
                    });
                }
            }
        }

        result.TotalDuplicateGroups = result.ExactDuplicates.Count;
        result.TotalSimilarPairs = result.SimilarConcepts.Count;

        return result;
    }

    /// <summary>
    /// Uses AI to analyze similar concept pairs and determine if they refer to the same thing.
    /// </summary>
    public async Task<List<AISimilarityResult>> AnalyzeSimilarityWithAIAsync(
        List<SimilarConceptPair> pairs,
        CancellationToken cancellationToken = default)
    {
        if (pairs.Count == 0)
            return [];

        var results = new List<AISimilarityResult>();

        // Batch pairs for efficiency (max 20 per request)
        var batches = pairs.Chunk(20);

        foreach (var batch in batches)
        {
            var pairDescriptions = batch.Select(p => new
            {
                conceptA = p.ConceptA.Title,
                summaryA = p.ConceptA.Summary,
                conceptB = p.ConceptB.Title,
                summaryB = p.ConceptB.Summary
            });

            var userPrompt = $"Analyze these concept pairs:\n{JsonSerializer.Serialize(pairDescriptions, new JsonSerializerOptions { WriteIndented = true })}";

            try
            {
                var apiKey = await opt.GetApiKeyAsync();
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Log.Warn("[ConceptMergeService] API key not configured");
                    continue;
                }

                var request = new
                {
                    model = opt.Model,
                    input = userPrompt,
                    instructions = SimilarityAnalysisPrompt
                };

                using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                req.Content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                var response = await http.SendAsync(req, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var aiResults = ParseAIResponse(json, batch);
                    results.AddRange(aiResults);
                }
                else
                {
                    Log.Warn($"[ConceptMergeService] AI analysis failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"[ConceptMergeService] AI analysis error: {ex.Message}");
            }
        }

        return results;
    }

    /// <summary>
    /// Merges exact duplicates within a single ConceptMap.
    /// Updates relationships to point to the canonical concept.
    /// </summary>
    public MergeResult MergeExactDuplicates(ConceptMap conceptMap)
    {
        var result = new MergeResult();
        var duplicates = FindDuplicates(conceptMap);

        foreach (var group in duplicates.ExactDuplicates)
        {
            var canonical = group.SuggestedCanonical;
            var toRemove = group.Concepts.Where(c => c.Id != canonical.Id).ToList();

            foreach (var duplicate in toRemove)
            {
                MergeConcepts(conceptMap, canonical, duplicate);
                result.MergedConceptIds.Add(duplicate.Id);
            }
        }

        result.MergedCount = result.MergedConceptIds.Count;
        result.RemainingConceptCount = conceptMap.Concepts.Count;

        return result;
    }

    /// <summary>
    /// Merges a list of approved similar concept pairs.
    /// </summary>
    public async Task<MergeResult> MergeApprovedPairsAsync(
        LoadedConceptMapCollection collection,
        List<ApprovedMerge> approvedMerges)
    {
        var result = new MergeResult();

        foreach (var merge in approvedMerges)
        {
            // Find the ConceptMaps containing these concepts
            var mapA = collection.ConceptMaps.FirstOrDefault(cm => cm.Concepts.Any(c => c.Id == merge.ConceptAId));
            var mapB = collection.ConceptMaps.FirstOrDefault(cm => cm.Concepts.Any(c => c.Id == merge.ConceptBId));

            if (mapA == null || mapB == null)
                continue;

            var conceptA = mapA.Concepts.FirstOrDefault(c => c.Id == merge.ConceptAId);
            var conceptB = mapB.Concepts.FirstOrDefault(c => c.Id == merge.ConceptBId);

            if (conceptA == null || conceptB == null)
                continue;

            // Determine canonical based on merge direction
            var (canonical, toRemove) = merge.KeepConceptAId == merge.ConceptAId
                ? (conceptA, conceptB)
                : (conceptB, conceptA);

            // Update the canonical's title if a new one was suggested
            if (!string.IsNullOrEmpty(merge.CanonicalTitle))
            {
                canonical.Title = merge.CanonicalTitle;
            }

            // If same map, merge directly
            if (mapA.Id == mapB.Id)
            {
                MergeConcepts(mapA, canonical, toRemove);
            }
            else
            {
                // Cross-map merge: update relationships in mapB to point to canonical in mapA
                MergeConceptsCrossMap(mapA, mapB, canonical, toRemove);
            }

            result.MergedConceptIds.Add(toRemove.Id);
        }

        // Save all modified ConceptMaps
        foreach (var map in collection.ConceptMaps)
        {
            await conceptMapStorage.SaveAsync(map);
        }

        result.MergedCount = result.MergedConceptIds.Count;
        result.RemainingConceptCount = collection.GetAllConcepts().Count();

        return result;
    }

    /// <summary>
    /// Merges two concepts within the same ConceptMap.
    /// The toRemove concept is deleted, and its data is merged into canonical.
    /// </summary>
    private void MergeConcepts(ConceptMap conceptMap, Concept canonical, Concept toRemove)
    {
        // Merge aliases
        foreach (var alias in toRemove.Aliases)
        {
            if (!canonical.Aliases.Contains(alias, StringComparer.OrdinalIgnoreCase))
                canonical.Aliases.Add(alias);
        }

        // Add the removed concept's title as an alias if different
        if (!canonical.Title.Equals(toRemove.Title, StringComparison.OrdinalIgnoreCase) &&
            !canonical.Aliases.Contains(toRemove.Title, StringComparer.OrdinalIgnoreCase))
        {
            canonical.Aliases.Add(toRemove.Title);
        }

        // Merge tags
        foreach (var tag in toRemove.Tags)
        {
            if (!canonical.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                canonical.Tags.Add(tag);
        }

        // Merge source resource IDs
        foreach (var sourceId in toRemove.SourceResourceIds)
        {
            if (!canonical.SourceResourceIds.Contains(sourceId))
                canonical.SourceResourceIds.Add(sourceId);
        }

        // Keep longer summary if different
        if (toRemove.Summary.Length > canonical.Summary.Length)
        {
            canonical.Summary = toRemove.Summary;
        }

        // Update all relationships pointing to toRemove to point to canonical
        foreach (var rel in conceptMap.Relations)
        {
            if (rel.SourceConceptId == toRemove.Id)
                rel.SourceConceptId = canonical.Id;
            if (rel.TargetConceptId == toRemove.Id)
                rel.TargetConceptId = canonical.Id;
        }

        // Remove self-referential relationships
        conceptMap.Relations.RemoveAll(r => r.SourceConceptId == r.TargetConceptId);

        // Remove duplicate relationships
        var seen = new HashSet<string>();
        conceptMap.Relations.RemoveAll(r =>
        {
            var key = $"{r.SourceConceptId}|{r.TargetConceptId}|{r.RelationType}";
            if (seen.Contains(key))
                return true;
            seen.Add(key);
            return false;
        });

        // Remove the duplicate concept
        conceptMap.Concepts.RemoveAll(c => c.Id == toRemove.Id);

        // Remove complexity entry for removed concept
        conceptMap.ComplexityOrder.RemoveAll(c => c.ConceptId == toRemove.Id);

        canonical.UpdatedAt = DateTime.UtcNow;
        conceptMap.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Merges concepts across different ConceptMaps.
    /// Relationships in the source map are updated to point to the canonical concept.
    /// </summary>
    private void MergeConceptsCrossMap(
        ConceptMap canonicalMap,
        ConceptMap sourceMap,
        Concept canonical,
        Concept toRemove)
    {
        // Merge metadata into canonical
        foreach (var alias in toRemove.Aliases)
        {
            if (!canonical.Aliases.Contains(alias, StringComparer.OrdinalIgnoreCase))
                canonical.Aliases.Add(alias);
        }

        if (!canonical.Title.Equals(toRemove.Title, StringComparison.OrdinalIgnoreCase) &&
            !canonical.Aliases.Contains(toRemove.Title, StringComparer.OrdinalIgnoreCase))
        {
            canonical.Aliases.Add(toRemove.Title);
        }

        foreach (var tag in toRemove.Tags)
        {
            if (!canonical.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                canonical.Tags.Add(tag);
        }

        foreach (var sourceId in toRemove.SourceResourceIds)
        {
            if (!canonical.SourceResourceIds.Contains(sourceId))
                canonical.SourceResourceIds.Add(sourceId);
        }

        // Update relationships in source map to point to canonical
        foreach (var rel in sourceMap.Relations)
        {
            if (rel.SourceConceptId == toRemove.Id)
                rel.SourceConceptId = canonical.Id;
            if (rel.TargetConceptId == toRemove.Id)
                rel.TargetConceptId = canonical.Id;
        }

        // Move relationships to canonical's map if they now point to canonical
        var relationsToMove = sourceMap.Relations
            .Where(r => r.SourceConceptId == canonical.Id || r.TargetConceptId == canonical.Id)
            .ToList();

        foreach (var rel in relationsToMove)
        {
            // Check if this relationship already exists in canonical's map
            var exists = canonicalMap.Relations.Any(r =>
                r.SourceConceptId == rel.SourceConceptId &&
                r.TargetConceptId == rel.TargetConceptId &&
                r.RelationType == rel.RelationType);

            if (!exists)
            {
                canonicalMap.Relations.Add(rel);
            }

            sourceMap.Relations.Remove(rel);
        }

        // Remove self-referential relationships
        canonicalMap.Relations.RemoveAll(r => r.SourceConceptId == r.TargetConceptId);
        sourceMap.Relations.RemoveAll(r => r.SourceConceptId == r.TargetConceptId);

        // Remove the duplicate concept from source map
        sourceMap.Concepts.RemoveAll(c => c.Id == toRemove.Id);
        sourceMap.ComplexityOrder.RemoveAll(c => c.ConceptId == toRemove.Id);

        canonical.UpdatedAt = DateTime.UtcNow;
        canonicalMap.UpdatedAt = DateTime.UtcNow;
        sourceMap.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Selects the best concept to use as the canonical version.
    /// Prefers the one with the longest/most complete title and summary.
    /// </summary>
    private Concept SelectCanonicalConcept(List<Concept> concepts)
    {
        return concepts
            .OrderByDescending(c => c.Title.Length)
            .ThenByDescending(c => c.Summary.Length)
            .ThenByDescending(c => c.Aliases.Count)
            .ThenBy(c => c.CreatedAt)
            .First();
    }

    private string NormalizeTitle(string title)
    {
        return title.ToLowerInvariant().Trim();
    }

    private string GetPairKey(string idA, string idB)
    {
        return string.Compare(idA, idB, StringComparison.Ordinal) < 0
            ? $"{idA}|{idB}"
            : $"{idB}|{idA}";
    }

    private List<AISimilarityResult> ParseAIResponse(string json, SimilarConceptPair[] originalPairs)
    {
        var results = new List<AISimilarityResult>();

        try
        {
            using var doc = JsonDocument.Parse(json);
            string? responseText = null;

            if (doc.RootElement.TryGetProperty("output_text", out var outText))
            {
                responseText = outText.GetString();
            }
            else if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                     choices.GetArrayLength() > 0)
            {
                var firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    responseText = content.GetString();
                }
            }

            if (string.IsNullOrEmpty(responseText))
                return results;

            // Extract JSON from response
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonContent = responseText[jsonStart..(jsonEnd + 1)];
                using var resultDoc = JsonDocument.Parse(jsonContent);

                if (resultDoc.RootElement.TryGetProperty("pairs", out var pairs))
                {
                    foreach (var pair in pairs.EnumerateArray())
                    {
                        var conceptA = pair.GetProperty("conceptA").GetString() ?? "";
                        var conceptB = pair.GetProperty("conceptB").GetString() ?? "";
                        var same = pair.GetProperty("same").GetBoolean();
                        var confidence = (float)pair.GetProperty("confidence").GetDouble();
                        var canonical = pair.TryGetProperty("canonical", out var c) ? c.GetString() : null;
                        var reason = pair.TryGetProperty("reason", out var r) ? r.GetString() : null;

                        // Find the matching original pair
                        var originalPair = originalPairs.FirstOrDefault(p =>
                            p.ConceptA.Title.Equals(conceptA, StringComparison.OrdinalIgnoreCase) &&
                            p.ConceptB.Title.Equals(conceptB, StringComparison.OrdinalIgnoreCase));

                        if (originalPair != null)
                        {
                            results.Add(new AISimilarityResult
                            {
                                Pair = originalPair,
                                IsSameConcept = same,
                                AIConfidence = confidence,
                                SuggestedCanonicalTitle = canonical,
                                Reason = reason
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warn($"[ConceptMergeService] Failed to parse AI response: {ex.Message}");
        }

        return results;
    }

    #region MergedCourseConceptMap Methods (Non-Destructive)

    /// <summary>
    /// Finds duplicates within a MergedCourseConceptMap (course-specific copy).
    /// This does NOT modify the original resource ConceptMaps.
    /// </summary>
    public DuplicateDetectionResult FindDuplicatesInMergedMap(MergedCourseConceptMap mergedMap)
    {
        var result = new DuplicateDetectionResult();
        var concepts = mergedMap.Concepts;

        if (concepts.Count < 2)
            return result;

        // Group by normalized title for exact duplicates
        var titleGroups = concepts
            .GroupBy(c => NormalizeTitle(c.Title))
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in titleGroups)
        {
            var conceptList = group.ToList();
            result.ExactDuplicates.Add(new DuplicateGroup
            {
                MatchType = DuplicateMatchType.Exact,
                Concepts = conceptList,
                Confidence = 1.0f,
                SuggestedCanonical = SelectCanonicalConcept(conceptList)
            });
        }

        // Find similar concepts using SimHash
        var checkedPairs = new HashSet<string>();
        foreach (var conceptA in concepts)
        {
            var sigA = simHashService.GetSignature64($"{conceptA.Title} {conceptA.Summary}");

            foreach (var conceptB in concepts)
            {
                if (conceptA.Id == conceptB.Id)
                    continue;

                var pairKey = GetPairKey(conceptA.Id, conceptB.Id);
                if (checkedPairs.Contains(pairKey))
                    continue;
                checkedPairs.Add(pairKey);

                // Skip if already in an exact duplicate group
                if (NormalizeTitle(conceptA.Title) == NormalizeTitle(conceptB.Title))
                    continue;

                var sigB = simHashService.GetSignature64($"{conceptB.Title} {conceptB.Summary}");
                var distance = SimHashService.HammingDistance(sigA, sigB);

                if (distance <= MaxHammingDistanceForSimilar)
                {
                    var similarity = 1.0f - (distance / 64.0f);

                    result.SimilarConcepts.Add(new SimilarConceptPair
                    {
                        ConceptA = conceptA,
                        ConceptB = conceptB,
                        SimilarityScore = similarity,
                        HammingDistance = distance,
                        NeedsUserConfirmation = true
                    });
                }
            }
        }

        result.TotalDuplicateGroups = result.ExactDuplicates.Count;
        result.TotalSimilarPairs = result.SimilarConcepts.Count;

        return result;
    }

    /// <summary>
    /// Merges approved concept pairs within a MergedCourseConceptMap.
    /// This only modifies the course-specific merged map, NOT the original ConceptMaps.
    /// </summary>
    public MergeResult MergeInMergedMap(MergedCourseConceptMap mergedMap, List<ApprovedMerge> approvedMerges)
    {
        var result = new MergeResult();

        foreach (var merge in approvedMerges)
        {
            var conceptA = mergedMap.Concepts.FirstOrDefault(c => c.Id == merge.ConceptAId);
            var conceptB = mergedMap.Concepts.FirstOrDefault(c => c.Id == merge.ConceptBId);

            if (conceptA == null || conceptB == null)
                continue;

            // Determine canonical based on merge direction
            var (canonical, toRemove) = merge.KeepConceptAId == merge.ConceptAId
                ? (conceptA, conceptB)
                : (conceptB, conceptA);

            // Update the canonical's title if a new one was suggested
            if (!string.IsNullOrEmpty(merge.CanonicalTitle))
            {
                canonical.Title = merge.CanonicalTitle;
            }

            // Merge the concepts
            MergeConceptsInMergedMap(mergedMap, canonical, toRemove);
            
            // Track the merge for audit/undo
            mergedMap.MergedConceptMapping[toRemove.Id] = canonical.Id;
            result.MergedConceptIds.Add(toRemove.Id);
        }

        mergedMap.UpdatedAt = DateTime.UtcNow;
        result.MergedCount = result.MergedConceptIds.Count;
        result.RemainingConceptCount = mergedMap.Concepts.Count;

        return result;
    }

    /// <summary>
    /// Auto-merges exact duplicates in a MergedCourseConceptMap.
    /// Returns the number of concepts merged.
    /// </summary>
    public MergeResult AutoMergeExactDuplicatesInMergedMap(MergedCourseConceptMap mergedMap)
    {
        var result = new MergeResult();
        var duplicates = FindDuplicatesInMergedMap(mergedMap);

        foreach (var group in duplicates.ExactDuplicates)
        {
            var canonical = group.SuggestedCanonical;
            var toRemove = group.Concepts.Where(c => c.Id != canonical.Id).ToList();

            foreach (var duplicate in toRemove)
            {
                MergeConceptsInMergedMap(mergedMap, canonical, duplicate);
                mergedMap.MergedConceptMapping[duplicate.Id] = canonical.Id;
                result.MergedConceptIds.Add(duplicate.Id);
            }
        }

        mergedMap.UpdatedAt = DateTime.UtcNow;
        result.MergedCount = result.MergedConceptIds.Count;
        result.RemainingConceptCount = mergedMap.Concepts.Count;

        return result;
    }

    /// <summary>
    /// Merges two concepts within a MergedCourseConceptMap.
    /// </summary>
    private void MergeConceptsInMergedMap(MergedCourseConceptMap mergedMap, Concept canonical, Concept toRemove)
    {
        // Merge aliases
        foreach (var alias in toRemove.Aliases)
        {
            if (!canonical.Aliases.Contains(alias, StringComparer.OrdinalIgnoreCase))
                canonical.Aliases.Add(alias);
        }

        // Add the removed concept's title as an alias if different
        if (!canonical.Title.Equals(toRemove.Title, StringComparison.OrdinalIgnoreCase) &&
            !canonical.Aliases.Contains(toRemove.Title, StringComparer.OrdinalIgnoreCase))
        {
            canonical.Aliases.Add(toRemove.Title);
        }

        // Merge tags
        foreach (var tag in toRemove.Tags)
        {
            if (!canonical.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                canonical.Tags.Add(tag);
        }

        // Merge source resource IDs
        foreach (var sourceId in toRemove.SourceResourceIds)
        {
            if (!canonical.SourceResourceIds.Contains(sourceId))
                canonical.SourceResourceIds.Add(sourceId);
        }

        // Keep longer summary if different
        if (toRemove.Summary.Length > canonical.Summary.Length)
        {
            canonical.Summary = toRemove.Summary;
        }

        // Update all relationships pointing to toRemove to point to canonical
        foreach (var rel in mergedMap.Relations)
        {
            if (rel.SourceConceptId == toRemove.Id)
                rel.SourceConceptId = canonical.Id;
            if (rel.TargetConceptId == toRemove.Id)
                rel.TargetConceptId = canonical.Id;
        }

        // Remove self-referential relationships
        mergedMap.Relations.RemoveAll(r => r.SourceConceptId == r.TargetConceptId);

        // Remove duplicate relationships
        var seen = new HashSet<string>();
        mergedMap.Relations.RemoveAll(r =>
        {
            var key = $"{r.SourceConceptId}|{r.TargetConceptId}|{r.RelationType}";
            if (seen.Contains(key))
                return true;
            seen.Add(key);
            return false;
        });

        // Remove the duplicate concept
        mergedMap.Concepts.RemoveAll(c => c.Id == toRemove.Id);
    }

    #endregion
}

/// <summary>
/// Result of duplicate detection analysis.
/// </summary>
public class DuplicateDetectionResult
{
    /// <summary>
    /// Groups of exact duplicate concepts (same title).
    /// </summary>
    public List<DuplicateGroup> ExactDuplicates { get; set; } = [];

    /// <summary>
    /// Pairs of similar concepts that may be duplicates.
    /// </summary>
    public List<SimilarConceptPair> SimilarConcepts { get; set; } = [];

    /// <summary>
    /// Total number of exact duplicate groups found.
    /// </summary>
    public int TotalDuplicateGroups { get; set; }

    /// <summary>
    /// Total number of similar pairs found.
    /// </summary>
    public int TotalSimilarPairs { get; set; }

    /// <summary>
    /// Whether any duplicates or similar concepts were found.
    /// </summary>
    public bool HasDuplicates => TotalDuplicateGroups > 0 || TotalSimilarPairs > 0;
}

/// <summary>
/// Type of duplicate match.
/// </summary>
public enum DuplicateMatchType
{
    Exact,
    Similar,
    AIConfirmed
}

/// <summary>
/// A group of concepts that are exact duplicates.
/// </summary>
public class DuplicateGroup
{
    public DuplicateMatchType MatchType { get; set; }
    public List<Concept> Concepts { get; set; } = [];
    public float Confidence { get; set; }
    public Concept SuggestedCanonical { get; set; } = null!;
}

/// <summary>
/// A pair of concepts that appear similar but may need user confirmation.
/// </summary>
public class SimilarConceptPair
{
    public Concept ConceptA { get; set; } = null!;
    public Concept ConceptB { get; set; } = null!;
    public float SimilarityScore { get; set; }
    public int HammingDistance { get; set; }
    public bool NeedsUserConfirmation { get; set; }
}

/// <summary>
/// Result from AI similarity analysis.
/// </summary>
public class AISimilarityResult
{
    public SimilarConceptPair Pair { get; set; } = null!;
    public bool IsSameConcept { get; set; }
    public float AIConfidence { get; set; }
    public string? SuggestedCanonicalTitle { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Represents a user-approved merge operation.
/// </summary>
public class ApprovedMerge
{
    public string ConceptAId { get; set; } = "";
    public string ConceptBId { get; set; } = "";
    public string KeepConceptAId { get; set; } = "";
    public string? CanonicalTitle { get; set; }
}

/// <summary>
/// Result of a merge operation.
/// </summary>
public class MergeResult
{
    public int MergedCount { get; set; }
    public List<string> MergedConceptIds { get; set; } = [];
    public int RemainingConceptCount { get; set; }
    public string? ErrorMessage { get; set; }
}
