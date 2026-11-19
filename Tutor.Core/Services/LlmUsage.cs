namespace Tutor.Core.Services;

/// <summary>
/// Process-wide counters for what a pipeline run asked of the LLM providers (TUT-T50): chat calls
/// through <see cref="LlmServiceRouter"/> and embedding batches through <see cref="EmbeddingService"/>.
/// MindAttic.Legion returns only the reply text, so real token counts are not available; the counters
/// hold characters sent and received, from which a caller can estimate tokens and cost.
/// Take a <see cref="Snapshot"/> before and after a stage and subtract.
/// </summary>
public static class LlmUsage
{
    private static long chatCalls, chatFailures, chatInputChars, chatOutputChars, chatMilliseconds;
    private static long embedCalls, embedTexts, embedInputChars, embedMilliseconds;

    /// <summary>
    /// Records one finished chat call. A failed call counts as a call, a failure and its time, but not
    /// its characters: a refused request isn't billed, and counting it would inflate the estimate.
    /// </summary>
    public static void RecordChat(long inputChars, long outputChars, long milliseconds, bool failed)
    {
        Interlocked.Increment(ref chatCalls);
        if (failed) Interlocked.Increment(ref chatFailures);
        else
        {
            Interlocked.Add(ref chatInputChars, inputChars);
            Interlocked.Add(ref chatOutputChars, outputChars);
        }
        Interlocked.Add(ref chatMilliseconds, milliseconds);
    }

    /// <summary>Records one finished embedding batch.</summary>
    public static void RecordEmbedding(int texts, long inputChars, long milliseconds)
    {
        Interlocked.Increment(ref embedCalls);
        Interlocked.Add(ref embedTexts, texts);
        Interlocked.Add(ref embedInputChars, inputChars);
        Interlocked.Add(ref embedMilliseconds, milliseconds);
    }

    /// <summary>The totals since the process started.</summary>
    public static LlmUsageSnapshot Snapshot() => new(
        Interlocked.Read(ref chatCalls), Interlocked.Read(ref chatFailures),
        Interlocked.Read(ref chatInputChars), Interlocked.Read(ref chatOutputChars), Interlocked.Read(ref chatMilliseconds),
        Interlocked.Read(ref embedCalls), Interlocked.Read(ref embedTexts),
        Interlocked.Read(ref embedInputChars), Interlocked.Read(ref embedMilliseconds));
}

/// <summary>
/// Totals from <see cref="LlmUsage"/>. <paramref name="ChatMilliseconds"/> is the sum of every call's
/// own duration, so with calls running in parallel it is larger than the wall-clock time.
/// </summary>
public sealed record LlmUsageSnapshot(
    long ChatCalls, long ChatFailures, long ChatInputChars, long ChatOutputChars, long ChatMilliseconds,
    long EmbedCalls, long EmbedTexts, long EmbedInputChars, long EmbedMilliseconds)
{
    /// <summary>What happened between <paramref name="earlier"/> and this snapshot.</summary>
    public LlmUsageSnapshot Since(LlmUsageSnapshot earlier) => new(
        ChatCalls - earlier.ChatCalls, ChatFailures - earlier.ChatFailures,
        ChatInputChars - earlier.ChatInputChars, ChatOutputChars - earlier.ChatOutputChars,
        ChatMilliseconds - earlier.ChatMilliseconds,
        EmbedCalls - earlier.EmbedCalls, EmbedTexts - earlier.EmbedTexts,
        EmbedInputChars - earlier.EmbedInputChars, EmbedMilliseconds - earlier.EmbedMilliseconds);
}
