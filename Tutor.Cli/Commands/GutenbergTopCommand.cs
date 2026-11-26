using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Tutor.Cli.Gutenberg;
using Tutor.Cli.Pipeline;
using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor gutenberg-top [--count N] [--only id,id] [--dry-run] [--allow-duplicate] [--quiz-mode baked|dynamic|both]
/// [--max-minutes M] [--report-dir &lt;dir&gt;]</c> (TUT-T50): reads Project Gutenberg's current "Top 100 EBooks
/// last 30 days" list, builds a course from each of the first N books through <see cref="BookImportPipeline"/>
/// (the same pipeline as <c>tutor gutenberg</c>), and writes a report of how long each stage took per book,
/// what the LLM providers were asked for, and how big each course came out.
/// <para/>
/// A book that is already a course (a resource named <c>pg&lt;id&gt;.txt</c>) is skipped unless
/// <c>--allow-duplicate</c> is given, so a re-run resumes. <c>--max-minutes</c> stops new books from starting
/// once the run is that old. The report (<c>.json</c> and <c>.md</c>) is rewritten after every book.
/// </summary>
public sealed class GutenbergTopCommand
{
    private readonly GutenbergFetcher fetcher;
    private readonly BookImportPipeline pipeline;
    private readonly CourseService courseService;
    private readonly VectorStoreService vectorStore;
    private readonly TutorSettingsSource settings;

    public GutenbergTopCommand(
        GutenbergFetcher fetcher,
        BookImportPipeline pipeline,
        CourseService courseService,
        VectorStoreService vectorStore,
        TutorSettingsSource settings)
    {
        this.fetcher = fetcher;
        this.pipeline = pipeline;
        this.courseService = courseService;
        this.vectorStore = vectorStore;
        this.settings = settings;
    }

    /// <summary>Returns 0 when every attempted book built, 1 when one failed, 64 on usage errors.</summary>
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (_, options) = Args.Parse(args);
        var s = settings.Current;
        var dryRun = options.ContainsKey("dry-run");
        var allowDuplicate = options.ContainsKey("allow-duplicate");
        QuizMode quizMode;
        var count = s.GutenbergTopCount;
        double? maxMinutes = null;
        var only = new HashSet<int>();
        try
        {
            quizMode = QuizModes.Parse(options.Get("quiz-mode"));
            if (options.Get("count") is { } c) count = int.Parse(c);
            if (options.Get("max-minutes") is { } m) maxMinutes = double.Parse(m, System.Globalization.CultureInfo.InvariantCulture);
            if (options.Get("only") is { } o) only = o.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToHashSet();
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException)
        {
            Console.Error.WriteLine($"Usage: tutor gutenberg-top [--count N] [--only id,id] [--dry-run] [--allow-duplicate] [--quiz-mode baked|dynamic|both] [--max-minutes M] [--report-dir <dir>] ({ex.Message})");
            return 64;
        }

        var reportDir = options.Get("report-dir") ?? Path.Combine(TutorDataRoot.Path, "Reports");
        var runClock = Stopwatch.StartNew();
        var started = DateTime.UtcNow;

        var (books, source) = await fetcher.FetchTopAsync(count, ct);
        Console.WriteLine($"Top {books.Count} from {source}:");

        var existingByBookId = await ExistingCoursesByBookIdAsync();
        var report = new RunReport
        {
            StartedUtc = started,
            ListSource = source,
            ChatModel = s.ActiveModel,
            QuizMode = quizMode.ToString().ToLowerInvariant(),
            DataRoot = TutorDataRoot.Path,
        };
        for (var i = 0; i < books.Count; i++)
        {
            var book = books[i];
            var row = new BookReport { Rank = i + 1, Id = book.Id, Title = book.Title, Author = book.Author };
            if (only.Count > 0 && !only.Contains(book.Id)) row.Status = "not attempted (not in --only)";
            else if (existingByBookId.TryGetValue(book.Id, out var existing) && !allowDuplicate)
            {
                row.Status = "skipped: already a course";
                row.CourseId = existing.Id;
                row.CourseName = existing.Name;
            }
            else row.Status = "planned";
            report.Books.Add(row);
            Console.WriteLine($"  {row.Rank,2}. #{book.Id,-6} {book.Title} - {book.Author}  [{row.Status}]");
        }

