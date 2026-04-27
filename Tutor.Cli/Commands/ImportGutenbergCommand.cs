using Tutor.Cli.Gutenberg;
using Tutor.Cli.Pipeline;

namespace Tutor.Cli.Commands;

public sealed class ImportGutenbergCommand
{
    private readonly GutenbergFetcher fetcher;
    private readonly BookImportPipeline pipeline;

    public ImportGutenbergCommand(GutenbergFetcher fetcher, BookImportPipeline pipeline)
    {
        this.fetcher = fetcher;
        this.pipeline = pipeline;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine("Usage: tutor gutenberg <book-id> [--course \"Name\"] [--description \"...\"]");
            return 64;
        }

        if (!int.TryParse(positionals[0], out var bookId))
        {
            Console.Error.WriteLine($"Invalid Gutenberg book ID: {positionals[0]}");
            return 64;
        }

        var catalogEntry = GutenbergCatalog.Top10.FirstOrDefault(b => b.Id == bookId);
        var defaultTitle = catalogEntry?.Title ?? $"Project Gutenberg #{bookId}";
        var defaultAuthor = catalogEntry?.Author ?? "";

        var courseName = options.Get("course") ?? defaultTitle;
        var description = options.Get("description") ?? $"Imported from Project Gutenberg (#{bookId}).";
        var author = options.Get("author") ?? defaultAuthor;

        Console.WriteLine($"Fetching Project Gutenberg #{bookId} ({defaultTitle})...");
        var content = await fetcher.FetchPlainTextAsync(bookId, ct);
        Console.WriteLine($"  Downloaded {content.Length:N0} chars after stripping PG header/footer.");

        var request = new BookImportRequest(
            CourseName: courseName,
            CourseDescription: description,
            ResourceTitle: defaultTitle,
            Author: author,
            ResourceDescription: $"Project Gutenberg eBook #{bookId}",
            FileName: $"pg{bookId}.txt",
            Content: content);

        var result = await pipeline.ImportAsync(request, ct);
        Console.WriteLine();
        Console.WriteLine($"View in the Blazor UI by opening course '{result.Course.Name}' (id: {result.Course.Id}).");
        return 0;
    }
}
