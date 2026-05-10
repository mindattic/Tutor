namespace Tutor.Core.Services.Ocr;

/// <summary>
/// Runs OCR on raw page-image bytes pulled from a PDF page that yielded little
/// or no extractable text. Returns the empty string when OCR is unavailable or
/// finds nothing — never throws so the surrounding parse can continue.
/// </summary>
public interface IPdfOcrService
{
    /// <summary>True when the service is configured and ready to OCR pages.</summary>
    bool IsAvailable { get; }

    /// <summary>
    /// OCRs an image (PNG/JPEG bytes preferred — implementations decode internally).
    /// Returns extracted text, or empty if the image is unrecognisable / OCR is off.
    /// </summary>
    Task<string> OcrImageAsync(byte[] imageBytes, CancellationToken ct = default);
}
