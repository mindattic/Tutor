using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Packaging;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Packaging;

/// <summary>
/// Shared fixture for packaging tests: a fully sandboxed service graph (temp
/// directories for every DataStorageSettings path + fake secure preferences) so
/// real CourseExporter/BundleImporter/CourseInstallService instances run their
/// production code paths without touching %LocalAppData% or any LLM.
/// </summary>
public sealed class PackagingTestHost : IDisposable
{
    private readonly string sandbox;
    private readonly string? prevKnowledgeBases;
    private readonly string? prevVectorStore;
    private readonly string? prevCourseStructures;
    private readonly string? prevLsh;
    private readonly FakeAppDataPathProvider pathProvider;
    private readonly ServiceProvider provider;

    public PackagingTestHost()
    {
        sandbox = Path.Combine(Path.GetTempPath(), "tutor-packaging-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);

        prevKnowledgeBases = DataStorageSettings.KnowledgeBasesPath;
        prevVectorStore = DataStorageSettings.VectorStorePath;
        prevCourseStructures = DataStorageSettings.CourseStructuresPath;
        prevLsh = DataStorageSettings.LSHPath;
        DataStorageSettings.KnowledgeBasesPath = Path.Combine(sandbox, "ConceptMaps");
        DataStorageSettings.VectorStorePath = Path.Combine(sandbox, "VectorStore");
        DataStorageSettings.CourseStructuresPath = Path.Combine(sandbox, "CourseStructures");
        DataStorageSettings.LSHPath = Path.Combine(sandbox, "LSH");

        pathProvider = new FakeAppDataPathProvider();

        var services = new ServiceCollection();
        services.AddSingleton<ISecurePreferences>(new FakeSecurePreferences());
        services.AddSingleton<IAppDataPathProvider>(pathProvider);
        services.AddSingleton<IFilePickerService, FakeFilePickerService>();
        services.AddSingleton(sp => new OpenAIOptions(sp.GetRequiredService<ISecurePreferences>()));
        services.AddLegionClient();
        services.AddSingleton<OpenAIService>();
        services.AddSingleton<ClaudeService>();
        services.AddSingleton<DeepSeekService>();
        services.AddSingleton<GeminiService>();
        services.AddSingleton<EmbeddingService>();
        services.AddSingleton<LlmServiceRouter>();
        services.AddSingleton<ILlmService>(sp => sp.GetRequiredService<LlmServiceRouter>());
        services.AddSingleton<ContentFormatterService>();
        services.AddSingleton<ChunkingService>();
        services.AddSingleton<LSHService>();
        services.AddSingleton<SimHashService>();
        services.AddSingleton<VectorStoreService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<FileResourceService>();
        services.AddSingleton<CourseService>();
        services.AddSingleton<ConceptMapStorageService>();
        services.AddSingleton<ConceptMapCollectionService>();
        services.AddSingleton<CourseStructureStorageService>();

        // The packaging lifecycle under test — same registrations as both front doors.
        services.AddSingleton<CourseExporter>();
        services.AddSingleton<BundleImporter>();
        services.AddSingleton<CourseDeleteService>();
        services.AddSingleton<InstalledCourseRegistry>();
        services.AddSingleton<CourseBlobStore>();
        services.AddSingleton<CourseInstallService>();

        provider = services.BuildServiceProvider();
    }

    public string Sandbox => sandbox;
    public T Get<T>() where T : notnull => provider.GetRequiredService<T>();

    /// <summary>
    /// Seeds a complete, deterministic course (two resources, two concept maps,
    /// a structure with one lesson, and chunks carrying fake embeddings) through
    /// the same services the app uses, and returns it ready for export.
    /// </summary>
    public async Task<Course> SeedCourseAsync(string name = "Dracula")
    {
        var courseService = Get<CourseService>();
        var conceptMapStorage = Get<ConceptMapStorageService>();
        var structureStorage = Get<CourseStructureStorageService>();
        var vectorStore = Get<VectorStoreService>();

        var course = await courseService.CreateCourseAsync(name, "A seeded test course.");
        var resourceIds = new List<string>();

        for (var i = 1; i <= 2; i++)
        {
            var resource = new CourseResource
            {
                Title = $"{name} Part {i}",
                Content = $"Original text of part {i}.",
                FormattedContent = $"# Part {i}\n\nFormatted text of part {i}.",
                FileName = $"part{i}.txt",
            };

            var map = new ConceptMap
            {
                Name = $"{name} Part {i} Concepts",
                ResourceId = resource.Id,
            };
            map.Concepts.Add(new Concept
            {
                Title = $"Concept {i}",
                Summary = $"Summary {i}",
                ConceptMapId = map.Id,
                SourceResourceIds = [resource.Id],
            });
            await conceptMapStorage.SaveAsync(map);

            resource.ConceptMapId = map.Id;
            await courseService.SaveResourceAsync(resource);
            resourceIds.Add(resource.Id);

            var chunks = Enumerable.Range(0, 3).Select(n => new ContentChunk
            {
                ResourceId = resource.Id,
                CurriculumId = course.Id,
                ChunkIndex = n,
                Content = $"Chunk {n} of part {i}.",
                Embedding = [0.1f * i, 0.2f * n, 0.3f, 0.4f],
                SourceTitle = resource.Title,
            }).ToList();
            await vectorStore.StoreChunksAsync(resource.Id, course.Id, chunks);
        }

        // Attach resources directly (AddResourceToCourseAsync would re-chunk and
        // re-embed, which needs a live LLM key — the chunks above already carry
        // their embeddings, exactly like a finished course).
        course.ResourceIds = resourceIds;
        course.ConceptMapCollectionId = $"collection_{course.Id}";
        await courseService.SaveCourseAsync(course);

        var refreshed = await courseService.GetCourseAsync(course.Id)
            ?? throw new InvalidOperationException("Seeded course vanished.");

        var structure = new CourseStructure
        {
            CourseId = refreshed.Id,
            KnowledgeBaseId = refreshed.ConceptMapCollectionId ?? "",
        };
        structure.Lessons.Add(new Lesson { Title = "Lesson 1", CourseStructureId = structure.Id });
        await structureStorage.SaveAsync(structure);

        refreshed.CourseStructureId = structure.Id;
        await courseService.SaveCourseAsync(refreshed);

        return refreshed;
    }

    public void Dispose()
    {
        DataStorageSettings.KnowledgeBasesPath = prevKnowledgeBases;
        DataStorageSettings.VectorStorePath = prevVectorStore;
        DataStorageSettings.CourseStructuresPath = prevCourseStructures;
        DataStorageSettings.LSHPath = prevLsh;

        provider.Dispose();
        pathProvider.Dispose();
        try { Directory.Delete(sandbox, recursive: true); } catch { }
    }
}
