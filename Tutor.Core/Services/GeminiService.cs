using System.Text.Json;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Gemini chat adapter for Tutor. Wire transport (endpoint, auth, response parsing,
/// retries, circuit breaker) is owned by MindAttic.Legion's LegionClient. This class
/// just resolves the API key + model from Tutor's secure prefs (with a fallback to
/// Legion's shared store at %APPDATA%/MindAttic/LLM) and adapts the call.
/// </summary>
public sealed class GeminiService : ILlmService
{
    private const string ApiKeyName = "GEMINI_API_KEY";
    private const string ModelKeyName = "GEMINI_MODEL";
    private const string DefaultModel = "gemini-2.5-flash";

    private readonly LegionClient legion;
    private readonly ISecurePreferences prefs;

    public string ProviderName => "Gemini";

    public GeminiService(LegionClient legion, ISecurePreferences prefs)
    {
        this.legion = legion;
        this.prefs  = prefs;
        Log.Debug("GeminiService initialized (delegating wire transport to MindAttic.Legion)");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var key = await prefs.GetAsync(ApiKeyName);
        if (!string.IsNullOrWhiteSpace(key)) return true;
        return !string.IsNullOrWhiteSpace(MindAtticCredentialStore.GetKey("gemini"));
    }

    public async Task<ChatReply> GetReplyAsync(
        IEnumerable<ChatMessage> messages,
        string? instructions = null,
        CancellationToken ct = default)
    {
        Log.Info("Gemini: Getting chat reply via Legion...");

        var apiKey = await prefs.GetAsync(ApiKeyName);
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = MindAtticCredentialStore.GetKey("gemini");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("Gemini: API key is missing");
            throw new InvalidOperationException("Gemini API key is missing. Set it in Settings > Credentials.");
        }

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        var turns = messages.Select(m => new ChatTurn(m.Role, m.Text)).ToList();
        Log.Debug($"Gemini: Sending {turns.Count} message(s), model={model}");

        try
        {
            var text = await legion.CallChatAsync(
                providerId: "gemini",
                apiKey: apiKey!,
                model: model!,
                messages: turns,
                systemPrompt: instructions,
                maxTokens: 4096,
                temperature: 0.7,
                ct: ct);

            var diagnostic = JsonSerializer.Serialize(new
            {
                provider = "gemini",
                model,
                text,
            }, new JsonSerializerOptions { WriteIndented = true });

            Log.Info($"Gemini: Reply received ({text.Length} chars)");
            return new ChatReply(text, diagnostic);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"Gemini: HTTP error - {ex.Message}", ex);
            throw new InvalidOperationException($"Gemini HTTP {(int?)ex.StatusCode}: {ex.Message}", ex);
        }
        catch (CircuitBreakerOpenException ex)
        {
            Log.Warn($"Gemini: circuit breaker open - {ex.Message}");
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}
