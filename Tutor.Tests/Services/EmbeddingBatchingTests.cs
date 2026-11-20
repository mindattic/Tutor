using System.Net;
using Tutor.Core.Services;

namespace Tutor.Tests.Services;

/// <summary>
/// Embedding request sizing (TUT-T61): batches stay under the Vault token budget and input cap, and an
/// OpenAI "request too large" refusal is recognized so the batch can be split and retried.
/// </summary>
[TestFixture]
public sealed class EmbeddingBatchingTests
{
    private static EmbeddingService.BatchLimits Limits(int tokens, double charsPerToken, int inputs) =>
        new(tokens, charsPerToken, inputs);

    [Test]
    public void Batches_StayUnderTheEstimatedTokenBudget()
    {
        // 100 texts of 1,000 chars at 2.5 chars/token = 400 tokens each; 2,000-token budget = 5 per batch.
        var texts = Enumerable.Range(0, 100).Select(_ => new string('x', 1000)).ToList();
        var limits = Limits(2000, 2.5, 2048);

        var batches = EmbeddingService.CreateBatches(texts, limits);

        Assert.That(batches, Has.Count.EqualTo(20));
        Assert.That(batches.All(b => b.Sum(limits.EstimateTokens) <= 2000), Is.True);
        Assert.That(batches.SelectMany(b => b), Is.EqualTo(texts));
    }

    [Test]
    public void Batches_RespectTheInputCap()
    {
        var texts = Enumerable.Range(0, 10).Select(i => $"t{i}").ToList();

        var batches = EmbeddingService.CreateBatches(texts, Limits(300000, 2.5, 3));

        Assert.That(batches.Select(b => b.Count), Is.EqualTo(new[] { 3, 3, 3, 1 }));
    }

    [Test]
    public void ALongTextStillGetsItsOwnBatch()
    {
        var texts = new List<string> { "short", new string('x', 10000), "short" };

        var batches = EmbeddingService.CreateBatches(texts, Limits(1000, 2.5, 2048));

        Assert.That(batches, Has.Count.EqualTo(3));
    }

    [Test]
    public void DefaultSettings_KeepTheDictionaryUnderOpenAIsCap()
    {
        // PG 37683 failed with 1,435 chunks over 2.27M chars in 3 batches under the old 4 chars/token estimate.
        var limits = EmbeddingService.BatchLimits.From(new TutorSettings());
        var texts = Enumerable.Range(0, 1435).Select(_ => new string('x', 2_270_000 / 1435)).ToList();

        var batches = EmbeddingService.CreateBatches(texts, limits);

        Assert.That(limits.MaxTokensPerRequest, Is.LessThan(300000));
        Assert.That(batches.All(b => b.Sum(t => (long)t.Length) / 2.0 < 300000), Is.True,
            "even at 2 chars/token every batch is under OpenAI's 300,000-token request cap");
    }

    [Test]
    public void TheTooLargeRefusalIsRecognizedThroughWrappers()
    {
        var refused = new HttpRequestException(
            "400 Bad Request: Invalid input: maximum request size is 300000 tokens per request", null, HttpStatusCode.BadRequest);

        Assert.That(EmbeddingService.IsRequestTooLarge(refused), Is.True);
        Assert.That(EmbeddingService.IsRequestTooLarge(new InvalidOperationException("wrapped", refused)), Is.True);
        Assert.That(EmbeddingService.IsRequestTooLarge(
            new HttpRequestException("400 Bad Request: input must not be empty", null, HttpStatusCode.BadRequest)), Is.False);
        Assert.That(EmbeddingService.IsRequestTooLarge(
            new HttpRequestException("401", null, HttpStatusCode.Unauthorized)), Is.False);
    }
}
