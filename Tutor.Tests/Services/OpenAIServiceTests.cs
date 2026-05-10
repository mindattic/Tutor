using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

/// <summary>
/// OpenAIService delegates wire transport to MindAttic.Legion's LegionClient,
/// so these tests stay above the wire — provider identity, configuration checks
/// against the secure-prefs store, conversation history, and argument
/// validation. The shared Legion credential store is sandboxed away from
/// %APPDATA% so a real key on the developer machine can't make
/// IsConfiguredAsync return a misleading true.
/// </summary>
public class OpenAIServiceTests
{
    private string sandbox = string.Empty;
    private string? prevCredsEnv;
    private ServiceProvider sp = null!;
    private LegionClient legion = null!;

    [SetUp]
    public void SetUp()
    {
        sandbox = Path.Combine(Path.GetTempPath(), "tutor-openai-test-" + Guid.NewGuid().ToString("N"));
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

    private (OpenAIService Service, FakeSecurePreferences Prefs, OpenAIOptions Options) Build()
    {
        var prefs = new FakeSecurePreferences();
        var opt = new OpenAIOptions(prefs);
        var svc = new OpenAIService(legion, opt);
        return (svc, prefs, opt);
    }

    [Test]
    public void ProviderName_IsChatGPT()
    {
        var (svc, _, _) = Build();
        Assert.That(svc.ProviderName, Is.EqualTo("ChatGPT"));
    }

    [Test]
    public async Task IsConfiguredAsync_NoKeyAnywhere_False()
    {
        var (svc, _, _) = Build();
        Assert.That(await svc.IsConfiguredAsync(), Is.False);
    }

    [Test]
    public async Task IsConfiguredAsync_PrefsKeySet_True()
    {
        var (svc, prefs, _) = Build();
        await prefs.SetAsync("OPENAI_API_KEY", "sk-test");

        Assert.That(await svc.IsConfiguredAsync(), Is.True);
    }

    [Test]
    public void AskAsync_Empty_ThrowsArgumentException()
    {
        var (svc, _, _) = Build();
        Assert.ThrowsAsync<ArgumentException>(() => svc.AskAsync(""));
        Assert.ThrowsAsync<ArgumentException>(() => svc.AskAsync("   "));
    }

    [Test]
    public void AskWithInstructionsAsync_Empty_ThrowsArgumentException()
    {
        var (svc, _, _) = Build();
        Assert.ThrowsAsync<ArgumentException>(
            () => svc.AskWithInstructionsAsync("", "system"));
    }

    [Test]
    public void History_StartsEmpty()
    {
        var (svc, _, _) = Build();
        Assert.That(svc.GetHistory(), Is.Empty);
    }

    [Test]
    public void ClearHistory_DoesNotThrowOnEmpty()
    {
        var (svc, _, _) = Build();
        svc.ClearHistory();
        Assert.That(svc.GetHistory(), Is.Empty);
    }

    [Test]
    public async Task Options_GetApiKeyAsync_ReadsFromPrefs()
    {
        var (_, prefs, opt) = Build();
        await prefs.SetAsync("OPENAI_API_KEY", "abc");

        Assert.That(await opt.GetApiKeyAsync(), Is.EqualTo("abc"));
    }

    [Test]
    public async Task Options_SetApiKeyAsync_WritesToPrefs()
    {
        var (_, prefs, opt) = Build();
        await opt.SetApiKeyAsync("xyz");

        Assert.That(await prefs.GetAsync("OPENAI_API_KEY"), Is.EqualTo("xyz"));
    }

    [Test]
    public async Task Options_ClearApiKey_RemovesFromPrefs()
    {
        var (_, prefs, opt) = Build();
        await opt.SetApiKeyAsync("xyz");

        opt.ClearApiKey();

        Assert.That(prefs.ContainsKey("OPENAI_API_KEY"), Is.False);
    }

    [Test]
    public async Task Options_GetModelAsync_StoredOverridesDefault()
    {
        var (_, prefs, opt) = Build();
        opt.Model = "default-model";
        await prefs.SetAsync("CHATGPT_MODEL", "stored-model");

        Assert.That(await opt.GetModelAsync(), Is.EqualTo("stored-model"));
    }

    [Test]
    public async Task Options_GetModelAsync_NoStored_FallsBackToDefault()
    {
        var (_, _, opt) = Build();
        opt.Model = "fallback-model";

        Assert.That(await opt.GetModelAsync(), Is.EqualTo("fallback-model"));
    }

    [Test]
    public void Options_Defaults_AreReasonable()
    {
        var opt = new OpenAIOptions(new FakeSecurePreferences());

        Assert.That(string.IsNullOrWhiteSpace(opt.Model), Is.False);
        Assert.That(opt.Temperature, Is.InRange(0.0, 2.0));
        Assert.That(opt.MaxTokens > 0, Is.True);
    }
}
