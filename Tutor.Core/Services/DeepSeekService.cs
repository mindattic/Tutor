using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Service for interacting with DeepSeek's chat API.
/// Uses OpenAI-compatible endpoint format.
/// </summary>
public sealed class DeepSeekService : ILlmService, IDisposable
{
    private readonly HttpClient http;
    private readonly ISecurePreferences prefs;
    private bool disposed;

    private const string ApiKeyName = "DEEPSEEK_API_KEY";
    private const string ModelKeyName = "DEEPSEEK_MODEL";
    private const string DefaultModel = "deepseek-chat";
    private const int MaxRetries = 5;
    private const int BaseDelayMs = 2000;

    public string ProviderName => "DeepSeek";

    public DeepSeekService(HttpClient http, ISecurePreferences prefs)
    {
        this.http = http;
        this.prefs = prefs;
        Log.Debug("DeepSeekService initialized");
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
                    Log.Error($"DeepSeek: API call failed after {maxRetries + 1} attempts");
                    throw;
                }

                var delayMs = BaseDelayMs * (int)Math.Pow(2, attempt);
                Log.Warn($"DeepSeek: Rate limited or server error, waiting {delayMs}ms before retry {attempt + 1}/{maxRetries}");
                await Task.Delay(delayMs, ct);
            }
        }

        throw lastException ?? new InvalidOperationException("Unexpected retry loop exit");
    }

    private async Task<ChatReply> GetReplyCoreAsync(IEnumerable<ChatMessage> messages, string? instructions, CancellationToken ct)
    {
        Log.Info("DeepSeek: Getting chat reply...");

        var apiKey = await prefs.GetAsync(ApiKeyName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("DeepSeek: API key is missing");
            throw new InvalidOperationException("DeepSeek API key is missing. Set it in Settings > Credentials.");
        }

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        // Build messages array (OpenAI-compatible format)
        var apiMessages = new List<object>();

        if (!string.IsNullOrWhiteSpace(instructions))
        {
            apiMessages.Add(new { role = "system", content = instructions });
            Log.Trace($"DeepSeek: Using system instructions ({instructions.Length} chars)");
        }

        foreach (var m in messages)
        {
            apiMessages.Add(new { role = m.Role, content = m.Text });
        }

        Log.Debug($"DeepSeek: Sending {apiMessages.Count} message(s), model={model}");

        var payload = new
        {
            model,
            messages = apiMessages,
            max_tokens = 4096
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            Log.Trace("DeepSeek: Sending HTTP request...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            if (statusCode != 200)
            {
                Log.Error($"DeepSeek: API error HTTP {statusCode} {resp.ReasonPhrase}");
                Log.Debug($"DeepSeek error body: {(json.Length > 500 ? json[..500] + "..." : json)}");

                var apiErrorMessage = ExtractErrorMessage(json);
                var errorMessage = statusCode switch
                {
                    400 => $"Bad Request - {apiErrorMessage ?? "Check your message format"}",
                    401 => "Unauthorized - Invalid or expired API key",
                    402 => "Insufficient balance - Check your DeepSeek account",
                    429 => $"Rate Limited - {apiErrorMessage ?? "Too many requests"}",
                    500 => "DeepSeek Server Error - Try again later",
                    502 => "Bad Gateway - DeepSeek temporarily unavailable",
                    503 => "Service Unavailable - DeepSeek overloaded",
                    _ => apiErrorMessage ?? json
                };

                throw new InvalidOperationException($"DeepSeek HTTP {statusCode}: {errorMessage}");
            }

            Log.Debug($"DeepSeek: Response HTTP {statusCode} ({json.Length} chars)");

            var formattedJson = FormatJson(json);
            var text = ExtractText(json);

            Log.Info($"DeepSeek: Reply received ({text.Length} chars)");
            return new ChatReply(text, formattedJson);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"DeepSeek: Network error - {ex.Message}", ex);
            throw new InvalidOperationException($"Network error: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            Log.Error("DeepSeek: Request timed out", ex);
            throw new InvalidOperationException("DeepSeek API request timed out", ex);
        }
    }

    private static string ExtractText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            // OpenAI-compatible: choices[0].message.content
            if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
            {
                foreach (var choice in choices.EnumerateArray())
                {
                    if (choice.TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var content))
                    {
                        return content.GetString() ?? "";
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
        message.Contains("502") || message.Contains("503");

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
