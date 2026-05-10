using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;
using Tutor.Tests.Fakes;

namespace Tutor.Tests;

/// <summary>
/// Verifies that the entire DI container resolves correctly —
/// every service registered in MauiProgram / Blazor Program can be constructed
/// without missing dependencies.
/// </summary>
public class DependencyInjectionTests
{
    private static ServiceProvider BuildTestContainer()
    {
        var services = new ServiceCollection();

        // Platform abstractions — use fakes
        var fakePrefs = new FakeSecurePreferences();
        var fakePath = new FakeAppDataPathProvider();
        services.AddSingleton<ISecurePreferences>(fakePrefs);
        services.AddSingleton<IAppDataPathProvider>(fakePath);
        services.AddSingleton<IFilePickerService, FakeFilePickerService>();

        // OpenAI options
        services.AddSingleton(sp =>
        {
            var prefs = sp.GetRequiredService<ISecurePreferences>();
            return new OpenAIOptions(prefs) { Model = "gpt-4.1-mini" };
        });

        // MindAttic.Legion — universal LLM client (used by OpenAIService + ClaudeService)
        services.AddLegionClient();

        // HTTP-based services — register with HttpClient factory
        services.AddSingleton<OpenAIService>();
        services.AddSingleton<ClaudeService>();
        services.AddSingleton<DeepSeekService>();
        services.AddSingleton<GeminiService>();
        services.AddSingleton<EmbeddingService>();

        // LLM router (register both the concrete type and the ILlmService alias
        // that QuizGenerationService and other interface-typed consumers expect)
        services.AddSingleton<LlmServiceRouter>();
        services.AddSingleton<ILlmService>(sp => sp.GetRequiredService<LlmServiceRouter>());

        // Core services (same registration order as MauiProgram)
        services.AddSingleton<ContentFormatterService>();
        services.AddSingleton<ChunkingService>();
        services.AddSingleton<LSHService>();
        services.AddSingleton<SimHashService>();
        services.AddSingleton<VectorStoreService>();
        services.AddSingleton<InstructionsService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<AppUiState>();
        services.AddSingleton<SideNavService>();
        services.AddSingleton<FileResourceService>();
        services.AddSingleton<CourseService>();
        services.AddSingleton<CoreConceptService>();
        services.AddSingleton<ConceptAutoCompleteService>();
        services.AddSingleton<ConceptExtractionService>();
        services.AddSingleton<ConceptCorrelationService>();
        services.AddSingleton<KnowledgeGraphService>();
        services.AddSingleton<UserProgressService>();
        services.AddSingleton<ConceptMapStorageService>();
        services.AddSingleton<ConceptMapCollectionService>();
        services.AddSingleton<CourseStructureStorageService>();
        services.AddSingleton<ConceptMapService>();
        services.AddSingleton<OrphanConceptLinkerService>();
        services.AddSingleton<DynamicConceptExpansionService>();
        services.AddSingleton<ConceptMergeService>();
        services.AddSingleton<CourseConceptMapService>();
        services.AddSingleton<SectionContentService>();
        services.AddSingleton<CourseStructureService>();
        services.AddSingleton<ResourceProcessingService>();
        services.AddSingleton<KnowledgeGraphBuildService>();
        services.AddSingleton<LogStorageService>();

        // Background queue
        services.AddSingleton<BackgroundQueueStorageService>();
        services.AddSingleton<ResourceUploadTaskHandler>();
        services.AddSingleton<ResourceFormatTaskHandler>();
        services.AddSingleton<ConceptMapBuildTaskHandler>();

        // Auth
        services.AddSingleton<IAuthController, LocalAuthController>();
        services.AddSingleton<AuthenticationService>();
        services.AddSingleton<UserStorageService>();

        // News
        services.AddHttpClient<OpenNewsApiController>();
        services.AddSingleton<INewsController>(sp => sp.GetRequiredService<OpenNewsApiController>());
        services.AddSingleton<NewsService>();

        // Quiz
        services.AddSingleton<QuizGenerationService>();
        services.AddSingleton<IQuizController, LocalQuizController>();
        services.AddSingleton<QuizService>();

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }

    [Test]
    public void Container_BuildsWithoutErrors()
    {
        using var sp = BuildTestContainer();
        Assert.That(sp, Is.Not.Null);
    }

