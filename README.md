# Tutor

**Drop a book in. Get a course out.**

Tutor turns books, papers, and documents into structured, navigable courses with quizzes and progress tracking. Hand it a PDF, an EPUB, a `.docx`, a Project Gutenberg ID — and a multi-LLM pipeline extracts the concepts, correlates them into a knowledge graph, and writes the learning path on the other side. RAG retrieval makes the source material searchable; near-duplicate detection keeps the graph clean as you stack more material on top.

**Why Tutor:**

- **Eight input formats, one pipeline.** PDF, EPUB, HTML, DOCX handled in managed C#; legacy `.doc`/`.rtf`/`.odt` shell out to LibreOffice; `.mobi`/`.azw`/`.azw3` shell out to Calibre's `ebook-convert`. The pipeline downstream of parsing doesn't know which format produced its chunks.
- **Provider-agnostic LLMs.** `LlmServiceRouter` picks OpenAI, Claude, DeepSeek, or Gemini at call time; transport (auth, retry, circuit breaker) is owned by `MindAttic.Legion` so every provider gets the same reliability surface.
- **The knowledge graph is the product.** `ConceptMapService` extracts a concept JSON per chunk; `KnowledgeGraphService` correlates concepts across resources via LSH + SimHash; `OrphanConceptLinkerService` reattaches strays. The course is generated *from* the graph, not the source bytes.
- **Same engine, two front doors.** The Blazor host and the `tutor` CLI register identical services, so a course produced by the CLI is fully readable in the UI without translation.
- **RAG that earns its keep.** `EmbeddingService` + `VectorStoreService` ground every quiz prompt and section fill in the actual source — no halluciation about content that isn't on the page.

---

## Table of Contents

- [What it does](#what-it-does)
- [Stack](#stack)
- [Project structure](#project-structure)
- [Getting started](#getting-started)
- [CLI commands](#cli-commands)
- [Configuration](#configuration)
- [Tests](#tests)
- [Code style](#code-style)

---

## What it does

```
SOURCE FILE ─► PARSE ─► CHUNK ─► EMBED ─► EXTRACT CONCEPTS ─► CONCEPT MAP
                                                                  │
                              KNOWLEDGE GRAPH ◄─ CORRELATE ◄──────┘
                                                                  │
                                          COURSE STRUCTURE ◄──────┘
```

- **Multi-format ingest.** Phase A parsers (managed C#) handle PDF, EPUB, HTML, DOCX. Phase B shells out to LibreOffice for legacy .doc/.rtf/.odt and to Calibre's `ebook-convert` for .mobi/.azw/.azw3.
- **Pluggable LLMs.** Routes to OpenAI, Claude, DeepSeek, or Gemini via `LlmServiceRouter`. Wire transport (auth, retry, circuit breaker) is owned by `MindAttic.Legion` so every provider gets the same reliability surface.
- **Concept map + knowledge graph.** `ConceptMapService` extracts a JSON concept document per chunk; `KnowledgeGraphService` correlates concepts across resources; `OrphanConceptLinkerService` reattaches strays.
- **Course generation.** `CourseStructureService` turns the merged map into a hierarchical learning path; `SectionContentService` fills each section with content drawn from the source material.
- **RAG retrieval.** `EmbeddingService` + `VectorStoreService` for semantic search; `SimHashService` and `LSHService` for lexical fingerprinting and near-duplicate detection.
- **Progress + quizzes.** Per-user progress tracking and an LLM-backed quiz controller.
- **Project Gutenberg.** First-class importer for Gutenberg works by ID; a `top10` convenience batch lets you seed a course library from the canon in one command.

---

## Stack

| Layer | Technology |
|---|---|
| Host | ASP.NET Core Blazor Server (.NET 10) |
| Headless | `tutor` console (Tutor.Cli) — same DI graph as the Blazor host |
| LLM transport | [MindAttic.Legion](../MindAttic.Legion/) (OpenAI / Claude / DeepSeek / Gemini) |
| Credentials | [MindAttic.Vault](../MindAttic.Vault/) — `%APPDATA%\MindAttic\LLM\providers.json` or `IConfiguration` |
| Parsing | UglyToad.PdfPig (PDF), VersOne.Epub (EPUB), AngleSharp (HTML), Open-XML (DOCX) + LibreOffice / Calibre shell-outs |
| RAG | In-process vector store + LSH/SimHash near-duplicate detection |
| Tests | NUnit (`Tutor.Tests`) + Cypress (`Tutor.Cypress`) |

---

## Project structure

| Project | Purpose |
|---|---|
| **Tutor.Core** | Parsers, services, models. The whole pipeline lives here. |
| **Tutor.Shared** | Razor components shared between the Blazor host and any sibling shells. |
| **Tutor.Blazor** | Blazor Server host app. |
| **Tutor.Cli** | Headless pipeline driver — `tutor` binary. Mirrors the Blazor service registration so courses produced from the CLI are readable from the UI without translation. |
| **Tutor.Tests** | NUnit unit/component test suite. |
| **Tutor.Cypress** | End-to-end browser tests against the live Blazor host (not in `Tutor.slnx`; lives as a sibling Node project). |

`Tutor.Core` references `MindAttic.Legion` (sibling repo, `..\..\MindAttic.Legion\MindAttic.Legion`) and `MindAttic.Vault`.

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Optional: LibreOffice (for `.doc`/`.rtf`/`.odt`) and Calibre (for `.mobi`/`.azw`)
- An API key for at least one LLM provider (OpenAI / Claude / DeepSeek / Gemini)

### Build and run

```powershell
# Build everything
dotnet build Tutor.slnx

# Run the Blazor host (default URL: https://localhost:5001)
dotnet run --project Tutor.Blazor

# Or run the CLI
dotnet run --project Tutor.Cli -- --help
```

LLM API keys are stored under `%APPDATA%\MindAttic\LLM\providers.json` (or `MindAttic:Vault:LLM:*` in `IConfiguration` / User Secrets / Azure App Service Application Settings) so a rotation propagates to every MindAttic app on the same machine.

---

## CLI commands

| Command | What it does |
|---|---|
| `import file` | Ingest a single source file end-to-end. |
| `import bundle` | Ingest a folder of source files. |
| `import gutenberg` | Pull a Project Gutenberg work by ID. |
| `gutenberg top10` | Convenience batch for the Gutenberg Top 10. |
| `parse-only` | Parse the file but skip the LLM pipeline. |
| `fetch-only` | Download remote source without parsing. |
| `list` | List ingested resources. |
| `delete` | Remove a resource and its derivatives. |
| `export` | Export a course to disk. |

Pass `--verbose` to enable trace logging.

---

## Configuration

Connection string priority chain (consistent with every other MindAttic .NET project):

1. `ConnectionStrings__Tutor` env var
2. `ConnectionStrings:Tutor` from `IConfiguration` (`appsettings.json`)
3. LocalDB fallback for development

LLM credentials follow `MindAttic.Vault`'s standard resolution chain — see [Vault's README](../MindAttic.Vault/README.md) for the full source-precedence diagram.

---

## Tests

```powershell
# Unit / component tests
dotnet test Tutor.Tests

# End-to-end browser tests (Blazor host must be running on its dev port)
cd Tutor.Cypress
npm install
npx cypress run
```

The NUnit suite covers parsers, services, the concept-map JSON shape, and the route between the CLI and the Blazor host. The Cypress suite drives the UI from import to course view.

---

## Code style

Per `CLAUDE.md`:

- Private fields are `camelCase` **without** an underscore prefix.
- Constructors use `this.x = x` to disambiguate field assignment from parameter.
