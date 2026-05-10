using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class ExtractedBookTests
{
    [Fact]
    public void Defaults_AreEmptyStringsAndNoWarnings()
    {
        var book = new ExtractedBook(PlainText: "body");

        Assert.Equal("", book.Title);
        Assert.Equal("", book.Author);
        Assert.Equal("", book.Description);
        Assert.Equal("", book.SourceFormat);
        Assert.NotNull(book.Warnings);
        Assert.Empty(book.Warnings);
    }

    [Fact]
    public void Warnings_NullArg_NormalizesToEmptyArray()
    {
        var book = new ExtractedBook(PlainText: "body", Warnings: null);

        Assert.NotNull(book.Warnings);
        Assert.Empty(book.Warnings);
    }

    [Fact]
    public void Warnings_RetainProvidedList()
    {
        var book = new ExtractedBook(
            PlainText: "body",
            Warnings: new[] { "first", "second" });

        Assert.Collection(book.Warnings,
            w => Assert.Equal("first", w),
            w => Assert.Equal("second", w));
    }
}
