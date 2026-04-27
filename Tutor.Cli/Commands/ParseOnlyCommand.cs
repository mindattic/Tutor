using Tutor.Core.Parsers;

namespace Tutor.Cli.Commands;

// Diagnostic: run a parser against a file and print the extracted preview
// without calling any LLM. Used to validate parser additions cheaply.
public sealed class ParseOnlyCommand
{
    private readonly ParserRegistry parsers;

    public ParseOnlyCommand(ParserRegistry parsers)
    {
        this.parsers = parsers;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, _) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine("Usage: tutor parse <path-to-file>");
            Console.Error.WriteLine($"Supported: {string.Join(", ", parsers.SupportedExtensions.OrderBy(s => s))}");
            return 64;
        }

        var path = positionals[0];
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"File not found: {path}");
            return 66;
        }

        if (!parsers.CanHandle(path))
        {
            Console.Error.WriteLine(
                $"Unsupported extension '{Path.GetExtension(path)}'. " +
                $"Supported: {string.Join(", ", parsers.SupportedExtensions.OrderBy(s => s))}");
            return 65;
        }

        var parser = parsers.Resolve(path)!;
        Console.WriteLine($"Parser: {parser.GetType().Name}");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = await parsers.ParseAsync(path, ct);
        sw.Stop();

        Console.WriteLine($"Format:        {result.SourceFormat}");
        Console.WriteLine($"Title:         {result.Title}");
        Console.WriteLine($"Author:        {result.Author}");
        Console.WriteLine($"Description:   {Truncate(result.Description, 200)}");
        Console.WriteLine($"Plain text:    {result.PlainText.Length:N0} chars");
        Console.WriteLine($"Parse time:    {sw.Elapsed.TotalSeconds:F2}s");
        if (result.Warnings.Count > 0)
        {
            Console.WriteLine("Warnings:");
            foreach (var w in result.Warnings) Console.WriteLine($"  - {w}");
        }
        Console.WriteLine();
        Console.WriteLine("--- first 600 chars ---");
        Console.WriteLine(Truncate(result.PlainText, 600));
        Console.WriteLine();
        Console.WriteLine("--- last 400 chars ---");
        var len = result.PlainText.Length;
        Console.WriteLine(len > 400 ? result.PlainText[^400..] : "");
        return 0;
    }

    private static string Truncate(string s, int max)
        => string.IsNullOrEmpty(s) ? "" : (s.Length <= max ? s : s[..max] + "...");
}
