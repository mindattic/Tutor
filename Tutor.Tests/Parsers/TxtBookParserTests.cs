using System.Text;
using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class TxtBookParserTests
{
    private readonly TxtBookParser sut = new();

    [Fact]
    public void SupportedExtensions_IncludesCommonTextExtensions()
    {
        Assert.Contains(".txt", sut.SupportedExtensions);
        Assert.Contains(".md", sut.SupportedExtensions);
        Assert.Contains(".markdown", sut.SupportedExtensions);
        Assert.Contains(".text", sut.SupportedExtensions);
        Assert.Contains(".log", sut.SupportedExtensions);
    }

    [Fact]
    public async Task ParseAsync_ReturnsTextContent()
    {
        var content = "Hello, world!\nThis is a plain text book.";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var book = await sut.ParseAsync(stream, "sample.txt");

        Assert.Equal(content, book.PlainText);
    }

    [Fact]
    public async Task ParseAsync_TitleIsFileNameWithoutExtension()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("body"));

        var book = await sut.ParseAsync(stream, "MyBook.txt");

        Assert.Equal("MyBook", book.Title);
    }

    [Fact]
    public async Task ParseAsync_SourceFormatIsExtensionLowercase()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("body"));

        var book = await sut.ParseAsync(stream, "Notes.MD");

        Assert.Equal("md", book.SourceFormat);
    }

    [Fact]
    public async Task ParseAsync_HandlesUtf8Bom()
    {
        var bytes = new List<byte> { 0xEF, 0xBB, 0xBF };
        bytes.AddRange(Encoding.UTF8.GetBytes("café"));
        using var stream = new MemoryStream(bytes.ToArray());

        var book = await sut.ParseAsync(stream, "bom.txt");

        Assert.Equal("café", book.PlainText);
    }

    [Fact]
    public async Task ParseAsync_EmptyStream_ReturnsEmptyText()
    {
        using var stream = new MemoryStream();

        var book = await sut.ParseAsync(stream, "empty.txt");

        Assert.Equal("", book.PlainText);
    }

    [Fact]
    public async Task ParseAsync_RespectsCancellation()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("anything"));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => sut.ParseAsync(stream, "x.txt", cts.Token));
    }
}
