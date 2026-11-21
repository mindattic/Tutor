namespace Tutor.Ingestion.Embedding;

/// <summary>
/// Turns text into embedding vectors. The host implements it over whatever provider it uses (Tutor wraps
/// MindAttic.Legion); the package only depends on this shape. Implementations drop empty inputs, so the
/// result can be shorter than the input list.
/// </summary>
public interface IEmbeddingProvider
{
    Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default);

    Task<List<float[]>> GetEmbeddingsAsync(List<string> texts, CancellationToken ct = default);
}

/// <summary>Vector math shared by the stores.</summary>
public static class EmbeddingMath
{
    /// <summary>
    /// Cosine similarity between two vectors, in [-1, 1]: 1 = same direction, 0 = orthogonal, -1 = opposite.
    /// Returns 0 for a null, empty, zero-length or mismatched pair.
    /// </summary>
    public static float CosineSimilarity(float[] a, float[] b)
    {
        if (a is null || b is null || a.Length == 0 || b.Length == 0 || a.Length != b.Length)
            return 0f;
        double dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot   += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        if (normA == 0 || normB == 0) return 0f;
        return (float)(dot / (Math.Sqrt(normA) * Math.Sqrt(normB)));
    }
}
