using System.Text.Json;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Kimi (Moonshot AI) chat adapter for Tutor. Wire transport (endpoint, auth,
/// response parsing, retries, circuit breaker) is owned by MindAttic.Legion's
/// LegionClient. This class resolves the API key + model from Tutor's secure
/// prefs (with a fallback to Legion's shared store at %APPDATA%/MindAttic/LLM)
/// and adapts the call. Kimi is OpenAI-compatible, so the wire shape is identical
/// to other bearer-token providers.
/// </summary>
public sealed class KimiService : ILlmService
{
    private const string ApiKeyName = "KIMI_API_KEY";
    private const string ModelKeyName = "KIMI_MODEL";
    private const string DefaultModel = "kimi-k2";

    private readonly LegionClient legion;
    private readonly ISecurePreferences prefs;

    public string ProviderName => "Kimi";

    public KimiService(LegionClient legion, ISecurePreferences prefs)
    {
        this.legion = legion;
        this.prefs  = prefs;
        Log.Debug("KimiService initialized (delegating wire transport to MindAttic.Legion)");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var key = await prefs.GetAsync(ApiKeyName);
        return !string.IsNullOrWhiteSpace(key);
    }

    public async Task<ChatReply> GetReplyAsync(
        IEnumerable<ChatMessage> messages,
        string? instructions = null,
        CancellationToken ct = default)
    {
        Log.Info("Kimi: Getting chat reply via Legion...");

        var apiKey = await prefs.GetAsync(ApiKeyName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("Kimi: API key is missing");
            throw new InvalidOperationException("Kimi API key is missing. Configure it in MindAttic Vault (%APPDATA%\\MindAttic\\LLM\\providers.json).");
        }

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        var turns = messages.Select(m => new ChatTurn(m.Role, m.Text)).ToList();
        Log.Debug($"Kimi: Sending {turns.Count} message(s), model={model}");

        try
        {
            var text = await legion.CallChatAsync(
                providerId: "kimi",
                apiKey: apiKey!,
                model: model!,
                messages: turns,
                systemPrompt: instructions,
                maxTokens: 4096,
                temperature: 0.7,
                ct: ct);

            var diagnostic = JsonSerializer.Serialize(new
            {
                provider = "kimi",
                model,
                text,
            }, new JsonSerializerOptions { WriteIndented = true });

            Log.Info($"Kimi: Reply received ({text.Length} chars)");
            return new ChatReply(text, diagnostic);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"Kimi: HTTP error - {ex.Message}", ex);
            throw new InvalidOperationException($"Kimi HTTP {(int?)ex.StatusCode}: {ex.Message}", ex);
        }
        catch (CircuitBreakerOpenException ex)
        {
            Log.Warn($"Kimi: circuit breaker open - {ex.Message}");
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}
