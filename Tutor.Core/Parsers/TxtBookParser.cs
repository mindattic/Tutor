using System.Text;

namespace Tutor.Core.Parsers;

// Plain-text passthrough. Auto-detects UTF-8 with/without BOM and falls back
// to the system default for older files.
public sealed class TxtBookParser : IBookParser
{
    public IReadOnlyCollection<string> SupportedExtensions { get; } =
        new[] { ".txt", ".md", ".markdown", ".text", ".log" };

    public async Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        using var reader = new StreamReader(input, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var text = await reader.ReadToEndAsync(ct);
        return new ExtractedBook(
            PlainText: text,
            Title: Path.GetFileNameWithoutExtension(fileName),
            SourceFormat: Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant());
    }
}
