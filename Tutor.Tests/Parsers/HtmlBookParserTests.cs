using System.Text;
using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class HtmlBookParserTests
{
    private readonly HtmlBookParser sut = new();

    [Test]
    public void SupportedExtensions_IncludesHtmlVariants()
    {
        Assert.That(sut.SupportedExtensions, Does.Contain(".html"));
        Assert.That(sut.SupportedExtensions, Does.Contain(".htm"));
        Assert.That(sut.SupportedExtensions, Does.Contain(".xhtml"));
    }

    [Test]
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

        Assert.That(book.PlainText, Does.Contain("quick brown fox"));
        Assert.That(book.SourceFormat, Is.EqualTo("html"));
    }

    [Test]
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

        Assert.That(book.PlainText, Does.Not.Contain("alert"));
        Assert.That(book.PlainText, Does.Not.Contain("color:red"));
        Assert.That(book.PlainText, Does.Contain("Visible content"));
    }

    [Test]
    public async Task ParseAsync_FallsBackToWholePageWhenNotReadable_AndAddsWarning()
    {
        // SmartReader needs substantial content; an essentially-empty body falls
        // back to whole-page extraction with a warning explaining why.
        const string html = "<html><body><span>hi</span></body></html>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

        var book = await sut.ParseAsync(stream, "stub.html");

        Assert.That(book.Warnings, Is.Not.Empty);
        Assert.That(book.Warnings, Has.Some.Matches<string>(w =>
            w.Contains("SmartReader", StringComparison.OrdinalIgnoreCase)));
    }

    [Test]
    public async Task ParseAsync_DefaultsTitleFromFileName()
    {
        const string html = "<html><body><span>x</span></body></html>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

        var book = await sut.ParseAsync(stream, "ChapterOne.html");

        // Title may be overridden by SmartReader on a readable article, but for
        // this stub it should fall through to the file-name default.
        Assert.That(book.Title, Is.EqualTo("ChapterOne"));
    }
}
