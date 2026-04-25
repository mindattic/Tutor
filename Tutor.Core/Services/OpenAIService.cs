// ============================================================================
// OpenAIService — AI-Powered Chat Service for Tutor Application
// ============================================================================
//
// PURPOSE:
// Provides AI-powered chat responses backed by OpenAI's chat-completions API.
// As of the MindAttic.Legion migration, the wire transport (endpoint, auth,
// payload shape, response parsing, retries, circuit breaker) is owned by
// MindAttic.Legion's LegionClient. This file keeps the Tutor-specific bits:
// the conversation history, the Ask* convenience methods, and the
// OpenAIOptions configuration class.
// ============================================================================

using System.Text.Json;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Response from the LLM containing the text reply and a diagnostic JSON blob.
/// </summary>
public record ChatReply(string Text, string FullJson);

/// <summary>
/// A simple chat message DTO with role and text content.
/// </summary>
public record ChatMessageDto(string Role, string Text);

/// <summary>
/// Service for interacting with OpenAI's API. Maintains conversation history
/// across turns; delegates the wire-level call to MindAttic.Legion.
/// </summary>
public sealed class OpenAIService : ILlmService
{
    private readonly LegionClient legion;
    private readonly OpenAIOptions opt;
    private readonly List<ChatMessageDto> conversationHistory = [];

    public string ProviderName => "ChatGPT";

    public OpenAIService(LegionClient legion, OpenAIOptions opt)
    {
        this.legion = legion;
        this.opt = opt;
        Log.Debug("OpenAIService initialized (delegating wire transport to MindAttic.Legion)");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var key = await opt.GetApiKeyAsync();
        if (!string.IsNullOrWhiteSpace(key)) return true;
        return !string.IsNullOrWhiteSpace(MindAtticCredentialStore.GetKey("openai"));
    }

    public async Task<ChatReply> AskAsync(string question, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("Question cannot be empty", nameof(question));

        conversationHistory.Add(new ChatMessageDto("user", question));

        var messages = conversationHistory
            .Select(m => new ChatMessage(m.Role, m.Text, m.Text));

        var reply = await GetReplyAsync(messages, null, ct);
        conversationHistory.Add(new ChatMessageDto("assistant", reply.Text));
        return reply;
    }

    public async Task<ChatReply> AskWithInstructionsAsync(
        string question,
        string systemInstructions,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("Question cannot be empty", nameof(question));

        var messages = new[] { new ChatMessage("user", question, question) };
        return await GetReplyAsync(messages, systemInstructions, ct);
    }

    public void ClearHistory()
    {
        conversationHistory.Clear();
        Log.Debug("OpenAI: Conversation history cleared");
    }

    public IReadOnlyList<ChatMessageDto> GetHistory() => conversationHistory.AsReadOnly();

    public async Task<ChatReply> GetReplyAsync(
        IEnumerable<ChatMessage> messages,
        string? instructions = null,
        CancellationToken ct = default)
    {
        Log.Info("OpenAI: Getting chat reply via Legion...");

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = MindAtticCredentialStore.GetKey("openai");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("OpenAI: API key is missing");
            throw new InvalidOperationException("OpenAI API key is missing. Set it in Settings > Credentials.");
        }

        var model = await opt.GetModelAsync();
        var turns = messages.Select(m => new ChatTurn(m.Role, m.Text)).ToList();
        Log.Debug($"OpenAI: Sending {turns.Count} message(s), model={model}");

        try
        {
            var text = await legion.CallChatAsync(
                providerId: "openai",
                apiKey: apiKey!,
                model: model,
                messages: turns,
                systemPrompt: instructions,
                maxTokens: opt.MaxTokens,
                temperature: opt.Temperature,
                ct: ct);

            var diagnostic = JsonSerializer.Serialize(new
            {
                provider = "openai",
                model,
                text,
            }, new JsonSerializerOptions { WriteIndented = true });

            Log.Info($"OpenAI: Reply received ({text.Length} chars)");
            return new ChatReply(text, diagnostic);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"OpenAI: HTTP error - {ex.Message}", ex);
            throw new InvalidOperationException($"OpenAI HTTP {(int?)ex.StatusCode}: {ex.Message}", ex);
        }
        catch (CircuitBreakerOpenException ex)
        {
            Log.Warn($"OpenAI: circuit breaker open - {ex.Message}");
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}

/// <summary>
/// Configuration options for OpenAI API.
/// </summary>
public sealed class OpenAIOptions
{
    private readonly ISecurePreferences securePreferences;

    public OpenAIOptions(ISecurePreferences securePreferences)
    {
        this.securePreferences = securePreferences;
    }

    /// <summary>The OpenAI model to use for chat completions.</summary>
    public string Model { get; set; } = "gpt-4.1-mini";

    /// <summary>Controls randomness (0.0 = deterministic, 2.0 = very random).</summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>Maximum tokens in the response.</summary>
    public int MaxTokens { get; set; } = 2000;

    /// <summary>Gets the API key from secure storage.</summary>
    public async Task<string?> GetApiKeyAsync()
    {
        try { return await securePreferences.GetAsync("OPENAI_API_KEY"); }
        catch { return null; }
    }

    /// <summary>
    /// Resolves the model at request time: secure storage (CHATGPT_MODEL) falls
    /// back to the <see cref="Model"/> default.
    /// </summary>
    public async Task<string> GetModelAsync()
    {
        try
        {
            var stored = await securePreferences.GetAsync("CHATGPT_MODEL");
            return string.IsNullOrWhiteSpace(stored) ? Model : stored;
        }
        catch { return Model; }
    }

    public async Task SetApiKeyAsync(string apiKey) =>
        await securePreferences.SetAsync("OPENAI_API_KEY", apiKey);

    public void ClearApiKey() =>
        securePreferences.Remove("OPENAI_API_KEY");
}
