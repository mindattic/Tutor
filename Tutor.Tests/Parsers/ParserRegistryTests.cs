using System.Text;
using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class ParserRegistryTests
{
    private static ParserRegistry CreateDefault()
        => new(new IBookParser[] { new TxtBookParser(), new HtmlBookParser() });

    [Fact]
    public void CanHandle_KnownExtension_True()
    {
        var registry = CreateDefault();
        Assert.True(registry.CanHandle("book.txt"));
        Assert.True(registry.CanHandle("page.html"));
    }

    [Fact]
    public void CanHandle_UnknownExtension_False()
    {
        var registry = CreateDefault();
        Assert.False(registry.CanHandle("book.docx"));
        Assert.False(registry.CanHandle("noext"));
    }

    [Fact]
    public void CanHandle_IsCaseInsensitive()
    {
        var registry = CreateDefault();
        Assert.True(registry.CanHandle("BOOK.TXT"));
        Assert.True(registry.CanHandle("Page.HTML"));
    }

    [Fact]
    public void Resolve_ReturnsCorrectParser()
    {
        var registry = CreateDefault();
        Assert.IsType<TxtBookParser>(registry.Resolve("a.md"));
        Assert.IsType<HtmlBookParser>(registry.Resolve("b.HTM"));
    }

    [Fact]
    public void Resolve_UnknownExtension_ReturnsNull()
    {
        var registry = CreateDefault();
        Assert.Null(registry.Resolve("a.docx"));
    }

    [Fact]
    public void SupportedExtensions_AggregatesAllParsers()
    {
        var registry = CreateDefault();
        Assert.Contains(".txt", registry.SupportedExtensions);
        Assert.Contains(".md", registry.SupportedExtensions);
        Assert.Contains(".html", registry.SupportedExtensions);
    }

    [Fact]
    public async Task ParseAsync_FromFile_RoundTripsTextContent()
    {
        var registry = CreateDefault();
        var path = Path.Combine(Path.GetTempPath(), $"tutor-test-{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(path, "hello-from-disk", Encoding.UTF8);
            var book = await registry.ParseAsync(path);
            Assert.Equal("hello-from-disk", book.PlainText);
        }
        finally
        {
            try { File.Delete(path); } catch { }
        }
    }

    [Fact]
    public async Task ParseAsync_UnknownExtension_ThrowsNotSupported()
    {
        var registry = CreateDefault();
        var path = Path.Combine(Path.GetTempPath(), $"tutor-test-{Guid.NewGuid():N}.xyz");
        try
        {
            await File.WriteAllTextAsync(path, "n/a");
            await Assert.ThrowsAsync<NotSupportedException>(
                () => registry.ParseAsync(path));
        }
        finally
        {
            try { File.Delete(path); } catch { }
        }
    }

    [Fact]
    public void LastRegisteredParser_WinsForOverlappingExtension()
    {
        var first = new StubParser(new[] { ".x" }, "first");
        var second = new StubParser(new[] { ".x" }, "second");
        var registry = new ParserRegistry(new IBookParser[] { first, second });

        Assert.Same(second, registry.Resolve("a.x"));
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
