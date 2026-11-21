namespace Tutor.Ingestion.Storage;

/// <summary>A store of embedded content chunks that can be searched by query vector.</summary>
public interface IVectorStore
{
    /// <summary>Stores a resource's chunks, replacing any it already had.</summary>
    Task StoreChunksAsync(string resourceId, string curriculumId, List<ContentChunk> chunks);

    Task RemoveChunksForResourceAsync(string resourceId);

    Task RemoveChunksForCurriculumAsync(string curriculumId);

    /// <summary>Most similar chunks of a curriculum to <paramref name="queryEmbedding"/> (cosine).</summary>
    Task<List<ContentChunk>> SearchAsync(float[] queryEmbedding, string curriculumId, int topK = 5, float minSimilarity = 0.3f);

    Task<List<ContentChunk>> GetChunksForResourceAsync(string resourceId);

    Task<bool> HasChunksForResourceAsync(string resourceId);
}
