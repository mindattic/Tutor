using System.Text;
using AngleSharp;

namespace Tutor.Ingestion.Parsers;

// HTML extraction. Strategy:
//   1. Try SmartReader (Mozilla Readability port) — extracts the "main article"
//      and returns clean text. Best for blog posts / Wikipedia / news.
//   2. If SmartReader can't find an article (e.g., a Project Gutenberg HTML
//      edition with no <article> tag), fall back to AngleSharp text extraction
//      after stripping <script>, <style>, <nav>, <header>, <footer>.
public sealed class HtmlBookParser : IBookParser
{
    private readonly ParserLimits limits;

    public HtmlBookParser(ParserLimits? limits = null)
    {
        this.limits = limits ?? ParserLimits.Unlimited;
    }

    public IReadOnlyCollection<string> SupportedExtensions { get; } = new[] { ".html", ".htm", ".xhtml" };

    public async Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        // SmartReader needs the raw HTML string and a URI. Use a synthetic
        // file:// URI built from the filename so relative-link logic doesn't crash.
        using var sr = new StreamReader(input, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var html = await sr.ReadToEndAsync(ct);
        limits.CheckChars(html.Length, "HTML page");
        var pseudoUri = new Uri($"file:///{Uri.EscapeDataString(fileName)}");

        var warnings = new List<string>();
        string title = Path.GetFileNameWithoutExtension(fileName);
        string author = "";
        string body;

        try
        {
            var reader = new SmartReader.Reader(pseudoUri.ToString(), html);
            var article = reader.GetArticle();
            if (article.IsReadable && !string.IsNullOrWhiteSpace(article.TextContent))
            {
                body = article.TextContent.Trim();
                if (!string.IsNullOrWhiteSpace(article.Title)) title = article.Title;
                if (!string.IsNullOrWhiteSpace(article.Author)) author = article.Author;
            }
            else
            {
                warnings.Add("SmartReader could not isolate a main article; falling back to whole-page text extraction.");
                body = ExtractWholePageText(html);
            }
        }
        catch (Exception ex)
        {
            warnings.Add($"SmartReader failed ({ex.Message}); falling back to whole-page text extraction.");
            body = ExtractWholePageText(html);
        }

        limits.CheckChars(body.Length, "HTML page");

        return new ExtractedBook(
            PlainText: body,
            Title: title,
            Author: author,
            SourceFormat: "html",
            Warnings: warnings)
        {
            Sections = SectionsByHeading(html),
        };
    }

    // Cuts the page at h1-h6 headings so a passage can be located by the heading above it. Best effort: any
    // failure yields no sections (the whole page is then one passage located by the title).
    private static IReadOnlyList<ExtractedSection> SectionsByHeading(string html)
    {
        try
        {
            var ctx = AngleSharp.BrowsingContext.New(AngleSharp.Configuration.Default);
            var doc = ctx.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();
            foreach (var sel in new[] { "script", "style", "nav", "header", "footer", "noscript", "form", "iframe" })
                foreach (var el in doc.QuerySelectorAll(sel).ToList())
                    el.Remove();

            var sections = new List<ExtractedSection>();
            var current = new StringBuilder();
            string label = "Start of page";

            void Flush()
            {
                var text = CollapseWhitespace(current.ToString());
                if (text.Length > 0) sections.Add(new ExtractedSection(label, text));
                current.Clear();
            }

            void Walk(AngleSharp.Dom.INode node)
            {
                foreach (var child in node.ChildNodes)
                {
                    if (child is AngleSharp.Dom.IText t) { current.Append(t.Data); continue; }
                    if (child is not AngleSharp.Dom.IElement e) continue;
                    var name = e.LocalName;
                    if (name.Length == 2 && name[0] == 'h' && name[1] >= '1' && name[1] <= '6')
                    {
                        Flush();
                        var heading = CollapseWhitespace(e.TextContent);
                        if (heading.Length > 0) label = heading.Length > 120 ? heading[..120] : heading;
                        continue;
                    }
                    Walk(e);
                    if (name is "p" or "div" or "li" or "br" or "tr" or "section" or "pre" or "blockquote")
                        current.Append('\n');
                }
            }

            Walk((AngleSharp.Dom.INode?)doc.Body ?? doc.DocumentElement);
            Flush();
            return sections.Count > 1 ? sections : [];
        }
        catch
        {
            return [];
        }
    }

    private static string ExtractWholePageText(string html)
    {
        var ctx = AngleSharp.BrowsingContext.New(AngleSharp.Configuration.Default);
        var doc = ctx.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();

        // Strip elements that almost never carry book content.
        foreach (var sel in new[] { "script", "style", "nav", "header", "footer", "noscript", "form", "iframe" })
        {
            foreach (var el in doc.QuerySelectorAll(sel))
                el.Remove();
        }

        var text = doc.Body?.TextContent ?? doc.DocumentElement.TextContent ?? "";
        return CollapseWhitespace(text);
    }

    private static string CollapseWhitespace(string text)
    {
        var sb = new StringBuilder(text.Length);
        var lastWasNewline = false;
        var lastWasSpace = true;
        foreach (var c in text)
        {
            if (c == '\r') continue;
            if (c == '\n')
            {
                if (!lastWasNewline) sb.Append('\n');
                lastWasNewline = true;
                lastWasSpace = true;
                continue;
            }
            if (char.IsWhiteSpace(c))
            {
                if (!lastWasSpace) sb.Append(' ');
                lastWasSpace = true;
                continue;
            }
            sb.Append(c);
            lastWasNewline = false;
            lastWasSpace = false;
        }
        return sb.ToString().Trim();
    }
}
