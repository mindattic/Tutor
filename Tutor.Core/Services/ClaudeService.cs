using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Service for interacting with Anthropic's Claude API.
/// </summary>
public sealed class ClaudeService : ILlmService, IDisposable
{
    private readonly HttpClient http;
    private readonly ISecurePreferences prefs;
    private bool disposed;

    private const string ApiKeyName = "CLAUDE_API_KEY";
    private const string ModelKeyName = "CLAUDE_MODEL";
    private const string DefaultModel = "claude-sonnet-4-20250514";
    private const int MaxRetries = 5;
    private const int BaseDelayMs = 2000;

    public string ProviderName => "Claude";

    public ClaudeService(HttpClient http, ISecurePreferences prefs)
    {
        this.http = http;
        this.prefs = prefs;
        Log.Debug("ClaudeService initialized");
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var key = await prefs.GetAsync(ApiKeyName);
        return !string.IsNullOrWhiteSpace(key);
    }

    public async Task<ChatReply> GetReplyAsync(IEnumerable<ChatMessage> messages, string? instructions = null, CancellationToken ct = default)
    {
        return await GetReplyWithRetryAsync(messages, instructions, ct, MaxRetries);
    }

    private async Task<ChatReply> GetReplyWithRetryAsync(
        IEnumerable<ChatMessage> messages, string? instructions, CancellationToken ct, int maxRetries)
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
                    Log.Error($"Claude: API call failed after {maxRetries + 1} attempts");
                    throw;
                }

                var delayMs = BaseDelayMs * (int)Math.Pow(2, attempt);
                Log.Warn($"Claude: Rate limited or server error, waiting {delayMs}ms before retry {attempt + 1}/{maxRetries}");
                await Task.Delay(delayMs, ct);
            }
        }

        throw lastException ?? new InvalidOperationException("Unexpected retry loop exit");
    }

    private async Task<ChatReply> GetReplyCoreAsync(IEnumerable<ChatMessage> messages, string? instructions, CancellationToken ct)
    {
        Log.Info("Claude: Getting chat reply...");

        var apiKey = await prefs.GetAsync(ApiKeyName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("Claude: API key is missing");
            throw new InvalidOperationException("Claude API key is missing. Set it in Settings > Credentials.");
        }

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        // Build messages array for Claude API
        var apiMessages = new List<object>();
        foreach (var m in messages)
        {
            var role = m.Role == "assistant" ? "assistant" : "user";
            apiMessages.Add(new { role, content = m.Text });
        }

        Log.Debug($"Claude: Sending {apiMessages.Count} message(s), model={model}");

        var payload = new Dictionary<string, object>
        {
            ["model"] = model,
            ["max_tokens"] = 4096,
            ["messages"] = apiMessages
        };

        if (!string.IsNullOrWhiteSpace(instructions))
        {
            // Use structured content blocks with cache_control for prompt caching.
            // Cached system prompts cost 90% less on cache hits after the initial write.
            payload["system"] = new[]
            {
                new
                {
                    type = "text",
                    text = instructions,
                    cache_control = new { type = "ephemeral" }
                }
            };
            Log.Trace($"Claude: Using system instructions ({instructions.Length} chars) with prompt caching");
        }

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        req.Headers.Add("x-api-key", apiKey);
        req.Headers.Add("anthropic-version", "2023-06-01");
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            Log.Trace("Claude: Sending HTTP request...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            if (statusCode != 200)
            {
                Log.Error($"Claude: API error HTTP {statusCode} {resp.ReasonPhrase}");
                Log.Debug($"Claude error body: {(json.Length > 500 ? json[..500] + "..." : json)}");

                var apiErrorMessage = ExtractErrorMessage(json);
                var errorMessage = statusCode switch
                {
                    400 => $"Bad Request - {apiErrorMessage ?? "Check your message format"}",
                    401 => "Unauthorized - Invalid or expired API key",
                    403 => "Forbidden - API key lacks permissions",
                    429 => $"Rate Limited - {apiErrorMessage ?? "Too many requests"}",
                    500 => "Anthropic Server Error - Try again later",
                    502 => "Bad Gateway - Anthropic temporarily unavailable",
                    503 => "Service Unavailable - Anthropic overloaded",
                    529 => "Anthropic API is overloaded - Try again later",
                    _ => apiErrorMessage ?? json
                };

                throw new InvalidOperationException($"Claude HTTP {statusCode}: {errorMessage}");
            }

            Log.Debug($"Claude: Response HTTP {statusCode} ({json.Length} chars)");

            var formattedJson = FormatJson(json);
            var text = ExtractText(json);

            Log.Info($"Claude: Reply received ({text.Length} chars)");
            return new ChatReply(text, formattedJson);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"Claude: Network error - {ex.Message}", ex);
            throw new InvalidOperationException($"Network error: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            Log.Error("Claude: Request timed out", ex);
            throw new InvalidOperationException("Claude API request timed out", ex);
        }
    }

    private static string ExtractText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            // Claude returns content[0].text
            if (doc.RootElement.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
            {
                foreach (var block in content.EnumerateArray())
                {
                    if (block.TryGetProperty("type", out var type) && type.GetString() == "text" &&
                        block.TryGetProperty("text", out var textProp))
                    {
                        return textProp.GetString() ?? "";
                    }
                }
            }
        }
        catch { }
        return json;
    }

    private static string? ExtractErrorMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var message))
            {
                return message.GetString();
            }
        }
        catch { }
        return null;
    }

    private static bool IsRetryableError(string message) =>
        message.Contains("429") || message.Contains("Rate") || message.Contains("overloaded") ||
        message.Contains("502") || message.Contains("503") || message.Contains("529");

    private static string FormatJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch { return json; }
    }

    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
        }
    }
}
