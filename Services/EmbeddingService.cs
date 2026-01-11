using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Tutor.Services;

/// <summary>
/// Service for generating text embeddings using OpenAI's embedding API.
/// </summary>
public sealed class EmbeddingService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    
    // Using text-embedding-3-small for good quality at low cost
    private const string EmbeddingModel = "text-embedding-3-small";
    private const int EmbeddingDimensions = 1536;

    public EmbeddingService(HttpClient http, OpenAIOptions opt)
    {
        this.http = http;
        this.opt = opt;
    }

    /// <summary>
    /// Generate an embedding vector for a single text.
    /// </summary>
    public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default)
    {
        var embeddings = await GetEmbeddingsAsync([text], ct);
        return embeddings.Count > 0 ? embeddings[0] : [];
    }

    /// <summary>
    /// Generate embedding vectors for multiple texts in a single API call (more efficient).
    /// </summary>
    public async Task<List<float[]>> GetEmbeddingsAsync(List<string> texts, CancellationToken ct = default)
    {
        if (texts.Count == 0)
            return [];

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is missing.");

        var payload = new
        {
            model = EmbeddingModel,
            input = texts,
            dimensions = EmbeddingDimensions
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var resp = await http.SendAsync(req, ct);
        var json = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI embeddings error {(int)resp.StatusCode}: {json}");
        }

        return ParseEmbeddingsResponse(json);
    }

    private static List<float[]> ParseEmbeddingsResponse(string json)
    {
        var result = new List<float[]>();

        try
        {
            using var doc = JsonDocument.Parse(json);
            
            if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in data.EnumerateArray())
                {
                    if (item.TryGetProperty("embedding", out var embedding) && embedding.ValueKind == JsonValueKind.Array)
                    {
                        var vector = new float[embedding.GetArrayLength()];
                        var i = 0;
                        foreach (var val in embedding.EnumerateArray())
                        {
                            vector[i++] = val.GetSingle();
                        }
                        result.Add(vector);
                    }
                }
            }
        }
        catch
        {
            // Return empty list on parse failure
        }

        return result;
    }

    /// <summary>
    /// Calculate cosine similarity between two vectors.
    /// Returns value between -1 and 1, where 1 means identical.
    /// </summary>
    public static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length || a.Length == 0)
            return 0;

        float dotProduct = 0;
        float normA = 0;
        float normB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        var denominator = MathF.Sqrt(normA) * MathF.Sqrt(normB);
        return denominator == 0 ? 0 : dotProduct / denominator;
    }
}
