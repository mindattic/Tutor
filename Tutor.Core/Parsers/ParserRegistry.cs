namespace Tutor.Core.Parsers;

// Routes a file path or stream to the correct IBookParser based on its
// extension. The registry is the single integration point between the CLI's
// `import` command, Blazor's upload flow, and any future bundle/import-bundle
// path that needs to ingest text.
public sealed class ParserRegistry
{
    private readonly Dictionary<string, IBookParser> byExt = new(StringComparer.OrdinalIgnoreCase);

    public ParserRegistry(IEnumerable<IBookParser> parsers)
    {
        foreach (var parser in parsers)
        {
            foreach (var ext in parser.SupportedExtensions)
            {
                var key = Normalize(ext);
                // Last-registered wins. DI iteration order is registration order, so
                // a more specific parser registered after a generic one will replace
                // it — which is what we want.
                byExt[key] = parser;
            }
        }
    }

    public IReadOnlyCollection<string> SupportedExtensions => byExt.Keys;

    public bool CanHandle(string fileNameOrPath)
        => byExt.ContainsKey(Normalize(Path.GetExtension(fileNameOrPath)));

    public IBookParser? Resolve(string fileNameOrPath)
        => byExt.TryGetValue(Normalize(Path.GetExtension(fileNameOrPath)), out var parser)
            ? parser
            : null;

    public async Task<ExtractedBook> ParseAsync(string filePath, CancellationToken ct = default)
    {
        var parser = Resolve(filePath)
            ?? throw new NotSupportedException(
                $"No parser is registered for '{Path.GetExtension(filePath)}'. " +
                $"Supported: {string.Join(", ", byExt.Keys.OrderBy(k => k))}");

        await using var stream = File.OpenRead(filePath);
        return await parser.ParseAsync(stream, Path.GetFileName(filePath), ct);
    }

    private static string Normalize(string ext)
        => string.IsNullOrEmpty(ext) ? "" : (ext.StartsWith('.') ? ext : "." + ext).ToLowerInvariant();
}
