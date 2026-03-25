using Microsoft.Extensions.DependencyInjection;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Routes LLM requests to the appropriate service based on the user's selected model.
/// Resolves services from the DI container on each call to avoid captive dependency issues.
/// </summary>
public sealed class LlmServiceRouter : ILlmService
{
    private readonly ISecurePreferences prefs;
    private readonly IServiceProvider serviceProvider;

    private const string SelectedModelKey = "SELECTED_MODEL";

    public string ProviderName => "Router";

    public LlmServiceRouter(ISecurePreferences prefs, IServiceProvider serviceProvider)
    {
        this.prefs = prefs;
        this.serviceProvider = serviceProvider;
        Log.Debug("LlmServiceRouter initialized");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var service = await GetCurrentServiceAsync();
        return await service.IsConfiguredAsync();
    }

    public async Task<ChatReply> GetReplyAsync(IEnumerable<ChatMessage> messages, string? instructions = null, CancellationToken ct = default)
    {
        var service = await GetCurrentServiceAsync();
        Log.Info($"LlmRouter: Routing to {service.ProviderName}");
        return await service.GetReplyAsync(messages, instructions, ct);
    }

    private async Task<ILlmService> GetCurrentServiceAsync()
    {
        try
        {
            var selected = await prefs.GetAsync(SelectedModelKey);
            return ResolveService(selected);
        }
        catch
        {
            return serviceProvider.GetRequiredService<OpenAIService>();
        }
    }

    private ILlmService ResolveService(string? selected)
    {
        return selected switch
        {
            "Claude" => serviceProvider.GetRequiredService<ClaudeService>(),
            "DeepSeek" => serviceProvider.GetRequiredService<DeepSeekService>(),
            "Gemini" => serviceProvider.GetRequiredService<GeminiService>(),
            _ => serviceProvider.GetRequiredService<OpenAIService>()
        };
    }
}
