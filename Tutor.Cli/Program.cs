using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Cli.Commands;
using Tutor.Cli.Export;
using Tutor.Cli.Gutenberg;
using Tutor.Cli.Pipeline;
using Tutor.Cli.Services;
using Tutor.Core.Parsers;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

// Forward Tutor.Core's Log.* output to stderr so the CLI shows progress and
// failures live. Trace is gated by the --verbose flag below.
var verbose = args.Contains("--verbose");
if (verbose)
{
    Log.Settings.LogTrace = true;
}
Log.Store.EntryAdded += (_, entry) =>
{
    var prefix = entry.Severity.ToString().ToUpperInvariant();
    Console.Error.WriteLine($"  [{prefix}] {entry.Message}");
    if (entry.Exception != null)
        Console.Error.WriteLine($"          {entry.Exception.GetType().Name}: {entry.Exception.Message}");
};
args = args.Where(a => a != "--verbose").ToArray();

var services = new ServiceCollection();

// Shared cross-MindAttic LLM credential store + Legion HTTP client.
services.AddSingleton<LlmCredentialStore>();
services.AddLegionClient();

// Platform abstractions — CLI variants of the Blazor implementations.
services.AddSingleton<ISecurePreferences, CliSecurePreferences>();
services.AddSingleton<IAppDataPathProvider, CliAppDataPathProvider>();
services.AddSingleton<IFilePickerService, CliFilePickerService>();

// LLM provider services — same registrations as Tutor.Blazor.Program.cs except
// MaxTokens is raised: ConceptMapService asks the model to emit a full JSON
// document of every concept in the chunk, which truncates around the default
// 2000-token cap and yields an unparseable response. gpt-4.1-mini supports up
// to 16,384 output tokens; we leave headroom under that.
services.AddSingleton(sp =>
{
    var prefs = sp.GetRequiredService<ISecurePreferences>();
    return new OpenAIOptions(prefs) { Model = "gpt-4.1-mini", MaxTokens = 16000 };
});
services.AddSingleton<OpenAIService>();
services.AddSingleton<ClaudeService>();
services.AddSingleton<DeepSeekService>();
services.AddSingleton<GeminiService>();
services.AddSingleton<LlmServiceRouter>();

// Pipeline services — must mirror Blazor wiring so the CLI produces courses
// that the Blazor UI can read without translation.
services.AddSingleton<EmbeddingService>();
services.AddSingleton<ContentFormatterService>();
services.AddSingleton<ChunkingService>();
services.AddSingleton<LSHService>();
services.AddSingleton<SimHashService>();
services.AddSingleton<VectorStoreService>();
services.AddSingleton<SettingsService>();
services.AddSingleton<FileResourceService>();
services.AddSingleton<CourseService>();
services.AddSingleton<CoreConceptService>();
services.AddSingleton<ConceptExtractionService>();
services.AddSingleton<ConceptCorrelationService>();
services.AddSingleton<KnowledgeGraphService>();
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
services.AddSingleton<UserProgressService>();

// Book parsers. ParserRegistry picks them up via IEnumerable<IBookParser>.
// Phase A — pure-managed (NuGet-based):
services.AddSingleton<IBookParser, TxtBookParser>();
services.AddSingleton<IBookParser, HtmlBookParser>();
services.AddSingleton<IBookParser, EpubBookParser>();
services.AddSingleton<IBookParser, PdfBookParser>();
services.AddSingleton<IBookParser, DocxBookParser>();
// Phase B — shell-out to external tools (Calibre, LibreOffice). Each errors
// cleanly with install instructions if the tool isn't on the machine.
services.AddSingleton<IBookParser, MobiBookParser>();
services.AddSingleton<IBookParser, LibreOfficeBookParser>();
services.AddSingleton<ParserRegistry>();

// CLI-specific services.
services.AddHttpClient<GutenbergFetcher>();
services.AddSingleton<BookImportPipeline>();
services.AddSingleton<CourseExporter>();
services.AddSingleton<BundleImporter>();
services.AddSingleton<ImportGutenbergCommand>();
services.AddSingleton<GutenbergTop10Command>();
services.AddSingleton<ImportFileCommand>();
services.AddSingleton<ExportCommand>();
services.AddSingleton<ImportBundleCommand>();
services.AddSingleton<ListCommand>();
services.AddSingleton<FetchOnlyCommand>();
services.AddSingleton<DeleteCommand>();
services.AddSingleton<ParseOnlyCommand>();

