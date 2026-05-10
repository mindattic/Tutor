using System.Text;
using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class HtmlBookParserTests
{
    private readonly HtmlBookParser sut = new();

    [Fact]
    public void SupportedExtensions_IncludesHtmlVariants()
    {
        Assert.Contains(".html", sut.SupportedExtensions);
        Assert.Contains(".htm", sut.SupportedExtensions);
        Assert.Contains(".xhtml", sut.SupportedExtensions);
    }

    [Fact]
    public async Task ParseAsync_ExtractsBodyText()
    {
        const string html = """
        <!DOCTYPE html>
        <html><body>
            <p>The quick brown fox jumps over the lazy dog.</p>
        </body></html>
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

        var book = await sut.ParseAsync(stream, "page.html");

        Assert.Contains("quick brown fox", book.PlainText);
        Assert.Equal("html", book.SourceFormat);
    }

    [Fact]
    public async Task ParseAsync_StripsScriptAndStyle_OnFallbackPath()
    {
        // Tiny page without enough content for SmartReader to claim "readable" —
        // forces the AngleSharp fallback so we can verify it strips noise tags.
        const string html = """
        <html><head><style>.a{color:red}</style></head>
        <body><script>alert('x')</script><div>Visible content here.</div></body></html>
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

        var book = await sut.ParseAsync(stream, "tiny.html");

        Assert.DoesNotContain("alert", book.PlainText);
        Assert.DoesNotContain("color:red", book.PlainText);
        Assert.Contains("Visible content", book.PlainText);
    }

    [Fact]
    public async Task ParseAsync_FallsBackToWholePageWhenNotReadable_AndAddsWarning()
    {
        // SmartReader needs substantial content; an essentially-empty body falls
        // back to whole-page extraction with a warning explaining why.
        const string html = "<html><body><span>hi</span></body></html>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

        var book = await sut.ParseAsync(stream, "stub.html");

        Assert.NotEmpty(book.Warnings);
        Assert.Contains(book.Warnings, w =>
            w.Contains("SmartReader", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ParseAsync_DefaultsTitleFromFileName()
    {
        const string html = "<html><body><span>x</span></body></html>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

        var book = await sut.ParseAsync(stream, "ChapterOne.html");

        // Title may be overridden by SmartReader on a readable article, but for
        // this stub it should fall through to the file-name default.
        Assert.Equal("ChapterOne", book.Title);
    }
}
