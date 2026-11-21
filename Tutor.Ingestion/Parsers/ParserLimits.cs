using System.IO.Compression;

namespace Tutor.Ingestion.Parsers;

/// <summary>
/// Ceilings a host puts on untrusted documents. The defaults are unlimited so existing behaviour is unchanged;
/// a host that parses uploads passes real numbers. A document that goes over a ceiling is refused with a
/// <see cref="DocumentLimitException"/> whose message is safe to show the user.
/// </summary>
/// <param name="MaxPages">Most PDF pages parsed.</param>
/// <param name="MaxChars">Most characters of extracted text.</param>
/// <param name="MaxUncompressedBytes">Most bytes a zip-based file (DOCX, EPUB) may expand to; stops zip bombs.</param>
public sealed record ParserLimits(
    int MaxPages = int.MaxValue,
    long MaxChars = long.MaxValue,
    long MaxUncompressedBytes = long.MaxValue)
{
    public static ParserLimits Unlimited { get; } = new();

    internal void CheckChars(long chars, string what)
    {
        if (chars > MaxChars)
            throw new DocumentLimitException($"The {what} has more than {MaxChars:N0} characters of text, over the limit.");
    }

    /// <summary>Refuses a seekable zip whose entries add up to more than <see cref="MaxUncompressedBytes"/>.</summary>
    internal void CheckZip(Stream seekable, string what)
    {
        if (MaxUncompressedBytes == long.MaxValue) return;
        var start = seekable.Position;
        try
        {
            using var zip = new ZipArchive(seekable, ZipArchiveMode.Read, leaveOpen: true);
            long total = 0;
            foreach (var entry in zip.Entries)
            {
                total += entry.Length;
                if (total > MaxUncompressedBytes)
                    throw new DocumentLimitException($"The {what} expands to more than {MaxUncompressedBytes / (1024 * 1024):N0} MB, over the limit.");
            }
        }
        catch (InvalidDataException ex)
        {
            throw new InvalidDataException($"The {what} is not a valid archive: {ex.Message}", ex);
        }
        finally
        {
            seekable.Position = start;
        }
    }
}

/// <summary>A document was refused because it is over a <see cref="ParserLimits"/> ceiling.</summary>
public sealed class DocumentLimitException : InvalidOperationException
{
    public DocumentLimitException(string message) : base(message) { }
}
