using System.Text.Json;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// DeepSeek chat adapter for Tutor. Wire transport (endpoint, auth, response parsing,
/// retries, circuit breaker) is owned by MindAttic.Legion's LegionClient. This class
/// just resolves the API key + model from Tutor's secure prefs (with a fallback to
/// Legion's shared store at %APPDATA%/MindAttic/LLM).
/// </summary>
public sealed class DeepSeekService : ILlmService
{
    private const string ApiKeyName = "DEEPSEEK_API_KEY";
    private const string ModelKeyName = "DEEPSEEK_MODEL";
    private const string DefaultModel = "deepseek-chat";

    private readonly LegionClient legion;
    private readonly ISecurePreferences prefs;

    public string ProviderName => "DeepSeek";

    public DeepSeekService(LegionClient legion, ISecurePreferences prefs)
    {
        this.legion = legion;
        this.prefs  = prefs;
        Log.Debug("DeepSeekService initialized (delegating wire transport to MindAttic.Legion)");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var key = await prefs.GetAsync(ApiKeyName);
        if (!string.IsNullOrWhiteSpace(key)) return true;
        return !string.IsNullOrWhiteSpace(MindAtticCredentialStore.GetKey("deepseek"));
    }

    public async Task<ChatReply> GetReplyAsync(
        IEnumerable<ChatMessage> messages,
        string? instructions = null,
        CancellationToken ct = default)
    {
        Log.Info("DeepSeek: Getting chat reply via Legion...");

        var apiKey = await prefs.GetAsync(ApiKeyName);
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = MindAtticCredentialStore.GetKey("deepseek");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("DeepSeek: API key is missing");
            throw new InvalidOperationException("DeepSeek API key is missing. Set it in Settings > Credentials.");
        }

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        var turns = messages.Select(m => new ChatTurn(m.Role, m.Text)).ToList();
        Log.Debug($"DeepSeek: Sending {turns.Count} message(s), model={model}");

        try
        {
            var text = await legion.CallChatAsync(
                providerId: "deepseek",
                apiKey: apiKey!,
                model: model!,
                messages: turns,
                systemPrompt: instructions,
                maxTokens: 4096,
                temperature: 0.7,
                ct: ct);

            var diagnostic = JsonSerializer.Serialize(new
            {
                provider = "deepseek",
                model,
                text,
            }, new JsonSerializerOptions { WriteIndented = true });

            Log.Info($"DeepSeek: Reply received ({text.Length} chars)");
            return new ChatReply(text, diagnostic);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"DeepSeek: HTTP error - {ex.Message}", ex);
            throw new InvalidOperationException($"DeepSeek HTTP {(int?)ex.StatusCode}: {ex.Message}", ex);
        }
        catch (CircuitBreakerOpenException ex)
        {
            Log.Warn($"DeepSeek: circuit breaker open - {ex.Message}");
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}
