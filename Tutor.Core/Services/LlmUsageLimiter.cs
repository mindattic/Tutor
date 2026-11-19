using System.Collections.Concurrent;

namespace Tutor.Core.Services;

/// <summary>Who an LLM request is made for. <see cref="Exempt"/> skips the per-user limits (authors, when the setting allows).</summary>
public sealed record LlmCallerInfo(string UserId, bool Exempt = false);

/// <summary>
/// The signed-in user behind the LLM call now running, carried in an <see cref="AsyncLocal{T}"/> so the
/// singleton LLM services can see it without a user-id parameter on every call (TUT-T59). The web host sets
/// it around every inbound activity on a Blazor circuit; work with no caller (startup, the background
/// queue, Tutor.Cli) is system work and is not limited per user.
/// </summary>
public static class LlmCaller
{
    private static readonly AsyncLocal<LlmCallerInfo?> current = new();

    public static LlmCallerInfo? Current => current.Value;

    /// <summary>
    /// Sets the caller for the current async flow. Called inside an async method, the change is undone when
    /// that method returns; the returned scope restores the previous caller for synchronous use.
    /// </summary>
    public static IDisposable Use(LlmCallerInfo? caller)
    {
        var previous = current.Value;
        current.Value = caller;
        return new Restore(previous);
    }

    private sealed class Restore(LlmCallerInfo? previous) : IDisposable
    {
        public void Dispose() => current.Value = previous;
    }
}

/// <summary>Thrown when a user reaches a per-user LLM limit; <see cref="Exception.Message"/> is written for the user.</summary>
public sealed class LlmLimitReachedException(string message, TimeSpan retryAfter) : InvalidOperationException(message)
{
    /// <summary>How long until the limit frees up.</summary>
    public TimeSpan RetryAfter { get; } = retryAfter;
}

/// <summary>
/// Per-user ceiling on LLM spending (TUT-T59, plan item 0.2(b)), applied at the single choke points
/// (<see cref="LlmServiceRouter"/> for chat, <see cref="EmbeddingService"/> for embeddings): requests per
/// minute and requests per UTC day for each signed-in user. Both numbers are Vault settings
/// (<see cref="TutorSettings.LlmRequestsPerMinutePerUser"/>, <see cref="TutorSettings.LlmRequestsPerDayPerUser"/>),
/// read on every call so a change applies at once; 0 turns that limit off.
/// </summary>
public sealed class LlmUsageLimiter
{
    private static readonly TimeSpan Minute = TimeSpan.FromMinutes(1);

    private readonly TutorSettingsSource settings;
    private readonly TimeProvider clock;
    private readonly ConcurrentDictionary<string, UserWindow> users = new(StringComparer.OrdinalIgnoreCase);

    public LlmUsageLimiter(TutorSettingsSource settings, TimeProvider? clock = null)
    {
        this.settings = settings;
        this.clock = clock ?? TimeProvider.System;
    }

    /// <summary>Counts one request for the current <see cref="LlmCaller"/>, or throws <see cref="LlmLimitReachedException"/>.</summary>
    public void Acquire() => Acquire(LlmCaller.Current);

    /// <summary>Counts one request for <paramref name="caller"/>, or throws <see cref="LlmLimitReachedException"/>.</summary>
    public void Acquire(LlmCallerInfo? caller)
    {
        if (caller is null || caller.Exempt || string.IsNullOrWhiteSpace(caller.UserId)) return;

        var s = settings.Current;
        var perMinute = s.LlmRequestsPerMinutePerUser;
        var perDay = s.LlmRequestsPerDayPerUser;
        if (perMinute <= 0 && perDay <= 0) return;

        var now = clock.GetUtcNow();
        var window = users.GetOrAdd(caller.UserId, _ => new UserWindow());
        lock (window)
        {
            var today = DateOnly.FromDateTime(now.UtcDateTime);
            if (window.Day != today)
            {
                window.Day = today;
                window.DayCount = 0;
            }
            while (window.Recent.Count > 0 && now - window.Recent.Peek() >= Minute)
                window.Recent.Dequeue();

            if (perDay > 0 && window.DayCount >= perDay)
            {
                var midnight = new DateTimeOffset(today.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
                throw new LlmLimitReachedException(
                    $"You have reached today's limit of {perDay} tutor requests. It resets at midnight UTC " +
                    $"(in about {Math.Max(1, (int)Math.Ceiling((midnight - now).TotalHours))} hour(s)).",
                    midnight - now);
            }
            if (perMinute > 0 && window.Recent.Count >= perMinute)
            {
                var wait = Minute - (now - window.Recent.Peek());
                throw new LlmLimitReachedException(
                    $"You have reached the limit of {perMinute} tutor requests a minute. " +
                    $"Please wait {Math.Max(1, (int)Math.Ceiling(wait.TotalSeconds))} seconds and try again.",
                    wait);
            }

            window.Recent.Enqueue(now);
            window.DayCount++;
        }
    }

    private sealed class UserWindow
    {
        public readonly Queue<DateTimeOffset> Recent = new();
        public DateOnly Day;
        public int DayCount;
    }
}
