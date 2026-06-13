using Tutor.Core.Services.Packaging;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor export &lt;course-id&gt; &lt;output.tutor&gt; [--key slug] [--course-version N]
/// [--author "..."] [--license "..."] [--description "..."] [--attribution "..."]</c> —
/// pack a course (resources, concept map, structure, embeddings) into a single
/// shareable bundle carrying stable identity, a payload SHA-256, and provenance.
/// </summary>
public sealed class ExportCommand
{
    private readonly CourseExporter exporter;

    public ExportCommand(CourseExporter exporter)
    {
        this.exporter = exporter;
    }

    /// <summary>
    /// Parses the command's positional arguments, runs the export, and prints a
    /// human-readable summary. Returns a process exit code (0 on success, 64 for
    /// usage errors).
    /// </summary>
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count < 2)
        {
            Console.Error.WriteLine(
                "Usage: tutor export <course-id> <output.tutor> [--key slug] [--course-version N] " +
                "[--author \"...\"] [--license \"...\"] [--description \"...\"] [--attribution \"...\"]");
            return 64;
        }

        var courseId = positionals[0];
        var outputPath = positionals[1];

        if (!outputPath.EndsWith(".tutor", StringComparison.OrdinalIgnoreCase) &&
            !outputPath.EndsWith(".tutorcourse", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("WARN: output path does not end in .tutor — proceeding anyway.");
        }

        var exportOptions = new BundleExportOptions
        {
            CourseKey = options.Get("key") ?? "",
            Author = options.Get("author"),
            License = options.Get("license"),
            Description = options.Get("description"),
            SourceAttribution = options.Get("attribution"),
        };
        var versionText = options.Get("course-version");
        if (versionText != null)
        {
            if (!int.TryParse(versionText, out var version) || version < 1)
            {
                Console.Error.WriteLine($"--course-version must be a whole number >= 1 (got '{versionText}').");
                return 64;
            }
            exportOptions.CourseVersion = version;
        }

        Console.WriteLine($"Exporting course '{courseId}' → {outputPath}");
        var manifest = await exporter.ExportAsync(courseId, outputPath, exportOptions, ct);

        var size = new FileInfo(outputPath).Length;
        Console.WriteLine();
        Console.WriteLine("Bundle written.");
        Console.WriteLine($"  Course:        {manifest.CourseName} ({manifest.CourseId})");
        Console.WriteLine($"  Identity:      {manifest.CourseKey} v{manifest.CourseVersion}");
        Console.WriteLine($"  SHA-256:       {manifest.Sha256}");
        Console.WriteLine($"  Resources:     {manifest.ResourceCount}");
        Console.WriteLine($"  ConceptMaps:   {manifest.ConceptMapCount}");
        Console.WriteLine($"  Chunks:        {manifest.ChunkCount}");
        Console.WriteLine($"  File size:     {size / 1024.0 / 1024.0:F2} MB");
        Console.WriteLine($"  Format ver.:   {manifest.FormatVersion}");
        return 0;
    }
}
