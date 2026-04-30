using Tutor.Cli.Gutenberg;
using Tutor.Cli.Pipeline;
using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

// Drives the curated GutenbergCatalog.Top10 list through the import pipeline
// sequentially. Each book gets its own course (per the "never silently combine
// books" directive). Books whose course name already exists are skipped unless
// --allow-duplicate is set, so re-running the command after a partial failure
// resumes from where it stopped without paying for already-imported books.
//
// This is a long-running command — Moby Dick alone took ~2 hours; the full
// top-10 will take many hours and meaningful API spend. Use --dry-run to
// preview which books would be imported.
public sealed class GutenbergTop10Command
{
    private readonly GutenbergFetcher fetcher;
    private readonly BookImportPipeline pipeline;
    private readonly CourseService courseService;

    public GutenbergTop10Command(
        GutenbergFetcher fetcher,
        BookImportPipeline pipeline,
        CourseService courseService)
    {
        this.fetcher = fetcher;
        this.pipeline = pipeline;
        this.courseService = courseService;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (_, options) = Args.Parse(args);
        var dryRun = options.ContainsKey("dry-run");
        var allowDuplicate = options.ContainsKey("allow-duplicate");

        var existingCourseNames = (await courseService.GetAllCoursesAsync())
            .Select(c => c.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var totalBooks = GutenbergCatalog.Top10.Count;
        var planned = new List<(GutenbergBook Book, string Status)>();
        foreach (var book in GutenbergCatalog.Top10)
        {
            var status = (existingCourseNames.Contains(book.Title) && !allowDuplicate)
                ? "SKIP (already exists)"
                : "WILL IMPORT";
            planned.Add((book, status));
        }

        Console.WriteLine($"Plan ({totalBooks} books):");
        foreach (var (book, status) in planned)
        {
            Console.WriteLine($"  [{status}] #{book.Id,-5} {book.Title} — {book.Author}");
        }

        if (dryRun)
        {
            Console.WriteLine();
            Console.WriteLine("Dry-run only; no books were imported.");
            return 0;
        }

        Console.WriteLine();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var imported = 0;
        var skipped = 0;
        var failed = new List<(GutenbergBook Book, string Error)>();

        for (var i = 0; i < planned.Count; i++)
        {
            var (book, status) = planned[i];
            Console.WriteLine();
            Console.WriteLine($"=== ({i + 1}/{totalBooks}) {book.Title} ===");

            if (status.StartsWith("SKIP"))
            {
                skipped++;
                Console.WriteLine("  Skipped — course already exists.");
                continue;
            }

            try
            {
                Console.WriteLine($"  Fetching Project Gutenberg #{book.Id}...");
                var content = await fetcher.FetchPlainTextAsync(book.Id, ct);
                Console.WriteLine($"  Downloaded {content.Length:N0} chars after stripping PG header/footer.");

                var request = new BookImportRequest(
                    CourseName: book.Title,
                    CourseDescription: $"Imported from Project Gutenberg (#{book.Id}).",
                    ResourceTitle: book.Title,
                    Author: book.Author,
                    ResourceDescription: $"Project Gutenberg eBook #{book.Id}",
                    FileName: $"pg{book.Id}.txt",
                    Content: content,
                    AllowDuplicate: allowDuplicate);

                await pipeline.ImportAsync(request, ct);
                imported++;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  FAILED: {ex.Message}");
                failed.Add((book, ex.Message));
            }
        }

        sw.Stop();
        Console.WriteLine();
        Console.WriteLine("=== Top-10 import complete ===");
        Console.WriteLine($"  Imported:  {imported}");
        Console.WriteLine($"  Skipped:   {skipped}");
        Console.WriteLine($"  Failed:    {failed.Count}");
        Console.WriteLine($"  Total time: {sw.Elapsed:hh\\:mm\\:ss}");
        if (failed.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Failures:");
            foreach (var (book, err) in failed)
            {
                Console.WriteLine($"  #{book.Id} {book.Title}: {err}");
            }
            return 1;
        }
        return 0;
    }
}
