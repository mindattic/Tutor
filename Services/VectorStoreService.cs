using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Service for storing and searching content chunks with embeddings.
/// Uses JSON file storage for simplicity (works across all MAUI platforms).
/// </summary>
public sealed class VectorStoreService
{
    private const string ChunksFileName = "content_chunks.json";
    private readonly string chunksFilePath;
    private List<ContentChunk>? cachedChunks;

    public VectorStoreService()
    {
        chunksFilePath = Path.Combine(FileSystem.AppDataDirectory, ChunksFileName);
    }

    /// <summary>
    /// Store chunks for a resource (replaces any existing chunks for that resource).
    /// </summary>
    public async Task StoreChunksAsync(string resourceId, string curriculumId, List<ContentChunk> chunks)
    {
        var allChunks = await LoadChunksAsync();
        
        // Remove existing chunks for this resource
        allChunks.RemoveAll(c => c.ResourceId == resourceId);
        
        // Add new chunks
        allChunks.AddRange(chunks);
        
        await SaveChunksAsync(allChunks);
    }

    /// <summary>
    /// Remove all chunks for a specific resource.
    /// </summary>
    public async Task RemoveChunksForResourceAsync(string resourceId)
    {
        var allChunks = await LoadChunksAsync();
        allChunks.RemoveAll(c => c.ResourceId == resourceId);
        await SaveChunksAsync(allChunks);
    }

    /// <summary>
    /// Remove all chunks for a specific curriculum.
    /// </summary>
    public async Task RemoveChunksForCurriculumAsync(string curriculumId)
    {
        var allChunks = await LoadChunksAsync();
        allChunks.RemoveAll(c => c.CurriculumId == curriculumId);
        await SaveChunksAsync(allChunks);
    }

    /// <summary>
    /// Search for the most relevant chunks given a query embedding.
    /// </summary>
    public async Task<List<ContentChunk>> SearchAsync(
        float[] queryEmbedding, 
        string curriculumId, 
        int topK = 5,
        float minSimilarity = 0.3f)
    {
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

        return results;
    }

    /// <summary>
    /// Search using a text query (requires embedding service to embed the query).
    /// </summary>
    public async Task<List<ContentChunk>> SearchAsync(
        string query,
        string curriculumId,
        EmbeddingService embeddingService,
        int topK = 5,
        float minSimilarity = 0.3f,
        CancellationToken ct = default)
    {
        var queryEmbedding = await embeddingService.GetEmbeddingAsync(query, ct);
        if (queryEmbedding.Length == 0)
            return [];

        return await SearchAsync(queryEmbedding, curriculumId, topK, minSimilarity);
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
