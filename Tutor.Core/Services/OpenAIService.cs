// ============================================================================
// OpenAIService - AI-Powered Chat Service for Tutor Application
// ============================================================================
//
// PURPOSE:
// Communicates with OpenAI's API to provide AI-powered chat responses,
// answer questions, and assist with tutoring tasks.
//
// USAGE:
// var reply = await openAIService.AskAsync("What is the Pythagorean theorem?");
// Console.WriteLine(reply.Text);
//
// CONFIGURATION:
// API key is stored in SecureStorage under "OPENAI_API_KEY".
// Use OpenAIOptions.SetApiKeyAsync() to configure.
//
// ============================================================================

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;


/// <summary>
/// Response from OpenAI containing the text reply and raw JSON.
/// </summary>
public record ChatReply(string Text, string FullJson);

/// <summary>
/// A simple chat message DTO with role and text content.
/// </summary>
public record ChatMessageDto(string Role, string Text);

/// <summary>
/// Service for interacting with OpenAI's API.
/// Supports chat completions with conversation history, retry logic, and custom instructions.
/// </summary>
public sealed class OpenAIService : ILlmService, IDisposable
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    private readonly List<ChatMessageDto> conversationHistory = [];
    private bool disposed;

    private const int MaxRetries = 5;
    private const int BaseDelayMs = 2000;

    public string ProviderName => "ChatGPT";

    public OpenAIService(HttpClient http, OpenAIOptions opt)
    {
        this.http = http;
        this.opt = opt;
        Log.Debug("OpenAIService initialized");
    }

    /// <summary>
    /// Checks if the API key is configured.
    /// </summary>
    public async Task<bool> IsConfiguredAsync()
    {
        var key = await opt.GetApiKeyAsync();
        return !string.IsNullOrWhiteSpace(key);
    }

    /// <summary>
    /// Ask a simple question and get a reply.
    /// Maintains conversation history for context.
    /// </summary>
    /// <param name="question">The question to ask.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The chat reply.</returns>
    public async Task<ChatReply> AskAsync(string question, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("Question cannot be empty", nameof(question));

        conversationHistory.Add(new ChatMessageDto("user", question));

        var messages = conversationHistory
            .Select(m => new ChatMessage(m.Role, m.Text, m.Text));

        var reply = await GetReplyWithRetryAsync(messages, null, ct, MaxRetries);

        conversationHistory.Add(new ChatMessageDto("assistant", reply.Text));

        return reply;
    }

    /// <summary>
    /// Ask a question with custom system instructions (does not use conversation history).
    /// </summary>
    /// <param name="question">The question to ask.</param>
    /// <param name="systemInstructions">Custom system instructions.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The chat reply.</returns>
    public async Task<ChatReply> AskWithInstructionsAsync(
        string question,
        string systemInstructions,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("Question cannot be empty", nameof(question));

        var messages = new[]
        {
            new ChatMessage("user", question, question)
        };

        return await GetReplyWithRetryAsync(messages, systemInstructions, ct, MaxRetries);
    }

    /// <summary>
    /// Clear conversation history.
    /// </summary>
    public void ClearHistory()
    {
        conversationHistory.Clear();
        Log.Debug("OpenAI: Conversation history cleared");
    }

    /// <summary>
    /// Get the current conversation history.
    /// </summary>
    public IReadOnlyList<ChatMessageDto> GetHistory() => conversationHistory.AsReadOnly();

    /// <summary>
    /// Get a chat reply with automatic retry for rate limiting.
    /// </summary>
    public async Task<ChatReply> GetReplyAsync(IEnumerable<ChatMessage> messages, string? instructions = null, CancellationToken ct = default)
    {
        return await GetReplyWithRetryAsync(messages, instructions, ct, MaxRetries);
    }

    /// <summary>
    /// Get a chat reply with retry logic for rate limiting and transient errors.
    /// </summary>
    private async Task<ChatReply> GetReplyWithRetryAsync(
        IEnumerable<ChatMessage> messages, 
        string? instructions, 
        CancellationToken ct,
        int maxRetries)
    {
        Exception? lastException = null;

        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await GetReplyCoreAsync(messages, instructions, ct);
            }
            catch (InvalidOperationException ex) when (IsRetryableError(ex.Message))
            {
                lastException = ex;

                if (attempt == maxRetries)
                {
                    Log.Error($"OpenAI: API call failed after {maxRetries + 1} attempts");
                    throw;
                }

                var delayMs = CalculateDelay(ex.Message, attempt);
                Log.Warn($"OpenAI: Rate limited or server error, waiting {delayMs}ms before retry {attempt + 1}/{maxRetries}");
                await Task.Delay(delayMs, ct);
            }
        }

        // Should never reach here, but compiler needs it
        throw lastException ?? new InvalidOperationException("Unexpected retry loop exit");
    }

    /// <summary>
    /// Determines if an error is retryable (rate limit, server overload, etc.).
    /// </summary>
    private static bool IsRetryableError(string message)
    {
        return message.Contains("429") ||
               message.Contains("Rate") ||
               message.Contains("overloaded") ||
               message.Contains("502") ||
               message.Contains("503");
    }

    /// <summary>
    /// Calculates delay for retry with exponential backoff.
    /// </summary>
    private static int CalculateDelay(string errorMessage, int attempt)
    {
        // Exponential backoff: 2s, 4s, 8s, 16s, 32s
        var delayMs = BaseDelayMs * (int)Math.Pow(2, attempt);

        // Try to extract wait time from error message
        var waitMatch = Regex.Match(errorMessage, @"try again in (\d+\.?\d*)s");
        if (waitMatch.Success && double.TryParse(
            waitMatch.Groups[1].Value,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out var waitSeconds))
        {
            delayMs = (int)(waitSeconds * 1000) + 500; // Add 500ms buffer
        }

        return delayMs;
    }

    /// <summary>
    /// Core implementation of GetReply (no retry logic).
    /// </summary>
    private async Task<ChatReply> GetReplyCoreAsync(IEnumerable<ChatMessage> messages, string? instructions, CancellationToken ct)
    {
        Log.Info("OpenAI: Getting chat reply...");
        
        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("OpenAI: API key is missing");
            throw new InvalidOperationException("OpenAI API key is missing. Set it in SecureStorage.");
        }

        var sb = new StringBuilder();
        var messageCount = 0;
        foreach (var m in messages)
        {
            sb.AppendLine($"{m.Role}: {m.Text}");
            messageCount++;
        }
        
        Log.Debug($"OpenAI: Sending {messageCount} message(s), model={opt.Model}");

        object payload;
        if (!string.IsNullOrWhiteSpace(instructions))
        {
            Log.Trace($"OpenAI: Using custom instructions ({instructions.Length} chars)");
            payload = new
            {
                model = opt.Model,
                input = sb.ToString(),
                instructions = instructions
            };
        }
        else
        {
            payload = new
            {
                model = opt.Model,
                input = sb.ToString(),
            };
        }

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            Log.Trace("OpenAI: Sending HTTP request...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            // Log all non-200 responses with detailed info
            if (statusCode != 200)
            {
                Log.Error($"OpenAI: API error HTTP {statusCode} {resp.ReasonPhrase}");
                Log.Debug($"OpenAI error body: {(json.Length > 500 ? json[..500] + "..." : json)}");
                
                // Try to extract actual error message from OpenAI response
                var apiErrorMessage = ExtractOpenAIErrorMessage(json);
                
                var errorMessage = statusCode switch
                {
                    400 => $"Bad Request - {apiErrorMessage ?? "Check your message format"}",
                    401 => "Unauthorized - Invalid or expired API key",
                    403 => "Forbidden - API key lacks permissions",
                    404 => "Not Found - Invalid endpoint or model name",
                    429 => $"Rate Limited - {apiErrorMessage ?? "Too many requests, wait before retrying"}",
                    500 => "OpenAI Server Error - Try again later",
                    502 => "Bad Gateway - OpenAI temporarily unavailable",
                    503 => "Service Unavailable - OpenAI overloaded",
                    _ => apiErrorMessage ?? json
                };
                
                throw new InvalidOperationException($"OpenAI HTTP {statusCode}: {errorMessage}");
            }
            
            Log.Debug($"OpenAI: Response HTTP {statusCode} ({json.Length} chars)");

            // Format the JSON for display
            var formattedJson = FormatJson(json);

            // Parse the text from the response
            var text = ExtractText(json);
            
            Log.Info($"OpenAI: Reply received ({text.Length} chars)");

            return new ChatReply(text, formattedJson);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"OpenAI: Network error - {ex.Message} (Status: {ex.StatusCode})", ex);
            throw new InvalidOperationException($"Network error: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            Log.Error("OpenAI: Request timed out after waiting", ex);
            throw new InvalidOperationException("OpenAI API request timed out", ex);
        }
    }

    /// <summary>
    /// Extract error message from OpenAI's error response JSON.
    /// </summary>
    private static string? ExtractOpenAIErrorMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                if (error.TryGetProperty("message", out var message))
                {
                    return message.GetString();
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return null;
    }

    private static string ExtractText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            
            // Try output_text first (convenience field)
            if (doc.RootElement.TryGetProperty("output_text", out var outText) && outText.ValueKind == JsonValueKind.String)
            {
                return outText.GetString() ?? "";
            }

            // Parse from output[0].content[0].text
            if (doc.RootElement.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in output.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var c in content.EnumerateArray())
                        {
                            if (c.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                            {
                                return textProp.GetString() ?? "";
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // Fall through to return raw json
        }

        return json;
    }

    private static string FormatJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return json;
        }
    }

    public void Dispose()
    {
        if (!disposed)
        {
            // Note: HttpClient is injected, so we don't dispose it here.
            // Clear conversation history to free memory.
            conversationHistory.Clear();
            disposed = true;
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

    // Valid OpenAI model options:
    // ── GPT-5 Series ──
    //   "gpt-5.2"             - Latest flagship, best quality + speed
    //   "gpt-5.1"             - Previous flagship
    //   "gpt-5"               - Original GPT-5
    //   "gpt-5-mini"          - Smaller GPT-5, cheaper, fast
    // ── GPT-4 Series ──
    //   "gpt-4.1"             - Latest GPT-4 generation
    //   "gpt-4.1-mini"        - Smaller GPT-4.1, good balance (default)
    //   "gpt-4.1-nano"        - Smallest GPT-4.1, fastest/cheapest
    //   "gpt-4o"              - GPT-4 Omni, multimodal
    //   "gpt-4o-mini"         - Smaller GPT-4o, cheap and fast
    //   "gpt-4-turbo"         - GPT-4 Turbo (legacy)
    // ── Reasoning Models ──
    //   "o3"                  - Advanced reasoning, slow, expensive
    //   "o3-mini"             - Smaller reasoning model
    //   "o4-mini"             - Latest small reasoning model
    //   "o1"                  - Original reasoning model
    //   "o1-mini"             - Smaller original reasoning

    /// <summary>
    /// The OpenAI model to use for chat completions.
    /// </summary>
    public string Model { get; set; } = "gpt-4.1-mini";

    /// <summary>
    /// Controls randomness in responses (0.0 = deterministic, 2.0 = very random).
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Maximum tokens in the response.
    /// </summary>
    public int MaxTokens { get; set; } = 2000;

    /// <summary>
    /// Gets the API key from secure storage.
    /// </summary>
    public async Task<string?> GetApiKeyAsync()
    {
        try
        {
            return await securePreferences.GetAsync("OPENAI_API_KEY");
        }
        catch
        {
            // SecureStorage can fail on some platforms if not configured.
            return null;
        }
    }

    /// <summary>
    /// Sets the API key in secure storage.
    /// </summary>
    public async Task SetApiKeyAsync(string apiKey)
    {
        await securePreferences.SetAsync("OPENAI_API_KEY", apiKey);
    }

    /// <summary>
    /// Removes the API key from secure storage.
    /// </summary>
    public void ClearApiKey()
    {
        securePreferences.Remove("OPENAI_API_KEY");
    }
}

