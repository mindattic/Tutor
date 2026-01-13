using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tutor.Components;
using Tutor.Services.Logging;

public record ChatReply(string Text, string FullJson);

// Simple message interface for the service
public record ChatMessageDto(string Role, string Text);

public sealed class OpenAIService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;

    public OpenAIService(HttpClient http, OpenAIOptions opt)
    {
        this.http = http;
        this.opt = opt;
        Log.Debug("OpenAIService initialized");
    }

    /// <summary>
    /// Get a chat reply with automatic retry for rate limiting.
    /// </summary>
    public async Task<ChatReply> GetReplyAsync(IEnumerable<Chat.ChatMessage> messages, string? instructions = null, CancellationToken ct = default)
    {
        return await GetReplyWithRetryAsync(messages, instructions, ct, maxRetries: 5);
    }

    /// <summary>
    /// Get a chat reply with retry logic for rate limiting and transient errors.
    /// </summary>
    private async Task<ChatReply> GetReplyWithRetryAsync(
        IEnumerable<Chat.ChatMessage> messages, 
        string? instructions, 
        CancellationToken ct,
        int maxRetries = 5)
    {
        var baseDelayMs = 2000; // Start with 2 seconds
        Exception? lastException = null;
        
        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await GetReplyCoreAsync(messages, instructions, ct);
            }
            catch (InvalidOperationException ex) when (
                ex.Message.Contains("429") || 
                ex.Message.Contains("Rate") ||
                ex.Message.Contains("overloaded") ||
                ex.Message.Contains("502") ||
                ex.Message.Contains("503"))
            {
                lastException = ex;
                
                if (attempt == maxRetries)
                {
                    Log.Error($"OpenAI: API call failed after {maxRetries + 1} attempts");
                    throw;
                }
                
                // Exponential backoff: 2s, 4s, 8s, 16s, 32s
                var delayMs = baseDelayMs * (int)Math.Pow(2, attempt);
                
                // Try to extract wait time from error message
                var waitMatch = Regex.Match(ex.Message, @"try again in (\d+\.?\d*)s");
                if (waitMatch.Success && double.TryParse(waitMatch.Groups[1].Value, 
                    System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    out var waitSeconds))
                {
                    delayMs = (int)(waitSeconds * 1000) + 500; // Add 500ms buffer
                }
                
                Log.Warn($"OpenAI: Rate limited or server error, waiting {delayMs}ms before retry {attempt + 1}/{maxRetries}");
                await Task.Delay(delayMs, ct);
            }
        }
        
        // Should never reach here, but compiler needs it
        throw lastException ?? new InvalidOperationException("Unexpected retry loop exit");
    }

    /// <summary>
    /// Core implementation of GetReply (no retry logic).
    /// </summary>
    private async Task<ChatReply> GetReplyCoreAsync(IEnumerable<Chat.ChatMessage> messages, string? instructions, CancellationToken ct)
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
}

public sealed class OpenAIOptions
{
    // Pick a default model you have access to. You can change this later.
    public string Model { get; set; } = "gpt-4.1-mini";

    public async Task<string?> GetApiKeyAsync()
    {
        try
        {
            return await Microsoft.Maui.Storage.SecureStorage.GetAsync("OPENAI_API_KEY");
        }
        catch
        {
            // SecureStorage can fail on some platforms if not configured.
            return null;
        }
    }

    public async Task SetApiKeyAsync(string apiKey)
    {
        await Microsoft.Maui.Storage.SecureStorage.SetAsync("OPENAI_API_KEY", apiKey);
    }
}

