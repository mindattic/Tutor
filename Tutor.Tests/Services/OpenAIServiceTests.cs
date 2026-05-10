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
public class OpenAIServiceTests : IDisposable
{
    private readonly string sandbox;
    private readonly string? prevCredsEnv;
    private readonly ServiceProvider sp;
    private readonly LegionClient legion;

    public OpenAIServiceTests()
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

    public void Dispose()
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

    [Fact]
    public void ProviderName_IsChatGPT()
    {
        var (svc, _, _) = Build();
        Assert.Equal("ChatGPT", svc.ProviderName);
    }

    [Fact]
    public async Task IsConfiguredAsync_NoKeyAnywhere_False()
    {
        var (svc, _, _) = Build();
        Assert.False(await svc.IsConfiguredAsync());
    }

    [Fact]
    public async Task IsConfiguredAsync_PrefsKeySet_True()
    {
        var (svc, prefs, _) = Build();
        await prefs.SetAsync("OPENAI_API_KEY", "sk-test");

        Assert.True(await svc.IsConfiguredAsync());
    }

    [Fact]
    public async Task AskAsync_Empty_ThrowsArgumentException()
    {
        var (svc, _, _) = Build();
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AskAsync(""));
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AskAsync("   "));
    }

    [Fact]
    public async Task AskWithInstructionsAsync_Empty_ThrowsArgumentException()
    {
        var (svc, _, _) = Build();
        await Assert.ThrowsAsync<ArgumentException>(
            () => svc.AskWithInstructionsAsync("", "system"));
    }

    [Fact]
    public void History_StartsEmpty()
    {
        var (svc, _, _) = Build();
        Assert.Empty(svc.GetHistory());
    }

    [Fact]
    public void ClearHistory_DoesNotThrowOnEmpty()
    {
        var (svc, _, _) = Build();
        svc.ClearHistory();
        Assert.Empty(svc.GetHistory());
    }

    [Fact]
    public async Task Options_GetApiKeyAsync_ReadsFromPrefs()
    {
        var (_, prefs, opt) = Build();
        await prefs.SetAsync("OPENAI_API_KEY", "abc");

        Assert.Equal("abc", await opt.GetApiKeyAsync());
    }

    [Fact]
    public async Task Options_SetApiKeyAsync_WritesToPrefs()
    {
        var (_, prefs, opt) = Build();
        await opt.SetApiKeyAsync("xyz");

        Assert.Equal("xyz", await prefs.GetAsync("OPENAI_API_KEY"));
    }

    [Fact]
    public async Task Options_ClearApiKey_RemovesFromPrefs()
    {
        var (_, prefs, opt) = Build();
        await opt.SetApiKeyAsync("xyz");

        opt.ClearApiKey();

        Assert.False(prefs.ContainsKey("OPENAI_API_KEY"));
    }

    [Fact]
    public async Task Options_GetModelAsync_StoredOverridesDefault()
    {
        var (_, prefs, opt) = Build();
        opt.Model = "default-model";
        await prefs.SetAsync("CHATGPT_MODEL", "stored-model");

        Assert.Equal("stored-model", await opt.GetModelAsync());
    }

    [Fact]
    public async Task Options_GetModelAsync_NoStored_FallsBackToDefault()
    {
        var (_, _, opt) = Build();
        opt.Model = "fallback-model";

        Assert.Equal("fallback-model", await opt.GetModelAsync());
    }

    [Fact]
    public void Options_Defaults_AreReasonable()
    {
        var opt = new OpenAIOptions(new FakeSecurePreferences());

        Assert.False(string.IsNullOrWhiteSpace(opt.Model));
        Assert.InRange(opt.Temperature, 0.0, 2.0);
        Assert.True(opt.MaxTokens > 0);
    }
}
