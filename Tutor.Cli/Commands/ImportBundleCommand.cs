using Tutor.Core.Services.Packaging;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor import-bundle &lt;file.tutor&gt; [--course "Override Name"] [--allow-duplicate]</c>
/// (also aliased as <c>tutor install</c>) — restores a course from a .tutor bundle
/// (legacy .tutorcourse files are also accepted). The bundle is validated
/// (manifest, format gate, SHA-256 integrity), planned against the installed
/// registry (upgrade vs. no-op vs. refused downgrade), and on install the
/// verbatim bundle is retained for re-share. Skips the LLM pipeline entirely
/// because the embeddings ride along in the bundle, so it's typically &lt;1s
/// regardless of book size.
/// </summary>
public sealed class ImportBundleCommand
{
    private readonly CourseInstallService installService;

    public ImportBundleCommand(CourseInstallService installService)
    {
        this.installService = installService;
    }

    /// <summary>Returns 0 on success/no-op, 1 when the bundle is invalid or refused, 64 on usage errors.</summary>
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine(
                "Usage: tutor import-bundle <file.tutor> [--course \"Override Name\"] [--allow-duplicate]");
            return 64;
        }

        var bundlePath = positionals[0];
        var overrideName = options.Get("course");
        var allowDuplicate = options.ContainsKey("allow-duplicate");

        Console.WriteLine($"Importing bundle: {bundlePath}");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var outcome = await installService.InstallAsync(bundlePath, overrideName, allowDuplicate, ct);
        sw.Stop();

        foreach (var warning in outcome.Preview.Validation.Warnings)
            Console.Error.WriteLine($"  WARN {warning}");

        if (!outcome.Preview.Validation.IsValid)
        {
            Console.Error.WriteLine("Bundle rejected:");
            foreach (var error in outcome.Preview.Validation.Errors)
                Console.Error.WriteLine($"  {error}");
            return 1;
        }

        if (!outcome.DidInstall)
        {
            Console.WriteLine(outcome.Preview.Plan!.Reason);
            return outcome.Preview.Plan.Kind == InstallPlanKind.NoOpAlreadyInstalled ? 0 : 1;
        }

        var result = outcome.Result!;
        Console.WriteLine();
        Console.WriteLine($"Imported ({outcome.Preview.Plan!.Kind}).");
        Console.WriteLine($"  Course:        {result.Course.Name} ({result.Course.Id})");
        Console.WriteLine($"  Identity:      {result.CourseKey} v{result.CourseVersion}");
        Console.WriteLine($"  Resources:     {result.ResourceCount}");
        Console.WriteLine($"  ConceptMaps:   {result.ConceptMapCount}");
        Console.WriteLine($"  Chunks:        {result.ChunkCount}");
        Console.WriteLine($"  Structure:     {(result.HasStructure ? "yes" : "no")}");
        Console.WriteLine($"  Retained at:   {outcome.Installed!.BlobPath}");
        Console.WriteLine($"  Time:          {sw.Elapsed.TotalSeconds:F2}s");
        return 0;
    }
}
