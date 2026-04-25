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

        // LLM router
        services.AddSingleton<LlmServiceRouter>();

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
        services.AddSingleton<IQuizController, LocalQuizController>();
        services.AddSingleton<QuizService>();

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }

    [Fact]
    public void Container_BuildsWithoutErrors()
    {
        using var sp = BuildTestContainer();
        Assert.NotNull(sp);
    }

    [Theory]
    [InlineData(typeof(ISecurePreferences))]
    [InlineData(typeof(IAppDataPathProvider))]
    [InlineData(typeof(IFilePickerService))]
    [InlineData(typeof(IAuthController))]
    [InlineData(typeof(INewsController))]
    [InlineData(typeof(IQuizController))]
    public void Interfaces_ResolveToImplementations(Type serviceType)
    {
        using var sp = BuildTestContainer();
        var instance = sp.GetRequiredService(serviceType);
        Assert.NotNull(instance);
    }

    [Theory]
    [InlineData(typeof(OpenAIService))]
    [InlineData(typeof(ClaudeService))]
    [InlineData(typeof(DeepSeekService))]
    [InlineData(typeof(GeminiService))]
    [InlineData(typeof(LlmServiceRouter))]
    [InlineData(typeof(EmbeddingService))]
    [InlineData(typeof(ContentFormatterService))]
    [InlineData(typeof(ChunkingService))]
    [InlineData(typeof(LSHService))]
    [InlineData(typeof(SimHashService))]
    [InlineData(typeof(VectorStoreService))]
    [InlineData(typeof(InstructionsService))]
    [InlineData(typeof(SettingsService))]
    [InlineData(typeof(ThemeService))]
    [InlineData(typeof(AppUiState))]
    [InlineData(typeof(SideNavService))]
    [InlineData(typeof(FileResourceService))]
    [InlineData(typeof(CourseService))]
    [InlineData(typeof(CoreConceptService))]
    [InlineData(typeof(ConceptAutoCompleteService))]
    [InlineData(typeof(ConceptExtractionService))]
    [InlineData(typeof(ConceptCorrelationService))]
    [InlineData(typeof(KnowledgeGraphService))]
    [InlineData(typeof(UserProgressService))]
    [InlineData(typeof(ConceptMapStorageService))]
    [InlineData(typeof(ConceptMapCollectionService))]
    [InlineData(typeof(CourseStructureStorageService))]
    [InlineData(typeof(ConceptMapService))]
    [InlineData(typeof(OrphanConceptLinkerService))]
    [InlineData(typeof(DynamicConceptExpansionService))]
    [InlineData(typeof(ConceptMergeService))]
    [InlineData(typeof(CourseConceptMapService))]
    [InlineData(typeof(SectionContentService))]
    [InlineData(typeof(CourseStructureService))]
    [InlineData(typeof(ResourceProcessingService))]
    [InlineData(typeof(KnowledgeGraphBuildService))]
    [InlineData(typeof(LogStorageService))]
    [InlineData(typeof(BackgroundQueueStorageService))]
    [InlineData(typeof(ResourceUploadTaskHandler))]
    [InlineData(typeof(ResourceFormatTaskHandler))]
    [InlineData(typeof(ConceptMapBuildTaskHandler))]
    [InlineData(typeof(AuthenticationService))]
    [InlineData(typeof(UserStorageService))]
    [InlineData(typeof(NewsService))]
    [InlineData(typeof(QuizService))]
    public void ConcreteService_Resolves(Type serviceType)
    {
        using var sp = BuildTestContainer();
        var instance = sp.GetRequiredService(serviceType);
        Assert.NotNull(instance);
    }

    [Fact]
    public void Singletons_ReturnSameInstance()
    {
        using var sp = BuildTestContainer();
        var a = sp.GetRequiredService<SettingsService>();
        var b = sp.GetRequiredService<SettingsService>();
        Assert.Same(a, b);
    }

    [Fact]
    public void LlmServiceRouter_ImplementsILlmService()
    {
        using var sp = BuildTestContainer();
        var router = sp.GetRequiredService<LlmServiceRouter>();
        Assert.IsAssignableFrom<ILlmService>(router);
    }

    [Fact]
    public void AuthController_IsLocalAuthController()
    {
        using var sp = BuildTestContainer();
        var controller = sp.GetRequiredService<IAuthController>();
        Assert.IsType<LocalAuthController>(controller);
    }

    [Fact]
    public void QuizController_IsLocalQuizController()
    {
        using var sp = BuildTestContainer();
        var controller = sp.GetRequiredService<IQuizController>();
        Assert.IsType<LocalQuizController>(controller);
    }

    [Fact]
    public void NewsController_IsOpenNewsApiController()
    {
        using var sp = BuildTestContainer();
        var controller = sp.GetRequiredService<INewsController>();
        Assert.IsType<OpenNewsApiController>(controller);
    }
}
