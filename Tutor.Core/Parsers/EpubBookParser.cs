using System.Text;
using AngleSharp;
using VersOne.Epub;

namespace Tutor.Core.Parsers;

// EPUB 2 + 3 via VersOne.Epub. We concatenate every reading-order chapter's
// text content (HTML stripped) so the AI pipeline sees the book as a single
// continuous document — same shape it expects from PG plaintext.
public sealed class EpubBookParser : IBookParser
{
    public IReadOnlyCollection<string> SupportedExtensions { get; } = new[] { ".epub" };

    public async Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        // VersOne.Epub doesn't expose a stream-based reader for the full book in
        // 3.x, so we materialise to a temp file and let it open from disk.
        var tempPath = Path.Combine(Path.GetTempPath(), $"tutor-epub-{Guid.NewGuid():N}.epub");
        try
        {
            await using (var temp = File.Create(tempPath))
            {
                await input.CopyToAsync(temp, ct);
            }

            var book = await EpubReader.ReadBookAsync(tempPath);

            var sb = new StringBuilder();
            foreach (var chapter in book.ReadingOrder)
            {
                ct.ThrowIfCancellationRequested();
                var html = chapter.Content;
                if (string.IsNullOrWhiteSpace(html)) continue;
                sb.AppendLine(HtmlToText(html));
                sb.AppendLine();
            }

            var title = book.Title ?? Path.GetFileNameWithoutExtension(fileName);
            var author = book.Author ?? string.Join(", ", book.AuthorList ?? new List<string>());
            var description = book.Schema.Package.Metadata.Descriptions?.FirstOrDefault()?.Description ?? "";

            return new ExtractedBook(
                PlainText: sb.ToString().Trim(),
                Title: title,
                Author: author,
                Description: description,
                SourceFormat: "epub");
        }
        finally
        {
            try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { /* best effort */ }
        }
    }

    private static string HtmlToText(string html)
    {
        var ctx = AngleSharp.BrowsingContext.New(AngleSharp.Configuration.Default);
        var doc = ctx.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();
        foreach (var sel in new[] { "script", "style" })
        {
            foreach (var el in doc.QuerySelectorAll(sel))
                el.Remove();
        }
        return doc.Body?.TextContent.Trim() ?? "";
    }
}
