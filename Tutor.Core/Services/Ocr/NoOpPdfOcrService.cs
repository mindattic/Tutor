namespace Tutor.Core.Services.Ocr;

/// <summary>
/// No-op fallback used when Tesseract isn't wired up (e.g. Tutor.Tests, or a
/// platform without native libtesseract). Reports unavailable and returns the
/// empty string for any image.
/// </summary>
public sealed class NoOpPdfOcrService : IPdfOcrService
{
    public bool IsAvailable => false;

    public Task<string> OcrImageAsync(byte[] imageBytes, CancellationToken ct = default)
        => Task.FromResult(string.Empty);
}