using var provider = services.BuildServiceProvider();

var verb = args.Length == 0 ? "help" : args[0].ToLowerInvariant();
var rest = args.Skip(1).ToArray();

try
{
    return verb switch
    {
        "gutenberg"     => await provider.GetRequiredService<ImportGutenbergCommand>().RunAsync(rest),
        "gutenberg-top10" => await provider.GetRequiredService<GutenbergTop10Command>().RunAsync(rest),
        "import"        => await provider.GetRequiredService<ImportFileCommand>().RunAsync(rest),
        "export"        => await provider.GetRequiredService<ExportCommand>().RunAsync(rest),
        "import-bundle" => await provider.GetRequiredService<ImportBundleCommand>().RunAsync(rest),
        "list"          => await provider.GetRequiredService<ListCommand>().RunAsync(rest),
        "delete"        => await provider.GetRequiredService<DeleteCommand>().RunAsync(rest),
        "fetch"         => await provider.GetRequiredService<FetchOnlyCommand>().RunAsync(rest),
        "parse"         => await provider.GetRequiredService<ParseOnlyCommand>().RunAsync(rest),
        "help" or "-h" or "--help" => PrintHelp(),
        _ => UnknownVerb(verb),
    };
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FATAL: {ex.Message}");
    Console.Error.WriteLine(ex.StackTrace);
    return 1;
}

static int PrintHelp()
{
    Console.WriteLine("""
        tutor — MindAttic Tutor CLI

        USAGE
          tutor gutenberg <book-id> [--course "Name"] [--description "..."] [--allow-duplicate]
              Download a book from Project Gutenberg by ID and import it as a new course.
              Example: tutor gutenberg 2701 --course "Moby Dick"

          tutor gutenberg-top10 [--dry-run] [--allow-duplicate]
              Drive the curated top-10 (Moby Dick, Pride and Prejudice, Frankenstein,
              Sherlock Holmes, Alice, Dorian Gray, Tom Sawyer, Treasure Island,
              Gulliver's Travels, Dracula) sequentially. Skips books whose course name
              already exists, so re-running after a partial failure resumes naturally.
              Long-running: ~2 hours per book and meaningful API spend.

          tutor import <path-to-file> --course "Name" [--description "..."] [--author "..."] [--title "..."] [--allow-duplicate]
              Import a local file as a new course. The parser is picked from the
              file extension. Phase A formats: .txt .md .html .htm .epub .pdf .docx
              Phase B formats: .doc .mobi .azw .azw3 .rtf .odt (require Calibre/LibreOffice)

          NOTE: by default a duplicate-name course is rejected. Use --allow-duplicate
          when you intentionally want a second copy (e.g. a different printing of
          the same book — Bible translations, pre/post-edit Stephen King, etc.).

          tutor export <course-id> <output.tutorcourse>
              Export a course (resources, concept map, structure, embeddings) to a single
              shareable bundle.

          tutor import-bundle <file.tutorcourse> [--course "Override Name"] [--allow-duplicate]
              Restore a course from a .tutorcourse bundle. All IDs are rewritten so a
              re-import never collides with the existing data — you'll get a brand-new
              course. Skips the LLM pipeline entirely (the embeddings ride along in the
              bundle), so it's typically <1s regardless of book size.

          tutor list
              List all courses on this machine.

          tutor delete <course-id> [--dry-run]
              Remove a course and cascade-delete its resources, concept maps,
              course structure, embeddings, and ConceptMapCollection file.
              --dry-run prints what would be removed without touching anything.

          tutor help
              Show this help.

        NOTES
          - Course data is stored at %LocalAppData%\Tutor\... and is shared with the
            Blazor UI, so anything imported via the CLI shows up there immediately.
          - LLM credentials come from the shared MindAttic credential store at
            %APPDATA%\MindAttic\LLM\. Configure them in the Blazor UI's settings page
            before running the CLI.
        """);
    return 0;
}

static int UnknownVerb(string verb)
{
    Console.Error.WriteLine($"Unknown command: {verb}");
    Console.Error.WriteLine("Run `tutor help` for usage.");
    return 64; // EX_USAGE
}
