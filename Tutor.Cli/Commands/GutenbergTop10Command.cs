using Tutor.Cli.Export;
using Tutor.Cli.Gutenberg;
using Tutor.Cli.Pipeline;
using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor gutenberg-top10 [--dry-run] [--allow-duplicate] [--export-dir &lt;dir&gt;] [--quiz-mode baked|dynamic|both]</c>
/// — drives the curated <see cref="GutenbergCatalog.Top10"/> list through
/// <see cref="BookImportPipeline"/> sequentially. Each book gets its own course;
/// existing course names are skipped unless <c>--allow-duplicate</c> is set, so
/// re-running after a partial failure resumes naturally without paying for
/// already-imported books. When <c>--export-dir</c> is given, every course (freshly
/// imported or pre-existing) is exported to <c>&lt;dir&gt;/&lt;Title&gt;.tutor</c>.
/// <para/>
/// Long-running — Moby Dick alone took ~2 hours; the full top-10 takes many hours
/// and meaningful API spend. Use <c>--dry-run</c> to preview the plan.
/// </summary>
public sealed class GutenbergTop10Command
{
    private readonly GutenbergFetcher fetcher;
    private readonly BookImportPipeline pipeline;
    private readonly CourseService courseService;
    private readonly CourseExporter exporter;

    public GutenbergTop10Command(
        GutenbergFetcher fetcher,
        BookImportPipeline pipeline,
        CourseService courseService,
        CourseExporter exporter)
    {
        this.fetcher = fetcher;
        this.pipeline = pipeline;
        this.courseService = courseService;
        this.exporter = exporter;
    }

    /// <summary>Returns 0 on success or full dry-run; 1 if any book failed to import or export.</summary>
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (_, options) = Args.Parse(args);
        var dryRun = options.ContainsKey("dry-run");
        var allowDuplicate = options.ContainsKey("allow-duplicate");
        var exportDir = options.Get("export-dir");
        QuizMode quizMode;
        try
        {
            quizMode = QuizModes.Parse(options.Get("quiz-mode"));
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 64;
        }

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

        Console.WriteLine($"Plan ({totalBooks} books, quiz-mode: {quizMode.ToString().ToLowerInvariant()}):");
        foreach (var (book, status) in planned)
        {
            Console.WriteLine($"  [{status}] #{book.Id,-5} {book.Title} — {book.Author}");
        }
        if (!string.IsNullOrWhiteSpace(exportDir))
            Console.WriteLine($"Exporting each course to: {Path.GetFullPath(exportDir)}");

        if (dryRun)
        {
            Console.WriteLine();
            Console.WriteLine("Dry-run only; no books were imported.");
            return 0;
        }

        if (!string.IsNullOrWhiteSpace(exportDir))
            Directory.CreateDirectory(exportDir);

        Console.WriteLine();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var imported = 0;
        var skipped = 0;
        var exported = 0;
        var failed = new List<(GutenbergBook Book, string Error)>();

        for (var i = 0; i < planned.Count; i++)
        {
            var (book, status) = planned[i];
            Console.WriteLine();
            Console.WriteLine($"=== ({i + 1}/{totalBooks}) {book.Title} ===");

            string? courseId = null;
            try
            {
                if (status.StartsWith("SKIP"))
                {
                    skipped++;
                    Console.WriteLine("  Skipped import — course already exists.");
                    courseId = (await courseService.GetAllCoursesAsync())
                        .FirstOrDefault(c => string.Equals(c.Name, book.Title, StringComparison.OrdinalIgnoreCase))?.Id;
                }
                else
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
                        QuizMode: quizMode,
                        AllowDuplicate: allowDuplicate);

                    var result = await pipeline.ImportAsync(request, ct);
                    courseId = result.Course.Id;
                    imported++;
                }

                if (!string.IsNullOrWhiteSpace(exportDir) && courseId != null)
                {
                    var outPath = Path.Combine(exportDir, SafeFileName(book.Title) + ".tutor");
                    var manifest = await exporter.ExportAsync(courseId, outPath, ct);
                    exported++;
                    Console.WriteLine($"  Exported → {outPath} ({manifest.ResourceCount} resources, {manifest.ChunkCount} chunks)");
                }
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
        if (!string.IsNullOrWhiteSpace(exportDir))
            Console.WriteLine($"  Exported:  {exported}");
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

    /// <summary>Strips characters that are illegal in Windows filenames from a course title.</summary>
    private static string SafeFileName(string title)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(title.Select(c => invalid.Contains(c) ? '_' : c).ToArray()).Trim();
        return string.IsNullOrEmpty(cleaned) ? "course" : cleaned;
    }
}
