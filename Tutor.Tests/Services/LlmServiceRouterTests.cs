using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class LlmServiceRouterTests
{
    private string sandbox = string.Empty;
    private string? prevCredsEnv;

    [SetUp]
    public void SetUp()
    {
        // Sandbox the shared credential store away from %APPDATA% so this machine's
        // real keys don't leak into tests via the LegionClient fallback path.
        sandbox = Path.Combine(Path.GetTempPath(), "tutor-router-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);
        prevCredsEnv = Environment.GetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS");
        Environment.SetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS", sandbox);
    }

    [TearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS", prevCredsEnv);
        try { Directory.Delete(sandbox, recursive: true); } catch { }
    }

    private static (LlmServiceRouter Router, FakeSecurePreferences Prefs) CreateRouter()
    {
        var prefs = new FakeSecurePreferences();
        var services = new ServiceCollection();
        services.AddSingleton<ISecurePreferences>(prefs);
        services.AddSingleton(sp => new OpenAIOptions(sp.GetRequiredService<ISecurePreferences>()) { Model = "gpt-4.1-mini" });
        services.AddLegionClient();
        services.AddSingleton<OpenAIService>();
        services.AddSingleton<ClaudeService>();
        services.AddSingleton<DeepSeekService>();
        services.AddSingleton<GeminiService>();
        var sp = services.BuildServiceProvider();
        var router = new LlmServiceRouter(prefs, sp);
        return (router, prefs);
    }

    [Test]
    public void ProviderName_IsRouter()
    {
        var (router, _) = CreateRouter();
        Assert.That(router.ProviderName, Is.EqualTo("Router"));
    }

    [Test]
    public void ImplementsILlmService()
    {
        var (router, _) = CreateRouter();
        Assert.That(router, Is.AssignableTo<ILlmService>());
    }

    [Test]
    public async Task IsConfiguredAsync_DelegatesToSelectedProvider()
    {
        var (router, _) = CreateRouter();
        // No API key set, so should return false
        var configured = await router.IsConfiguredAsync();
        Assert.That(configured, Is.False);
    }

    [Test]
    public async Task DefaultProvider_IsOpenAI()
    {
        var (router, prefs) = CreateRouter();
        // No SELECTED_MODEL set — should default to OpenAI
        var configured = await router.IsConfiguredAsync();
        // Just verify it doesn't throw — routing to OpenAI is the default
        Assert.That(configured, Is.False); // No API key set
    }

    [TestCase("Claude")]
    [TestCase("DeepSeek")]
    [TestCase("Gemini")]
    [TestCase("UnknownProvider")]
    public async Task RoutesToProvider_WithoutError(string providerName)
    {
        var (router, prefs) = CreateRouter();
        await prefs.SetAsync("SELECTED_MODEL", providerName);
        // Should not throw — just verify routing works
        var configured = await router.IsConfiguredAsync();
        Assert.That(configured, Is.False); // No API key
    }

}
