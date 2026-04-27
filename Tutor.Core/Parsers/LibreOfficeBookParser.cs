namespace Tutor.Core.Parsers;

// .doc, .rtf, .odt — all routed through LibreOffice's headless converter.
//   - .doc: legacy binary Word. NPOI 2.8 dropped HWPF; no maintained pure-managed
//          alternative exists for modern .NET. LibreOffice handles it reliably.
//   - .rtf: there is no maintained pure-managed RTF text-extraction library.
//   - .odt: free bonus once we have LibreOffice as a dependency anyway.
public sealed class LibreOfficeBookParser : ExternalToolBookParser
{
    public override IReadOnlyCollection<string> SupportedExtensions { get; } =
        new[] { ".doc", ".rtf", ".odt" };

    protected override string ToolDisplayName => "LibreOffice (soffice)";

    protected override IEnumerable<string> ExecutableCandidates => new[]
    {
        @"C:\Program Files\LibreOffice\program\soffice.exe",
        @"C:\Program Files (x86)\LibreOffice\program\soffice.exe",
        "/Applications/LibreOffice.app/Contents/MacOS/soffice",
        "soffice",
        "libreoffice",
    };

    protected override string InstallHint => "https://www.libreoffice.org/download/download/";

    protected override IEnumerable<string> BuildArguments(string inputPath, string outputPath)
    {
        var outDir = Path.GetDirectoryName(outputPath)!;
        yield return "--headless";
        yield return "--convert-to";
        yield return "txt:Text";
        yield return "--outdir";
        yield return outDir;
        yield return inputPath;
    }

    // LibreOffice writes <outdir>/<inputBaseName>.txt regardless of any output
    // filename we'd like — point the base class at where the file actually lands.
    protected override string ResolveActualOutputPath(string inputPath, string outputPath)
    {
        var outDir = Path.GetDirectoryName(outputPath)!;
        var baseName = Path.GetFileNameWithoutExtension(inputPath);
        return Path.Combine(outDir, baseName + ".txt");
    }
}
