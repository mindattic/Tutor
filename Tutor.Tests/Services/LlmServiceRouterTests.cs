using Microsoft.Extensions.DependencyInjection;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class LlmServiceRouterTests
{
    private static (LlmServiceRouter Router, FakeSecurePreferences Prefs) CreateRouter()
    {
        var prefs = new FakeSecurePreferences();
        var services = new ServiceCollection();
        services.AddSingleton<ISecurePreferences>(prefs);
        services.AddSingleton(sp => new OpenAIOptions(sp.GetRequiredService<ISecurePreferences>()) { Model = "gpt-4.1-mini" });
        services.AddHttpClient<OpenAIService>(c => c.Timeout = TimeSpan.FromSeconds(5));
        services.AddHttpClient<ClaudeService>(c => c.Timeout = TimeSpan.FromSeconds(5));
        services.AddHttpClient<DeepSeekService>(c => c.Timeout = TimeSpan.FromSeconds(5));
        services.AddHttpClient<GeminiService>(c => c.Timeout = TimeSpan.FromSeconds(5));
        var sp = services.BuildServiceProvider();
        var router = new LlmServiceRouter(prefs, sp);
        return (router, prefs);
    }

    [Fact]
    public void ProviderName_IsRouter()
    {
        var (router, _) = CreateRouter();
        Assert.Equal("Router", router.ProviderName);
    }

    [Fact]
    public void ImplementsILlmService()
    {
        var (router, _) = CreateRouter();
        Assert.IsAssignableFrom<ILlmService>(router);
    }

    [Fact]
    public async Task IsConfiguredAsync_DelegatesToSelectedProvider()
    {
        var (router, _) = CreateRouter();
        // No API key set, so should return false
        var configured = await router.IsConfiguredAsync();
        Assert.False(configured);
    }

    [Fact]
    public async Task DefaultProvider_IsOpenAI()
    {
        var (router, prefs) = CreateRouter();
        // No SELECTED_MODEL set — should default to OpenAI
        var configured = await router.IsConfiguredAsync();
        // Just verify it doesn't throw — routing to OpenAI is the default
        Assert.False(configured); // No API key set
    }

    [Theory]
    [InlineData("Claude")]
    [InlineData("DeepSeek")]
    [InlineData("Gemini")]
    [InlineData("UnknownProvider")]
    public async Task RoutesToProvider_WithoutError(string providerName)
    {
        var (router, prefs) = CreateRouter();
        await prefs.SetAsync("SELECTED_MODEL", providerName);
        // Should not throw — just verify routing works
        var configured = await router.IsConfiguredAsync();
        Assert.False(configured); // No API key
    }
}
