using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MindAttic.Authentication.Web;
using MindAttic.Legion;
using MindAttic.Vault.Configuration;
using MindAttic.Vault.DependencyInjection;
using Tutor.Core.Data;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Auth;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;
using Tutor.Blazor.Components;
using Tutor.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Cloud-native configuration chain. Layered so existing dev workflows keep working:
//   AddJsonFile (already added by WebApplicationBuilder for appsettings.json).
//   AddMindAtticVaultFiles surfaces %APPDATA%\MindAttic\LLM\providers.json on dev
//     machines — the single local source of truth for credentials.
//   AddEnvironmentVariables (already present) picks up App Service Application Settings
//     and Azure Key Vault references in production.
// "Security" is the MindAttic.Authentication trust domain (pepper, bootstrap-token,
// reset-token-key); it is NOT in the default bucket list, so it must be named explicitly
// or the auth secrets never bind and AuthBootstrapper fail-closes.
builder.Configuration
    .AddMindAtticVaultFiles(o => o.Buckets = new[] { "LLM", "Security" });

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// All LLM credentials resolve (read-only) through MindAttic.Vault's
// LlmCredentialResolver: IConfiguration (User Secrets / env / App Service / Key Vault)
// → the shared %APPDATA%\MindAttic\LLM\providers.json. The app never stores keys itself.
builder.Services.AddMindAtticVault(builder.Configuration);

// MindAttic.Legion — universal LLM-call client. Owns endpoints, auth headers,
// retry/backoff, and circuit breaker for every supported provider. Tutor's
// per-provider services (Claude, OpenAI, …) delegate the wire transport here.
builder.Services.AddLegionClient();

// Platform abstractions
builder.Services.AddSingleton<ISecurePreferences, BlazorSecurePreferences>();
builder.Services.AddSingleton<IAppDataPathProvider, BlazorAppDataPathProvider>();
builder.Services.AddSingleton<IFilePickerService, BlazorFilePickerService>();

// Register OpenAI configuration and service. MaxTokens is raised from the
// 2000-token default because ConceptMapService.ExtractConceptsAsync asks the
// model to emit a full JSON document of every concept in a chunk. On dense
// content (e.g. a full book) the response truncates around the default cap
// and produces unparseable output — surfaced upstream as the misleading
// "no concepts extracted" error. gpt-4.1-mini supports up to 16,384 output
// tokens; we leave a small buffer.
builder.Services.AddSingleton(sp =>
{
    var prefs = sp.GetRequiredService<ISecurePreferences>();
    return new OpenAIOptions(prefs) { Model = "gpt-4.1-mini", MaxTokens = 16000 };
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
builder.Services.AddSingleton<ILlmService>(sp => sp.GetRequiredService<LlmServiceRouter>());

// Embedding service for RAG — wire transport delegated to LegionClient.
builder.Services.AddSingleton<EmbeddingService>();

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

// Learning path service (mastery-gated spiral progression + exam eligibility)
builder.Services.AddSingleton<LearningPathService>();

// Final exam service (comprehensive assessment + course completion)
builder.Services.AddSingleton<FinalExamService>();

// Certificate of completion (unsigned today; ICertificateAuthority is the signing seam)
builder.Services.AddSingleton<ICertificateAuthority, LocalCertificateAuthority>();
builder.Services.AddSingleton<CertificateService>();

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
builder.Services.AddSingleton<CourseStructureBuildTaskHandler>();

// --- Auth: the unified, Vault-backed MindAttic.Authentication engine, replacing the interim
//     in-memory LocalAuthController/AuthenticationService singletons. Tutor's FIRST SQL database
//     (auth-only); courses/progress stay JSON. MFA off for now ⇒ MaPolicies.Admin = role-only. ---
var authConnectionString =
    builder.Configuration.GetConnectionString("TutorAuth")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__TutorAuth")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=TutorAuth;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<TutorAuthDbContext>(o => o.UseSqlServer(authConnectionString));   // scoped IAuthDataContext
builder.Services.AddMindAtticAuthentication<TutorAuthDbContext>(builder.Configuration, o =>
{
    o.AppName = "Tutor";                                   // per-app Data Protection trust boundary
    o.IsProduction = !builder.Environment.IsDevelopment();
    if (o.IsProduction)
    {
        o.ConfigureDataProtection = dp =>
        {
            var cred = new Azure.Identity.DefaultAzureCredential();
            var blobUri = builder.Configuration["DataProtection:BlobUri"]
                ?? throw new InvalidOperationException("DataProtection:BlobUri is required in production.");
            var kvKeyId = builder.Configuration["DataProtection:KeyVaultKeyId"]
                ?? throw new InvalidOperationException("DataProtection:KeyVaultKeyId is required in production.");
            dp.PersistKeysToAzureBlobStorage(new Uri(blobUri), cred)
              .ProtectKeysWithAzureKeyVault(new Uri(kvKeyId), cred);
        };
    }
});
// Idempotent legacy Users.json (unsalted SHA256) -> AuthUser import (upgrade-on-login).
builder.Services.AddScoped<AuthUserImportService>();

// User storage service (JSON file-based per-user progress; username-keyed, kept as-is).
builder.Services.AddSingleton<UserStorageService>();

// News services
builder.Services.AddHttpClient<OpenNewsApiController>();
builder.Services.AddSingleton<INewsController>(sp =>
    sp.GetRequiredService<OpenNewsApiController>());
builder.Services.AddSingleton<NewsService>();

// Quiz services
builder.Services.AddSingleton<QuizGenerationService>();
builder.Services.AddSingleton<IQuizController, LocalQuizController>();
builder.Services.AddSingleton<QuizService>();

var app = builder.Build();

// --- Auth startup orchestration: migrate (dev) -> import legacy users -> seed bootstrap admin.
//     MigrateAsync is dev-only (prod DDL runs in CI under db_ddladmin). Import runs before Seed so
//     ryan/erin come in first (SeedAdminAsync no-ops once any user exists). ---
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    if (app.Environment.IsDevelopment())
        await sp.GetRequiredService<TutorAuthDbContext>().Database.MigrateAsync();
    var imported = await sp.GetRequiredService<AuthUserImportService>().ImportAsync();
    Console.WriteLine($"[auth] legacy user import: {imported} account(s) migrated.");
    await sp.GetRequiredService<MindAttic.Authentication.Services.AuthBootstrapper>().SeedAdminAsync();
}

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

// Honor the reverse proxy's forwarded scheme/IP (secure cookie + real client IP) before auth.
var forwardedHeaders = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
};
forwardedHeaders.KnownIPNetworks.Clear();
forwardedHeaders.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeaders);

app.UseStaticFiles();

// authn + authz + forced-step (MustChangePassword -> /account/change-password) + scoped CSP.
app.UseMindAtticAuthentication();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Tutor.Shared.Components.Pages.Home).Assembly);

// MindAttic.Authentication HTTP endpoints — /_ma-auth/{login,mfa-challenge,logout,change-password,reset/*}.
app.MapMindAtticAuthEndpoints();

app.Run();
