using Tutor.Core.Parsers;

namespace Tutor.Tests.Parsers;

public class ExtractedBookTests
{
    [Test]
    public void Defaults_AreEmptyStringsAndNoWarnings()
    {
        var book = new ExtractedBook(PlainText: "body");

        Assert.That(book.Title, Is.EqualTo(""));
        Assert.That(book.Author, Is.EqualTo(""));
        Assert.That(book.Description, Is.EqualTo(""));
        Assert.That(book.SourceFormat, Is.EqualTo(""));
        Assert.That(book.Warnings, Is.Not.Null);
        Assert.That(book.Warnings, Is.Empty);
    }

    [Test]
    public void Warnings_NullArg_NormalizesToEmptyArray()
    {
        var book = new ExtractedBook(PlainText: "body", Warnings: null);

        Assert.That(book.Warnings, Is.Not.Null);
        Assert.That(book.Warnings, Is.Empty);
    }

    [Test]
    public void Warnings_RetainProvidedList()
    {
        var book = new ExtractedBook(
            PlainText: "body",
            Warnings: new[] { "first", "second" });

        var warnings = book.Warnings.ToList();
        Assert.That(warnings, Has.Count.EqualTo(2));
        Assert.That(warnings[0], Is.EqualTo("first"));
        Assert.That(warnings[1], Is.EqualTo("second"));
    }
}
