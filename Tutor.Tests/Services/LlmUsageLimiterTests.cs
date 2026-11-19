using Tutor.Core.Services;

namespace Tutor.Tests.Services;

/// <summary>Per-user LLM ceiling (TUT-T59, plan item 0.2(b)): requests per minute and per UTC day, both Vault settings.</summary>
[TestFixture]
public sealed class LlmUsageLimiterTests
{
    private sealed class ManualClock(DateTimeOffset start) : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = start;
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private static readonly DateTimeOffset Noon = new(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);

    private static (LlmUsageLimiter Limiter, ManualClock Clock) Create(int perMinute, int perDay)
    {
        var settings = TestLamp.Settings(s =>
        {
            s.LlmRequestsPerMinutePerUser = perMinute;
            s.LlmRequestsPerDayPerUser = perDay;
        });
        var clock = new ManualClock(Noon);
        return (new LlmUsageLimiter(settings, clock), clock);
    }

    private static readonly LlmCallerInfo Student = new("student-1");

    [Test]
    public void PerMinute_LimitReached_ThrowsWithAClearMessage()
    {
        var (limiter, _) = Create(perMinute: 2, perDay: 0);
        limiter.Acquire(Student);
        limiter.Acquire(Student);

        var ex = Assert.Throws<LlmLimitReachedException>(() => limiter.Acquire(Student));
        Assert.That(ex!.Message, Does.Contain("limit of 2 tutor requests a minute"));
        Assert.That(ex.RetryAfter, Is.GreaterThan(TimeSpan.Zero));
    }

    [Test]
    public void PerMinute_WindowSlides()
    {
        var (limiter, clock) = Create(perMinute: 1, perDay: 0);
        limiter.Acquire(Student);
        clock.Now += TimeSpan.FromSeconds(61);

        Assert.DoesNotThrow(() => limiter.Acquire(Student));
    }

    [Test]
    public void PerDay_LimitReached_ThenResetsAtUtcMidnight()
    {
        var (limiter, clock) = Create(perMinute: 0, perDay: 2);
        limiter.Acquire(Student);
        clock.Now += TimeSpan.FromMinutes(5);
        limiter.Acquire(Student);
        clock.Now += TimeSpan.FromMinutes(5);

        var ex = Assert.Throws<LlmLimitReachedException>(() => limiter.Acquire(Student));
        Assert.That(ex!.Message, Does.Contain("today's limit of 2 tutor requests"));

        clock.Now = new DateTimeOffset(2026, 11, 20, 0, 0, 1, TimeSpan.Zero);
        Assert.DoesNotThrow(() => limiter.Acquire(Student));
    }

    [Test]
    public void Users_AreCountedSeparately()
    {
        var (limiter, _) = Create(perMinute: 1, perDay: 0);
        limiter.Acquire(Student);

        Assert.DoesNotThrow(() => limiter.Acquire(new LlmCallerInfo("student-2")));
        Assert.Throws<LlmLimitReachedException>(() => limiter.Acquire(Student));
    }

    [Test]
    public void NoCaller_ExemptCaller_AndZeroLimits_AreNotLimited()
    {
        var (limiter, _) = Create(perMinute: 1, perDay: 1);
        for (var i = 0; i < 5; i++)
        {
            limiter.Acquire(null);                                    // system work (background queue, CLI)
            limiter.Acquire(new LlmCallerInfo("author", Exempt: true));
        }

        var (off, _) = Create(perMinute: 0, perDay: 0);
        for (var i = 0; i < 50; i++) off.Acquire(Student);
        Assert.Pass();
    }

    [Test]
    public void AmbientCaller_IsUsedByAcquire()
    {
        var (limiter, _) = Create(perMinute: 1, perDay: 0);
        using (LlmCaller.Use(Student))
        {
            limiter.Acquire();
            Assert.Throws<LlmLimitReachedException>(() => limiter.Acquire());
        }
        Assert.That(LlmCaller.Current, Is.Null);
        Assert.DoesNotThrow(() => limiter.Acquire());
    }
}
