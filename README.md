# Tutor

A Blazor Server application that turns books and documents into navigable
courses. Tutor parses source material (PDF, EPUB, HTML, DOCX, .doc, .rtf,
.odt, .mobi, .azw), extracts concepts via an LLM, builds a concept map
plus a knowledge graph, then generates a structured learning path you can
read, quiz against, and track progress through.

## What it does

```
SOURCE FILE ─► PARSE ─► CHUNK ─► EMBED ─► EXTRACT CONCEPTS ─► CONCEPT MAP
                                                                  │
                              KNOWLEDGE GRAPH ◄─ CORRELATE ◄──────┘
                                                                  │
                                          COURSE STRUCTURE ◄──────┘
```

- **Multi-format ingest.** Phase A parsers (managed C#) handle PDF, EPUB,
  HTML, DOCX. Phase B shells out to LibreOffice for legacy .doc/.rtf/.odt
  and to Calibre's `ebook-convert` for .mobi/.azw/.azw3.
- **Pluggable LLMs.** Routes to OpenAI, Claude, DeepSeek, or Gemini via
  `LlmServiceRouter`. Wire transport (auth, retry, circuit breaker) is
  owned by `MindAttic.Legion` so every provider gets the same reliability
  surface.
- **Concept map + knowledge graph.** `ConceptMapService` extracts a JSON
  concept document per chunk; `KnowledgeGraphService` correlates concepts
  across resources; `OrphanConceptLinkerService` reattaches strays.
- **Course generation.** `CourseStructureService` turns the merged map
  into a hierarchical learning path; `SectionContentService` fills each
  section with content drawn from the source material.
- **RAG retrieval.** `EmbeddingService` + `VectorStoreService` for
  semantic search; `SimHashService` and `LSHService` for lexical
  fingerprinting and near-duplicate detection.
- **Progress + quizzes.** Per-user progress tracking and an LLM-backed
  quiz controller.

## Project structure

| Project | Purpose |
|---------|---------|
| **Tutor.Core** | Parsers, services, models. The whole pipeline lives here. |
| **Tutor.Shared** | Razor components shared between the Blazor host and any future MAUI shell. |
| **Tutor.Blazor** | Blazor Server host app. |
| **Tutor.Cli** | Headless pipeline driver — `tutor` binary. Mirrors the Blazor service registration so courses produced from the CLI are readable from the UI without translation. |
| **Tutor.Tests** | NUnit test suite. |

`Tutor.Core` references `MindAttic.Legion` (sibling repo, `..\..\MindAttic.Legion\MindAttic.Legion`).

## Getting started

```powershell
# Build everything
dotnet build Tutor.slnx

# Run the Blazor host
dotnet run --project Tutor.Blazor

# Or run the CLI
dotnet run --project Tutor.Cli -- --help
```

LLM API keys are stored under `%APPDATA%\MindAttic\LLM\` so a rotation
propagates to every MindAttic app on the same machine.

## CLI commands

| Command | What it does |
|---------|--------------|
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

## Code style

Per `CLAUDE.md`: private fields are camelCase without an underscore
prefix; constructors use `this.x = x` to disambiguate field assignment
from parameter.
