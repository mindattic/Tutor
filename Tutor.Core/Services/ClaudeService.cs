using System.Text.Json;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Claude chat adapter. The wire-level scaffolding (endpoint, headers, request
/// shape, response parsing, retries, circuit breaker) lives in MindAttic.Legion
/// — this class is just a thin Tutor-flavoured wrapper that maps Tutor's
/// ChatMessage/ChatReply types onto Legion's ChatTurn API and pulls credentials
/// from Tutor's secure preferences (with a fallback to the shared Legion store
/// at %APPDATA%/MindAttic/LLM).
/// </summary>
public sealed class ClaudeService : ILlmService
{
    private const string ApiKeyName = "CLAUDE_API_KEY";
    private const string ModelKeyName = "CLAUDE_MODEL";
    private const string DefaultModel = "claude-sonnet-5";

    private readonly LegionClient legion;
    private readonly ISecurePreferences prefs;

    public string ProviderName => "Claude";

    public ClaudeService(LegionClient legion, ISecurePreferences prefs)
    {
        this.legion = legion;
        this.prefs  = prefs;
        Log.Debug("ClaudeService initialized (delegating wire transport to MindAttic.Legion)");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var keys = await prefs.GetApiKeysAsync(ApiKeyName);
        return keys.Count > 0;
    }

    public async Task<ChatReply> GetReplyAsync(
        IEnumerable<ChatMessage> messages,
        string? instructions = null,
        CancellationToken ct = default)
    {
        Log.Info("Claude: Getting chat reply via Legion...");

        var apiKeys = await prefs.GetApiKeysAsync(ApiKeyName);
        if (apiKeys.Count == 0)
        {
            Log.Error("Claude: API key is missing");
            throw new InvalidOperationException("Claude API key is missing. Configure it in MindAttic Vault (%APPDATA%\\MindAttic\\LLM\\providers.json).");
        }

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        var turns = messages.Select(m => new ChatTurn(m.Role, m.Text)).ToList();
        Log.Debug($"Claude: Sending {turns.Count} message(s), model={model}");

        try
        {
            // More than one key configured (a rotation/failover pool): the first key is used
            // until it fails with an auth/rate-limit/server/network error, then the next is
            // tried (KeyPoolFailover). A single key behaves exactly as before.
            var text = await KeyPoolFailover.ExecuteAsync(apiKeys, ct, apiKey => legion.CallChatAsync(
                providerId: "claude",
                apiKey: apiKey,
                model: model!,
                messages: turns,
                systemPrompt: instructions,
                maxTokens: 4096,
                temperature: 0.7,
                ct: ct));

            // Synthesize a minimal JSON payload for Tutor's diagnostic "View" button.
            var diagnostic = JsonSerializer.Serialize(new
            {
                provider = "claude",
                model,
                text,
            }, new JsonSerializerOptions { WriteIndented = true });

            Log.Info($"Claude: Reply received ({text.Length} chars)");
            return new ChatReply(text, diagnostic);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"Claude: HTTP error - {ex.Message}", ex);
            throw new InvalidOperationException($"Claude HTTP {(int?)ex.StatusCode}: {ex.Message}", ex);
        }
        catch (CircuitBreakerOpenException ex)
        {
            Log.Warn($"Claude: circuit breaker open - {ex.Message}");
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}
