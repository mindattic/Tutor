namespace Tutor.Core.Parsers;

// .mobi / .azw / .azw3 via Calibre's ebook-convert. There is no maintained
// pure-managed C# library that can read modern MOBI/KFX formats reliably,
// so we shell out. ebook-convert ships with every Calibre install and can
// convert any e-book format Calibre supports to plain text.
public sealed class MobiBookParser : ExternalToolBookParser
{
    public override IReadOnlyCollection<string> SupportedExtensions { get; } =
        new[] { ".mobi", ".azw", ".azw3", ".prc" };

    protected override string ToolDisplayName => "Calibre's ebook-convert";

    protected override IEnumerable<string> ExecutableCandidates => new[]
    {
        // Windows defaults
        @"C:\Program Files\Calibre2\ebook-convert.exe",
        @"C:\Program Files (x86)\Calibre2\ebook-convert.exe",
        // PATH fallback
        "ebook-convert",
        "ebook-convert.exe",
    };

    protected override string InstallHint => "https://calibre-ebook.com/download";

    protected override IEnumerable<string> BuildArguments(string inputPath, string outputPath)
    {
        // ebook-convert input.mobi output.txt
        // Calibre infers the conversion from the file extensions.
        yield return inputPath;
        yield return outputPath;
    }
}
