using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for storing and searching content chunks with embeddings.
/// Uses JSON file storage for simplicity (works across all MAUI platforms).
/// Supports LSH-based candidate generation and combined similarity scoring.
/// </summary>
public sealed class VectorStoreService
{
    private const string ChunksFileName = "content_chunks.json";
    private readonly string chunksFilePath;
    private readonly LSHService lshService;
    private readonly SimHashService simHashService;
    private List<ContentChunk>? cachedChunks;

    // Default LSH threshold for candidate generation (max Hamming distance)
    private const int DefaultLshThreshold = 64;

    // Scoring weights for combined similarity
    private const float CosineWeight = 0.70f;
    private const float SemanticWeight = 0.20f;
    private const float LexicalWeight = 0.10f;

    public VectorStoreService(LSHService lshService, SimHashService simHashService)
    {
        chunksFilePath = Path.Combine(FileSystem.AppDataDirectory, ChunksFileName);
        this.lshService = lshService;
        this.simHashService = simHashService;
        Log.Debug($"VectorStoreService initialized at: {chunksFilePath}");
    }

    /// <summary>
    /// Store chunks for a resource (replaces any existing chunks for that resource).
    /// </summary>
    public async Task StoreChunksAsync(string resourceId, string curriculumId, List<ContentChunk> chunks)
    {
        Log.Info($"VectorStore: Storing {chunks.Count} chunks for resource {resourceId}");
        var allChunks = await LoadChunksAsync();
        
        // Remove existing chunks for this resource
        var removed = allChunks.RemoveAll(c => c.ResourceId == resourceId);
        if (removed > 0)
            Log.Debug($"VectorStore: Removed {removed} existing chunks for resource {resourceId}");
        
        // Add new chunks
        allChunks.AddRange(chunks);
        
        await SaveChunksAsync(allChunks);
        Log.Debug($"VectorStore: Total chunks now: {allChunks.Count}");
    }

    /// <summary>
    /// Remove all chunks for a specific resource.
    /// </summary>
    public async Task RemoveChunksForResourceAsync(string resourceId)
    {
        Log.Debug($"VectorStore: Removing chunks for resource {resourceId}");
        var allChunks = await LoadChunksAsync();
        var removed = allChunks.RemoveAll(c => c.ResourceId == resourceId);
        Log.Info($"VectorStore: Removed {removed} chunks for resource {resourceId}");
        await SaveChunksAsync(allChunks);
    }

    /// <summary>
    /// Remove all chunks for a specific curriculum.
    /// </summary>
    public async Task RemoveChunksForCurriculumAsync(string curriculumId)
    {
        Log.Debug($"VectorStore: Removing chunks for curriculum {curriculumId}");
        var allChunks = await LoadChunksAsync();
        var removed = allChunks.RemoveAll(c => c.CurriculumId == curriculumId);
        Log.Info($"VectorStore: Removed {removed} chunks for curriculum {curriculumId}");
        await SaveChunksAsync(allChunks);
    }

    /// <summary>
    /// Search for the most relevant chunks given a query embedding.
    /// Uses cosine similarity only (for backward compatibility when signatures are not available).
    /// </summary>
    public async Task<List<ContentChunk>> SearchAsync(
        float[] queryEmbedding, 
        string curriculumId, 
        int topK = 5,
        float minSimilarity = 0.3f)
    {
        Log.Debug($"VectorStore: Searching curriculum {curriculumId} (topK={topK})");
        var allChunks = await LoadChunksAsync();
        
        // Filter to curriculum and calculate similarities
        var results = allChunks
            .Where(c => c.CurriculumId == curriculumId && c.Embedding.Length > 0)
            .Select(c => new { Chunk = c, Similarity = EmbeddingService.CosineSimilarity(queryEmbedding, c.Embedding) })
            .Where(x => x.Similarity >= minSimilarity)
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .Select(x => x.Chunk)
            .ToList();
        
        Log.Debug($"VectorStore: Found {results.Count} matching chunks");

        return results;
    }

    /// <summary>
    /// Search using a text query with combined similarity scoring.
    /// Uses LSH for candidate generation and combines cosine, semantic, and lexical similarity.
    /// </summary>
    public async Task<List<ContentChunk>> SearchAsync(
        string query,
        string curriculumId,
        EmbeddingService embeddingService,
        int topK = 5,
        float minSimilarity = 0.3f,
        CancellationToken ct = default)
    {
        return await SearchAsync(query, curriculumId, embeddingService, topK, minSimilarity, DefaultLshThreshold, ct);
    }

