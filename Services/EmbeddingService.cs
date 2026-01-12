using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Services.Logging;

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
        Log.Debug("EmbeddingService initialized");
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

        Log.Debug($"EmbeddingService: Generating embeddings for {texts.Count} text(s)");

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("EmbeddingService: API key is missing");
            throw new InvalidOperationException("OpenAI API key is missing.");
        }

        var payload = new
        {
            model = EmbeddingModel,
            input = texts,
            dimensions = EmbeddingDimensions
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            Log.Trace("EmbeddingService: Sending HTTP request...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            // Log all non-200 responses with detailed info
            if (statusCode != 200)
            {
                Log.Error($"EmbeddingService: API error HTTP {statusCode} {resp.ReasonPhrase}");
                Log.Debug($"EmbeddingService error body: {(json.Length > 500 ? json[..500] + "..." : json)}");
                
                var errorMessage = statusCode switch
                {
                    400 => "Bad Request - Invalid embedding parameters",
                    401 => "Unauthorized - Invalid or expired API key",
                    403 => "Forbidden - API key lacks permissions",
                    404 => "Not Found - Invalid endpoint or model",
                    429 => "Rate Limited - Too many requests",
                    500 => "OpenAI Server Error",
                    502 => "Bad Gateway - OpenAI temporarily unavailable",
                    503 => "Service Unavailable - OpenAI overloaded",
                    _ => json
                };
                
                throw new InvalidOperationException($"Embedding HTTP {statusCode}: {errorMessage}");
            }

            Log.Debug($"EmbeddingService: Response HTTP {statusCode} ({json.Length} chars)");
            var result = ParseEmbeddingsResponse(json);
            Log.Debug($"EmbeddingService: Generated {result.Count} embedding(s)");
            return result;
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"EmbeddingService: Network error - {ex.Message} (Status: {ex.StatusCode})", ex);
            throw new InvalidOperationException($"Network error generating embeddings: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            Log.Error("EmbeddingService: Request timed out", ex);
            throw new InvalidOperationException("Embedding request timed out", ex);
        }
        catch (TaskCanceledException)
        {
            Log.Debug("EmbeddingService: Request cancelled by user");
            throw;
        }
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
        catch (Exception ex)
        {
            Log.Warn($"EmbeddingService: Failed to parse embeddings response - {ex.Message}");
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
