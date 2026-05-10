using Tutor.Core.Services.Ocr;

namespace Tutor.Tests.Services;

public class NoOpPdfOcrServiceTests
{
    [Fact]
    public void IsAvailable_ReportsFalse()
    {
        var ocr = new NoOpPdfOcrService();
        Assert.False(ocr.IsAvailable);
    }

    [Fact]
    public async Task OcrImageAsync_ReturnsEmpty_ForAnyInput()
    {
        var ocr = new NoOpPdfOcrService();

        Assert.Equal("", await ocr.OcrImageAsync(Array.Empty<byte>()));
        Assert.Equal("", await ocr.OcrImageAsync(new byte[] { 0x89, 0x50, 0x4E, 0x47 }));
    }
}
