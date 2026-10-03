# Tutor

Blazor Server app and CLI that turn books and documents into structured courses: a multi-LLM pipeline builds a knowledge graph, then lessons, grounded quizzes and progress tracking from it.

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/) [![Blazor Server](https://img.shields.io/badge/Blazor-Server-5C2D91)](https://learn.microsoft.com/aspnet/core/blazor/) [![C#](https://img.shields.io/badge/language-C%23-239120)](https://learn.microsoft.com/dotnet/csharp/) [![Tests](https://img.shields.io/badge/tests-NUnit-2E7D32)](docs/BIBLE.md) [![License](https://img.shields.io/badge/license-all%20rights%20reserved-lightgrey)](#license)

```text
  Moby Dick.epub ---+
  lecture.pdf ------+--> PARSE --> CHUNK --> EMBED --> EXTRACT CONCEPTS --> CONCEPT MAP
  Gutenberg #2701 --+                                                          |
                                         KNOWLEDGE GRAPH <-- CORRELATE <-------+
                                                |
                                    COURSE STRUCTURE (lessons > sections > concepts)
                                                |
                     quizzes + final exam + certificate, grounded by RAG on the source
                                                |
                                    "Moby Dick.tutor" bundle  (share / install / delete)
```

Tutor runs locally; there is no hosted demo. Ten ready-made courses ship in [Courses/](Courses).

## Why

- Drop a book in, get a course out: no manual authoring between the PDF and the first lesson.
- Learn from what the book says, not what a model imagines: every section and quiz is retrieved from the source through embeddings and RAG.
- Move at your own pace: lessons unlock as you show mastery, and a final exam ends in a certificate.
- Pay for the LLM pipeline once: a finished course is a `.tutor` bundle with its embeddings inside, and it installs on another machine in about a second.
- Script it or click it: the CLI and the Blazor app share one engine, so a course built in one opens in the other.
- Pick your model: Claude by default, or OpenAI, DeepSeek or Gemini for a single run.

## Features

### Ingest almost anything

| Parser | Extensions | Notes |
| --- | --- | --- |
| `TxtBookParser` | `.txt`, `.md`, `.markdown`, `.text`, `.log` | Managed |
| `HtmlBookParser` | `.html`, `.htm`, `.xhtml` | AngleSharp, SmartReader |
| `EpubBookParser` | `.epub` | VersOne.Epub |
| `PdfBookParser` | `.pdf` | PdfPig, with Tesseract OCR fallback for scanned pages |
| `DocxBookParser` | `.docx` | DocumentFormat.OpenXml |
| `LibreOfficeBookParser` | `.doc`, `.rtf`, `.odt` | Needs LibreOffice installed |
| `MobiBookParser` | `.mobi`, `.azw`, `.azw3`, `.prc` | Needs Calibre's `ebook-convert` |

- Project Gutenberg works can be fetched by ID straight from the CLI.
- The shell-out parsers fail cleanly with install instructions when LibreOffice or Calibre is missing.
- `TesseractPdfOcrService` reads scanned PDFs; if the native libraries or trained data are missing it falls back to text-only extraction instead of crashing.

### A knowledge graph, then a course

- `ConceptExtractionService` and `ConceptMapService` extract a concept map per chunk.
- `ConceptCorrelationService` and `KnowledgeGraphService` correlate concepts across resources with LSH and SimHash; `OrphanConceptLinkerService` reattaches strays; `DynamicConceptExpansionService` and `ConceptMergeService` refine the graph.
- `CourseStructureService` generates the learning path from the graph: ordered lessons, hierarchical sections, concept IDs and baked quiz questions.
- `EmbeddingService` and `VectorStoreService` give semantic search over content chunks for grounding.

### Learning

- Per-user progress with mastery-gated lesson unlocking (`UserProgressService`, `LearningPathService`).
- Section quizzes, either pre-generated and bundled or generated live from the concept maps (`QuizGenerationService`, `QuizService`).
- A final exam and a course certificate (`FinalExamService`, `CertificateService`).
- Pages for the library, courses, learning view, concept graph, settings, users, account and login (`Tutor.Shared/Components/Pages`).
- Sign-in through MindAttic.Authentication: SQL Server user store, Argon2id hashing with a pepper, idle and absolute session expiry.

### Portable courses

- `tutor export` writes a course with its resources, concept maps, structure and embeddings to one `.tutor` file.
- `tutor install` validates the bundle (format version, SHA-256, safe entry paths), plans the install against what is already there, and restores it without re-running the LLM pipeline. The same key and version is a no-op, a newer version upgrades, an older one is refused, and `--allow-duplicate` installs an independent copy with every ID rewritten.
- The `/library` page lists installed courses and offers a re-share download of the exact bundle each was installed from; admins can upload and install a bundle there, and unload (hide without deleting), load or remove a course. Those actions are authorized against the Admin policy on the server, not only hidden in the UI (`LibraryAdminActions`).
- Bundles are independent and never merged (`TUT-LAW-3`).

### Pluggable LLMs

- `LlmServiceRouter` sends chat and reasoning calls to Claude, OpenAI, DeepSeek or Gemini based on the shared `SELECTED_MODEL` preference. The CLI defaults to Claude.
- Embeddings always use OpenAI, whatever chat provider is selected.
- Transport, auth, retries and circuit breaking belong to MindAttic.Legion; pipeline code never references a vendor SDK.
- Each provider can hold a pool of your own keys, tried in order with failover on auth, rate-limit or server errors (`ApiKeyPoolService`).

## Quick start

Prerequisites: the .NET 10 SDK, SQL Server LocalDB for sign-in (or another SQL Server via `ConnectionStrings:TutorAuth`), and at least one LLM API key. OpenAI is needed for embeddings. LibreOffice and Calibre are optional.

```powershell
git clone https://github.com/mindattic/Tutor.git
cd Tutor
dotnet build Tutor.slnx
dotnet run --project Tutor.Blazor
```

The app listens on `https://localhost:7200` and `http://localhost:5200` (from `Tutor.Blazor/Properties/launchSettings.json`).

Install a bundled course and list it from the CLI:

```powershell
dotnet run --project Tutor.Cli -- install "Courses/Treasure Island.tutor"
dotnet run --project Tutor.Cli -- list
```

The course appears in the Blazor app's library right away, because the CLI and the app share the same data folder.

## How it works

The engine lives in `Tutor.Core`. Two front doors, `Tutor.Blazor` and `Tutor.Cli`, register the same service graph, so anything one builds the other reads without translation.

### Domain model

The nouns in [Tutor.Core/Models](Tutor.Core/Models):

| Model | Meaning |
| --- | --- |
| `CourseResource` | One ingested source with original and AI-formatted content. Produces exactly one `ConceptMap`. |
| `ConceptMap` | Concepts, relationships and complexity ordering for one resource. |
| `ConceptMapCollection` | The combined graph across all of a course's resources. |
| `Course` | Metadata and references only: resource IDs, the collection ID, the structure ID. |
| `CourseStructure` | The learning path: ordered lessons, hierarchical sections, concept IDs and pre-generated quiz questions. |
| `ContentChunk` | A text snippet with its embedding and LSH and SimHash signatures. The RAG substrate. |
| `UserProgress` | Per-user, per-course mastery state that drives lesson gating. |
| `CourseCertificate` | Issued when the final exam is passed. |

A `Course` never embeds content. Content lives in resources, structure, concept maps and chunks, and the course points at them, which keeps bundles composable and storage deduplicated.

### Stack

| Layer | Technology |
| --- | --- |
| Host | ASP.NET Core Blazor Server, net10.0 |
| Headless | `tutor` CLI (`Tutor.Cli`), same DI graph as the host |
| Agent tools | `Tutor.Mcp`, a stdio Model Context Protocol server |
| LLM transport | MindAttic.Legion 25.0.0 |
| Credentials | MindAttic.Vault 5.0.0 |
| Parsing | PdfPig, VersOne.Epub, AngleSharp, SmartReader, DocumentFormat.OpenXml, plus LibreOffice and Calibre |
| OCR | Tesseract 5.2.0, trained data in `Tutor.Core/tessdata` |
| RAG | In-process vector store plus LSH and SimHash |
| Auth | MindAttic.Authentication 6.0.0 on SQL Server (`TutorAuthDbContext`); reset links and security alerts over SMTP from the Vault `Notifications` bucket when configured; self-service reset at `/forgot-password` → `/account/reset` |
| Tests | NUnit (`Tutor.Tests`) and Cypress (`Tutor.Cypress`) |

## CLI reference

`Tutor.Cli` builds the `tutor` command. Verbs are dispatched in [Tutor.Cli/Program.cs](Tutor.Cli/Program.cs) and implemented under [Tutor.Cli/Commands](Tutor.Cli/Commands). From the repo you can run any of them as `dotnet run --project Tutor.Cli -- <verb>`.

| Command | What it does |
| --- | --- |
| `tutor gutenberg <book-id> [--course "Name"] [--description "..."] [--allow-duplicate]` | Download a Project Gutenberg work by ID and import it as a new course. |
| `tutor gutenberg-top10 [--dry-run] [--allow-duplicate] [--export-dir <dir>] [--quiz-mode <mode>]` | Build the curated top ten (Moby Dick, Pride and Prejudice, Frankenstein, Sherlock Holmes, Alice, Dorian Gray, Tom Sawyer, Treasure Island, Gulliver's Travels, Dracula) one after another. Skips existing course names, so a re-run resumes. With `--export-dir`, writes `<dir>/<Title>.tutor`. Roughly two hours per book and real API spend. |
| `tutor import <path> --course "Name" [--description "..."] [--author "..."] [--title "..."] [--quiz-mode <mode>] [--allow-duplicate]` | Import one local file as a new course; the parser is chosen by extension. |
| `tutor build-course <dir-or-zip> [--course "Override Name"] [--quiz-mode <mode>] [--export <out.tutor>] [--allow-duplicate]` | Build one course from many files listed in a `manifest.json`, the headless version of adding resources in the UI and clicking Build Course. `--export` also writes the bundle. |
| `tutor export <course-id> <output.tutor>` | Export a course to a shareable `.tutor` bundle. |
| `tutor import-bundle <file.tutor> [--course "Override Name"] [--allow-duplicate]` | Install a course from a bundle (alias `tutor install`). Accepts legacy `.tutorcourse` files, rewrites all IDs, skips the LLM pipeline, typically under a second. |
| `tutor list` | List the courses on this machine. |
| `tutor delete <course-id> [--dry-run]` | Remove a course and its resources, concept maps, structure, embeddings and collection file. `--dry-run` only prints what would go. |
| `tutor keys --provider <provider> <action>` | Manage the key pool for claude, openai, gemini or deepseek. Actions: `--list`, one or more `--set-key <key>`, `--add-key <key>`, `--remove-key <key>`, `--clear`. |
| `tutor fetch` | Download a remote source without parsing it (`FetchOnlyCommand`). |
| `tutor parse` | Parse a source without running the LLM pipeline (`ParseOnlyCommand`). |
| `tutor diag-keys` | Print masked key and `SELECTED_MODEL` diagnostics. |
| `tutor help` | Show usage. |

Global flags, accepted by every verb:

| Flag | Effect |
| --- | --- |
| `--verbose` | Trace-level `Tutor.Core` logging to stderr as `[LEVEL] message`. |
| `--llm <provider>` | Chat provider for this run: openai, claude, deepseek or gemini (default claude). Written to the shared `SELECTED_MODEL` preference. Embeddings still use OpenAI. |
| `--quiz-mode <mode>` | `baked` or `both` (default) pre-generate and bundle section quizzes; `dynamic` generates them live from the bundled concept maps. |
| `--allow-duplicate` | Allow a second course with an existing name, for example a different translation. |

The `build-course` manifest:

```json
{
  "name": "...",
  "description": "...",
  "quizMode": "both",
  "items": [
    { "file": "ch1.epub", "title": "...", "author": "..." }
  ]
}
```

## Courses

[Courses/](Courses) ships ten pre-built bundles, all from Project Gutenberg: Alice's Adventures in Wonderland, Dracula, Frankenstein, Gulliver's Travels, Moby Dick; Or, The Whale, Pride and Prejudice, The Adventures of Sherlock Holmes, The Adventures of Tom Sawyer, The Picture of Dorian Gray and Treasure Island. Each is a zip with course metadata, the learning structure, concept maps, baked quiz questions and embeddings, between about 1 MB and 10.5 MB. [Courses/README.md](Courses/README.md) is the authoritative list.

```powershell
tutor install "Courses/Moby Dick; Or, The Whale.tutor"
tutor list                 # find the course id
tutor delete <course-id>   # cascades resources, structure, concept maps, embeddings
```

To add a bundle, build it with the CLI and commit the `.tutor` file:

```powershell
tutor gutenberg-top10 --export-dir Courses --quiz-mode both
tutor build-course <dir-or-zip> --export "Courses/My Course.tutor"
```

[BIBLE §4.4](docs/BIBLE.md#TUT-§4) documents the archive layout (`manifest.json`, `course.json`, `courseStructure.json`, per-resource JSON and text, concept maps, `chunks.json`) and the install, load, unload and remove lifecycle.

## Configuration

- Course data is file-based under `%LocalAppData%\Tutor` (concept maps, knowledge graphs, vector store, LSH, course structures, logs). Individual folders can be redirected through `data-storage.settings.json` (`DataStorageSettings`). The CLI and the Blazor app share this folder.
- The sign-in database connection string is read from `ConnectionStrings:TutorAuth` in configuration, then the `ConnectionStrings__TutorAuth` environment variable, then falls back to LocalDB (`(localdb)\MSSQLLocalDB`, database `TutorAuth`).
- LLM keys resolve through MindAttic.Vault: `%APPDATA%\MindAttic\LLM\providers.json` layered under environment variables (`MindAttic:Vault:LLM:*`), so the CLI uses the same keys as every other MindAttic app. Per-provider key pools are managed with `tutor keys`, the MCP server, or the Settings page.
- In production the host stores Data Protection keys in Azure Blob Storage protected by Key Vault and requires `DataProtection:BlobUri` and `DataProtection:KeyVaultKeyId`.

## MCP server

`Tutor.Mcp` is a stdio Model Context Protocol server that exposes the key-pool operations (list, set, add, remove, clear, per provider) as tools, so an assistant can rotate Tutor's keys without a human running `tutor keys`. It is not part of `Tutor.slnx`; run it with `dotnet run --project Tutor.Mcp` from an MCP client.

## Testing

```powershell
dotnet test Tutor.Tests
```

`Tutor.Tests` (NUnit) is organised into `Fakes`, `Models`, `Packaging`, `Parsers` and `Services`. It covers the parsers, the concept-map JSON shape, export and import round-trips, auth import and admin contracts, and the full course lifecycle (lock, unlock, final exam, certificate, unload) in `FullCourseLifecycleTests`. The 2026-10-03 run: 454 passed, 0 failed, 0 skipped ([BIBLE §6](docs/BIBLE.md#TUT-§6)). The paid, non-deterministic LLM pipeline itself is deliberately not automated, so those stories are marked partial in [docs/USER%5FSTORIES.md](docs/USER%5FSTORIES.md).

End-to-end tests need the app running on `http://localhost:5200` first:

```powershell
cd Tutor.Cypress
npm install
npm run cypress:run
```

Use `npm run cypress:open` for the interactive runner. Override the address with `$env:CYPRESS_BASE_URL = "https://localhost:7200"`. Authenticated specs use a `cy.login()` command; supply credentials through `$env:CYPRESS_username` and `$env:CYPRESS_password`.

| Spec | Covers |
| --- | --- |
| `smoke.cy.ts` | The app is alive. |
| `auth.cy.ts` | Login and redirect guards. |
| `courses.cy.ts` | Course library listing and navigation. |
| `course-flow.cy.ts` | Deterministic course flow: lock, unlock, navigation. |
| `concept-graph.cy.ts` | Concept graph page rendering. |
| `quiz.cy.ts` | Quiz UI wiring (the generated content is pinned by `FullCourseLifecycleTests`). |

A feature is done when `dotnet build Tutor.slnx` is clean, `Tutor.Tests` is green and, for anything user-facing, a Cypress guard or lifecycle assertion covers it.

## Project layout

| Project or folder | Purpose |
| --- | --- |
| `Tutor.Core` | Parsers, the pipeline, domain models and storage services. |
| `Tutor.Shared` | Razor components: layout, pages, quiz, exam and certificate tabs, chat. |
| `Tutor.Blazor` | Blazor Server host: `Program.cs` composition root, middleware, `appsettings.json`. |
| `Tutor.Cli` | The `tutor` command; mirrors the host's DI graph and owns bundle export, import and Gutenberg fetch. |
| `Tutor.Mcp` | MCP server for key-pool management (outside the solution). |
| `Tutor.Tests` | NUnit suite. |
| `Tutor.Cypress` | Cypress end-to-end tests (separate Node project). |
| `Courses/` | The ten shipped `.tutor` bundles. |
| `dist/` | Local scratch output from CLI runs; not part of the build. |
| `docs/` | Codex documentation canon. |
| `tools/` | `codex.ps1` (docs digest and doctor) and `build-readme.ps1` (README.md to README.htm). |
| `Export.ps1` | Source-export utility, see below. |

### Export.ps1

`Export.ps1` is a generic source-bundling script (its header still describes a Unity export). It walks the tree, collects `.cs` files, skips `.git`, `.vs`, `obj`, `bin` and similar folders, and writes `ExportedScripts.txt`: a JSON manifest (path, SHA-256, size, line count) followed by every file between `<<<FILE START>>>` and `<<<FILE END>>>` markers. `ExportedScripts.txt` in the repo is its latest output.

```powershell
powershell -File Export.ps1
```

### Code style

- Private fields are `camelCase` with no underscore prefix.
- Constructors use `this.x = x` to disambiguate.

## Limitations

- Quiz attribution: `QuizService` reads the user ID from `IHttpContextAccessor`, which is null after the first Blazor Server render, so quizzes started mid-circuit are attributed to "anonymous". Tracked as a TODO in [QuizService.cs](Tutor.Core/Services/Quiz/QuizService.cs) and as story B5.
- Single host: Blazor Server with SQL-backed auth, not built for scale-out or per-tenant isolation.
- `KimiService` exists but is not wired into `LlmServiceRouter` or the key pools.
- Debug builds reference a sibling `MindAttic.Authentication` checkout, so a Debug build expects the MindAttic workspace layout.

## Documentation

Tutor follows the MindAttic Codex standard: each fact lives in one layer, linked by a stable ID.

- [docs/BIBLE.md](docs/BIBLE.md) - what Tutor is and is not, architecture, the Laws (`TUT-LAW-n`), verified state, glossary. Where this README and the Bible disagree, the Bible wins.
- [docs/AMENDMENTS.md](docs/AMENDMENTS.md) - pending decisions not yet folded into the Bible; normally empty.
- [User stories](docs/USER_STORIES.md) - acceptance stories `TUT-US-<Epic><n>`, each done story citing its test.
- [docs/BIBLE.digest.md](docs/BIBLE.digest.md) - generated by `tools/codex.ps1 digest`; never hand-edit.
- [AGENTS.md](AGENTS.md) - instructions for coding agents working in this repo.

```powershell
powershell -File tools/codex.ps1 doctor    # must pass before editing docs/
powershell -File tools/codex.ps1 digest    # after touching BIBLE.md
powershell -NoProfile -ExecutionPolicy Bypass -File tools\build-readme.ps1    # README.md to README.htm
```

## License

This repository has no LICENSE file. All rights reserved. The bundled courses are built from public-domain Project Gutenberg texts.

---

Part of [MindAttic](https://mindattic.com) — see more projects at [github.com/mindattic](https://github.com/mindattic). Related: [MindAttic.Legion](https://github.com/mindattic/MindAttic.Legion) (LLM transport), [MindAttic.Vault](https://github.com/mindattic/MindAttic.Vault) (credentials), [MindAttic.Authentication](https://github.com/mindattic/MindAttic.Authentication) (sign-in).
