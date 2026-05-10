using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

/// <summary>
/// Above-the-wire tests for ClaudeService — wire transport is owned by Legion
/// and not exercised here. The shared Legion credential store is sandboxed so
/// IsConfiguredAsync can't accidentally pick up a real machine-local key.
/// </summary>
public class ClaudeServiceTests
{
    private string sandbox = string.Empty;
    private string? prevCredsEnv;
    private ServiceProvider sp = null!;
    private LegionClient legion = null!;

    [SetUp]
    public void SetUp()
    {
        sandbox = Path.Combine(Path.GetTempPath(), "tutor-claude-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);
        prevCredsEnv = Environment.GetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS");
        Environment.SetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS", sandbox);

        var services = new ServiceCollection();
        services.AddLegionClient();
        sp = services.BuildServiceProvider();
        legion = sp.GetRequiredService<LegionClient>();
    }

    [TearDown]
    public void TearDown()
    {
        sp.Dispose();
        Environment.SetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS", prevCredsEnv);
        try { Directory.Delete(sandbox, recursive: true); } catch { }
    }

    [Test]
    public void ProviderName_IsClaude()
    {
        var prefs = new FakeSecurePreferences();
        var svc = new ClaudeService(legion, prefs);
        Assert.That(svc.ProviderName, Is.EqualTo("Claude"));
    }

    [Test]
    public async Task IsConfiguredAsync_NoKey_False()
    {
        var prefs = new FakeSecurePreferences();
        var svc = new ClaudeService(legion, prefs);
        Assert.That(await svc.IsConfiguredAsync(), Is.False);
    }

    [Test]
    public async Task IsConfiguredAsync_PrefsKey_True()
    {
        var prefs = new FakeSecurePreferences();
        await prefs.SetAsync("CLAUDE_API_KEY", "sk-ant-test");
        var svc = new ClaudeService(legion, prefs);

        Assert.That(await svc.IsConfiguredAsync(), Is.True);
    }

}
