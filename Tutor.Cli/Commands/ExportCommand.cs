using Tutor.Cli.Export;

namespace Tutor.Cli.Commands;

public sealed class ExportCommand
{
    private readonly CourseExporter exporter;

    public ExportCommand(CourseExporter exporter)
    {
        this.exporter = exporter;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, _) = Args.Parse(args);
        if (positionals.Count < 2)
        {
            Console.Error.WriteLine("Usage: tutor export <course-id> <output.tutorcourse>");
            return 64;
        }

        var courseId = positionals[0];
        var outputPath = positionals[1];

        if (!outputPath.EndsWith(".tutorcourse", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("WARN: output path does not end in .tutorcourse — proceeding anyway.");
        }

        Console.WriteLine($"Exporting course '{courseId}' → {outputPath}");
        var manifest = await exporter.ExportAsync(courseId, outputPath, ct);

        var size = new FileInfo(outputPath).Length;
        Console.WriteLine();
        Console.WriteLine("Bundle written.");
        Console.WriteLine($"  Course:        {manifest.CourseName} ({manifest.CourseId})");
        Console.WriteLine($"  Resources:     {manifest.ResourceCount}");
        Console.WriteLine($"  ConceptMaps:   {manifest.ConceptMapCount}");
        Console.WriteLine($"  Chunks:        {manifest.ChunkCount}");
        Console.WriteLine($"  File size:     {size / 1024.0 / 1024.0:F2} MB");
        Console.WriteLine($"  Format ver.:   {manifest.FormatVersion}");
        return 0;
    }
}
