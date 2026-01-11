using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; 
using System.Net.Http;
using Tutor.Services;

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

            // HttpClient + OpenAI service
            builder.Services.AddHttpClient<OpenAIService>();

            // Embedding service for RAG
            builder.Services.AddHttpClient<EmbeddingService>();

            // Content formatter service for auto-formatting imports
            builder.Services.AddHttpClient<ContentFormatterService>();

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

            // File-based resource storage service
            builder.Services.AddSingleton<FileResourceService>();

            // Course service (manages courses and resources)
            builder.Services.AddSingleton<CourseService>();

            // Core Concept service
            builder.Services.AddSingleton<CoreConceptService>();

            // Concept extraction service (LLM-based concept discovery)
            builder.Services.AddHttpClient<ConceptExtractionService>();

            // Concept correlation service (relationship discovery)
            builder.Services.AddHttpClient<ConceptCorrelationService>();




            // Knowledge graph service (graph management and persistence)
            builder.Services.AddSingleton<KnowledgeGraphService>();

            // Table of contents service (learning path generation)
            builder.Services.AddSingleton<TableOfContentsService>();

            // Resource processing service (async pipeline with throttling)
            builder.Services.AddSingleton<ResourceProcessingService>();

            // Knowledge graph build service (async graph building with queue)
            builder.Services.AddSingleton<KnowledgeGraphBuildService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
