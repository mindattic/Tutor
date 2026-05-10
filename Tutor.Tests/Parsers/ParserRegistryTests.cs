using System.Text;
using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class ParserRegistryTests
{
    private static ParserRegistry CreateDefault()
        => new(new IBookParser[] { new TxtBookParser(), new HtmlBookParser() });

    [Test]
    public void CanHandle_KnownExtension_True()
    {
        var registry = CreateDefault();
        Assert.That(registry.CanHandle("book.txt"), Is.True);
        Assert.That(registry.CanHandle("page.html"), Is.True);
    }

    [Test]
    public void CanHandle_UnknownExtension_False()
    {
        var registry = CreateDefault();
        Assert.That(registry.CanHandle("book.docx"), Is.False);
        Assert.That(registry.CanHandle("noext"), Is.False);
    }

    [Test]
    public void CanHandle_IsCaseInsensitive()
    {
        var registry = CreateDefault();
        Assert.That(registry.CanHandle("BOOK.TXT"), Is.True);
        Assert.That(registry.CanHandle("Page.HTML"), Is.True);
    }

    [Test]
    public void Resolve_ReturnsCorrectParser()
    {
        var registry = CreateDefault();
        Assert.That(registry.Resolve("a.md"), Is.InstanceOf<TxtBookParser>());
        Assert.That(registry.Resolve("b.HTM"), Is.InstanceOf<HtmlBookParser>());
    }

    [Test]
    public void Resolve_UnknownExtension_ReturnsNull()
    {
        var registry = CreateDefault();
        Assert.That(registry.Resolve("a.docx"), Is.Null);
    }

    [Test]
    public void SupportedExtensions_AggregatesAllParsers()
    {
        var registry = CreateDefault();
        Assert.That(registry.SupportedExtensions, Does.Contain(".txt"));
        Assert.That(registry.SupportedExtensions, Does.Contain(".md"));
        Assert.That(registry.SupportedExtensions, Does.Contain(".html"));
    }

    [Test]
    public async Task ParseAsync_FromFile_RoundTripsTextContent()
    {
        var registry = CreateDefault();
        var path = Path.Combine(Path.GetTempPath(), $"tutor-test-{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(path, "hello-from-disk", Encoding.UTF8);
            var book = await registry.ParseAsync(path);
            Assert.That(book.PlainText, Is.EqualTo("hello-from-disk"));
        }
        finally
        {
            try { File.Delete(path); } catch { }
        }
    }

    [Test]
    public async Task ParseAsync_UnknownExtension_ThrowsNotSupported()
    {
        var registry = CreateDefault();
        var path = Path.Combine(Path.GetTempPath(), $"tutor-test-{Guid.NewGuid():N}.xyz");
        try
        {
            await File.WriteAllTextAsync(path, "n/a");
            Assert.ThrowsAsync<NotSupportedException>(
                () => registry.ParseAsync(path));
        }
        finally
        {
            try { File.Delete(path); } catch { }
        }
    }

    [Test]
    public void LastRegisteredParser_WinsForOverlappingExtension()
    {
        var first = new StubParser(new[] { ".x" }, "first");
        var second = new StubParser(new[] { ".x" }, "second");
        var registry = new ParserRegistry(new IBookParser[] { first, second });

        Assert.That(registry.Resolve("a.x"), Is.SameAs(second));
    }

    private sealed class StubParser : IBookParser
    {
        public StubParser(IReadOnlyCollection<string> exts, string id)
        {
            SupportedExtensions = exts;
            Id = id;
        }

        public string Id { get; }
        public IReadOnlyCollection<string> SupportedExtensions { get; }

        public Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
            => Task.FromResult(new ExtractedBook(PlainText: Id));
    }
}
