using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Tutor.Core.Parsers;

// DOCX (Office Open XML) via DocumentFormat.OpenXml — Microsoft's official
// library. We walk the body, emitting paragraphs separated by blank lines.
// Tables are flattened cell-by-cell; that's lossy but keeps the AI pipeline's
// expectation of plain prose intact.
public sealed class DocxBookParser : IBookParser
{
    public IReadOnlyCollection<string> SupportedExtensions { get; } = new[] { ".docx" };

    public Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        // OpenXml requires a seekable stream that it can read AND write to (it
        // opens read-only by passing false, but still needs seek). Buffer to be safe.
        Stream docStream = input.CanSeek ? input : BufferToMemory(input);

        using var doc = WordprocessingDocument.Open(docStream, false);
        var body = doc.MainDocumentPart?.Document?.Body;

        var sb = new StringBuilder();
        string title = Path.GetFileNameWithoutExtension(fileName);
        string author = "";

        // Core properties may carry better metadata.
        try
        {
            var core = doc.PackageProperties;
            if (!string.IsNullOrWhiteSpace(core.Title)) title = core.Title!;
            if (!string.IsNullOrWhiteSpace(core.Creator)) author = core.Creator!;
        }
        catch { /* optional */ }

        if (body != null)
        {
            foreach (var element in body.Elements())
            {
                ct.ThrowIfCancellationRequested();
                AppendElementText(element, sb);
            }
        }

        if (docStream != input) docStream.Dispose();

        return Task.FromResult(new ExtractedBook(
            PlainText: sb.ToString().Trim(),
            Title: title,
            Author: author,
            SourceFormat: "docx"));
    }

    private static void AppendElementText(DocumentFormat.OpenXml.OpenXmlElement element, StringBuilder sb)
    {
        switch (element)
        {
            case Paragraph p:
                var text = p.InnerText;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    sb.AppendLine(text);
                }
                sb.AppendLine();
                break;

            case Table t:
                foreach (var row in t.Elements<TableRow>())
                {
                    var cells = row.Elements<TableCell>().Select(c => c.InnerText.Trim()).Where(s => s.Length > 0);
                    sb.AppendLine(string.Join(" | ", cells));
                }
                sb.AppendLine();
                break;

            default:
                // Section breaks, bookmarks, etc. — emit raw text if any.
                var inner = element.InnerText;
                if (!string.IsNullOrWhiteSpace(inner))
                {
                    sb.AppendLine(inner);
                    sb.AppendLine();
                }
                break;
        }
    }

    private static MemoryStream BufferToMemory(Stream s)
    {
        var ms = new MemoryStream();
        s.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }
}
