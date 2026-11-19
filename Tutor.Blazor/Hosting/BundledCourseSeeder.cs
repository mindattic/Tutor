using Tutor.Core.Services;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Packaging;

namespace Tutor.Blazor.Hosting;

/// <summary>
/// Installs the bundled courses named in Tutor's <see cref="TutorSettings.SeedBundledCourses"/> setting at startup
/// (TUT-T60, the small first step of improvement-plan item 1.4). The <c>.tutor</c> files ship with the site under
/// <see cref="BundleFolder"/>; each goes through <see cref="CourseInstallService"/>, the same install path the Library
/// page and the CLI use, so a bundle already in the install registry is skipped and a restart installs nothing.
/// The setting is empty by default: the whole-file vector store (plan item 1.2) makes every course cost memory on the
/// shared plan, so the host lists only the courses it can afford.
/// </summary>
public static class BundledCourseSeeder
{
    /// <summary>Folder under the content root that holds the bundles published with the site.</summary>
    public const string BundleFolder = "BundledCourses";

    /// <summary>Starts seeding in the background so a slow install never delays the site answering.</summary>
    public static void Start(IServiceProvider services, string contentRoot) =>
        _ = Task.Run(async () =>
        {
            try { await SeedAsync(services, contentRoot); }
            catch (Exception ex) { Log.Error($"BundledCourseSeeder: seeding failed: {ex.Message}"); }
        });

    /// <summary>Installs every listed bundle that is not installed yet. Returns how many were installed.</summary>
    public static async Task<int> SeedAsync(IServiceProvider services, string contentRoot, CancellationToken ct = default)
    {
        var wanted = Parse(services.GetRequiredService<TutorSettingsSource>().Current.SeedBundledCourses);
        if (wanted.Count == 0) return 0;

        var folder = Path.Combine(contentRoot, BundleFolder);
        if (!Directory.Exists(folder))
        {
            Log.Warn($"BundledCourseSeeder: {wanted.Count} bundle(s) requested but {folder} does not exist.");
            return 0;
        }

        var all = wanted.Contains("*");
        var bundles = Directory.GetFiles(folder, "*.tutor")
            .Where(f => all || wanted.Contains(Path.GetFileNameWithoutExtension(f)))
            .OrderBy(f => new FileInfo(f).Length)
            .ToList();
        foreach (var name in wanted.Where(n => n != "*" && !bundles.Any(b => string.Equals(Path.GetFileNameWithoutExtension(b), n, StringComparison.OrdinalIgnoreCase))))
            Log.Warn($"BundledCourseSeeder: no bundle named '{name}' in {folder}.");

        var installer = services.GetRequiredService<CourseInstallService>();
        var installed = 0;
        foreach (var bundle in bundles)
        {
            ct.ThrowIfCancellationRequested();
            var outcome = await installer.InstallAsync(bundle, ct: ct);
            if (outcome.DidInstall)
            {
                installed++;
                Log.Info($"BundledCourseSeeder: installed '{Path.GetFileName(bundle)}'.");
            }
            else
            {
                var reason = outcome.Preview.Plan?.Reason ?? string.Join("; ", outcome.Preview.Validation.Errors.Select(e => e.Message));
                Log.Info($"BundledCourseSeeder: skipped '{Path.GetFileName(bundle)}': {reason}");
            }
        }
        return installed;
    }

    /// <summary>The setting's names, separated by <c>|</c> (course names contain commas and semicolons); <c>*</c> means all.</summary>
    public static HashSet<string> Parse(string? value) =>
        new((value ?? string.Empty).Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            StringComparer.OrdinalIgnoreCase);
}
