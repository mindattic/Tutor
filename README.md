# Tutor

**Drop a book in. Get a course out.**

Tutor turns books, papers, and documents into structured, navigable courses with quizzes and progress tracking. Hand it a PDF, EPUB, DOCX, or a Project Gutenberg ID — a multi-LLM pipeline extracts concepts, correlates them into a knowledge graph, and writes the learning path on the other side. RAG retrieval keeps every quiz and section grounded in the actual source material.

Full architecture rationale, invariants, and the non-negotiable rules live in [`docs/BIBLE.md`](docs/BIBLE.md) — this file is the practical "how to build, run, and use it" tour. Where the two disagree, the bible wins.

---

## Table of contents

- [What it does](#what-it-does)
- [How it works](#how-it-works)
- [Stack](#stack)
- [Project layout](#project-layout)
- [Domain model](#domain-model)
- [Getting started](#getting-started)
- [CLI reference (`Tutor.Cli`)](#cli-reference-tutorcli)
- [The `Courses/` content model](#the-courses-content-model)
- [Configuration](#configuration)
- [Export.ps1](#exportps1)
- [Tests](#tests)
- [Documentation canon (`docs/`)](#documentation-canon-docs)
- [Code style](#code-style)
- [Known limitations](#known-limitations)

---

## What it does

1. **You bring the source. Tutor builds the course.** A PDF, EPUB, DOCX, HTML, legacy `.doc`/`.rtf`/`.odt`, `.mobi`/`.azw`/`.azw3`, or a Project Gutenberg ID goes in; a hierarchical course (Lessons → Sections → concepts, with quizzes) comes out. No manual authoring required.
2. **The knowledge graph is the product, not the source bytes.** Concepts are extracted per chunk, correlated across resources into one graph, and the course is generated *from* the graph. The source is retained for grounding, never replayed verbatim.
3. **Answers are grounded.** Every quiz prompt and section fill is retrieved from the actual source via embeddings + RAG — Tutor does not invent content that isn't on the page.
4. **Courses travel.** A finished course is a self-contained `.tutor` bundle — structure, concepts, quizzes, and pre-computed embeddings — that installs without re-running the (slow, paid) LLM pipeline.
5. **Same engine, two front doors.** `Tutor.Cli` and `Tutor.Blazor` register the identical service graph, so anything the CLI builds, the Blazor app can read with zero translation, and vice versa.

## How it works

```
SOURCE FILE -> PARSE -> CHUNK -> EMBED -> EXTRACT CONCEPTS -> CONCEPT MAP
                                                               |
                           KNOWLEDGE GRAPH <- CORRELATE <------+
                                                               |
                                       COURSE STRUCTURE <------+
                                                               |
                                       .tutor BUNDLE <---------+   (share / load / unload)
```

- **Eight input formats.** Phase A (managed C#, NuGet-based): `.txt`, `.md`, `.html`/`.htm`, `.epub`, `.pdf`, `.docx`. Phase B (shell-out): `.doc`/`.rtf`/`.odt` via LibreOffice; `.mobi`/`.azw`/`.azw3` via Calibre's `ebook-convert`. Each Phase B parser fails cleanly with install instructions if the external tool isn't on the machine.
- **OCR fallback.** `TesseractPdfOcrService` handles scanned PDFs; if the native Tesseract libs or trained data aren't available it goes silent and falls back to text-only extraction rather than crashing the run.
- **Pluggable LLMs.** `LlmServiceRouter` routes chat/reasoning calls to `OpenAIService`, `ClaudeService`, `DeepSeekService`, or `GeminiService` based on a `SELECTED_MODEL` preference (default `Claude`, overridable per CLI invocation with `--llm`). Embeddings always go through OpenAI regardless of the selected chat provider. Transport (auth, retry, circuit breaker) is owned by `MindAttic.Legion`; no pipeline code may reference a vendor SDK directly. A `KimiService` also exists in `Tutor.Core/Services/KimiService.cs` but is not currently wired into `LlmServiceRouter`'s switch.
- **Knowledge graph.** `ConceptExtractionService`/`ConceptMapService` extract a concept JSON per chunk; `ConceptCorrelationService` + `KnowledgeGraphService` correlate concepts across resources via LSH + SimHash; `OrphanConceptLinkerService` reattaches strays; `DynamicConceptExpansionService` and `ConceptMergeService` refine the graph further. The course is generated *from* the graph, not authored by hand.
- **RAG.** `EmbeddingService` + `VectorStoreService` provide semantic search over `ContentChunk`s; `SimHashService` + `LSHService` do near-duplicate detection.
- **Progress and quizzes.** `UserProgressService` and `LearningPathService` drive per-user mastery-gated lesson unlocking; `QuizGenerationService`/`QuizService` back an LLM-generated (or pre-baked) quiz controller; `FinalExamService` + `CertificateService`/`CertificateAuthority` handle course completion.

## Stack

| Layer | Technology |
| --- | --- |
| Host | ASP.NET Core Blazor Server (`net10.0`) |
| Headless | `tutor` CLI (`Tutor.Cli`) — same DI graph as the Blazor host |
| LLM transport | `MindAttic.Legion` (OpenAI / Claude / DeepSeek / Gemini) |
| Credentials | `MindAttic.Vault` — `%APPDATA%\MindAttic\LLM\providers.json` |
| Parsing | UglyToad.PdfPig (PDF), VersOne.Epub (EPUB), AngleSharp (HTML), Open-XML (DOCX) + LibreOffice / Calibre shell-outs |
| OCR | Tesseract (`TesseractPdfOcrService`), trained data downloaded to `Tutor.Core/tessdata` on first build |
| RAG | In-process vector store (`VectorStoreService`) + LSH/SimHash |
| Auth | `MindAttic.Authentication` v1.0.0 — SQL Server (`TutorAuthDbContext`), Argon2id + pepper, idle/absolute session expiry |
| Tests | NUnit (`Tutor.Tests`) + Cypress (`Tutor.Cypress`) |

## Project layout

| Project / folder | Purpose |
| --- | --- |
| `Tutor.Core` | Parsers, the full pipeline, domain models, storage services. Everything substantive lives here. |
| `Tutor.Shared` | Razor components shared by the host — layout, pages (`Home`, `Library`, `Learn`, `Courses`, `ConceptGraph`, `Settings`, `Users`, `Login`, …), quiz/exam/certificate tabs, chat. |
| `Tutor.Blazor` | Blazor Server host. Composition root (`Program.cs`), DI wiring, middleware, `appsettings.json`. |
| `Tutor.Cli` | `tutor` headless binary. Mirrors Blazor's DI graph; owns bundle export/import and Gutenberg fetch. |
| `Tutor.Tests` | NUnit unit/component suite (parsers, services, packaging, models — see [Tests](#tests)). |
| `Tutor.Cypress` | End-to-end browser tests (sibling Node project, not part of `Tutor.slnx`). |
| `Courses/` | Pre-built `.tutor` bundles shipped with the repo — see [content model](#the-courses-content-model). |
| `dist/` | Local scratch output from CLI runs (smoke tests, batch logs, exported bundles) — not part of the build. |
| `docs/` | Codex documentation canon — see [below](#documentation-canon-docs). |
| `tools/` | `codex.ps1` (docs digest/doctor) and `build-readme.ps1` (README → README.htm renderer, see below). |
| `scripts/` | Landing-page build scripts referenced by `package.json` (see [note](#exportps1) — currently empty; kept for the npm script wiring). |
| `Export.ps1` | Ad-hoc source-export utility — see [below](#exportps1). |
| `index.htm` | The `mindattic.com/tutor` landing page (built separately from README.md; **do not confuse with `README.htm`**). |

`Tutor.Core` references `MindAttic.Legion` (`..\..\MindAttic.Legion\MindAttic.Legion`) and `MindAttic.Vault` as sibling-repo project references — Tutor is not standalone-buildable outside the `MindAttic` workspace layout.

## Domain model

The nouns the pipeline and storage layer operate on ([`Tutor.Core/Models`](Tutor.Core/Models)):

| Model | Meaning |
| --- | --- |
| `CourseResource` | One ingested source (book/paper/doc). Has original + AI-formatted content. Produces exactly one `ConceptMap` (1:1). |
| `ConceptMap` | Concepts + relationships + complexity ordering for a single resource. |
| `ConceptMapCollection` | The aggregated graph across all of a course's resources (`collection_{courseId}`). |
| `Course` | Lightweight metadata + **references only** — resource IDs, the collection ID, the structure ID. Carries no content directly. |
| `CourseStructure` | The learning path: ordered `Lesson`s → hierarchical `Section`s → concept IDs, plus baked `PreGeneratedQuestions` (quiz). |
| `ContentChunk` | A text snippet + its embedding + LSH/SimHash signatures. The RAG substrate. |
| `UserProgress` | Per-user, per-course mastery state driving lesson gating. |
| `CourseCertificate` | Issued on passing the final exam. |

> **Invariant:** a `Course` never embeds content. Content lives in resources, structure, concept maps, and chunks; the course just points at them — this keeps bundles composable and storage deduplicated (see [`docs/BIBLE.md` §4.2](docs/BIBLE.md#TUT-§4)).

## Getting started

Prerequisites: .NET 10 SDK. Optional: LibreOffice (`.doc`/`.rtf`/`.odt`) and Calibre (`.mobi`/`.azw`/`.azw3`). At least one LLM provider API key registered with `MindAttic.Vault`.

```powershell
dotnet build Tutor.slnx

dotnet run --project Tutor.Blazor
# -> https://localhost:7200 (HTTPS) or http://localhost:5200 (HTTP)
# see Tutor.Blazor/Properties/launchSettings.json

dotnet run --project Tutor.Cli -- help
```

LLM API keys resolve through `MindAttic.Vault`'s standard chain: `%APPDATA%\MindAttic\LLM\providers.json`, layered under environment variables, so the CLI uses the same working keys as every other MindAttic app. Configure them from the Blazor UI's Settings page before running the CLI.

## CLI reference (`Tutor.Cli`)

The `tutor` binary shares Tutor.Core's full DI graph with the Blazor host, so a course built by one is readable by the other with zero translation. Verbs are dispatched in [`Tutor.Cli/Program.cs`](Tutor.Cli/Program.cs); each is backed by a class under [`Tutor.Cli/Commands/`](Tutor.Cli/Commands).

| Command | What it does |
| --- | --- |
| `tutor gutenberg <book-id> [--course "Name"] [--description "..."] [--allow-duplicate]` | Download a Project Gutenberg work by ID and import it as a new course. |
| `tutor gutenberg-top10 [--dry-run] [--allow-duplicate] [--export-dir <dir>] [--quiz-mode baked\|dynamic\|both]` | Drive the curated top-10 (Moby Dick, Pride and Prejudice, Frankenstein, Sherlock Holmes, Alice, Dorian Gray, Tom Sawyer, Treasure Island, Gulliver's Travels, Dracula) sequentially. Skips books whose course name already exists, so a re-run after a partial failure resumes naturally. With `--export-dir`, writes each course to `<dir>/<Title>.tutor`. Long-running: roughly 2 hours per book and meaningful API spend. |
| `tutor import <path> --course "Name" [--description "..."] [--author "..."] [--title "..."] [--quiz-mode ...] [--allow-duplicate]` | Import one local file as a new course. Parser is picked from the file extension. |
| `tutor build-course <dir-or-zip> [--course "Override Name"] [--quiz-mode ...] [--export <out.tutor>] [--allow-duplicate]` | Build **one** course out of **many** source files in a single command — the headless equivalent of adding each resource in the UI and clicking "Build Course". Point it at a directory or `.zip` containing a `manifest.json` plus the files it lists (see shape below). With `--export`, also writes the redistributable `.tutor` bundle. |
| `tutor export <course-id> <output.tutor>` | Export a course (resources, concept map, structure, embeddings) to a single shareable `.tutor` bundle. |
| `tutor import-bundle <file.tutor> [--course "Override Name"] [--allow-duplicate]` / `tutor install <file.tutor> [...]` | Restore a course from a `.tutor` bundle (alias: `install`). Legacy `.tutorcourse` files are also accepted. All IDs are rewritten so a re-import never collides with existing data. Skips the LLM pipeline entirely (embeddings ride along) — typically under 1 second regardless of book size. |
| `tutor list` | List all courses on this machine. |
| `tutor delete <course-id> [--dry-run]` | Remove a course and cascade-delete its resources, concept maps, course structure, embeddings, and `ConceptMapCollection` file. `--dry-run` prints what would be removed without touching anything. |
| `tutor fetch <...>` | `FetchOnlyCommand` — download a remote source without parsing it. |
| `tutor parse <...>` | `ParseOnlyCommand` — parse a source without running the LLM pipeline. |
| `tutor diag-keys` | Print masked LLM key/preference diagnostics (Vault-resolved key lengths/suffixes, current `SELECTED_MODEL`). |
| `tutor help` | Show CLI usage. |

Global flags (stripped before the verb dispatch, so any command accepts them):

| Flag | Effect |
| --- | --- |
| `--verbose` | Turns on `Tutor.Core`'s trace-level logging, forwarded to stderr as `[LEVEL] message`. |
| `--llm <openai\|claude\|deepseek\|gemini>` | Overrides the chat/reasoning provider for this run (default `claude`). Embeddings always use OpenAI regardless. Persisted to the shared `SELECTED_MODEL` preference that `LlmServiceRouter` reads. |
| `--quiz-mode baked\|dynamic\|both` | Controls section quizzes: `baked`/`both` (default) pre-generate and bundle questions for offline play; `dynamic` skips pre-generation and lets the runtime generate them live from the bundled concept maps. |
| `--allow-duplicate` | By default a duplicate-name course is rejected; pass this to intentionally create a second copy (e.g. a different translation/printing). |

`build-course`'s `manifest.json` shape:

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

Course data is stored at `%LocalAppData%\Tutor\...` and shared with the Blazor UI — anything imported via the CLI shows up there immediately.

> **Note on drift:** the CLI verbs above (`gutenberg-top10`, `import`, `build-course`, `fetch`, `parse`, `diag-keys`) are read directly from `Program.cs`'s verb switch and its embedded `PrintHelp()` text. If you edit `Program.cs`'s command surface, update this table in the same change.

## The `Courses/` content model

[`Courses/`](Courses) ships ten pre-built `.tutor` bundles — self-contained zips carrying course metadata, the learning structure (lessons → topics → sections), concept maps, baked quiz questions, and RAG embeddings, so they install without re-running the (slow, paid) LLM pipeline. One file per course; bundles are independent and are **never** merged ([`TUT-LAW-3`](docs/BIBLE.md#TUT-LAW-3)).

Included (all Project Gutenberg sources): *Alice's Adventures in Wonderland*, *Dracula*, *Frankenstein*, *Gulliver's Travels*, *Moby Dick; Or, The Whale*, *Pride and Prejudice*, *The Adventures of Sherlock Holmes*, *The Adventures of Tom Sawyer*, *The Picture of Dorian Gray*, *Treasure Island*. See [`Courses/README.md`](Courses/README.md) for the authoritative list.

**Install a bundled course:**

```powershell
tutor install "Courses/Moby Dick; Or, The Whale.tutor"
```

(`tutor install` is an alias of `tutor import-bundle`.) All IDs are rewritten on import, so installing the same bundle twice yields two distinct courses rather than overwriting one. Once installed, the course appears in the Tutor Blazor UI automatically.

**Remove a course:**

```powershell
tutor list                 # find the course id
tutor delete <course-id>   # cascades resources, structure, concept maps, embeddings
```

**Add a new bundle to `Courses/`** — build it with the CLI, then commit the resulting `.tutor` file:

```powershell
tutor gutenberg-top10 --export-dir Courses --quiz-mode both
# or, for an arbitrary source directory/zip:
tutor build-course <dir-or-zip> --export "Courses/My Course.tutor"
```

Bundles carry embeddings and can be several MB each; they are committed to the repo so courses travel with it. See [RFC 0001 — Course Packaging & Sharing](docs/rfc/0001-course-packaging.md) for the archive's internal layout (`manifest.json`, `course.json`, `courseStructure.json`, per-resource JSON/text, concept maps, `chunks.json`) and the roadmap toward an in-app load/unload library.

## Configuration

Connection string resolution priority (Tutor.Blazor's data store):

1. `ConnectionStrings__Tutor` environment variable
2. `ConnectionStrings:Tutor` in `appsettings.json`
3. LocalDB fallback

The auth database (`TutorAuthDbContext`) resolves its own connection string the same way, keyed `TutorAuth` (`ConnectionStrings__TutorAuth` env var, else `ConnectionStrings:TutorAuth`, else LocalDB).

LLM credentials follow `MindAttic.Vault`'s standard resolution chain — `%APPDATA%\MindAttic\LLM\providers.json` or `MindAttic:Vault:LLM:*` in `IConfiguration`.

## Export.ps1

`Export.ps1` at the repo root is a generic **source-bundling utility**, not Tutor-specific tooling — its header comments describe a Unity project export ("Export Unity project sources... Default: ONLY .cs files") and it does not reference any Tutor concept. In this repo it walks the tree from wherever it's invoked, collects `.cs` files (extra extensions like `.prefab`/`.meta`/`.unity` are commented out and unused here), skips noisy directories (`.git`, `.vs`, `obj`, `bin`, etc.), and writes a single `ExportedScripts.txt` containing a JSON manifest (path/SHA-256/size/line count per file) followed by the full text of every file, delimited by `<<<FILE START>>>` / `<<<FILE END>>>` markers. It appears to be a shared personal script reused across repos (including non-.NET ones) rather than a maintained part of the Tutor build. Run it with:

```powershell
powershell -File Export.ps1
```

`ExportedScripts.txt` (checked into the repo, ~860 KB) is its most recent output.

## Tests

```powershell
dotnet test Tutor.Tests
```

`Tutor.Tests` (NUnit) is organized into `Fakes/`, `Models/`, `Packaging/`, `Parsers/`, and `Services/` subfolders, covering parsers, the concept-map JSON shape, packaging (export/import round-trips), auth import/admin contracts, and the full course lifecycle (lock → unlock → final exam → certificate → unload) via `FullCourseLifecycleTests`. As of the last recorded bible update (2026-06-07): **380 passed, 0 failed, 0 skipped**, build clean. The end-to-end LLM pipeline itself (extraction, section fill) is deliberately *not* automated — it's paid and non-deterministic — so those stories are marked 🟡 partial in [`docs/USER_STORIES.md`](docs/USER_STORIES.md) even though the surrounding primitives are pinned.

```powershell
cd Tutor.Cypress
npm install
npm run cypress:run    # headless; Tutor.Blazor must be running first
# or: npm run cypress:open   (interactive runner)
```

Tutor.Blazor must be reachable at `http://localhost:5200` (default) before running Cypress — start it with `dotnet run --project Tutor.Blazor`. Override the base URL with `$env:CYPRESS_BASE_URL = "https://localhost:7200"`. Authenticated specs use a `cy.login()` custom command; provide credentials via `$env:CYPRESS_username` / `$env:CYPRESS_password`.

Specs in [`Tutor.Cypress/e2e/`](Tutor.Cypress/e2e):

| Spec | Covers |
| --- | --- |
| `smoke.cy.ts` | Basic app-is-alive check. |
| `auth.cy.ts` | Login/redirect guards. |
| `courses.cy.ts` | Course library listing/navigation. |
| `course-flow.cy.ts` | Deterministic course-flow wiring (lock/unlock, navigation). |
| `concept-graph.cy.ts` | Concept graph page rendering. |
| `quiz.cy.ts` | Quiz UI wiring (not the LLM-generated content itself — that's pinned by `FullCourseLifecycleTests` instead). |

**Definition of done for a feature:** clean `dotnet build Tutor.slnx`, green `Tutor.Tests`, and — for anything user-facing — a Cypress guard or a lifecycle assertion. No vendor lock-in introduced; whole-number versioning respected ([HOUSE-LAW-8](../MindAttic.HouseRules.md#HOUSE-LAW-8)).

## Documentation canon (`docs/`)

Tutor has adopted the MindAttic "Codex" documentation standard — a fact lives in exactly one layer, linked by stable ID, never by line number:

| Layer | File | Role |
| --- | --- | --- |
| L0 | [`docs/BIBLE.md`](docs/BIBLE.md) | What Tutor **is**/is not, architecture canon, the Laws (`{#TUT-LAW-n}`), verified state, active frontier, glossary. |
| L1 | [`docs/AMENDMENTS.md`](docs/AMENDMENTS.md) | Append-only change log (`TUT-A<n>`); an amendment **wins** over the bible. Currently empty — bible is epoch 0. |
| L2 | [`docs/USER_STORIES.md`](docs/USER_STORIES.md) | Acceptance stories (`TUT-US-<Epic><n>`), each `✅` citing its verifying test. |
| rfc | [`docs/rfc/0001-course-packaging.md`](docs/rfc/0001-course-packaging.md) | Design note for the in-app course load/unload/share roadmap — compares `.tutor` to `MindAttic.Ideas`'s `.idea` packaging and lays out the upgrade path. |
| generated | [`docs/BIBLE.digest.md`](docs/BIBLE.digest.md) | Produced by `tools/codex.ps1 digest`; injected as session context. **Never hand-edit.** |

Org-wide rules (whole-number versioning, soft-disable-never-delete, credentials-through-Vault, provider-agnostic LLMs, guarded-zip packaging, one-engine-many-front-doors, MindAttic.Authentication, verified-not-asserted done) live once in `../MindAttic.HouseRules.md` and are inherited by reference from [`docs/BIBLE.md` §5](docs/BIBLE.md#TUT-§5) — see that section for how each maps onto Tutor specifically.

Before editing anything under `docs/`, run `pwsh tools/codex.ps1 doctor` (must pass); run `pwsh tools/codex.ps1 digest` after touching `BIBLE.md`.

**Regenerating this file as HTML:** `tools/build-readme.ps1` renders this `README.md` to `README.htm` at the repo root, using the shared engine at `../codex-standard/build-readme.ps1` (one engine for every MindAttic repo, so every `README.htm` looks and behaves identically). Run it with:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\build-readme.ps1
```

`README.htm` is a separate artifact from `index.htm` at the repo root — `index.htm` is the `mindattic.com` landing page (built by a different, currently-retired pipeline; see the note under [Project layout](#project-layout)) and should not be confused with, or overwritten by, the README renderer.

## Code style

- Private fields: `camelCase` without underscore prefix.
- Constructors: `this.x = x` to disambiguate.

## Known limitations

- **Quiz attribution during mid-circuit starts.** `QuizService` reads the user ID from `IHttpContextAccessor`, which is `null` after the initial Blazor Server render, so mid-circuit quiz starts attribute to `"anonymous"`. Tracked as a TODO in [`Tutor.Core/Services/Quiz/QuizService.cs`](Tutor.Core/Services/Quiz/QuizService.cs) and as [story B5](docs/USER_STORIES.md#TUT-EPIC-B). Fix is to thread the ID from `AuthenticationState` instead.
- **Not multi-tenant.** Single-host Blazor Server app with SQL-backed auth; designs should not assume horizontal scale-out or per-tenant isolation ([`docs/BIBLE.md` §3](docs/BIBLE.md#TUT-§3)).
- **Landing-page build scripts are stale.** `package.json`'s `build`/`deploy` npm scripts point at `scripts/cli/build-html.js` and `scripts/cli/deploy.ps1`, but that directory is currently empty — the local FTP landing-page machinery was retired in favor of routing `/deploy` through `MindAttic.Deploy` (see git history on `scripts/cli`). The npm script entries were left in place but do not currently resolve to files in this repo.
