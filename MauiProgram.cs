using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; 
using System.Net.Http;
using Tutor.Services;
using Tutor.Services.Logging;
using Tutor.Services.Queue;

namespace Tutor
{
    public static class MauiProgram
    {
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


            // Register OpenAI configuration and service
            builder.Services.AddSingleton(new OpenAIOptions
            {
                // Change this if you want a different model
                Model = "gpt-4.1-mini"
            });

            // HttpClient + OpenAI service (with extended timeout for AI calls)
            builder.Services.AddHttpClient<OpenAIService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // Embedding service for RAG (with extended timeout)
            builder.Services.AddHttpClient<EmbeddingService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(2);
            });

            // Content formatter service for auto-formatting imports (with extended timeout)
            builder.Services.AddHttpClient<ContentFormatterService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
            });

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


            // Concept extraction service (LLM-based concept discovery) (with extended timeout)
            builder.Services.AddHttpClient<ConceptExtractionService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
            });

            // Concept correlation service (relationship discovery) (with extended timeout)
            builder.Services.AddHttpClient<ConceptCorrelationService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
            });

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

            // Concept map service (builds ConceptMap from Resources) (with extended timeout)
            builder.Services.AddHttpClient<ConceptMapService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // Orphan concept linker service (detects and links disconnected concepts)
            builder.Services.AddHttpClient<OrphanConceptLinkerService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
            });

            // Dynamic concept expansion service (expands concepts from user queries)
            builder.Services.AddHttpClient<DynamicConceptExpansionService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
            });

            // Course structure service (generates learning path from ConceptMap) (with extended timeout)
            builder.Services.AddHttpClient<CourseStructureService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
            });

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

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
