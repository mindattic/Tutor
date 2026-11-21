namespace Tutor.Ingestion.Parsers;

// Result of extracting a book from any source format. Only PlainText is
// guaranteed to be populated; the metadata fields are best-effort and the
// caller (typically the CLI's import command) should fall back to defaults
// (e.g., file name) when they're empty.
//
// Sections is the same text cut at the format's natural locations (PDF pages,
// EPUB chapters, DOCX/HTML headings) for callers that need to say where a
// passage came from. It is empty for formats with no such structure (plain text).
public sealed record ExtractedBook(
    string PlainText,
    string Title = "",
    string Author = "",
    string Description = "",
    string SourceFormat = "",
    IReadOnlyList<string>? Warnings = null)
{
    public IReadOnlyList<string> Warnings { get; init; } = Warnings ?? Array.Empty<string>();

    /// <summary>The text split at the format's own locations, in document order.</summary>
    public IReadOnlyList<ExtractedSection> Sections { get; init; } = Array.Empty<ExtractedSection>();
}

/// <summary>One located stretch of an <see cref="ExtractedBook"/>.</summary>
/// <param name="Label">Human location, e.g. "Page 3", "Chapter 2: Setup" or a heading.</param>
/// <param name="Text">The section's text (never empty).</param>
/// <param name="Page">1-based page number when the format has pages (PDF), else null.</param>
public sealed record ExtractedSection(string Label, string Text, int? Page = null);
