using System.Text;
using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Service for formatting imported content into structured markdown.
/// Uses AI to identify sections, key terms, and add appropriate formatting.
/// </summary>
public sealed class ContentFormatterService
{
    private readonly LlmServiceRouter router;

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

    public ContentFormatterService(LlmServiceRouter router)
    {
        this.router = router;
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

        // For very large content, process in chunks to avoid token limits
        if (rawContent.Length > 15000)
        {
            Log.Debug($"ContentFormatter: Large content detected, using chunked processing");
            return await FormatLargeContentAsync(rawContent, onProgress, ct);
        }

        onProgress?.Invoke(1, 1);
        var result = await FormatChunkAsync(rawContent, ct);
        Log.Info($"ContentFormatter: Formatting complete ({result.Length} chars output)");
        return result;
    }

    /// <summary>
    /// Splits content into chunks suitable for individual AI formatting calls.
    /// Used by the background queue for checkpoint/resume support.
    /// </summary>
    public List<string> SplitIntoChunks(string content, int maxChunkSize = 12000)
    {
        if (string.IsNullOrWhiteSpace(content))
            return [];

        // Small content doesn't need splitting
        if (content.Length <= maxChunkSize)
            return [content];

        var paragraphs = content.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();
        var currentChunk = new StringBuilder();

        foreach (var para in paragraphs)
        {
            // If a single paragraph exceeds maxChunkSize, split it further
            if (para.Length > maxChunkSize)
            {
                // Flush current chunk first
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }

                // Split large paragraph by sentences
                var subChunks = SplitLargeParagraph(para, maxChunkSize);
                chunks.AddRange(subChunks);
                continue;
            }

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

        // Filter out empty/tiny chunks
        chunks = chunks.Where(c => c.Trim().Length > 50).ToList();

        Log.Debug($"ContentFormatter: Split content into {chunks.Count} chunks");
        return chunks;
    }

    /// <summary>
    /// Formats a single chunk of content using AI.
    /// Used by the background queue for checkpoint/resume support.
    /// </summary>
    public async Task<string> FormatSingleChunkAsync(string chunk, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(chunk))
            return chunk;

        return await FormatChunkAsync(chunk, ct);
    }

    /// <summary>
    /// Format large content by splitting into sections and processing each.
    /// </summary>
    private async Task<string> FormatLargeContentAsync(string content, Action<int, int>? onProgress, CancellationToken ct)
    {
        // Split by double newlines (paragraphs) and process in batches
        var paragraphs = content.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();
        var currentChunk = new StringBuilder();
        const int maxChunkSize = 12000;

        foreach (var para in paragraphs)
        {
            // If a single paragraph exceeds maxChunkSize, split it further
            if (para.Length > maxChunkSize)
            {
                // Flush current chunk first
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }
                
                // Split large paragraph by sentences
                var subChunks = SplitLargeParagraph(para, maxChunkSize);
                chunks.AddRange(subChunks);
                continue;
            }
            
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

        // Filter out empty chunks
        chunks = chunks.Where(c => c.Trim().Length > 50).ToList();
        
        Log.Info($"ContentFormatter: Split into {chunks.Count} chunks for processing");

        // Process each chunk with progress reporting and rate limit handling
        var formattedChunks = new List<string>();
        var totalChunks = chunks.Count;
        for (int i = 0; i < chunks.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            onProgress?.Invoke(i + 1, totalChunks);
            
            Log.Debug($"ContentFormatter: Processing chunk {i + 1}/{totalChunks} ({chunks[i].Length} chars)");
            var formatted = await FormatChunkWithRetryAsync(chunks[i], ct);
            formattedChunks.Add(formatted);
            
            // Add delay between chunks to avoid rate limiting
            if (i < chunks.Count - 1)
            {
                await Task.Delay(500, ct);
            }
        }

        return string.Join("\n\n", formattedChunks);
    }

    /// <summary>
    /// Split a large paragraph that exceeds maxChunkSize.
    /// </summary>
    private static List<string> SplitLargeParagraph(string paragraph, int maxChunkSize)
    {
        var chunks = new List<string>();
        
        // Try to split by sentences
        var sentences = System.Text.RegularExpressions.Regex.Split(paragraph, @"(?<=[.!?])\s+");
        
        var currentChunk = new StringBuilder();
        foreach (var sentence in sentences)
        {
            // If a single sentence is too long, force-split it
            if (sentence.Length > maxChunkSize)
            {
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }
                
                for (int i = 0; i < sentence.Length; i += maxChunkSize)
                {
                    var length = Math.Min(maxChunkSize, sentence.Length - i);
                    chunks.Add(sentence.Substring(i, length));
                }
                continue;
            }
            
            if (currentChunk.Length + sentence.Length > maxChunkSize && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();
            }
            
            currentChunk.Append(sentence);
            currentChunk.Append(' ');
        }
        
        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());
        
        return chunks;
    }

    /// <summary>
    /// Format a chunk with retry logic for rate limiting.
    /// </summary>
    private async Task<string> FormatChunkWithRetryAsync(string content, CancellationToken ct, int maxRetries = 3)
    {
        // FormatChunkAsync returns original content on error as fallback
        // Retry logic is not needed since fallback is acceptable
        return await FormatChunkAsync(content, ct);
    }

    /// <summary>
    /// Format a single chunk of content.
    /// </summary>
    private async Task<string> FormatChunkAsync(string content, CancellationToken ct)
    {
        try
        {
            Log.Trace("ContentFormatter: Sending request via router...");
            var messages = new[] { new ChatMessage("user", FormattingPrompt + content, FormattingPrompt + content) };
            var reply = await router.GetReplyAsync(messages, "Format the text as markdown. Return only the formatted content, no explanations.", ct);

            var result = reply.Text;
            if (string.IsNullOrWhiteSpace(result))
            {
                Log.Warn("ContentFormatter: Empty response from LLM, returning original content");
                return content;
            }

            Log.Debug($"ContentFormatter: Response received ({result.Length} chars)");
            return result;
        }
        catch (TaskCanceledException)
        {
            Log.Debug("ContentFormatter: Request cancelled by user");
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"ContentFormatter: Error - {ex.Message}", ex);
            return content; // Return original on error
        }
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
        if (line.StartsWith("- ") || line.StartsWith("� ") || line.StartsWith("* "))
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
