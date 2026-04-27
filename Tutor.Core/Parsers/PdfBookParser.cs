using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Tutor.Core.Parsers;

// PDF text extraction via PdfPig. PdfPig is pure-managed and Apache 2.0 — iText
// is more capable but AGPL, so we stay on PdfPig. Scanned-image PDFs will yield
// little/no text — that's a documented non-goal (no OCR in scope).
public sealed class PdfBookParser : IBookParser
{
    public IReadOnlyCollection<string> SupportedExtensions { get; } = new[] { ".pdf" };

    public Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        // PdfPig requires a seekable stream. Buffer if the source isn't.
        Stream pdfStream = input.CanSeek ? input : BufferToMemory(input);

        var sb = new StringBuilder();
        var warnings = new List<string>();
        string title = Path.GetFileNameWithoutExtension(fileName);
        string author = "";

        using (var doc = PdfDocument.Open(pdfStream))
        {
            // Information dictionary metadata when available.
            try
            {
                if (!string.IsNullOrWhiteSpace(doc.Information?.Title)) title = doc.Information!.Title!;
                if (!string.IsNullOrWhiteSpace(doc.Information?.Author)) author = doc.Information!.Author!;
            }
            catch { /* metadata is optional */ }

            var pageCount = doc.NumberOfPages;
            for (int i = 1; i <= pageCount; i++)
            {
                ct.ThrowIfCancellationRequested();
                Page page;
                try
                {
                    page = doc.GetPage(i);
                }
                catch (Exception ex)
                {
                    warnings.Add($"Page {i}: failed to read ({ex.Message}); skipped.");
                    continue;
                }

                // ContentOrderTextExtractor preserves reading order better than the
                // raw GetText() call, which streams text in PDF-content-stream order.
                string pageText;
                try
                {
                    pageText = ContentOrderTextExtractor.GetText(page) ?? "";
                }
                catch
                {
                    pageText = page.Text ?? "";
                }

                if (!string.IsNullOrWhiteSpace(pageText))
                {
                    sb.AppendLine(pageText);
                    sb.AppendLine();
                }
            }

            if (sb.Length == 0)
            {
                warnings.Add("No extractable text found. The PDF may be image-only (scanned) — OCR is not supported.");
            }
        }

        if (pdfStream != input) pdfStream.Dispose();

        return Task.FromResult(new ExtractedBook(
            PlainText: sb.ToString().Trim(),
            Title: title,
            Author: author,
            SourceFormat: "pdf",
            Warnings: warnings));
    }

    private static MemoryStream BufferToMemory(Stream s)
    {
        var ms = new MemoryStream();
        s.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }
}