        if (dryRun)
        {
            Console.WriteLine();
            Console.WriteLine("Dry-run only; no books were imported.");
            return 0;
        }

        Directory.CreateDirectory(reportDir);
        var reportBase = Path.Combine(reportDir, $"gutenberg-top-{started:yyyyMMdd-HHmmss}");

        foreach (var row in report.Books.Where(b => b.Status == "planned"))
        {
            ct.ThrowIfCancellationRequested();
            if (maxMinutes is { } limit && runClock.Elapsed.TotalMinutes >= limit)
            {
                row.Status = $"not attempted (run passed --max-minutes {limit})";
                continue;
            }

            Console.WriteLine();
            Console.WriteLine($"=== ({row.Rank}/{report.Books.Count}) #{row.Id} {row.Title} ===");
            var bookClock = Stopwatch.StartNew();
            var usageBefore = LlmUsage.Snapshot();
            var diskBefore = DirectoryBytes(TutorDataRoot.Path);
            try
            {
                var stage = Stopwatch.StartNew();
                var (raw, url) = await fetcher.FetchRawAsync(row.Id, ct);
                row.SourceUrl = url;
                row.SourceBytes = Encoding.UTF8.GetByteCount(raw);
                row.Stages["download"] = stage.Elapsed.TotalSeconds;

                stage.Restart();
                var content = GutenbergFetcher.StripHeaderFooter(raw);
                row.ContentChars = content.Length;
                row.Stages["strip license"] = stage.Elapsed.TotalSeconds;
                Console.WriteLine($"  Downloaded {row.SourceBytes:N0} bytes from {url}; {content.Length:N0} chars after stripping PG header/footer.");

                var by = string.IsNullOrWhiteSpace(row.Author) ? "" : $" by {row.Author}";
                var attribution = $"{row.Title}{by}. Project Gutenberg eBook #{row.Id} (https://www.gutenberg.org/ebooks/{row.Id}). Public domain in the USA.";
                var request = new BookImportRequest(
                    CourseName: row.Title,
                    CourseDescription: attribution,
                    ResourceTitle: row.Title,
                    Author: row.Author,
                    ResourceDescription: attribution,
                    FileName: $"pg{row.Id}.txt",
                    Content: content,
                    QuizMode: quizMode,
                    AllowDuplicate: allowDuplicate);

                var result = await pipeline.ImportAsync(request, ct);
                foreach (var st in result.Stages) row.Stages[st.Name] = st.Elapsed.TotalSeconds;

                row.CourseId = result.Course.Id;
                row.CourseName = result.Course.Name;
                var sections = result.Structure.GetAllSectionsFlattened().ToList();
                row.Lessons = result.Structure.TotalLessons;
                row.Topics = result.Structure.TotalTopics;
                row.Sections = sections.Count;
                row.SectionsWithContent = sections.Count(x => !string.IsNullOrWhiteSpace(x.Content));
                row.QuizQuestions = result.BakedQuizCount;
                row.Chunks = (await vectorStore.GetChunksForResourceAsync(result.Resource.Id)).Count;
                var collection = await courseService.GetCourseConceptMapCollectionAsync(result.Course.Id, ct);
                row.Concepts = collection?.GetAllConcepts().Count() ?? 0;
                row.Relations = collection?.GetAllRelations().Count() ?? 0;
                row.Status = "done";
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                row.Status = "failed";
                row.Error = $"{ex.GetType().Name}: {ex.Message}";
                // The stages that finished before the failure (only once the pipeline has started).
                if (row.ContentChars > 0)
                    foreach (var st in pipeline.LastStages) row.Stages[st.Name] = st.Elapsed.TotalSeconds;
                Console.Error.WriteLine($"  FAILED: {row.Error}");
            }

            row.TotalSeconds = bookClock.Elapsed.TotalSeconds;
            row.DataBytesAdded = DirectoryBytes(TutorDataRoot.Path) - diskBefore;
            ApplyUsage(row, LlmUsage.Snapshot().Since(usageBefore), settings.Current);
            report.TotalSeconds = runClock.Elapsed.TotalSeconds;
            WriteReport(report, reportBase);
        }

