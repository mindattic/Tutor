using Tutor.Core.Services.Ocr;

namespace Tutor.Tests.Services;

public class NoOpPdfOcrServiceTests
{
    [Test]
    public void IsAvailable_ReportsFalse()
    {
        var ocr = new NoOpPdfOcrService();
        Assert.That(ocr.IsAvailable, Is.False);
    }

    [Test]
    public async Task OcrImageAsync_ReturnsEmpty_ForAnyInput()
    {
        var ocr = new NoOpPdfOcrService();

        Assert.That(await ocr.OcrImageAsync(Array.Empty<byte>()), Is.EqualTo(""));
        Assert.That(await ocr.OcrImageAsync(new byte[] { 0x89, 0x50, 0x4E, 0x47 }), Is.EqualTo(""));
    }
}