    /// <summary>
    /// Search using a text query with combined similarity scoring and configurable LSH threshold.
    /// </summary>
    /// <param name="query">The search query text.</param>
    /// <param name="curriculumId">The curriculum to search within.</param>
    /// <param name="embeddingService">Service to generate query embedding.</param>
    /// <param name="topK">Maximum number of results to return.</param>
    /// <param name="minSimilarity">Minimum cosine similarity threshold.</param>
    /// <param name="lshThreshold">Maximum LSH Hamming distance for candidate selection.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of most relevant content chunks.</returns>
    public async Task<List<ContentChunk>> SearchAsync(
        string query,
        string curriculumId,
        EmbeddingService embeddingService,
        int topK,
        float minSimilarity,
        int lshThreshold,
        CancellationToken ct = default)
    {
        // Get query embedding
        var queryEmbedding = await embeddingService.GetEmbeddingAsync(query, ct);
        if (queryEmbedding.Length == 0)
            return [];

        // Compute query signatures
        var querySemanticSig = lshService.GetSignature(queryEmbedding);
        var queryLexicalSig = simHashService.GetSignature64(query);

        var allChunks = await LoadChunksAsync();

        // Filter to curriculum with valid embeddings
        var candidateChunks = allChunks
            .Where(c => c.CurriculumId == curriculumId && c.Embedding.Length > 0)
            .ToList();

        if (candidateChunks.Count == 0)
            return [];

        // Check if any chunks have semantic signatures for LSH filtering
        var chunksWithSignatures = candidateChunks
            .Where(c => c.SemanticSignature != null && c.SemanticSignature.Length > 0)
            .ToList();

        List<ContentChunk> candidates;

        if (chunksWithSignatures.Count > 0 && querySemanticSig.Length > 0)
        {
            // LSH candidate generation: filter by Hamming distance
            candidates = chunksWithSignatures
                .Where(c => LSHService.HammingDistance(querySemanticSig, c.SemanticSignature) <= lshThreshold)
                .ToList();

            // If no candidates pass LSH filter, fall back to all chunks with signatures
            if (candidates.Count == 0)
            {
                candidates = chunksWithSignatures;
            }
        }
        else
        {
            // No signatures available, use all candidate chunks (fallback to cosine-only)
            candidates = candidateChunks;
        }

        // Score and rank candidates
        var scoredResults = candidates
            .Select(c => new
            {
                Chunk = c,
                Cosine = EmbeddingService.CosineSimilarity(queryEmbedding, c.Embedding),
                SemanticHamming = c.SemanticSignature != null && c.SemanticSignature.Length > 0
                    ? LSHService.HammingDistance(querySemanticSig, c.SemanticSignature)
                    : lshService.BitCount, // Max distance if no signature
                LexicalHamming = SimHashService.HammingDistance(queryLexicalSig, c.LexicalSignature)
            })
            .Where(x => x.Cosine >= minSimilarity) // Enforce minimum cosine similarity
            .Select(x => new
            {
                x.Chunk,
                x.Cosine,
                SemanticScore = 1.0f - ((float)x.SemanticHamming / lshService.BitCount),
                LexicalScore = 1.0f - ((float)x.LexicalHamming / SimHashService.BitCount),
            })
            .Select(x => new
            {
                x.Chunk,
                CombinedScore = (CosineWeight * x.Cosine) + (SemanticWeight * x.SemanticScore) + (LexicalWeight * x.LexicalScore)
            })
            .OrderByDescending(x => x.CombinedScore)
            .Take(topK)
            .Select(x => x.Chunk)
            .ToList();

        return scoredResults;
    }

    /// <summary>
    /// Get all chunks for a specific resource.
    /// </summary>
    public async Task<List<ContentChunk>> GetChunksForResourceAsync(string resourceId)
    {
        var allChunks = await LoadChunksAsync();
        return [.. allChunks.Where(c => c.ResourceId == resourceId).OrderBy(c => c.ChunkIndex)];
    }

    /// <summary>
    /// Check if a resource has been chunked and embedded.
    /// </summary>
    public async Task<bool> HasChunksForResourceAsync(string resourceId)
    {
        var allChunks = await LoadChunksAsync();
        return allChunks.Any(c => c.ResourceId == resourceId);
    }

    /// <summary>
    /// Get statistics about stored chunks.
    /// </summary>
    public async Task<(int TotalChunks, int ResourceCount, int CurriculumCount)> GetStatsAsync()
    {
        var allChunks = await LoadChunksAsync();
        return (
            allChunks.Count,
            allChunks.Select(c => c.ResourceId).Distinct().Count(),
            allChunks.Select(c => c.CurriculumId).Distinct().Count()
        );
    }

    private async Task<List<ContentChunk>> LoadChunksAsync()
    {
        if (cachedChunks != null)
            return cachedChunks;

        try
        {
            if (File.Exists(chunksFilePath))
            {
                var json = await File.ReadAllTextAsync(chunksFilePath);
                cachedChunks = JsonSerializer.Deserialize<List<ContentChunk>>(json) ?? [];
            }
            else
            {
                cachedChunks = [];
            }
        }
        catch
        {
            cachedChunks = [];
        }

        return cachedChunks;
    }

    private async Task SaveChunksAsync(List<ContentChunk> chunks)
    {
        cachedChunks = chunks;
        
        try
        {
            var json = JsonSerializer.Serialize(chunks, new JsonSerializerOptions { WriteIndented = false });
            await File.WriteAllTextAsync(chunksFilePath, json);
        }
        catch
        {
            // Log or handle error
        }
    }

    /// <summary>
    /// Clear all stored chunks (for debugging/reset).
    /// </summary>
    public async Task ClearAllAsync()
    {
        cachedChunks = [];
        try
        {
            if (File.Exists(chunksFilePath))
            {
                File.Delete(chunksFilePath);
            }
        }
        catch { }
        await Task.CompletedTask;
    }
}
