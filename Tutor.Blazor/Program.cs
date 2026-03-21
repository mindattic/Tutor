using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;
using Tutor.Blazor.Components;
using Tutor.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

// Concept autocomplete service (search suggestions)
builder.Services.AddSingleton<ConceptAutoCompleteService>();

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

// Concept merge service (detects and merges duplicate/similar concepts)
builder.Services.AddHttpClient<ConceptMergeService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(3);
});

// Course ConceptMap service (manages course-specific merged maps)
builder.Services.AddSingleton<CourseConceptMapService>();

// Section content service (generates hierarchical sections with content)
builder.Services.AddHttpClient<SectionContentService>(client =>
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