    [TestCase(typeof(ISecurePreferences))]
    [TestCase(typeof(IAppDataPathProvider))]
    [TestCase(typeof(IFilePickerService))]
    [TestCase(typeof(IAuthController))]
    [TestCase(typeof(INewsController))]
    [TestCase(typeof(IQuizController))]
    public void Interfaces_ResolveToImplementations(Type serviceType)
    {
        using var sp = BuildTestContainer();
        var instance = sp.GetRequiredService(serviceType);
        Assert.That(instance, Is.Not.Null);
    }

    [TestCase(typeof(OpenAIService))]
    [TestCase(typeof(ClaudeService))]
    [TestCase(typeof(DeepSeekService))]
    [TestCase(typeof(GeminiService))]
    [TestCase(typeof(LlmServiceRouter))]
    [TestCase(typeof(EmbeddingService))]
    [TestCase(typeof(ContentFormatterService))]
    [TestCase(typeof(ChunkingService))]
    [TestCase(typeof(LSHService))]
    [TestCase(typeof(SimHashService))]
    [TestCase(typeof(VectorStoreService))]
    [TestCase(typeof(InstructionsService))]
    [TestCase(typeof(SettingsService))]
    [TestCase(typeof(ThemeService))]
    [TestCase(typeof(AppUiState))]
    [TestCase(typeof(SideNavService))]
    [TestCase(typeof(FileResourceService))]
    [TestCase(typeof(CourseService))]
    [TestCase(typeof(CoreConceptService))]
    [TestCase(typeof(ConceptAutoCompleteService))]
    [TestCase(typeof(ConceptExtractionService))]
    [TestCase(typeof(ConceptCorrelationService))]
    [TestCase(typeof(KnowledgeGraphService))]
    [TestCase(typeof(UserProgressService))]
    [TestCase(typeof(ConceptMapStorageService))]
    [TestCase(typeof(ConceptMapCollectionService))]
    [TestCase(typeof(CourseStructureStorageService))]
    [TestCase(typeof(ConceptMapService))]
    [TestCase(typeof(OrphanConceptLinkerService))]
    [TestCase(typeof(DynamicConceptExpansionService))]
    [TestCase(typeof(ConceptMergeService))]
    [TestCase(typeof(CourseConceptMapService))]
    [TestCase(typeof(SectionContentService))]
    [TestCase(typeof(CourseStructureService))]
    [TestCase(typeof(ResourceProcessingService))]
    [TestCase(typeof(KnowledgeGraphBuildService))]
    [TestCase(typeof(LogStorageService))]
    [TestCase(typeof(BackgroundQueueStorageService))]
    [TestCase(typeof(ResourceUploadTaskHandler))]
    [TestCase(typeof(ResourceFormatTaskHandler))]
    [TestCase(typeof(ConceptMapBuildTaskHandler))]
    [TestCase(typeof(AuthenticationService))]
    [TestCase(typeof(UserStorageService))]
    [TestCase(typeof(NewsService))]
    [TestCase(typeof(QuizService))]
    public void ConcreteService_Resolves(Type serviceType)
    {
        using var sp = BuildTestContainer();
        var instance = sp.GetRequiredService(serviceType);
        Assert.That(instance, Is.Not.Null);
    }

    [Test]
    public void Singletons_ReturnSameInstance()
    {
        using var sp = BuildTestContainer();
        var a = sp.GetRequiredService<SettingsService>();
        var b = sp.GetRequiredService<SettingsService>();
        Assert.That(b, Is.SameAs(a));
    }

    [Test]
    public void LlmServiceRouter_ImplementsILlmService()
    {
        using var sp = BuildTestContainer();
        var router = sp.GetRequiredService<LlmServiceRouter>();
        Assert.That(router, Is.AssignableTo<ILlmService>());
    }

    [Test]
    public void AuthController_IsLocalAuthController()
    {
        using var sp = BuildTestContainer();
        var controller = sp.GetRequiredService<IAuthController>();
        Assert.That(controller, Is.InstanceOf<LocalAuthController>());
    }

    [Test]
    public void QuizController_IsLocalQuizController()
    {
        using var sp = BuildTestContainer();
        var controller = sp.GetRequiredService<IQuizController>();
        Assert.That(controller, Is.InstanceOf<LocalQuizController>());
    }

    [Test]
    public void NewsController_IsOpenNewsApiController()
    {
        using var sp = BuildTestContainer();
        var controller = sp.GetRequiredService<INewsController>();
        Assert.That(controller, Is.InstanceOf<OpenNewsApiController>());
    }
}
