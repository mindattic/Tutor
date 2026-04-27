using Tutor.Cli.Gutenberg;

namespace Tutor.Cli.Commands;

// Diagnostic command: fetch + strip only, no LLM pipeline. Useful for verifying
// network access and the header/footer stripper without spending API credit.
public sealed class FetchOnlyCommand
{
    private readonly GutenbergFetcher fetcher;

    public FetchOnlyCommand(GutenbergFetcher fetcher)
    {
        this.fetcher = fetcher;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, _) = Args.Parse(args);
        if (positionals.Count == 0 || !int.TryParse(positionals[0], out var bookId))
        {
            Console.Error.WriteLine("Usage: tutor fetch <book-id>");
            return 64;
        }

        Console.WriteLine($"Fetching Project Gutenberg #{bookId}...");
        var content = await fetcher.FetchPlainTextAsync(bookId, ct);

        Console.WriteLine($"Stripped length: {content.Length:N0} chars");
        Console.WriteLine();
        Console.WriteLine("--- first 400 chars ---");
        Console.WriteLine(content[..Math.Min(400, content.Length)]);
        Console.WriteLine();
        Console.WriteLine("--- last 400 chars ---");
        Console.WriteLine(content[Math.Max(0, content.Length - 400)..]);
        return 0;
    }
}
