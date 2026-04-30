using Tutor.Cli.Export;

namespace Tutor.Cli.Commands;

public sealed class ImportBundleCommand
{
    private readonly BundleImporter importer;

    public ImportBundleCommand(BundleImporter importer)
    {
        this.importer = importer;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine(
                "Usage: tutor import-bundle <file.tutorcourse> [--course \"Override Name\"] [--allow-duplicate]");
            return 64;
        }

        var bundlePath = positionals[0];
        var overrideName = options.Get("course");
        var allowDuplicate = options.ContainsKey("allow-duplicate");

        Console.WriteLine($"Importing bundle: {bundlePath}");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = await importer.ImportAsync(bundlePath, overrideName, allowDuplicate, ct);
        sw.Stop();

        Console.WriteLine();
        Console.WriteLine("Imported.");
        Console.WriteLine($"  Course:        {result.Course.Name} ({result.Course.Id})");
        Console.WriteLine($"  Resources:     {result.ResourceCount}");
        Console.WriteLine($"  ConceptMaps:   {result.ConceptMapCount}");
        Console.WriteLine($"  Chunks:        {result.ChunkCount}");
        Console.WriteLine($"  Structure:     {(result.HasStructure ? "yes" : "no")}");
        Console.WriteLine($"  Time:          {sw.Elapsed.TotalSeconds:F2}s");
        return 0;
    }
}
