using System.Text;
using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class TxtBookParserTests
{
    private readonly TxtBookParser sut = new();

    [Test]
    public void SupportedExtensions_IncludesCommonTextExtensions()
    {
        Assert.That(sut.SupportedExtensions, Does.Contain(".txt"));
        Assert.That(sut.SupportedExtensions, Does.Contain(".md"));
        Assert.That(sut.SupportedExtensions, Does.Contain(".markdown"));
        Assert.That(sut.SupportedExtensions, Does.Contain(".text"));
        Assert.That(sut.SupportedExtensions, Does.Contain(".log"));
    }

    [Test]
    public async Task ParseAsync_ReturnsTextContent()
    {
        var content = "Hello, world!\nThis is a plain text book.";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var book = await sut.ParseAsync(stream, "sample.txt");

        Assert.That(book.PlainText, Is.EqualTo(content));
    }

    [Test]
    public async Task ParseAsync_TitleIsFileNameWithoutExtension()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("body"));

        var book = await sut.ParseAsync(stream, "MyBook.txt");

        Assert.That(book.Title, Is.EqualTo("MyBook"));
    }

    [Test]
    public async Task ParseAsync_SourceFormatIsExtensionLowercase()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("body"));

        var book = await sut.ParseAsync(stream, "Notes.MD");

        Assert.That(book.SourceFormat, Is.EqualTo("md"));
    }

    [Test]
    public async Task ParseAsync_HandlesUtf8Bom()
    {
        var bytes = new List<byte> { 0xEF, 0xBB, 0xBF };
        bytes.AddRange(Encoding.UTF8.GetBytes("café"));
        using var stream = new MemoryStream(bytes.ToArray());

        var book = await sut.ParseAsync(stream, "bom.txt");

        Assert.That(book.PlainText, Is.EqualTo("café"));
    }

    [Test]
    public async Task ParseAsync_EmptyStream_ReturnsEmptyText()
    {
        using var stream = new MemoryStream();

        var book = await sut.ParseAsync(stream, "empty.txt");

        Assert.That(book.PlainText, Is.EqualTo(""));
    }

    [Test]
    public void ParseAsync_RespectsCancellation()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("anything"));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Assert.CatchAsync matches T or any derived type — equivalent to xUnit's ThrowsAnyAsync.
        Assert.CatchAsync<OperationCanceledException>(
            () => sut.ParseAsync(stream, "x.txt", cts.Token));
    }
}
