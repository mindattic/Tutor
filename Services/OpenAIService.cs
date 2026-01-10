using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Components;

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
    }

    public async Task<ChatReply> GetReplyAsync(IEnumerable<Chat.ChatMessage> messages, string? instructions = null, CancellationToken ct = default)
    {
        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is missing. Set it in SecureStorage.");

        var sb = new StringBuilder();
        foreach (var m in messages)
        {
            sb.AppendLine($"{m.Role}: {m.Text}");
        }

        object payload;
        if (!string.IsNullOrWhiteSpace(instructions))
        {
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

        using var resp = await http.SendAsync(req, ct);
        var json = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI error {(int)resp.StatusCode}: {json}");
        }

        // Format the JSON for display
        var formattedJson = FormatJson(json);

        // Parse the text from the response
        var text = ExtractText(json);

        return new ChatReply(text, formattedJson);
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

