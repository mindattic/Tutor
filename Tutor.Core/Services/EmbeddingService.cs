using MindAttic.Legion;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Generates text embeddings via MindAttic.Legion. Wire transport (endpoint,
/// auth, response parsing, retries with backoff, circuit breaker) is owned by
/// Legion. This class keeps the Tutor-specific concerns: text validation,
/// length capping, batching to respect token limits, and inter-batch pacing.
/// </summary>
public sealed class EmbeddingService
{
    private readonly LegionClient legion;
    private readonly OpenAIOptions opt;

    private const string EmbeddingModel = "text-embedding-3-small";
    private const int EmbeddingDimensions = 1536;

    // Maximum characters per text input (rough estimate: 8192 tokens * 4 chars/token,
    // conservative for non-English text and tokenization variance)
    private const int MaxTextLength = 30000;

    // Maximum tokens per batch request (OpenAI limit is 300,000; safety margin to 250,000)
    private const int MaxTokensPerBatch = 250000;

    // Rough estimate: 1 token ≈ 4 characters for English text
    private const int CharsPerToken = 4;

    public EmbeddingService(LegionClient legion, OpenAIOptions opt)
    {
        this.legion = legion;
        this.opt = opt;
        Log.Debug("EmbeddingService initialized (delegating wire transport to MindAttic.Legion)");
    }

    public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default)
    {
        var embeddings = await GetEmbeddingsAsync([text], ct);
        return embeddings.Count > 0 ? embeddings[0] : [];
    }

    public async Task<List<float[]>> GetEmbeddingsAsync(List<string> texts, CancellationToken ct = default)
    {
        if (texts.Count == 0) return [];

        // Filter empties (OpenAI rejects with HTTP 400) and cap individual lengths.
        var validTexts = texts
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Length > MaxTextLength ? t[..MaxTextLength] : t)
            .ToList();

        if (validTexts.Count == 0)
        {
            Log.Warn("EmbeddingService: All input texts are empty or whitespace");
            return [];
        }

        var filteredCount  = texts.Count(t => string.IsNullOrWhiteSpace(t));
        var truncatedCount = texts.Count(t => !string.IsNullOrWhiteSpace(t) && t.Length > MaxTextLength);
        if (filteredCount > 0)
            Log.Warn($"EmbeddingService: Filtered out {filteredCount} empty/whitespace texts");
        if (truncatedCount > 0)
            Log.Warn($"EmbeddingService: Truncated {truncatedCount} texts exceeding {MaxTextLength} chars");

        var batches = CreateBatches(validTexts);
        Log.Debug($"EmbeddingService: Processing {validTexts.Count} text(s) in {batches.Count} batch(es)");

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = MindAtticCredentialStore.GetKey("openai");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("EmbeddingService: API key is missing");
            throw new InvalidOperationException("OpenAI API key is missing.");
        }

        var allEmbeddings = new List<float[]>();
        if (batches.Count > 1)
            Log.Info($"EmbeddingService: Large document — processing {validTexts.Count} texts in {batches.Count} batches");

        for (int batchIndex = 0; batchIndex < batches.Count; batchIndex++)
        {
            ct.ThrowIfCancellationRequested();
            var batch = batches[batchIndex];

            if (batches.Count > 1)
                Log.Info($"EmbeddingService: Batch {batchIndex + 1}/{batches.Count} ({batch.Count} texts, {allEmbeddings.Count}/{validTexts.Count} done)");
            else
                Log.Debug($"EmbeddingService: Processing {batch.Count} texts");

            IReadOnlyList<float[]> batchEmbeddings;
            try
            {
                batchEmbeddings = await legion.EmbedAsync(
                    providerId: "openai",
                    apiKey: apiKey!,
                    model: EmbeddingModel,
                    inputs: batch,
                    dimensions: EmbeddingDimensions,
                    ct: ct);
            }
            catch (CircuitBreakerOpenException ex)
            {
                Log.Warn($"[Tutor] EmbeddingService: Legion circuit breaker open for openai - {ex.Message}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                Log.Error($"[Tutor] EmbeddingService: Legion embed call failed (status={ex.StatusCode}) - {ex.Message}", ex);
                throw;
            }
            allEmbeddings.AddRange(batchEmbeddings);

            // Inter-batch pacing for very large documents.
            if (batchIndex < batches.Count - 1)
            {
                var delayMs = batches.Count > 4 ? 1000 : 200;
                await Task.Delay(delayMs, ct);
            }
        }

        Log.Info($"EmbeddingService: Generated {allEmbeddings.Count} embedding(s) total");
        return allEmbeddings;
    }

    private static List<List<string>> CreateBatches(List<string> texts)
    {
        var batches = new List<List<string>>();
        var current = new List<string>();
        var tokens = 0;
        foreach (var text in texts)
        {
            var estimated = text.Length / CharsPerToken;
            if (current.Count > 0 && tokens + estimated > MaxTokensPerBatch)
            {
                batches.Add(current);
                current = new List<string>();
                tokens = 0;
            }
            current.Add(text);
            tokens += estimated;
        }
        if (current.Count > 0) batches.Add(current);
        return batches;
    }

    /// <summary>
    /// Cosine similarity between two embedding vectors. Returns a value in [-1, 1];
    /// 1 = identical direction, 0 = orthogonal, -1 = opposite.
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
