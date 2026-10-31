using Microsoft.Extensions.DependencyInjection;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Routes LLM requests to the appropriate service based on <see cref="TutorSettings.ActiveModel"/>.
/// Resolves services from the DI container on each call to avoid captive dependency issues.
/// </summary>
public sealed class LlmServiceRouter : ILlmService
{
    private readonly TutorSettingsSource settings;
    private readonly IServiceProvider serviceProvider;

    public string ProviderName => "Router";

    public LlmServiceRouter(TutorSettingsSource settings, IServiceProvider serviceProvider)
    {
        this.settings = settings;
        this.serviceProvider = serviceProvider;
        Log.Debug("LlmServiceRouter initialized");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var service = GetCurrentService();
        return await service.IsConfiguredAsync();
    }

    public async Task<ChatReply> GetReplyAsync(IEnumerable<ChatMessage> messages, string? instructions = null, CancellationToken ct = default)
    {
        var service = GetCurrentService();
        Log.Info($"LlmRouter: Routing to {service.ProviderName}");
        return await service.GetReplyAsync(messages, instructions, ct);
    }

    /// <summary>The service the active model setting selects (OpenAI for ChatGPT or anything unknown).</summary>
    public ILlmService GetCurrentService()
    {
        try
        {
            return ResolveService(settings.Current.ActiveModel);
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
