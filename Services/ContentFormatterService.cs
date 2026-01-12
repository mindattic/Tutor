using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for formatting imported content into structured markdown.
/// Uses AI to identify sections, key terms, and add appropriate formatting.
/// </summary>
public sealed class ContentFormatterService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;

    // Formatting instructions for the AI
    private const string FormattingPrompt = @"You are a document formatter. Convert the following raw text into well-structured markdown for educational use.

FORMATTING RULES:
- Add ## headers for major sections/topics
- Add ### headers for subsections
- Use **bold** for key terms, concepts, definitions, and important vocabulary
- Use bullet points (-) for lists of items, features, or characteristics
- Use numbered lists only for sequential steps or processes
- Keep paragraphs short (2-3 sentences max)
- Add blank lines between sections for readability
- Preserve all original information - do not summarize or remove content
- Do not add explanations or commentary - just format the existing content
- If content has chapter/section numbers, preserve them in headers

IMPORTANT: 
- Bold ALL important terms, names, dates, and concepts that a student should remember
- Every paragraph should have at least 1-2 bolded terms if relevant
- Do not wrap the response in code blocks - return plain markdown

Return ONLY the formatted markdown, nothing else.

TEXT TO FORMAT:
";

    public ContentFormatterService(HttpClient http, OpenAIOptions opt)
    {
        this.http = http;
        this.opt = opt;
        Log.Debug("ContentFormatterService initialized");
    }

    /// <summary>
    /// Format raw text content into structured markdown using AI.
    /// </summary>
    public async Task<string> FormatContentAsync(string rawContent, CancellationToken ct = default)
    {
        return await FormatContentAsync(rawContent, null, ct);
    }

    /// <summary>
    /// Format raw text content into structured markdown using AI with progress reporting.
    /// </summary>
    public async Task<string> FormatContentAsync(string rawContent, Action<int, int>? onProgress, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
            return rawContent;

        Log.Info($"ContentFormatter: Starting formatting ({rawContent.Length} chars)");

        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("ContentFormatter: API key is missing");
            throw new InvalidOperationException("OpenAI API key is missing.");
        }

        // For very large content, process in chunks to avoid token limits
        if (rawContent.Length > 15000)
        {
            Log.Debug($"ContentFormatter: Large content detected, using chunked processing");
            return await FormatLargeContentAsync(rawContent, apiKey, onProgress, ct);
        }

        onProgress?.Invoke(1, 1);
        var result = await FormatChunkAsync(rawContent, apiKey, ct);
        Log.Info($"ContentFormatter: Formatting complete ({result.Length} chars output)");
        return result;
    }

    /// <summary>
    /// Format large content by splitting into sections and processing each.
    /// </summary>
    private async Task<string> FormatLargeContentAsync(string content, string apiKey, Action<int, int>? onProgress, CancellationToken ct)
    {
        // Split by double newlines (paragraphs) and process in batches
        var paragraphs = content.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();
        var currentChunk = new StringBuilder();
        const int maxChunkSize = 12000;

        foreach (var para in paragraphs)
        {
            if (currentChunk.Length + para.Length > maxChunkSize && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();
            }
            currentChunk.AppendLine(para);
            currentChunk.AppendLine();
        }

        if (currentChunk.Length > 0)
        {
            chunks.Add(currentChunk.ToString());
        }

        // Process each chunk with progress reporting
        var formattedChunks = new List<string>();
        var totalChunks = chunks.Count;
        for (int i = 0; i < chunks.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            onProgress?.Invoke(i + 1, totalChunks);
            var formatted = await FormatChunkAsync(chunks[i], apiKey, ct);
            formattedChunks.Add(formatted);
        }

        return string.Join("\n\n", formattedChunks);
    }

    /// <summary>
    /// Format a single chunk of content.
    /// </summary>
    private async Task<string> FormatChunkAsync(string content, string apiKey, CancellationToken ct)
    {
        var payload = new
        {
            model = "gpt-4.1-mini",
            input = FormattingPrompt + content,
            instructions = "Format the text as markdown. Return only the formatted content, no explanations."
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            Log.Trace("ContentFormatter: Sending HTTP request...");
            using var resp = await http.SendAsync(req, ct);
            var statusCode = (int)resp.StatusCode;
            var json = await resp.Content.ReadAsStringAsync(ct);

            if (statusCode != 200)
            {
                Log.Error($"ContentFormatter: API error HTTP {statusCode} {resp.ReasonPhrase}");
                Log.Debug($"ContentFormatter error body: {(json.Length > 500 ? json[..500] + "..." : json)}");
                
                // On failure, return original content with a warning
                Log.Warn($"ContentFormatter: Returning original content due to API error {statusCode}");
                return content;
            }

            Log.Debug($"ContentFormatter: Response HTTP {statusCode} ({json.Length} chars)");
            return ExtractText(json);
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"ContentFormatter: Network error - {ex.Message} (Status: {ex.StatusCode})", ex);
            return content; // Return original on network error
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            Log.Error("ContentFormatter: Request timed out", ex);
            return content; // Return original on timeout
        }
        catch (TaskCanceledException)
        {
            Log.Debug("ContentFormatter: Request cancelled by user");
            throw;
        }
    }

    private static string ExtractText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("output_text", out var outText) && outText.ValueKind == JsonValueKind.String)
            {
                return outText.GetString() ?? "";
            }

            if (doc.RootElement.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in output.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out var contentArr) && contentArr.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var c in contentArr.EnumerateArray())
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
        catch { }

        return "";
    }

    /// <summary>
    /// Quick formatting without AI - applies basic markdown rules.
    /// Useful as a fallback or for already-structured content.
    /// </summary>
    public static string QuickFormat(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return content;

        var lines = content.Split('\n');
        var result = new StringBuilder();
        var inList = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            // Skip empty lines but preserve spacing
            if (string.IsNullOrWhiteSpace(line))
            {
                if (inList)
                {
                    inList = false;
                }
                result.AppendLine();
                continue;
            }

            // Detect potential headers (all caps, short lines, or ending with colon)
            if (IsLikelyHeader(line))
            {
                result.AppendLine();
                result.AppendLine($"## {ToTitleCase(line.TrimEnd(':'))}");
                result.AppendLine();
                continue;
            }

            // Detect list items (starting with numbers, letters, dashes, bullets)
            if (IsLikelyListItem(line, out var listContent))
            {
                result.AppendLine($"- {listContent}");
                inList = true;
                continue;
            }

            // Regular paragraph
            result.AppendLine(line);
        }

        return result.ToString().Trim();
    }

    private static bool IsLikelyHeader(string line)
    {
        // Short lines in all caps
        if (line.Length < 60 && line == line.ToUpperInvariant() && line.Any(char.IsLetter))
            return true;

        // Lines ending with colon that are short
        if (line.EndsWith(':') && line.Length < 50)
            return true;

        // Chapter/Section patterns
        if (line.StartsWith("Chapter", StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith("Section", StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith("Part", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static bool IsLikelyListItem(string line, out string content)
    {
        content = line;

        // Numbered list: "1." "1)" "a." "a)"
        if (line.Length > 2)
        {
            var firstTwo = line[..2];
            if (char.IsDigit(line[0]) && (line[1] == '.' || line[1] == ')'))
            {
                content = line[2..].Trim();
                return true;
            }
            if (char.IsLetter(line[0]) && (line[1] == '.' || line[1] == ')'))
            {
                content = line[2..].Trim();
                return true;
            }
        }

        // Bullet points
        if (line.StartsWith("- ") || line.StartsWith("• ") || line.StartsWith("* "))
        {
            content = line[2..].Trim();
            return true;
        }

        return false;
    }

    private static string ToTitleCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        // If all caps, convert to title case
        if (text == text.ToUpperInvariant())
        {
            var words = text.ToLowerInvariant().Split(' ');
            return string.Join(' ', words.Select(w =>
                w.Length > 0 ? char.ToUpperInvariant(w[0]) + w[1..] : w));
        }

        return text;
    }
}
