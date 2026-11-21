using MindAttic.Legion;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Generates text embeddings via MindAttic.Legion. Wire transport (endpoint,
/// auth, response parsing, retries with backoff, circuit breaker) is owned by
/// Legion. This class keeps the Tutor-specific concerns: text validation,
/// length capping, batching under the Vault token budget (splitting a batch OpenAI still
/// refuses as too large), and inter-batch pacing.
/// </summary>
public sealed class EmbeddingService : IEmbeddingProvider
{
    private readonly LegionClient legion;
    private readonly TutorKeys keys;

    private const string EmbeddingModel = "text-embedding-3-small";
    private const int EmbeddingDimensions = 1536;

    // text-embedding-3-small's per-input limit (a property of the model, not a tunable): each text is
    // capped at this many estimated tokens, sized with the Vault chars-per-token estimate.
    private const int ModelMaxTokensPerInput = 8191;

    private readonly LlmUsageLimiter? limiter;
    private readonly TutorSettingsSource? settings;

    /// <summary>The request-sizing numbers, from the Vault settings (TUT-T61).</summary>
    public readonly record struct BatchLimits(int MaxTokensPerRequest, double CharsPerToken, int MaxInputsPerRequest)
    {
        /// <summary>Limits from <paramref name="s"/>, clamped to sane values.</summary>
        public static BatchLimits From(TutorSettings s) => new(
            Math.Max(1, s.EmbeddingMaxTokensPerRequest),
            s.EmbeddingCharsPerToken > 0 ? s.EmbeddingCharsPerToken : 1,
            Math.Max(1, s.EmbeddingMaxInputsPerRequest));

        /// <summary>Estimated tokens in <paramref name="text"/> (rounded up).</summary>
        public long EstimateTokens(string text) => (long)Math.Ceiling(text.Length / CharsPerToken);

        /// <summary>Longest text sent as one input.</summary>
        public int MaxTextLength => (int)Math.Clamp(Math.Floor(ModelMaxTokensPerInput * CharsPerToken), 1, int.MaxValue);
    }

    /// <param name="limiter">Per-user ceiling (TUT-T59); the web host registers one, Tutor.Cli runs without.</param>
    /// <param name="settings">Vault settings for request sizing (TUT-T61); the defaults are used when absent.</param>
    public EmbeddingService(LegionClient legion, TutorKeys keys, LlmUsageLimiter? limiter = null, TutorSettingsSource? settings = null)
    {
        this.legion = legion;
        this.keys = keys;
        this.limiter = limiter;
        this.settings = settings;
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

        // The one embedding choke point: one request counted per call against the signed-in caller's limits.
        limiter?.Acquire();

        var limits = BatchLimits.From(settings?.Current ?? new TutorSettings());
        var maxTextLength = limits.MaxTextLength;

        // Filter empties (OpenAI rejects with HTTP 400) and cap individual lengths.
        var validTexts = texts
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Length > maxTextLength ? t[..maxTextLength] : t)
            .ToList();

        if (validTexts.Count == 0)
        {
            Log.Warn("EmbeddingService: All input texts are empty or whitespace");
            return [];
        }

        var filteredCount  = texts.Count(t => string.IsNullOrWhiteSpace(t));
        var truncatedCount = texts.Count(t => !string.IsNullOrWhiteSpace(t) && t.Length > maxTextLength);
        if (filteredCount > 0)
            Log.Warn($"EmbeddingService: Filtered out {filteredCount} empty/whitespace texts");
        if (truncatedCount > 0)
            Log.Warn($"EmbeddingService: Truncated {truncatedCount} texts exceeding {maxTextLength} chars");

        var batches = CreateBatches(validTexts, limits);
        Log.Debug($"EmbeddingService: Processing {validTexts.Count} text(s) in {batches.Count} batch(es)");

        var allEmbeddings = new List<float[]>();
        if (batches.Count > 1)
            Log.Info($"EmbeddingService: Large document - processing {validTexts.Count} texts in {batches.Count} batches");

        for (int batchIndex = 0; batchIndex < batches.Count; batchIndex++)
        {
            ct.ThrowIfCancellationRequested();
            var batch = batches[batchIndex];

            if (batches.Count > 1)
                Log.Info($"EmbeddingService: Batch {batchIndex + 1}/{batches.Count} ({batch.Count} texts, {allEmbeddings.Count}/{validTexts.Count} done)");
            else
                Log.Debug($"EmbeddingService: Processing {batch.Count} texts");

            allEmbeddings.AddRange(await EmbedBatchAsync(batch, ct));

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

    /// <summary>
    /// Embeds one batch. When OpenAI still refuses it as too large (the token estimate ran low for
    /// this text), the batch is split in half and each half is sent on its own (TUT-T61).
    /// </summary>
    private async Task<List<float[]>> EmbedBatchAsync(List<string> batch, CancellationToken ct)
    {
        IReadOnlyList<float[]> batchEmbeddings;
        var batchClock = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Keys from Vault's shared named lists (LAMP), failing over when one is refused.
            batchEmbeddings = await keys.ExecuteAsync("openai", (apiKey, callCt) => legion.EmbedAsync(
                providerId: "openai",
                apiKey: apiKey,
                model: EmbeddingModel,
                inputs: batch,
                dimensions: EmbeddingDimensions,
                ct: callCt), ct);
        }
        catch (CircuitBreakerOpenException ex)
        {
            Log.Warn($"[Tutor] EmbeddingService: Legion circuit breaker open for openai - {ex.Message}");
            throw;
        }
        catch (Exception ex) when (batch.Count > 1 && IsRequestTooLarge(ex))
        {
            var half = batch.Count / 2;
            Log.Warn($"[Tutor] EmbeddingService: a request of {batch.Count} texts was refused as too large; splitting it into {half} + {batch.Count - half} - {ex.Message}");
            var first = await EmbedBatchAsync(batch.GetRange(0, half), ct);
            var second = await EmbedBatchAsync(batch.GetRange(half, batch.Count - half), ct);
            first.AddRange(second);
            return first;
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"[Tutor] EmbeddingService: Legion embed call failed (status={ex.StatusCode}) - {ex.Message}", ex);
            throw;
        }
        LlmUsage.RecordEmbedding(batch.Count, batch.Sum(t => (long)t.Length), batchClock.ElapsedMilliseconds);
        return [.. batchEmbeddings];
    }

    /// <summary>
    /// True for OpenAI refusing a request as over its size limit (HTTP 413, or a 400 such as "maximum
    /// request size is 300000 tokens per request"), anywhere in the exception chain.
    /// </summary>
    public static bool IsRequestTooLarge(Exception ex)
    {
        for (var e = ex; e is not null; e = e.InnerException)
        {
            if (e is not HttpRequestException hre) continue;
            if (hre.StatusCode == System.Net.HttpStatusCode.RequestEntityTooLarge) return true;
            if (hre.StatusCode == System.Net.HttpStatusCode.BadRequest
                && (hre.Message.Contains("maximum request size", StringComparison.OrdinalIgnoreCase)
                    || hre.Message.Contains("tokens per request", StringComparison.OrdinalIgnoreCase)))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Splits <paramref name="texts"/> into request batches, each within the estimated token budget and
    /// the input-count cap of <paramref name="limits"/>. A text is never split across batches.
    /// </summary>
    public static List<List<string>> CreateBatches(IReadOnlyList<string> texts, BatchLimits limits)
    {
        var batches = new List<List<string>>();
        var current = new List<string>();
        long tokens = 0;
        foreach (var text in texts)
        {
            var estimated = limits.EstimateTokens(text);
            if (current.Count > 0 && (tokens + estimated > limits.MaxTokensPerRequest || current.Count >= limits.MaxInputsPerRequest))
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
    public static float CosineSimilarity(float[] a, float[] b) => EmbeddingMath.CosineSimilarity(a, b);
}
