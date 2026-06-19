# Tutor

**Drop a book in. Get a course out.**

Tutor turns books, papers, and documents into structured, navigable courses with quizzes and progress tracking. Hand it a PDF, EPUB, DOCX, or a Project Gutenberg ID — a multi-LLM pipeline extracts concepts, correlates them into a knowledge graph, and writes the learning path on the other side. RAG retrieval keeps every quiz and section grounded in the actual source material.

---

## How it works

```
SOURCE FILE -> PARSE -> CHUNK -> EMBED -> EXTRACT CONCEPTS -> CONCEPT MAP
                                                               |
                           KNOWLEDGE GRAPH <- CORRELATE <------+
                                                               |
                                       COURSE STRUCTURE <------+
```

- **Eight input formats.** Phase A (managed C#): PDF, EPUB, HTML, DOCX. Phase B (shell-out): `.doc`/`.rtf`/`.odt` via LibreOffice; `.mobi`/`.azw`/`.azw3` via Calibre `ebook-convert`.
- **Pluggable LLMs.** Routes to OpenAI, Claude, DeepSeek, or Gemini via `LlmServiceRouter`. Transport (auth, retry, circuit breaker) is owned by `MindAttic.Legion`.
- **Knowledge graph.** `ConceptMapService` extracts a concept JSON per chunk; `KnowledgeGraphService` correlates concepts across resources via LSH + SimHash; `OrphanConceptLinkerService` reattaches strays. The course is generated *from* the graph.
- **RAG.** `EmbeddingService` + `VectorStoreService` for semantic search; `SimHashService` + `LSHService` for near-duplicate detection.
- **Progress and quizzes.** Per-user progress tracking and an LLM-backed quiz controller.

---

## Stack

| Layer | Technology |
| --- | --- |
| Host | ASP.NET Core Blazor Server (.NET 10) |
| Headless | `tutor` CLI (`Tutor.Cli`) — same DI graph as the Blazor host |
| LLM transport | `MindAttic.Legion` (OpenAI / Claude / DeepSeek / Gemini) |
| Credentials | `MindAttic.Vault` — `%APPDATA%\MindAttic\LLM\providers.json` |
| Parsing | UglyToad.PdfPig (PDF), VersOne.Epub (EPUB), AngleSharp (HTML), Open-XML (DOCX) + LibreOffice / Calibre shell-outs |
| RAG | In-process vector store + LSH/SimHash |
| Tests | NUnit (`Tutor.Tests`) + Cypress (`Tutor.Cypress`) |

---

## Project structure

| Project | Purpose |
| --- | --- |
| `Tutor.Core` | Parsers, services, models — the whole pipeline lives here |
| `Tutor.Shared` | Razor components shared between hosts |
| `Tutor.Blazor` | Blazor Server host |
| `Tutor.Cli` | Headless pipeline driver — `tutor` binary |
| `Tutor.Tests` | NUnit unit/component tests |
| `Tutor.Cypress` | End-to-end browser tests (sibling Node project, not in `Tutor.slnx`) |

`Tutor.Core` references `MindAttic.Legion` (`..\..\MindAttic.Legion\MindAttic.Legion`) and `MindAttic.Vault`.

---

## Getting started

Prerequisites: .NET 10 SDK. Optional: LibreOffice (`.doc`/`.rtf`/`.odt`) and Calibre (`.mobi`/`.azw`). At least one LLM provider API key.

```powershell
dotnet build Tutor.slnx

dotnet run --project Tutor.Blazor
# -> https://localhost:5001

dotnet run --project Tutor.Cli -- --help
```

LLM API keys: `%APPDATA%\MindAttic\LLM\providers.json` or `MindAttic:Vault:LLM:*` in `IConfiguration`.

---

## CLI commands

| Command | What it does |
| --- | --- |
| `import file` | Ingest a single source file end-to-end |
| `import bundle` | Ingest a folder of source files |
| `import gutenberg` | Pull a Project Gutenberg work by ID |
| `gutenberg top10` | Convenience batch for Gutenberg Top 10 |
| `parse-only` | Parse without running the LLM pipeline |
| `fetch-only` | Download remote source without parsing |
| `list` | List ingested resources |
| `delete` | Remove a resource and its derivatives |
| `export` | Export a course to disk |
| `install` | Install a `.tutor` bundle (alias: `import-bundle`) |

Pass `--verbose` for trace logging.

---

## Pre-built courses (`Courses/`)

The `Courses/` directory ships `.tutor` bundles — self-contained zips with course structure, concept maps, quiz questions, and RAG embeddings. Install without re-running the LLM pipeline:

```powershell
tutor install "Courses/Moby Dick; Or, The Whale.tutor"
```

Included: Alice's Adventures in Wonderland, Dracula, Frankenstein, Gulliver's Travels, Moby Dick, Pride and Prejudice, The Adventures of Sherlock Holmes, The Adventures of Tom Sawyer, The Picture of Dorian Gray, Treasure Island.

---

## Configuration

Connection string priority:

1. `ConnectionStrings__Tutor` env var
2. `ConnectionStrings:Tutor` from `appsettings.json`
3. LocalDB fallback

LLM credentials follow `MindAttic.Vault`'s standard resolution chain.

---

## Tests

```powershell
dotnet test Tutor.Tests

cd Tutor.Cypress
npm install
npx cypress run    # Blazor host must be running on http://localhost:5200
```

---

## Code style

- Private fields: `camelCase` without underscore prefix.
- Constructors: `this.x = x` to disambiguate.
