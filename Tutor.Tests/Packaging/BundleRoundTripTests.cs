using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tutor.Core.Services;
using Tutor.Core.Services.Packaging;

namespace Tutor.Tests.Packaging;

/// <summary>
/// Pins course export/import end to end with real exporter/importer instances
/// over a sandboxed store: manifest identity + integrity + provenance, content
/// round-trip with embeddings, double-import independence (TUT-LAW-3), tamper
/// rejection, the forgiving format gate, and legacy-bundle acceptance.
/// </summary>
public class BundleRoundTripTests
{
    private PackagingTestHost host = null!;

    [SetUp]
    public void SetUp() => host = new PackagingTestHost();

    [TearDown]
    public void TearDown() => host.Dispose();

    private string BundlePath(string name = "bundle.tutor") => Path.Combine(host.Sandbox, name);

    [Test]
    public async Task Export_WritesManifest_WithIdentityIntegrityAndProvenance()
    {
        var course = await host.SeedCourseAsync("Dracula");
        var exporter = host.Get<CourseExporter>();

        var manifest = await exporter.ExportAsync(course.Id, BundlePath(), new BundleExportOptions
        {
            CourseVersion = 2,
            Author = "Bram Stoker",
            License = "Public Domain",
            Description = "The classic vampire novel.",
            SourceAttribution = "Project Gutenberg #345",
        });

        Assert.That(File.Exists(BundlePath()), Is.True);
        Assert.That(manifest.CourseKey, Is.EqualTo("dracula"));
        Assert.That(manifest.CourseVersion, Is.EqualTo(2));
        Assert.That(manifest.Sha256, Has.Length.EqualTo(64));
        Assert.That(manifest.ResourceCount, Is.EqualTo(2));
        Assert.That(manifest.ConceptMapCount, Is.EqualTo(2));
        Assert.That(manifest.ChunkCount, Is.EqualTo(6));
        Assert.That(manifest.IncludesEmbeddings, Is.True);
        Assert.That(manifest.Author, Is.EqualTo("Bram Stoker"));
        Assert.That(manifest.License, Is.EqualTo("Public Domain"));
        Assert.That(manifest.SourceAttribution, Is.EqualTo("Project Gutenberg #345"));
    }

    [Test]
    public void DeriveCourseKey_SlugsDisplayNames()
    {
        Assert.That(CourseExporter.DeriveCourseKey("Dracula"), Is.EqualTo("dracula"));
        Assert.That(CourseExporter.DeriveCourseKey("A Tale of Two Cities"), Is.EqualTo("a-tale-of-two-cities"));
        Assert.That(CourseExporter.DeriveCourseKey("  Moby-Dick; Or, The Whale  "), Is.EqualTo("moby-dick-or-the-whale"));
    }

