using Tutor.Cli.Export;
using Tutor.Cli.Pipeline;
using Tutor.Core.Parsers;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor build-course &lt;dir-or-zip&gt; [--course "Override Name"] [--quiz-mode baked|dynamic|both]
/// [--export &lt;out.tutor&gt;] [--allow-duplicate]</c> — builds ONE course out of MANY
/// source files described by a package's <c>manifest.json</c>, then optionally exports
/// it to a redistributable <c>.tutor</c> bundle. This is the headless equivalent of
/// adding each resource through the Blazor UI and clicking "Build Course".
/// </summary>
public sealed class BuildCourseCommand
{
    private readonly CourseBuildPipeline pipeline;
    private readonly ParserRegistry parsers;
    private readonly CourseExporter exporter;

    public BuildCourseCommand(CourseBuildPipeline pipeline, ParserRegistry parsers, CourseExporter exporter)
    {
        this.pipeline = pipeline;
        this.parsers = parsers;
        this.exporter = exporter;
    }

    /// <summary>Returns 0 on success, 64 on usage errors, 65 on unparseable items, 66 if the package is missing.</summary>
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine(
                "Usage: tutor build-course <dir-or-zip> [--course \"Override Name\"] " +
                "[--quiz-mode baked|dynamic|both] [--export <out.tutor>] [--allow-duplicate]");
            Console.Error.WriteLine("The package must contain a manifest.json plus the source files it lists.");
            return 64;
        }

        CoursePackage package;
        try
        {
            package = CoursePackage.Open(positionals[0]);
        }
        catch (FileNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 66;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 65;
        }

        using (package)
        {
            var manifest = package.Manifest;
            var courseName = options.Get("course") ?? manifest.Name;
            if (string.IsNullOrWhiteSpace(courseName))
            {
                Console.Error.WriteLine("Course name is required — set \"name\" in manifest.json or pass --course \"Name\".");
                return 64;
            }

            QuizMode quizMode;
            try
            {
                // CLI flag wins over the manifest default.
                quizMode = QuizModes.Parse(options.Get("quiz-mode") ?? manifest.QuizMode);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 64;
            }

            Console.WriteLine($"Building course '{courseName}' from {manifest.Items.Count} item(s) in '{positionals[0]}'");

            var items = new List<CourseItem>();
            foreach (var entry in manifest.Items)
            {
                string filePath;
                try
                {
                    filePath = package.ResolveItemPath(entry);
                }
                catch (FileNotFoundException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    return 66;
                }

                if (!parsers.CanHandle(filePath))
                {
                    Console.Error.WriteLine(
                        $"Unsupported file extension '{Path.GetExtension(filePath)}' for item '{entry.File}'. " +
                        $"Supported: {string.Join(", ", parsers.SupportedExtensions.OrderBy(s => s))}");
                    return 65;
                }

                Console.WriteLine($"  Parsing {entry.File} via {parsers.Resolve(filePath)!.GetType().Name}...");
                var extracted = await parsers.ParseAsync(filePath, ct);
                if (string.IsNullOrWhiteSpace(extracted.PlainText))
                {
                    Console.Error.WriteLine($"  Parser produced no text content for '{entry.File}'.");
                    return 65;
                }

                var title = entry.Title
                    ?? (string.IsNullOrWhiteSpace(extracted.Title) ? Path.GetFileNameWithoutExtension(filePath) : extracted.Title);
                var author = entry.Author ?? extracted.Author ?? "";
                var description = entry.Description ?? extracted.Description ?? "";

                items.Add(new CourseItem(
                    Title: title,
                    Author: author,
                    Description: description,
                    FileName: Path.GetFileName(filePath),
                    Content: extracted.PlainText));
            }

            var request = new CourseBuildRequest(
                CourseName: courseName,
                CourseDescription: manifest.Description,
                Items: items,
                QuizMode: quizMode,
                AllowDuplicate: options.ContainsKey("allow-duplicate"));

            var result = await pipeline.BuildAsync(request, ct);

            var exportPath = options.Get("export");
            if (!string.IsNullOrWhiteSpace(exportPath))
            {
                Console.WriteLine();
                Console.WriteLine($"Exporting → {exportPath}");
                var bundle = await exporter.ExportAsync(result.Course.Id, exportPath, ct);
                var size = new FileInfo(exportPath).Length;
                Console.WriteLine($"  Bundle: {bundle.ResourceCount} resources, {bundle.ChunkCount} chunks, {size / 1024.0 / 1024.0:F2} MB");
            }

            Console.WriteLine();
            Console.WriteLine($"View in the Blazor UI by opening course '{result.Course.Name}' (id: {result.Course.Id}).");
            return 0;
        }
    }
}
