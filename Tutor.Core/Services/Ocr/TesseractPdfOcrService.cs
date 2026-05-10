using Tesseract;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Ocr;

/// <summary>
/// Tesseract-backed OCR. Loads <c>eng.traineddata</c> from <c>tessdata/</c> next to
/// the running assembly (the path the Tutor.Core.csproj build target populates),
/// and falls back to <c>%LocalAppData%\Tutor\tessdata</c> for the case where the
/// binary was deployed without the trained data and a previous run cached it
/// there. Engine creation is deferred to first use so a missing native library
/// or traineddata file shows up as <see cref="IsAvailable"/> = false rather than
/// a startup crash.
/// </summary>
public sealed class TesseractPdfOcrService : IPdfOcrService, IDisposable
{
    private readonly object initLock = new();
    private TesseractEngine? engine;
    private bool initFailed;
    private string? resolvedTessdataDir;

    public bool IsAvailable
    {
        get
        {
            EnsureEngine();
            return engine != null;
        }
    }

    public Task<string> OcrImageAsync(byte[] imageBytes, CancellationToken ct = default)
    {
        if (imageBytes == null || imageBytes.Length == 0) return Task.FromResult("");
        EnsureEngine();
        if (engine == null) return Task.FromResult("");

        try
        {
            ct.ThrowIfCancellationRequested();
            using var pix = Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(pix);
            return Task.FromResult(page.GetText() ?? "");
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            // OCR is a fallback. Log and return empty so the caller keeps moving.
            Log.Warn($"Tesseract OCR failed on a page: {ex.Message}");
            return Task.FromResult("");
        }
    }

    private void EnsureEngine()
    {
        if (engine != null || initFailed) return;
        lock (initLock)
        {
            if (engine != null || initFailed) return;
            try
            {
                resolvedTessdataDir = ResolveTessdataDir();
                if (resolvedTessdataDir == null)
                {
                    initFailed = true;
                    Log.Warn("Tesseract OCR disabled: eng.traineddata not found. " +
                                "Build Tutor.Core (the build target downloads it) or place " +
                                "eng.traineddata in %LocalAppData%\\Tutor\\tessdata.");
                    return;
                }
                engine = new TesseractEngine(resolvedTessdataDir, "eng", EngineMode.Default);
            }
            catch (Exception ex)
            {
                initFailed = true;
                Log.Warn($"Tesseract OCR disabled — engine init failed: {ex.Message}");
            }
        }
    }

    private static string? ResolveTessdataDir()
    {
        var candidates = new List<string>
        {
            Path.Combine(AppContext.BaseDirectory, "tessdata"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor", "tessdata"),
        };
        foreach (var dir in candidates)
        {
            if (File.Exists(Path.Combine(dir, "eng.traineddata"))) return dir;
        }
        return null;
    }

    public void Dispose()
    {
        engine?.Dispose();
        engine = null;
    }
}
