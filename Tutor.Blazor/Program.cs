using MindAttic.Legion;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;
using Tutor.Blazor.Components;
using Tutor.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Shared cross-MindAttic LLM credential store (%APPDATA%/MindAttic/LLM/).
// First stop for every LLM API key — a rotation here propagates to every
// MindAttic app that reads from the same folder.
builder.Services.AddSingleton<LlmCredentialStore>();

// MindAttic.Legion — universal LLM-call client. Owns endpoints, auth headers,
// retry/backoff, and circuit breaker for every supported provider. Tutor's
// per-provider services (Claude, OpenAI, …) delegate the wire transport here.
builder.Services.AddLegionClient();

// Platform abstractions
builder.Services.AddSingleton<ISecurePreferences, BlazorSecurePreferences>();
builder.Services.AddSingleton<IAppDataPathProvider, BlazorAppDataPathProvider>();
builder.Services.AddSingleton<IFilePickerService, BlazorFilePickerService>();

// Register OpenAI configuration and service
builder.Services.AddSingleton(sp =>
{
    var prefs = sp.GetRequiredService<ISecurePreferences>();
    return new OpenAIOptions(prefs) { Model = "gpt-4.1-mini" };
});

// OpenAI + Claude services delegate transport to LegionClient — they no longer
// own an HttpClient. Register them as singletons so the conversation history
// (held by OpenAIService) survives across requests.
builder.Services.AddSingleton<OpenAIService>();
builder.Services.AddSingleton<ClaudeService>();

// DeepSeek + Gemini services delegate transport to LegionClient — they no longer
// own an HttpClient. Singletons so any in-class state survives across requests.
builder.Services.AddSingleton<DeepSeekService>();
builder.Services.AddSingleton<GeminiService>();

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

var app = builder.Build();

// Initialize services that need startup initialization
var logStorage = app.Services.GetRequiredService<LogStorageService>();
logStorage.Initialize();

var settings = app.Services.GetRequiredService<SettingsService>();
_ = settings.LoadLogSettingsAsync();

_ = BackgroundQueueService.InitializeAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Tutor.Shared.Components.Pages.Home).Assembly);

app.Run();