        report.TotalSeconds = runClock.Elapsed.TotalSeconds;
        WriteReport(report, reportBase);

        Console.WriteLine();
        Console.WriteLine(ToMarkdown(report));
        Console.WriteLine($"Report: {reportBase}.md and .json");
        return report.Books.Any(b => b.Status == "failed") ? 1 : 0;
    }

    /// <summary>Courses that already hold a Gutenberg book, keyed by ebook number (resource file name <c>pg&lt;id&gt;.txt</c>).</summary>
    private async Task<Dictionary<int, Tutor.Core.Models.Course>> ExistingCoursesByBookIdAsync()
    {
        var found = new Dictionary<int, Tutor.Core.Models.Course>();
        foreach (var course in await courseService.GetAllCoursesAsync())
        {
            foreach (var resource in await courseService.GetCourseResourcesAsync(course.Id))
            {
                var name = Path.GetFileNameWithoutExtension(resource.FileName ?? "");
                if (name.StartsWith("pg", StringComparison.OrdinalIgnoreCase) && int.TryParse(name[2..], out var id))
                    found.TryAdd(id, course);
            }
        }
        return found;
    }

    private static void ApplyUsage(BookReport row, LlmUsageSnapshot used, TutorSettings s)
    {
        var charsPerToken = s.EstimateCharsPerToken <= 0 ? 4.0 : s.EstimateCharsPerToken;
        row.ChatCalls = used.ChatCalls;
        row.ChatFailures = used.ChatFailures;
        row.ChatInputChars = used.ChatInputChars;
        row.ChatOutputChars = used.ChatOutputChars;
        row.ChatCallSeconds = used.ChatMilliseconds / 1000.0;
        row.EmbedCalls = used.EmbedCalls;
        row.EmbedTexts = used.EmbedTexts;
        row.EmbedInputChars = used.EmbedInputChars;
        row.EstimatedInputTokens = (long)(used.ChatInputChars / charsPerToken);
        row.EstimatedOutputTokens = (long)(used.ChatOutputChars / charsPerToken);
        row.EstimatedCostUsd = Math.Round(
            row.EstimatedInputTokens / 1e6 * s.EstimateChatInputUsdPerMillionTokens
            + row.EstimatedOutputTokens / 1e6 * s.EstimateChatOutputUsdPerMillionTokens
            + used.EmbedInputChars / charsPerToken / 1e6 * s.EstimateEmbeddingUsdPerMillionTokens, 4);
    }

    private static long DirectoryBytes(string path)
    {
        try
        {
            return Directory.Exists(path)
                ? new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length)
                : 0;
        }
        catch (IOException) { return 0; }
        catch (UnauthorizedAccessException) { return 0; }
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static void WriteReport(RunReport report, string basePath)
    {
        File.WriteAllText(basePath + ".json", JsonSerializer.Serialize(report, JsonOptions));
        File.WriteAllText(basePath + ".md", ToMarkdown(report));
    }

    private static readonly string[] StageOrder =
    [
        "download", "strip license", StageTiming.ChunkAndEmbed, StageTiming.Format, StageTiming.ConceptMap,
        StageTiming.Structure, StageTiming.Quizzes, StageTiming.Save,
    ];

    private static string ToMarkdown(RunReport r)
    {
        static string T(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{(long)ts.TotalHours}:" + ts.ToString(@"mm\:ss"); // total hours: a 100-book run passes 24 h
        }
        var sb = new StringBuilder();
        sb.AppendLine($"# Gutenberg top {r.Books.Count}: course build timings");
        sb.AppendLine();
        sb.AppendLine($"Started {r.StartedUtc:yyyy-MM-dd HH:mm} UTC; list from {r.ListSource}; chat model setting {r.ChatModel}; quiz mode {r.QuizMode}; total {T(r.TotalSeconds)}. Tokens and cost are estimates from characters sent and received.");
        sb.AppendLine();
        sb.AppendLine($"| # | Book | PG id | Source | {string.Join(" | ", StageOrder)} | Total | Course | Chat calls (failed) | Est. tokens in/out | Est. cost | Data added | Status |");
        sb.AppendLine($"|---|---|---|---|{string.Concat(StageOrder.Select(_ => "---|"))}---|---|---|---|---|---|---|");
        foreach (var b in r.Books)
        {
            var ran = b.TotalSeconds > 0;
            var stages = string.Join(" | ", StageOrder.Select(n => b.Stages.TryGetValue(n, out var v) ? T(v) : ""));
            var size = b.Status == "done"
                ? $"{b.Lessons} lessons, {b.Topics} topics, {b.Sections} sections ({b.SectionsWithContent} with content), {b.QuizQuestions} quiz questions, {b.Concepts} concepts, {b.Relations} relations, {b.Chunks} chunks"
                : "";
            var status = b.Error is null ? b.Status : $"{b.Status}: {b.Error.Replace("|", "/").ReplaceLineEndings(" ")}";
            sb.AppendLine($"| {b.Rank} | {b.Title} | {b.Id} | {(b.SourceBytes > 0 ? $"{b.SourceBytes / 1024.0:N0} KB" : "")} | {stages} | {(ran ? T(b.TotalSeconds) : "")} | {size} | {(ran ? $"{b.ChatCalls} ({b.ChatFailures})" : "")} | {(ran ? $"{b.EstimatedInputTokens:N0} / {b.EstimatedOutputTokens:N0}" : "")} | {(ran ? $"${b.EstimatedCostUsd:N2}" : "")} | {(ran ? $"{b.DataBytesAdded / 1048576.0:N1} MB" : "")} | {status} |");
        }
        return sb.ToString();
    }

    /// <summary>One run of <c>tutor gutenberg-top</c>, as written to the report files.</summary>
    public sealed class RunReport
    {
        public DateTime StartedUtc { get; set; }
        public string ListSource { get; set; } = "";
        public string ChatModel { get; set; } = "";
        public string QuizMode { get; set; } = "";
        public string DataRoot { get; set; } = "";
        public double TotalSeconds { get; set; }
        public List<BookReport> Books { get; set; } = [];
    }

    /// <summary>One book's row. Stage times are wall-clock seconds; chat call seconds sum every call's own duration.</summary>
    public sealed class BookReport
    {
        public int Rank { get; set; }
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Error { get; set; }
        public string? SourceUrl { get; set; }
        public long SourceBytes { get; set; }
        public int ContentChars { get; set; }
        public Dictionary<string, double> Stages { get; set; } = [];
        public double TotalSeconds { get; set; }
        public string? CourseId { get; set; }
        public string? CourseName { get; set; }
        public int Lessons { get; set; }
        public int Topics { get; set; }
        public int Sections { get; set; }
        public int SectionsWithContent { get; set; }
        public int QuizQuestions { get; set; }
        public int Concepts { get; set; }
        public int Relations { get; set; }
        public int Chunks { get; set; }
        public long DataBytesAdded { get; set; }
        public long ChatCalls { get; set; }
        public long ChatFailures { get; set; }
        public long ChatInputChars { get; set; }
        public long ChatOutputChars { get; set; }
        public double ChatCallSeconds { get; set; }
        public long EmbedCalls { get; set; }
        public long EmbedTexts { get; set; }
        public long EmbedInputChars { get; set; }
        public long EstimatedInputTokens { get; set; }
        public long EstimatedOutputTokens { get; set; }
        public double EstimatedCostUsd { get; set; }
    }
}
