using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;
using Tutor.MAUI.Services;

namespace Tutor.MAUI
{
    /// <summary>
    /// Application entry point and DI composition root for the Tutor platform.
    /// Registers 40+ services spanning AI-powered concept extraction, knowledge graph construction,
    /// vector embeddings, course management, quizzes, news, and background task processing.
    /// HTTP clients for AI services use extended timeouts (2-5 minutes) to accommodate
    /// large document processing and LLM inference latency.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Builds the MAUI application with all services registered in the DI container.
        /// Service categories include platform abstractions, OpenAI integration, content processing,
        /// knowledge graph services, course management, authentication, and background queues.
        /// </summary>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Platform abstractions
            builder.Services.AddSingleton<ISecurePreferences, MauiSecurePreferences>();
            builder.Services.AddSingleton<IAppDataPathProvider, MauiAppDataPathProvider>();
            builder.Services.AddSingleton<IFilePickerService, MauiFilePickerService>();

            // Register OpenAI configuration and service
            builder.Services.AddSingleton(sp =>
            {
                var prefs = sp.GetRequiredService<ISecurePreferences>();
                return new OpenAIOptions(prefs) { Model = "gpt-4.1-mini" };
            });

            // HttpClient + OpenAI service (with extended timeout for AI calls)
            builder.Services.AddHttpClient<OpenAIService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // Claude service (Anthropic)
            builder.Services.AddHttpClient<ClaudeService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // DeepSeek service
            builder.Services.AddHttpClient<DeepSeekService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // Gemini service (Google AI)
            builder.Services.AddHttpClient<GeminiService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // LLM router - routes to the user's selected provider
            builder.Services.AddSingleton<LlmServiceRouter>();

            // Embedding service for RAG (with extended timeout)
            builder.Services.AddHttpClient<EmbeddingService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(2);
            });

            // Content formatter service for auto-formatting imports
            builder.Services.AddSingleton<ContentFormatterService>();

            // Chunking service for splitting content
            builder.Services.AddSingleton<ChunkingService>();

            // LSH service for locality-sensitive hashing (semantic signatures)
            builder.Services.AddSingleton<LSHService>();

            // SimHash service for lexical fingerprinting
            builder.Services.AddSingleton<SimHashService>();

            // Vector store service for storing/searching embeddings
            builder.Services.AddSingleton<VectorStoreService>();

            // Instructions service
            builder.Services.AddSingleton<InstructionsService>();

            // Settings service
            builder.Services.AddSingleton<SettingsService>();

            // Theme service
            builder.Services.AddSingleton<ThemeService>();

            // App UI state service (shared navigation state)
            builder.Services.AddSingleton<AppUiState>();

            // Side nav service (populates side navigation)
            builder.Services.AddSingleton<SideNavService>();

            // File-based resource storage service
            builder.Services.AddSingleton<FileResourceService>();

            // Course service (manages courses and resources)
            builder.Services.AddSingleton<CourseService>();

            // Core Concept service
            builder.Services.AddSingleton<CoreConceptService>();

            // Concept autocomplete service (search suggestions)
            builder.Services.AddSingleton<ConceptAutoCompleteService>();

            // Concept extraction service (LLM-based concept discovery)
            builder.Services.AddSingleton<ConceptExtractionService>();

            // Concept correlation service (relationship discovery)
            builder.Services.AddSingleton<ConceptCorrelationService>();

            // Knowledge graph service (graph management and persistence)
            builder.Services.AddSingleton<KnowledgeGraphService>();

            // User progress service (learning progress tracking)
            builder.Services.AddSingleton<UserProgressService>();

            // ConceptMap storage service (persistence for individual ConceptMaps)
            builder.Services.AddSingleton<ConceptMapStorageService>();

            // ConceptMap collection service (manages collections for courses)
            builder.Services.AddSingleton<ConceptMapCollectionService>();

            // Course structure storage service (persistence for CourseStructures)
            builder.Services.AddSingleton<CourseStructureStorageService>();

            // Concept map service (builds ConceptMap from Resources)
            builder.Services.AddSingleton<ConceptMapService>();

            // Orphan concept linker service (detects and links disconnected concepts)
            builder.Services.AddSingleton<OrphanConceptLinkerService>();

            // Dynamic concept expansion service (expands concepts from user queries)
            builder.Services.AddSingleton<DynamicConceptExpansionService>();

            // Concept merge service (detects and merges duplicate/similar concepts)
            builder.Services.AddSingleton<ConceptMergeService>();

            // Course ConceptMap service (manages course-specific merged maps)
            builder.Services.AddSingleton<CourseConceptMapService>();

            // Section content service (generates hierarchical sections with content)
            builder.Services.AddSingleton<SectionContentService>();

            // Course structure service (generates learning path from ConceptMap)
            builder.Services.AddSingleton<CourseStructureService>();

            // Resource processing service (async pipeline with throttling)
            builder.Services.AddSingleton<ResourceProcessingService>();

            // Knowledge graph build service (async graph building with queue)
            builder.Services.AddSingleton<KnowledgeGraphBuildService>();

            // Logging storage service (persists logs to file)
            builder.Services.AddSingleton<LogStorageService>();

            // Background queue services
            builder.Services.AddSingleton<BackgroundQueueStorageService>();
            builder.Services.AddSingleton<ResourceUploadTaskHandler>();
            builder.Services.AddSingleton<ResourceFormatTaskHandler>();
            builder.Services.AddSingleton<ConceptMapBuildTaskHandler>();

            // Authentication services
            builder.Services.AddSingleton<IAuthController, LocalAuthController>();
            builder.Services.AddSingleton<AuthenticationService>();

            // User storage service (JSON file-based user data)
            builder.Services.AddSingleton<UserStorageService>();

            // News services
            builder.Services.AddHttpClient<OpenNewsApiController>();
            builder.Services.AddSingleton<INewsController>(sp =>
                sp.GetRequiredService<OpenNewsApiController>());
            builder.Services.AddSingleton<NewsService>();

            // Quiz services
            builder.Services.AddSingleton<IQuizController, LocalQuizController>();
            builder.Services.AddSingleton<QuizService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
