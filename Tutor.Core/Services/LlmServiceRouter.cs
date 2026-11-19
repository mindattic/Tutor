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

        // Usage counters for a pipeline run's report (TUT-T50): characters, since Legion returns no token counts.
        var sent = messages as IReadOnlyCollection<ChatMessage> ?? messages.ToList();
        long inputChars = (instructions?.Length ?? 0) + sent.Sum(m => (long)(m.Text?.Length ?? 0));
        var clock = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var reply = await service.GetReplyAsync(sent, instructions, ct);
            LlmUsage.RecordChat(inputChars, reply.Text?.Length ?? 0, clock.ElapsedMilliseconds, failed: false);
            return reply;
        }
        catch
        {
            LlmUsage.RecordChat(inputChars, 0, clock.ElapsedMilliseconds, failed: true);
            throw;
        }
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
