using System.Text;
using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Service for interacting with Google's Gemini API.
/// </summary>
public sealed class GeminiService : ILlmService, IDisposable
{
    private readonly HttpClient http;
    private readonly ISecurePreferences prefs;
    private bool disposed;

    private const string ApiKeyName = "GEMINI_API_KEY";
    private const string ModelKeyName = "GEMINI_MODEL";
    private const string DefaultModel = "gemini-2.5-flash";
    private const int MaxRetries = 5;
    private const int BaseDelayMs = 2000;

    public string ProviderName => "Gemini";

    public GeminiService(HttpClient http, ISecurePreferences prefs)
    {
        this.http = http;
        this.prefs = prefs;
        Log.Debug("GeminiService initialized");
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
                    Log.Error($"Gemini: API call failed after {maxRetries + 1} attempts");
                    throw;
                }

                var delayMs = BaseDelayMs * (int)Math.Pow(2, attempt);
                Log.Warn($"Gemini: Rate limited or server error, waiting {delayMs}ms before retry {attempt + 1}/{maxRetries}");
                await Task.Delay(delayMs, ct);
            }
        }

        throw lastException ?? new InvalidOperationException("Unexpected retry loop exit");
    }

    private async Task<ChatReply> GetReplyCoreAsync(IEnumerable<ChatMessage> messages, string? instructions, CancellationToken ct)
    {
        Log.Info("Gemini: Getting chat reply...");

        var apiKey = await prefs.GetAsync(ApiKeyName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("Gemini: API key is missing");
            throw new InvalidOperationException("Gemini API key is missing. Set it in Settings > Credentials.");
        }

        var model = await prefs.GetAsync(ModelKeyName);
        if (string.IsNullOrWhiteSpace(model)) model = DefaultModel;

        // Build Gemini contents array
        var contents = new List<object>();

        foreach (var m in messages)
        {
            var role = m.Role == "assistant" ? "model" : "user";
            contents.Add(new
            {
                role,
                parts = new[] { new { text = m.Text } }
            });
        }

        Log.Debug($"Gemini: Sending {contents.Count} message(s), model={model}");

        var payload = new Dictionary<string, object>
        {
            ["contents"] = contents
        };

        // Add system instruction if provided
        if (!string.IsNullOrWhiteSpace(instructions))
        {
            payload["systemInstruction"] = new
            {
                parts = new[] { new { text = instructions } }
            };
            Log.Trace($"Gemini: Using system instructions ({instructions.Length} chars)");
        }

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            Log.Trace("Gemini: Sending HTTP request...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            if (statusCode != 200)
            {
                Log.Error($"Gemini: API error HTTP {statusCode} {resp.ReasonPhrase}");
                Log.Debug($"Gemini error body: {(json.Length > 500 ? json[..500] + "..." : json)}");

                var apiErrorMessage = ExtractErrorMessage(json);
                var errorMessage = statusCode switch
                {
                    400 => $"Bad Request - {apiErrorMessage ?? "Check your message format"}",
                    401 => "Unauthorized - Invalid or expired API key",
                    403 => "Forbidden - API key lacks permissions",
                    429 => $"Rate Limited - {apiErrorMessage ?? "Too many requests"}",
                    500 => "Google AI Server Error - Try again later",
                    503 => "Service Unavailable - Google AI overloaded",
                    _ => apiErrorMessage ?? json
                };

                throw new InvalidOperationException($"Gemini HTTP {statusCode}: {errorMessage}");
            }

            Log.Debug($"Gemini: Response HTTP {statusCode} ({json.Length} chars)");

            var formattedJson = FormatJson(json);
            var text = ExtractText(json);

            Log.Info($"Gemini: Reply received ({text.Length} chars)");
            return new ChatReply(text, formattedJson);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"Gemini: Network error - {ex.Message}", ex);
            throw new InvalidOperationException($"Network error: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            Log.Error("Gemini: Request timed out", ex);
            throw new InvalidOperationException("Gemini API request timed out", ex);
        }
    }

    private static string ExtractText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            // Gemini returns candidates[0].content.parts[0].text
            if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.ValueKind == JsonValueKind.Array)
            {
                foreach (var candidate in candidates.EnumerateArray())
                {
                    if (candidate.TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) && parts.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var part in parts.EnumerateArray())
                        {
                            if (part.TryGetProperty("text", out var textProp))
                            {
                                return textProp.GetString() ?? "";
                            }
                        }
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
