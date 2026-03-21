using System.Text;
using System.Text.RegularExpressions;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Service for splitting text content into chunks suitable for embedding.
/// Uses semantic boundaries (paragraphs, sentences) when possible.
/// </summary>
public sealed partial class ChunkingService
{
    // Target ~500 tokens per chunk. Rough estimate: 1 token ? 4 characters for English
    private const int TargetChunkSize = 2000; // characters (~500 tokens)
    private const int MaxChunkSize = 2500;    // absolute max
    private const int MinChunkSize = 200;     // don't create tiny chunks
    private const int OverlapSize = 200;      // overlap between chunks for context continuity

    /// <summary>
    /// Split content into chunks with semantic boundaries and overlap.
    /// </summary>
    public List<string> ChunkContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return [];

        // Normalize whitespace
        content = NormalizeWhitespace(content);

        // If content is small enough, return as single chunk
        if (content.Length <= MaxChunkSize)
            return [content];

        var chunks = new List<string>();
        var paragraphs = SplitIntoParagraphs(content);
        var currentChunk = new StringBuilder();

        foreach (var paragraph in paragraphs)
        {
            // If adding this paragraph exceeds target, finalize current chunk
            if (currentChunk.Length > 0 && currentChunk.Length + paragraph.Length > TargetChunkSize)
            {
                chunks.Add(currentChunk.ToString().Trim());
                
                // Start new chunk with overlap from end of previous
                var overlap = GetOverlapText(currentChunk.ToString());
                currentChunk.Clear();
                if (!string.IsNullOrWhiteSpace(overlap))
                {
                    currentChunk.Append(overlap);
                    currentChunk.Append(' ');
                }
            }

            // If single paragraph is too large, split by sentences
            if (paragraph.Length > MaxChunkSize)
            {
                var sentences = SplitIntoSentences(paragraph);
                foreach (var sentence in sentences)
                {
                    if (currentChunk.Length > 0 && currentChunk.Length + sentence.Length > TargetChunkSize)
                    {
                        chunks.Add(currentChunk.ToString().Trim());
                        var overlap = GetOverlapText(currentChunk.ToString());
                        currentChunk.Clear();
                        if (!string.IsNullOrWhiteSpace(overlap))
                        {
                            currentChunk.Append(overlap);
                            currentChunk.Append(' ');
                        }
                    }
                    currentChunk.Append(sentence);
                    currentChunk.Append(' ');
                }
            }
            else
            {
                currentChunk.Append(paragraph);
                currentChunk.Append("\n\n");
            }
        }

        // Add final chunk if it has content
        if (currentChunk.Length >= MinChunkSize)
        {
            chunks.Add(currentChunk.ToString().Trim());
        }
        else if (currentChunk.Length > 0 && chunks.Count > 0)
        {
            // Append small remainder to last chunk
            chunks[^1] = chunks[^1] + "\n\n" + currentChunk.ToString().Trim();
        }
        else if (currentChunk.Length > 0)
        {
            chunks.Add(currentChunk.ToString().Trim());
        }

        Log.Debug($"ChunkingService: Split {content.Length} chars into {chunks.Count} chunks");
        return chunks;
    }

    private static string NormalizeWhitespace(string text)
    {
        // Replace multiple newlines with double newline (paragraph break)
        text = MultipleNewlines().Replace(text, "\n\n");
        // Replace multiple spaces with single space
        text = MultipleSpaces().Replace(text, " ");
        return text.Trim();
    }

    private static List<string> SplitIntoParagraphs(string text)
    {
        return [.. text.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))];
    }

    private static List<string> SplitIntoSentences(string text)
    {
        // Split on sentence-ending punctuation followed by space or end
        var sentences = SentenceEnd().Split(text);
        return [.. sentences.Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s))];
    }

    private static string GetOverlapText(string text)
    {
        if (text.Length <= OverlapSize)
            return text;

        // Try to find a sentence boundary near the end for cleaner overlap
        // Ensure we don't try to access beyond the start of the string
        var lookbackSize = Math.Min(text.Length, OverlapSize + 100);
        var endPortion = text[^lookbackSize..];
        var sentenceMatch = LastSentenceEnd().Match(endPortion);
        
        if (sentenceMatch.Success)
        {
            return endPortion[(sentenceMatch.Index + sentenceMatch.Length)..].Trim();
        }

        // Fall back to word boundary
        var overlapStart = Math.Min(text.Length, OverlapSize);
        var words = text[^overlapStart..].Split(' ');
        return words.Length > 1 ? string.Join(' ', words[1..]) : text[^overlapStart..];
    }

    [GeneratedRegex(@"\n{2,}")]
    private static partial Regex MultipleNewlines();

    [GeneratedRegex(@" {2,}")]
    private static partial Regex MultipleSpaces();

    [GeneratedRegex(@"(?<=[.!?])\s+")]
    private static partial Regex SentenceEnd();

    [GeneratedRegex(@"[.!?]\s+")]
    private static partial Regex LastSentenceEnd();
}
