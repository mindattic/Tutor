using System.Diagnostics;
using System.Text;

namespace Tutor.Core.Parsers;

// Shared base for parsers that delegate to an external CLI tool (Calibre's
// ebook-convert for MOBI, LibreOffice's soffice for RTF/ODT). Each subclass
// provides the tool name, candidate executable paths, and the argument
// template to convert input → plaintext on stdout or via a temp file.
public abstract class ExternalToolBookParser : IBookParser
{
    public abstract IReadOnlyCollection<string> SupportedExtensions { get; }

    /// <summary>Friendly name shown in error messages, e.g. "Calibre's ebook-convert".</summary>
    protected abstract string ToolDisplayName { get; }

    /// <summary>Candidate executable names/paths to probe in order.</summary>
    protected abstract IEnumerable<string> ExecutableCandidates { get; }

    /// <summary>Documentation URL shown in the "tool not installed" error.</summary>
    protected abstract string InstallHint { get; }

    /// <summary>Build the argument list for `exe inputPath outputPath`.</summary>
    protected abstract IEnumerable<string> BuildArguments(string inputPath, string outputPath);

    /// <summary>
    /// Where the tool actually writes its output. Default: the path we asked for.
    /// LibreOffice ignores the precise path and writes &lt;outdir&gt;/&lt;baseName&gt;.txt,
    /// so its subclass overrides this.
    /// </summary>
    protected virtual string ResolveActualOutputPath(string inputPath, string outputPath)
        => outputPath;

    public async Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        var exe = ResolveExecutable()
            ?? throw new InvalidOperationException(BuildMissingToolMessage());

        var inputPath = Path.Combine(Path.GetTempPath(), $"tutor-in-{Guid.NewGuid():N}{Path.GetExtension(fileName)}");
        var outputPath = Path.Combine(Path.GetTempPath(), $"tutor-out-{Guid.NewGuid():N}.txt");

        try
        {
            await using (var temp = File.Create(inputPath))
            {
                await input.CopyToAsync(temp, ct);
            }

            var args = BuildArguments(inputPath, outputPath).ToList();

            var psi = new ProcessStartInfo
            {
                FileName = exe,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            foreach (var a in args) psi.ArgumentList.Add(a);

            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException($"Failed to start {ToolDisplayName} ({exe}).");

            // Capture stderr so we can surface it as a warning.
            var stderr = await process.StandardError.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"{ToolDisplayName} exited with code {process.ExitCode}. " +
                    $"stderr:\n{stderr.TrimEnd()}");
            }

            string text = "";
            var actualOutput = ResolveActualOutputPath(inputPath, outputPath);
            if (File.Exists(actualOutput))
            {
                text = await File.ReadAllTextAsync(actualOutput, Encoding.UTF8, ct);
                if (actualOutput != outputPath)
                {
                    try { File.Delete(actualOutput); } catch { }
                }
            }

            var warnings = string.IsNullOrWhiteSpace(stderr)
                ? Array.Empty<string>()
                : new[] { $"{ToolDisplayName} stderr: {stderr.Trim()}" };

            return new ExtractedBook(
                PlainText: text.Trim(),
                Title: Path.GetFileNameWithoutExtension(fileName),
                SourceFormat: Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant(),
                Warnings: warnings);
        }
        finally
        {
            try { if (File.Exists(inputPath)) File.Delete(inputPath); } catch { }
            try { if (File.Exists(outputPath)) File.Delete(outputPath); } catch { }
        }
    }

    private string? ResolveExecutable()
    {
        foreach (var candidate in ExecutableCandidates)
        {
            // Absolute path that exists.
            if (Path.IsPathRooted(candidate) && File.Exists(candidate))
                return candidate;

            // Bare name — let the OS resolve via PATH by trying to start it with --version.
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = candidate,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    proc.WaitForExit(5000);
                    return candidate;
                }
            }
            catch { /* not on PATH or not executable; try next candidate */ }
        }
        return null;
    }

    private string BuildMissingToolMessage()
        => $"{ToolDisplayName} was not found. Tutor's parser for "
         + $"{string.Join("/", SupportedExtensions)} delegates to {ToolDisplayName} because there is no "
         + $"reliable pure-managed alternative.\n"
         + $"Install it from: {InstallHint}\n"
         + $"Tried: {string.Join(", ", ExecutableCandidates)}";
}
