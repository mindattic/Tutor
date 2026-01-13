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
    /// Generate embedding vectors for multiple texts, automatically batching to respect API limits.
    /// </summary>
    // Maximum characters per text input (rough estimate: 8192 tokens * 4 chars/token)
    // Being conservative to account for non-English text and tokenization variance
    private const int MaxTextLength = 30000;
    
    // Maximum tokens per batch request (OpenAI limit is 300,000, we use 250,000 for safety margin)
    private const int MaxTokensPerBatch = 250000;
    
    // Rough estimate: 1 token ? 4 characters for English text
    private const int CharsPerToken = 4;

    public async Task<List<float[]>> GetEmbeddingsAsync(List<string> texts, CancellationToken ct = default)
    {
        if (texts.Count == 0)
            return [];

        // Filter out empty or whitespace-only strings (OpenAI API rejects these with HTTP 400)
        // Also truncate texts that are too long
        var validTexts = texts
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Length > MaxTextLength ? t[..MaxTextLength] : t)
            .ToList();
            
        if (validTexts.Count == 0)
        {
            Log.Warn("EmbeddingService: All input texts are empty or whitespace");
            return [];
        }

        var filteredCount = texts.Count(t => string.IsNullOrWhiteSpace(t));
        var truncatedCount = texts.Count(t => !string.IsNullOrWhiteSpace(t) && t.Length > MaxTextLength);
        
        if (filteredCount > 0)
        {
            Log.Warn($"EmbeddingService: Filtered out {filteredCount} empty/whitespace texts");
        }
        if (truncatedCount > 0)
        {
            Log.Warn($"EmbeddingService: Truncated {truncatedCount} texts exceeding {MaxTextLength} chars");
        }

        // Split into batches to respect token limits
        var batches = CreateBatches(validTexts);
        Log.Debug($"EmbeddingService: Processing {validTexts.Count} text(s) in {batches.Count} batch(es)");

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("EmbeddingService: API key is missing");
            throw new InvalidOperationException("OpenAI API key is missing.");
        }


        var allEmbeddings = new List<float[]>();
        
        // Log at Info level for visibility with large documents
        if (batches.Count > 1)
        {
            Log.Info($"EmbeddingService: Large document - processing {validTexts.Count} texts in {batches.Count} batches");
        }
        
        for (int batchIndex = 0; batchIndex < batches.Count; batchIndex++)
        {
            ct.ThrowIfCancellationRequested();
            
            var batch = batches[batchIndex];
            
            if (batches.Count > 1)
            {
                Log.Info($"EmbeddingService: Processing batch {batchIndex + 1}/{batches.Count} ({batch.Count} texts, {allEmbeddings.Count}/{validTexts.Count} complete)");
            }
            else
            {
                Log.Debug($"EmbeddingService: Processing {batch.Count} texts");
            }
            
            // Retry with exponential backoff for rate limiting
            var batchEmbeddings = await GetEmbeddingsBatchWithRetryAsync(batch, apiKey, ct);
            allEmbeddings.AddRange(batchEmbeddings);
            
            // Delay between batches to avoid rate limiting (longer for multi-batch)
            if (batchIndex < batches.Count - 1)
            {
                var delayMs = batches.Count > 4 ? 1000 : 200; // Longer delay for very large documents
                await Task.Delay(delayMs, ct);
            }
        }

        Log.Info($"EmbeddingService: Generated {allEmbeddings.Count} embedding(s) total");
        return allEmbeddings;
    }

    /// <summary>
    /// Split texts into batches that fit within the token limit.
    /// </summary>
    private static List<List<string>> CreateBatches(List<string> texts)
    {
        var batches = new List<List<string>>();
        var currentBatch = new List<string>();
        var currentTokenCount = 0;

        foreach (var text in texts)
        {
            var estimatedTokens = text.Length / CharsPerToken;
            
            // If adding this text would exceed the limit, start a new batch
            if (currentBatch.Count > 0 && currentTokenCount + estimatedTokens > MaxTokensPerBatch)
            {
                batches.Add(currentBatch);
                currentBatch = new List<string>();
                currentTokenCount = 0;
            }
            
            currentBatch.Add(text);
            currentTokenCount += estimatedTokens;
        }
        
        // Add the last batch if it has any items
        if (currentBatch.Count > 0)
        {
            batches.Add(currentBatch);
        }

        return batches;
    }

    /// <summary>
    /// Retry a batch request with exponential backoff for rate limiting.
    /// </summary>
    private async Task<List<float[]>> GetEmbeddingsBatchWithRetryAsync(
        List<string> texts, 
        string apiKey, 
        CancellationToken ct,
        int maxRetries = 5)
    {
        var baseDelayMs = 2000; // Start with 2 seconds
        
        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await GetEmbeddingsBatchAsync(texts, apiKey, ct);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("429") || ex.Message.Contains("Rate"))
            {
                if (attempt == maxRetries)
                {
                    Log.Error($"EmbeddingService: Rate limit exceeded after {maxRetries + 1} attempts");
                    throw;
                }
                
                // Exponential backoff: 2s, 4s, 8s, 16s, 32s
                var delayMs = baseDelayMs * (int)Math.Pow(2, attempt);
                
                // Try to extract wait time from error message (e.g., "Please try again in 5.302s")
                var waitMatch = System.Text.RegularExpressions.Regex.Match(ex.Message, @"try again in (\d+\.?\d*)s");
                if (waitMatch.Success && double.TryParse(waitMatch.Groups[1].Value, out var waitSeconds))
                {
                    delayMs = (int)(waitSeconds * 1000) + 500; // Add 500ms buffer
                }
                
                Log.Warn($"EmbeddingService: Rate limited, waiting {delayMs}ms before retry {attempt + 1}/{maxRetries}");
                await Task.Delay(delayMs, ct);
            }
        }
        
        // Should never reach here, but compiler needs it
        throw new InvalidOperationException("Unexpected retry loop exit");
    }

    /// <summary>
    /// Send a single batch to the OpenAI API.
    /// </summary>
    private async Task<List<float[]>> GetEmbeddingsBatchAsync(List<string> texts, string apiKey, CancellationToken ct)
    {
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
            Log.Trace($"EmbeddingService: Sending batch request ({texts.Count} texts)...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            // Log all non-200 responses with detailed info
            if (statusCode != 200)
            {
                // Try to extract the actual error message from OpenAI's response
                var apiErrorMessage = ExtractOpenAIErrorMessage(json);
                
                Log.Error($"EmbeddingService: API error HTTP {statusCode} {resp.ReasonPhrase}" + 
                    (apiErrorMessage != null ? $" - {apiErrorMessage}" : ""));
                Log.Debug($"EmbeddingService error body: {(json.Length > 500 ? json[..500] + "..." : json)}");
                
                var errorMessage = statusCode switch
                {
                    400 => $"Bad Request - {apiErrorMessage ?? "Invalid embedding parameters"}",
                    401 => "Unauthorized - Invalid or expired API key",
                    403 => "Forbidden - API key lacks permissions",
                    404 => "Not Found - Invalid endpoint or model",
                    429 => "Rate Limited - Too many requests",
                    500 => "OpenAI Server Error",
                    502 => "Bad Gateway - OpenAI temporarily unavailable",
                    503 => "Service Unavailable - OpenAI overloaded",
                    _ => apiErrorMessage ?? json
                };
                
                throw new InvalidOperationException($"Embedding HTTP {statusCode}: {errorMessage}");
            }

            Log.Trace($"EmbeddingService: Batch response HTTP {statusCode} ({json.Length} chars)");
            return ParseEmbeddingsResponse(json);
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
    /// Extract error message from OpenAI's error response JSON.
    /// </summary>
    private static string? ExtractOpenAIErrorMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                if (error.TryGetProperty("message", out var message))
                {
                    return message.GetString();
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return null;
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
