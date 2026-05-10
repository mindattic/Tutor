using System.Text;
using Tutor.Core.Services.Ocr;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Tutor.Core.Parsers;

// PDF text extraction via PdfPig. PdfPig is pure-managed and Apache 2.0 — iText
// is more capable but AGPL, so we stay on PdfPig. Pages that yield little or
// no extractable text (scanned image-only PDFs, or hybrid books with embedded
// scans) get OCR'd via the injected IPdfOcrService. The default DI registration
// is TesseractPdfOcrService; tests can substitute NoOpPdfOcrService.
public sealed class PdfBookParser : IBookParser
{
    // Below this character count we assume the page is image-heavy and OCR it.
    // 50 chars catches "page 47" headers/footers without false-positiving on a
    // page of normal body text.
    private const int OcrThresholdChars = 50;

    private readonly IPdfOcrService ocr;

    public PdfBookParser(IPdfOcrService ocr)
    {
        this.ocr = ocr;
    }

    public IReadOnlyCollection<string> SupportedExtensions { get; } = new[] { ".pdf" };

    public async Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        // PdfPig requires a seekable stream. Buffer if the source isn't.
        Stream pdfStream = input.CanSeek ? input : BufferToMemory(input);

        var sb = new StringBuilder();
        var warnings = new List<string>();
        string title = Path.GetFileNameWithoutExtension(fileName);
        string author = "";
        var ocrPagesAttempted = 0;
        var ocrPagesSucceeded = 0;

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

                // Page is text-light: try OCR on each embedded image. Fall back
                // to whatever sparse text we did get if OCR is unavailable.
                if (pageText.Trim().Length < OcrThresholdChars && ocr.IsAvailable)
                {
                    ocrPagesAttempted++;
                    var ocrText = await TryOcrPageAsync(page, ct);
                    if (!string.IsNullOrWhiteSpace(ocrText))
                    {
                        ocrPagesSucceeded++;
                        pageText = ocrText;
                    }
                }

                if (!string.IsNullOrWhiteSpace(pageText))
                {
                    sb.AppendLine(pageText);
                    sb.AppendLine();
                }
            }

            if (sb.Length == 0)
            {
                if (!ocr.IsAvailable)
                    warnings.Add("No extractable text found and OCR is not available. The PDF may be image-only (scanned) — install Tesseract trained data to enable OCR.");
                else
                    warnings.Add("No extractable text found and OCR returned nothing.");
            }
            else if (ocrPagesAttempted > 0)
            {
                warnings.Add($"OCR'd {ocrPagesSucceeded}/{ocrPagesAttempted} text-light pages.");
            }
        }

        if (pdfStream != input) pdfStream.Dispose();

        return new ExtractedBook(
            PlainText: sb.ToString().Trim(),
            Title: title,
            Author: author,
            SourceFormat: "pdf",
            Warnings: warnings);
    }

    private async Task<string> TryOcrPageAsync(Page page, CancellationToken ct)
    {
        // PdfPig exposes embedded images via page.GetImages(). For scanned books
        // there's typically a single full-page image; for hybrid PDFs we OCR
        // every image and concatenate (whitespace-joined) so figure captions,
        // diagrams, and the body text all contribute.
        IEnumerable<IPdfImage> images;
        try { images = page.GetImages(); }
        catch { return ""; }

        var combined = new StringBuilder();
        foreach (var image in images)
        {
            ct.ThrowIfCancellationRequested();
            byte[]? bytes = null;
            try
            {
                if (image.TryGetPng(out var png) && png != null && png.Length > 0)
                {
                    bytes = png;
                }
                else
                {
                    // RawBytes is a Span<byte> and can't cross the await below;
                    // copy via RawMemory before the OCR call. Tesseract's Pix
                    // loader auto-detects JPEG/PNG/etc. via Leptonica magic bytes.
                    var raw = image.RawMemory.ToArray();
                    if (raw.Length > 0) bytes = raw;
                }
            }
            catch { /* unsupported encoding — skip */ }

            if (bytes == null || bytes.Length == 0) continue;

            var text = await ocr.OcrImageAsync(bytes, ct);
            if (!string.IsNullOrWhiteSpace(text))
            {
                combined.AppendLine(text);
            }
        }
        return combined.ToString().Trim();
    }

    private static MemoryStream BufferToMemory(Stream s)
    {
        var ms = new MemoryStream();
        s.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }
}
