using System.Text.Json;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services;
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
    private const string ModelKeyName = "DEEPSEEK_MODEL";
    private const string DefaultModel = "deepseek-v4-flash";

    private readonly LegionClient legion;
    private readonly ISecurePreferences prefs;
    private readonly TutorKeys keys;

    public string ProviderName => "DeepSeek";

    public DeepSeekService(LegionClient legion, ISecurePreferences prefs, TutorKeys keys)
    {
        this.legion = legion;
        this.prefs  = prefs;
        this.keys   = keys;
        Log.Debug("DeepSeekService initialized (delegating wire transport to MindAttic.Legion)");
    }

    /// <summary>True when Vault has a usable DeepSeek key for Tutor (shared list or Tutor's assignment).</summary>
    public Task<bool> IsConfiguredAsync() => Task.FromResult(keys.HasKey("deepseek"));

    public async Task<ChatReply> GetReplyAsync(
        IEnumerable<ChatMessage> messages,
        string? instructions = null,
        CancellationToken ct = default)
    {
        Log.Info("DeepSeek: Getting chat reply via Legion...");

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        var turns = messages.Select(m => new ChatTurn(m.Role, m.Text)).ToList();
        Log.Debug($"DeepSeek: Sending {turns.Count} message(s), model={model}");

        try
        {
            // Keys come from Vault's shared named lists (LAMP): Tutor's assigned key first, then the
            // next one whenever a key is refused (401/403/quota/429); the refused key is flagged in Vault.
            var text = await keys.ExecuteAsync("deepseek", (apiKey, callCt) => legion.CallChatAsync(
                providerId: "deepseek",
                apiKey: apiKey,
                model: model!,
                messages: turns,
                systemPrompt: instructions,
                maxTokens: 4096,
                temperature: 0.7,
                ct: callCt), ct);

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