    [Test]
    public async Task Import_RoundTrips_ContentStructureAndEmbeddings()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath());

        var result = await host.Get<BundleImporter>().ImportAsync(BundlePath(), "Dracula (imported)", allowDuplicate: false);

        Assert.That(result.CourseKey, Is.EqualTo("dracula"));
        Assert.That(result.ResourceCount, Is.EqualTo(2));
        Assert.That(result.ConceptMapCount, Is.EqualTo(2));
        Assert.That(result.ChunkCount, Is.EqualTo(6));
        Assert.That(result.HasStructure, Is.True);

        // The imported course is fully usable through the same services the app uses.
        var courseService = host.Get<CourseService>();
        var imported = await courseService.GetCourseAsync(result.Course.Id);
        Assert.That(imported, Is.Not.Null);
        Assert.That(imported!.Name, Is.EqualTo("Dracula (imported)"));

        var resources = await courseService.GetCourseResourcesAsync(imported.Id);
        Assert.That(resources, Has.Count.EqualTo(2));
        Assert.That(resources.Select(r => r.Content), Has.All.Not.Empty);

        var structure = await host.Get<CourseStructureStorageService>().LoadByCourseIdAsync(imported.Id);
        Assert.That(structure, Is.Not.Null);
        Assert.That(structure!.Lessons, Has.Count.EqualTo(1));

        // Embeddings rode along — no LLM pipeline was involved anywhere in this test.
        var vectorStore = host.Get<VectorStoreService>();
        foreach (var resource in resources)
        {
            var chunks = await vectorStore.GetChunksForResourceAsync(resource.Id);
            Assert.That(chunks, Has.Count.EqualTo(3));
            Assert.That(chunks.Select(c => c.Embedding), Has.All.Not.Empty);
            Assert.That(chunks.Select(c => c.CurriculumId), Has.All.EqualTo(imported.Id));
        }
    }

    [Test]
    public async Task ImportingTwice_YieldsTwoIndependentCourses()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath());
        var importer = host.Get<BundleImporter>();

        var first = await importer.ImportAsync(BundlePath(), "Copy A", allowDuplicate: true);
        var second = await importer.ImportAsync(BundlePath(), "Copy B", allowDuplicate: true);

        // Every cross-entity ID was remapped: nothing collides, nothing is shared.
        Assert.That(second.Course.Id, Is.Not.EqualTo(first.Course.Id));
        Assert.That(second.Course.CourseStructureId, Is.Not.EqualTo(first.Course.CourseStructureId));
        Assert.That(first.Course.ResourceIds.Intersect(second.Course.ResourceIds), Is.Empty);

        var courseService = host.Get<CourseService>();
        var firstResources = await courseService.GetCourseResourcesAsync(first.Course.Id);
        var secondResources = await courseService.GetCourseResourcesAsync(second.Course.Id);
        Assert.That(firstResources, Has.Count.EqualTo(2));
        Assert.That(secondResources, Has.Count.EqualTo(2));
        Assert.That(firstResources.Select(r => r.ConceptMapId)
                .Intersect(secondResources.Select(r => r.ConceptMapId)), Is.Empty);

        // Chunks are scoped to their own course; deleting one course's chunks
        // cannot touch the other's.
        var vectorStore = host.Get<VectorStoreService>();
        await vectorStore.RemoveChunksForCurriculumAsync(first.Course.Id);
        var survivor = await vectorStore.GetChunksForResourceAsync(secondResources[0].Id);
        Assert.That(survivor, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task TamperedBundle_IsRejected_WithShaMismatch()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath());

        // Flip one payload entry after export — the manifest hash no longer matches.
        using (var archive = ZipFile.Open(BundlePath(), ZipArchiveMode.Update))
        {
            archive.GetEntry("course.json")!.Delete();
            var entry = archive.CreateEntry("course.json");
            using var writer = new StreamWriter(entry.Open());
            writer.Write("{\"Id\":\"tampered\",\"Name\":\"Tampered\",\"ResourceIds\":[]}");
        }

        var importer = host.Get<BundleImporter>();

        var inspection = await importer.InspectAsync(BundlePath());
        Assert.That(inspection.Validation.IsValid, Is.False);
        Assert.That(inspection.Validation.Errors.Select(e => e.Code),
            Does.Contain(ValidationCodes.ShaMismatch));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            importer.ImportAsync(BundlePath(), null, allowDuplicate: true));
    }

    [Test]
    public async Task FormatNewerThanBuild_IsRefused()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath());
        RewriteManifest(BundlePath(), m => m["FormatVersion"] = BundleManifest.HostMaxFormatVersion + 1);

        var inspection = await host.Get<BundleImporter>().InspectAsync(BundlePath());

        Assert.That(inspection.Validation.IsValid, Is.False);
        Assert.That(inspection.Validation.Errors.Select(e => e.Code),
            Does.Contain(ValidationCodes.FormatTooNew));
    }

    [Test]
    public async Task LegacyBundle_WithoutKeyShaOrNewFields_StillImports()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath());

        // Strip everything a pre-identity bundle didn't have, and mark it format 0.
        RewriteManifest(BundlePath(), m =>
        {
            m.Remove("CourseKey");
            m.Remove("CourseVersion");
            m.Remove("Sha256");
            m.Remove("Author");
            m.Remove("License");
            m.Remove("Description");
            m.Remove("SourceAttribution");
            m["FormatVersion"] = 0;
        });

        var result = await host.Get<BundleImporter>().ImportAsync(BundlePath(), "Legacy Import", allowDuplicate: true);

        Assert.That(result.CourseKey, Is.EqualTo("dracula"), "key is derived from the course name");
        Assert.That(result.CourseVersion, Is.EqualTo(1));
        Assert.That(result.Warnings.Select(w => w.Code),
            Does.Contain(ValidationCodes.LegacyNoKey).And.Contain(ValidationCodes.LegacyNoSha));
        Assert.That(result.ResourceCount, Is.EqualTo(2));
    }

    [Test]
    public async Task UnknownManifestFields_RoundTripThroughExtra()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath());
        RewriteManifest(BundlePath(), m => m["FutureField"] = "from-a-newer-build");

        var inspection = await host.Get<BundleImporter>().InspectAsync(BundlePath());

        Assert.That(inspection.Manifest, Is.Not.Null);
        Assert.That(inspection.Manifest!.Extra, Contains.Key("FutureField"));
        Assert.That(inspection.Manifest.Extra["FutureField"].GetString(), Is.EqualTo("from-a-newer-build"));

        // And writing the manifest back preserves the unknown field verbatim.
        var rewritten = JsonSerializer.Serialize(inspection.Manifest);
        Assert.That(rewritten, Does.Contain("FutureField"));
    }

    [Test]
    public async Task DuplicateName_IsRefused_WithoutAllowDuplicate()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath());

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
            host.Get<BundleImporter>().ImportAsync(BundlePath(), null, allowDuplicate: false));
        Assert.That(ex!.Message, Does.Contain("already exists"));
    }

    private static void RewriteManifest(string bundlePath, Action<JsonObject> mutate)
    {
        using var archive = ZipFile.Open(bundlePath, ZipArchiveMode.Update);
        var entry = archive.GetEntry("manifest.json")!;

        JsonObject manifest;
        using (var reader = new StreamReader(entry.Open(), Encoding.UTF8))
        {
            manifest = (JsonObject)JsonNode.Parse(reader.ReadToEnd())!;
        }
        mutate(manifest);

        entry.Delete();
        var replacement = archive.CreateEntry("manifest.json");
        using var writer = new StreamWriter(replacement.Open(), Encoding.UTF8);
        writer.Write(manifest.ToJsonString());
    }
}
