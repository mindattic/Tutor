namespace Tutor.Core.Parsers;

// Result of extracting a book from any source format. Only PlainText is
// guaranteed to be populated; the metadata fields are best-effort and the
// caller (typically the CLI's import command) should fall back to defaults
// (e.g., file name) when they're empty.
public sealed record ExtractedBook(
    string PlainText,
    string Title = "",
    string Author = "",
    string Description = "",
    string SourceFormat = "",
    IReadOnlyList<string>? Warnings = null)
{
    public IReadOnlyList<string> Warnings { get; init; } = Warnings ?? Array.Empty<string>();
}
