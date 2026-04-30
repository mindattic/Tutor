using Tutor.Cli.Pipeline;
using Tutor.Core.Parsers;

namespace Tutor.Cli.Commands;

// Routes any supported file format (.txt/.md/.html/.htm/.epub/.pdf/.docx and
// later .doc/.mobi/.rtf) through ParserRegistry → ExtractedBook →
// BookImportPipeline. Format-specific concerns live in the parsers; this
// command stays format-agnostic.
public sealed class ImportFileCommand
{
    private readonly BookImportPipeline pipeline;
    private readonly ParserRegistry parsers;

    public ImportFileCommand(BookImportPipeline pipeline, ParserRegistry parsers)
    {
        this.pipeline = pipeline;
        this.parsers = parsers;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine(
                "Usage: tutor import <path-to-file> --course \"Name\" [--description \"...\"] [--author \"...\"] [--title \"...\"] [--allow-duplicate]");
            Console.Error.WriteLine($"Supported formats: {string.Join(", ", parsers.SupportedExtensions.OrderBy(s => s))}");
            return 64;
        }

        var path = positionals[0];
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"File not found: {path}");
            return 66; // EX_NOINPUT
        }

        var courseName = options.Get("course");
        if (string.IsNullOrWhiteSpace(courseName))
        {
            Console.Error.WriteLine("--course \"Name\" is required.");
            return 64;
        }

        if (!parsers.CanHandle(path))
        {
            Console.Error.WriteLine(
                $"Unsupported file extension '{Path.GetExtension(path)}'. " +
                $"Supported: {string.Join(", ", parsers.SupportedExtensions.OrderBy(s => s))}");
            return 65; // EX_DATAERR
        }

        Console.WriteLine($"Parsing {Path.GetFileName(path)} via {parsers.Resolve(path)!.GetType().Name}...");
        var extracted = await parsers.ParseAsync(path, ct);

        if (string.IsNullOrWhiteSpace(extracted.PlainText))
        {
            Console.Error.WriteLine("Parser produced no text content.");
            foreach (var w in extracted.Warnings) Console.Error.WriteLine($"  warning: {w}");
            return 65;
        }

        Console.WriteLine($"  Extracted {extracted.PlainText.Length:N0} chars (format: {extracted.SourceFormat}).");
        if (!string.IsNullOrWhiteSpace(extracted.Title))  Console.WriteLine($"  Title (from file): {extracted.Title}");
        if (!string.IsNullOrWhiteSpace(extracted.Author)) Console.WriteLine($"  Author (from file): {extracted.Author}");
        foreach (var w in extracted.Warnings) Console.WriteLine($"  warning: {w}");

        // CLI flags override what the parser detected. File-derived metadata is
        // a fallback, not a force.
        var title  = options.Get("title")  ?? (string.IsNullOrWhiteSpace(extracted.Title)  ? Path.GetFileNameWithoutExtension(path) : extracted.Title);
        var author = options.Get("author") ?? extracted.Author;
        var desc   = options.Get("description") ?? extracted.Description;

        var request = new BookImportRequest(
            CourseName: courseName,
            CourseDescription: desc,
            ResourceTitle: title,
            Author: author,
            ResourceDescription: "",
            FileName: Path.GetFileName(path),
            Content: extracted.PlainText,
            AllowDuplicate: options.ContainsKey("allow-duplicate"));

        var result = await pipeline.ImportAsync(request, ct);
        Console.WriteLine();
        Console.WriteLine($"View in the Blazor UI by opening course '{result.Course.Name}' (id: {result.Course.Id}).");
        return 0;
    }
}
