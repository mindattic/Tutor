using Tutor.Core.Services;
using Tutor.Core.Services.Packaging;

namespace Tutor.Tests.Packaging;

/// <summary>
/// Pins the load/unload/share lifecycle around the installed-courses registry:
/// install records a row + retains the verbatim blob, unload soft-disables
/// without destroying data or progress, load re-enables, re-install no-ops,
/// downgrades are refused, and Remove is the only hard cascade.
/// </summary>
public class CourseLifecycleRegistryTests
{
    private PackagingTestHost host = null!;

    [SetUp]
    public void SetUp() => host = new PackagingTestHost();

    [TearDown]
    public void TearDown() => host.Dispose();

    private string BundlePath(string name = "bundle.tutor") => Path.Combine(host.Sandbox, name);

    private async Task<string> ExportSeededAsync(string name = "Dracula", int version = 1)
    {
        var course = await host.SeedCourseAsync(name);
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath(),
            new BundleExportOptions { CourseVersion = version });
        return course.Id;
    }

    [Test]
    public async Task Install_RecordsRegistryRow_AndRetainsVerbatimBlob()
    {
        await ExportSeededAsync();
        var installService = host.Get<CourseInstallService>();

        var outcome = await installService.InstallAsync(BundlePath(), "Dracula (installed)");

        Assert.That(outcome.DidInstall, Is.True);
        Assert.That(outcome.Preview.Plan!.Kind, Is.EqualTo(InstallPlanKind.Install));

        // E3: the registry knows what is installed — name, key, version, hash, state, date.
        var rows = await host.Get<InstalledCourseRegistry>().GetAllAsync();
        Assert.That(rows, Has.Count.EqualTo(1));
        var row = rows[0];
        Assert.That(row.CourseKey, Is.EqualTo("dracula"));
        Assert.That(row.CourseVersion, Is.EqualTo(1));
        Assert.That(row.Name, Is.EqualTo("Dracula (installed)"));
        Assert.That(row.Sha256, Has.Length.EqualTo(64));
        Assert.That(row.Enabled, Is.True);
        Assert.That(row.InstalledUtc, Is.Not.EqualTo(default(DateTime)));

        // E4: the verbatim .tutor is retained byte-for-byte for re-share.
        Assert.That(row.BlobPath, Is.Not.Null);
        Assert.That(File.Exists(row.BlobPath!), Is.True);
        Assert.That(File.ReadAllBytes(row.BlobPath!), Is.EqualTo(File.ReadAllBytes(BundlePath())));

        var blobStore = host.Get<CourseBlobStore>();
        Assert.That(blobStore.Exists("dracula", 1), Is.True);
        using var stream = blobStore.Open("dracula", 1);
        Assert.That(stream, Is.Not.Null);
    }

    [Test]
    public async Task ReinstallingSameVersion_NoOps_WithoutDuplicating()
    {
        await ExportSeededAsync();
        var installService = host.Get<CourseInstallService>();

        await installService.InstallAsync(BundlePath(), "First");
        var second = await installService.InstallAsync(BundlePath(), "Second");

        Assert.That(second.DidInstall, Is.False);
        Assert.That(second.Preview.Plan!.Kind, Is.EqualTo(InstallPlanKind.NoOpAlreadyInstalled));
        Assert.That(await host.Get<InstalledCourseRegistry>().GetAllAsync(), Has.Count.EqualTo(1));
    }

    [Test]
    public async Task ExplicitDuplicate_InstallsSideBySide_AsIndependentCourse()
    {
        await ExportSeededAsync();
        var installService = host.Get<CourseInstallService>();

        var first = await installService.InstallAsync(BundlePath(), "Copy A");
        var second = await installService.InstallAsync(BundlePath(), "Copy B", allowDuplicate: true);

        Assert.That(second.DidInstall, Is.True);
        Assert.That(second.Preview.Plan!.Kind, Is.EqualTo(InstallPlanKind.InstallSideBySide));
        Assert.That(second.Installed!.CourseId, Is.Not.EqualTo(first.Installed!.CourseId));
        Assert.That(await host.Get<InstalledCourseRegistry>().GetAllAsync(), Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Downgrade_IsRefused()
    {
        var seededId = await ExportSeededAsync(version: 2);
        var installService = host.Get<CourseInstallService>();
        await installService.InstallAsync(BundlePath(), "Dracula v2");

        // Export the same course again as the older v1 and try to install it.
        await host.Get<CourseExporter>().ExportAsync(seededId, BundlePath("older.tutor"),
            new BundleExportOptions { CourseVersion = 1 });
        var downgrade = await installService.InstallAsync(BundlePath("older.tutor"), "Dracula v1");

        Assert.That(downgrade.DidInstall, Is.False);
        Assert.That(downgrade.Preview.Plan!.Kind, Is.EqualTo(InstallPlanKind.RejectDowngrade));
    }

    [Test]
    public async Task NewerVersion_InstallsAsUpgrade()
    {
        var seededId = await ExportSeededAsync(version: 1);
        var installService = host.Get<CourseInstallService>();
        await installService.InstallAsync(BundlePath(), "Dracula v1");

        await host.Get<CourseExporter>().ExportAsync(seededId, BundlePath("newer.tutor"),
            new BundleExportOptions { CourseVersion = 2 });
        var upgrade = await installService.InstallAsync(BundlePath("newer.tutor"), "Dracula v2");

        Assert.That(upgrade.DidInstall, Is.True);
        Assert.That(upgrade.Preview.Plan!.Kind, Is.EqualTo(InstallPlanKind.Upgrade));

        var versions = await host.Get<InstalledCourseRegistry>().GetByKeyAsync("dracula");
        Assert.That(versions.Select(v => v.CourseVersion), Is.EquivalentTo(new[] { 1, 2 }));
    }

    [Test]
    public async Task Unload_SoftDisables_WithoutDestroyingDataOrProgress()
    {
        await ExportSeededAsync();
        var installService = host.Get<CourseInstallService>();
        var outcome = await installService.InstallAsync(BundlePath(), "Dracula (installed)");
        var courseId = outcome.Installed!.CourseId;
        var registry = host.Get<InstalledCourseRegistry>();

        // E2: unload hides, it doesn't erase.
        Assert.That(await installService.UnloadAsync(courseId), Is.True);
        Assert.That(await registry.IsEnabledAsync(courseId), Is.False);

        var courseService = host.Get<CourseService>();
        Assert.That(await courseService.GetCourseAsync(courseId), Is.Not.Null, "course data survives unload");
        Assert.That(await courseService.GetCourseResourcesAsync(courseId), Has.Count.EqualTo(2));
        Assert.That(await host.Get<CourseStructureStorageService>().LoadByCourseIdAsync(courseId), Is.Not.Null);

        // Load brings it straight back.
        Assert.That(await installService.LoadAsync(courseId), Is.True);
        Assert.That(await registry.IsEnabledAsync(courseId), Is.True);
    }

    [Test]
    public async Task Remove_HardCascades_DataRegistryRowAndBlob()
    {
        await ExportSeededAsync();
        var installService = host.Get<CourseInstallService>();
        var outcome = await installService.InstallAsync(BundlePath(), "Dracula (installed)");
        var courseId = outcome.Installed!.CourseId;
        var blobPath = outcome.Installed.BlobPath!;
        var courseService = host.Get<CourseService>();
        var resources = await courseService.GetCourseResourcesAsync(courseId);

        var plan = await installService.RemoveAsync(courseId);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.ResourceCount, Is.EqualTo(2));
        Assert.That(plan.ConceptMapCount, Is.EqualTo(2));
        Assert.That(plan.HasStructure, Is.True);

        // D3: the cascade removed the course and every derivative.
        Assert.That(await courseService.GetCourseAsync(courseId), Is.Null);
        Assert.That(await courseService.GetCourseResourcesAsync(courseId), Is.Empty);
        Assert.That(await host.Get<CourseStructureStorageService>().LoadByCourseIdAsync(courseId), Is.Null);
        var vectorStore = host.Get<VectorStoreService>();
        foreach (var resource in resources)
            Assert.That(await vectorStore.GetChunksForResourceAsync(resource.Id), Is.Empty);

        // …including the registry row and the retained blob.
        Assert.That(await host.Get<InstalledCourseRegistry>().GetByCourseIdAsync(courseId), Is.Null);
        Assert.That(File.Exists(blobPath), Is.False);
    }

    [Test]
    public async Task DeleteService_DryRunPlan_DoesNotDeleteAnything()
    {
        var courseId = await ExportSeededAsync();
        var deleteService = host.Get<CourseDeleteService>();

        var plan = await deleteService.PlanAsync(courseId);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.ResourceCount, Is.EqualTo(2));
        Assert.That(await host.Get<CourseService>().GetCourseAsync(courseId), Is.Not.Null);
    }

    [Test]
    public async Task InvalidBundle_NeverTouchesRegistryOrStore()
    {
        await ExportSeededAsync();

        // Corrupt the bundle by truncating it.
        var bytes = File.ReadAllBytes(BundlePath());
        File.WriteAllBytes(BundlePath("corrupt.tutor"), bytes[..(bytes.Length / 2)]);

        var installService = host.Get<CourseInstallService>();
        Assert.CatchAsync(() => installService.InstallAsync(BundlePath("corrupt.tutor"), "Corrupt"));
        Assert.That(await host.Get<InstalledCourseRegistry>().GetAllAsync(), Is.Empty);
    }
}
