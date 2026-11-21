using Tesseract;


namespace Tutor.Ingestion.Ocr;

/// <summary>
/// Tesseract-backed OCR. Loads <c>eng.traineddata</c> from <c>tessdata/</c> next to
/// the running assembly (the package ships it and copies it to the application output),
/// and then searches the other folders the injected <see cref="IIngestionPaths"/> names.
/// Engine creation is deferred to first use so a missing native library
/// or traineddata file shows up as <see cref="IsAvailable"/> = false rather than
/// a startup crash.
/// </summary>
public sealed class TesseractPdfOcrService : IPdfOcrService, IDisposable
{
    private readonly IIngestionPaths paths;
    private readonly object initLock = new();

    /// <param name="paths">Folders searched for <c>eng.traineddata</c>; the application's <c>tessdata/</c> folder by default.</param>
    public TesseractPdfOcrService(IIngestionPaths? paths = null)
    {
        this.paths = paths ?? new DefaultIngestionPaths();
    }
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
            IngestionLog.Warn($"Tesseract OCR failed on a page: {ex.Message}");
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
                    IngestionLog.Warn("Tesseract OCR disabled: eng.traineddata not found. " +
                                "Reinstall the Tutor.Ingestion package or place " +
                                "eng.traineddata in a folder IIngestionPaths lists.");
                    return;
                }
                engine = new TesseractEngine(resolvedTessdataDir, "eng", EngineMode.Default);
            }
            catch (Exception ex)
            {
                initFailed = true;
                IngestionLog.Warn($"Tesseract OCR disabled — engine init failed: {ex.Message}");
            }
        }
    }

    private string? ResolveTessdataDir()
    {
        foreach (var dir in paths.GetTessdataDirectories())
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
