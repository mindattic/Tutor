# Tutor — Project Bible

> The single source of truth for *what Tutor is, what it is not, and the rules
> that keep it coherent*. The `README.md` tells you how to build and run; this
> tells you how to think about the system and why it's shaped the way it is.

---

## 1. The one sentence

**Drop a book in. Get a course out.** Tutor turns books, papers, and documents
into structured, navigable courses — with a concept graph, a learning path,
baked quizzes, and RAG‑grounded answers — and lets those courses be packaged,
shared, loaded, and unloaded.

## 2. The product promise

1. **You bring the source. Tutor builds the course.** A PDF, EPUB, DOCX, HTML,
   legacy `.doc/.rtf/.odt`, `.mobi/.azw/.azw3`, or a Project Gutenberg ID goes
   in; a hierarchical course (Lessons → Sections → concepts, with quizzes) comes
   out. No manual authoring required.
2. **The knowledge graph is the product, not the source bytes.** Concepts are
   extracted per chunk, correlated across resources into one graph, and the
   course is generated *from the graph*. The source is retained for grounding,
   not replayed verbatim.
3. **Answers are grounded.** Every quiz prompt and section fill is retrieved
   from the actual source via embeddings + RAG. Tutor does not hallucinate
   content that isn't on the page.
4. **Courses travel.** A finished course is a self‑contained `.tutor` bundle —
   structure, concepts, quizzes, and pre‑computed embeddings — that installs
   without re‑running the (slow, paid) LLM pipeline.
5. **Same engine, two front doors.** Anything the CLI can build, the Blazor app
   can read, because both register the identical service graph.

## 3. What Tutor is NOT

- **Not a live tutor chatbot bolted onto a PDF.** The graph and structure are
  first‑class artifacts; chat is grounded in them, not a free‑floating assistant.
- **Not a multi‑tenant SaaS (yet).** It is a single‑host Blazor Server app with
  SQL‑backed auth. Designs should not assume horizontal scale-out or per‑tenant
  isolation unless that becomes an explicit goal.
- **Not provider‑locked.** No code path may hard‑code a single LLM vendor; all
  LLM calls route through `LlmServiceRouter` over `MindAttic.Legion`.
- **Not a content store that merges courses.** Bundles are independent and are
  never merged into one another.

---

## 4. Architecture canon

```
SOURCE FILE ─► PARSE ─► CHUNK ─► EMBED ─► EXTRACT CONCEPTS ─► CONCEPT MAP
                                                                  │
                              KNOWLEDGE GRAPH ◄─ CORRELATE ◄──────┘
                                                                  │
                                          COURSE STRUCTURE ◄──────┘
                                                                  │
                                          .tutor BUNDLE ◄─────────┘  (share / load / unload)
```

### 4.1 Projects (the only legitimate homes for code)

| Project | Owns |
|---|---|
| **Tutor.Core** | Parsers, the full pipeline, domain models, storage services. *Everything substantive lives here.* |
| **Tutor.Shared** | Razor components shared by the host (and any sibling shell). |
| **Tutor.Blazor** | Blazor Server host. Composition root (`Program.cs`), DI wiring, middleware. |
| **Tutor.Cli** | `tutor` headless binary. Mirrors Blazor's DI graph; owns bundle export/import. |
| **Tutor.Tests** | NUnit unit/component suite. |
| **Tutor.Cypress** | E2E browser tests (Node sibling, not in `Tutor.slnx`). |

### 4.2 Domain model (the nouns)

- **CourseResource** — one ingested source (book/paper/doc). Has original +
  AI‑formatted content. Produces exactly **one ConceptMap** (1:1).
- **ConceptMap** — concepts + relationships + complexity ordering for a single
  resource.
- **ConceptMapCollection** (`collection_{courseId}`) — the aggregated graph
  across all of a course's resources.
- **Course** — lightweight metadata + **references only** (resource IDs, the
  collection ID, the structure ID). Carries no content directly.
- **CourseStructure** — the learning path: ordered **Lessons → hierarchical
  Sections → ConceptIds**, plus baked `PreGeneratedQuestions`.
- **ContentChunk** — a text snippet + its 1536‑dim embedding + LSH/SimHash
  signatures. The RAG substrate.
- **UserProgress** — per‑user, per‑course mastery state driving lesson gating.

> **Invariant:** a `Course` never embeds content. Content lives in resources,
> structure, concept maps, and chunks; the course just points at them. Keep it
> that way — it's what makes bundles composable and storage deduplicated.

### 4.3 Key services (the verbs)

`LlmServiceRouter` (provider selection) · `ConceptMapService` (extract) ·
`KnowledgeGraphService` (correlate via LSH+SimHash) · `OrphanConceptLinkerService`
(reattach strays) · `CourseStructureService` (graph → learning path) ·
`SectionContentService` (fill sections) · `EmbeddingService` + `VectorStoreService`
(RAG) · `QuizService` (LLM‑backed quizzing) · `LearningPathService` (mastery
gating) · `FinalExamService` + `CertificateService` (completion).

---

## 5. The laws (non‑negotiable rules)

1. **Provider‑agnostic LLMs.** Never reference a vendor SDK directly from
   pipeline code. Route through `LlmServiceRouter`; transport (auth, retry,
   circuit breaker) belongs to `MindAttic.Legion`.
2. **Same DI graph in both front doors.** A service registered for Blazor must
   be registered identically for the CLI. A course built by one is readable by
   the other with **zero translation**.
3. **Grounding over generation.** Content shown to a learner is retrieved from
   source via RAG, not invented. Quizzes and section fills cite the page.
4. **Whole‑number versioning.** Packages/assemblies and bundle versions bump by
   whole numbers only (`1`, `2`, `3` — never `2.6.0`). House rule, all MindAttic
   projects.
5. **Soft‑disable, don't destroy.** User administration exposes **no hard
   delete** (`IUserAdminService` is pinned by a contract test). Course
   load/unload should follow the same instinct: unload hides, it doesn't erase.
6. **Credentials never live in code.** LLM keys resolve through `MindAttic.Vault`
   (`%APPDATA%\MindAttic\LLM\providers.json` or `IConfiguration`). Auth secrets
   resolve through the Vault `Security` bucket.
7. **Code style** (from `CLAUDE.md`): private fields are `camelCase` **without**
   underscore prefix; constructors use `this.x = x`.

---

## 6. Authentication canon *(verified working — build clean, 81/81 tests pass)*

Tutor adopted **MindAttic.Authentication v1.0.0**, retiring the old in‑memory
JSON auth. The shape:

- **Storage:** SQL Server (`TutorAuthDbContext`). `AuthUsers` (Argon2id + pepper)
  and `AuthSessions` (idle 30 min / absolute 8 h, IP+UA bound, revocable).
  Connection string `TutorAuth`, LocalDB fallback in dev.
- **Wiring:** `AddMindAtticAuthentication<TutorAuthDbContext>` +
  `UseMindAtticAuthentication()` + `MapMindAtticAuthEndpoints()` (`/_ma-auth/*`).
- **Bootstrap:** dev migrate → `AuthUserImportService` migrates legacy
  `Users.json` (SHA‑256 carried forward, upgraded transparently on first login,
  weak dev seeds flagged `MustChangePassword`) → `AuthBootstrapper.SeedAdminAsync`
  (operator‑provided Vault `Security:bootstrap-token`, fail‑closed).
- **UI:** shared `UserCircle` (avatar/role menu + logout), `UserTimeout`
  (idle‑logout modal), `UserLogin` (styles `MaLogin`). Admin **Users** page at
  `/users` behind `[Authorize(Policy = MaPolicies.Admin)]` — create / edit role /
  reset password / **soft‑disable** (no hard delete).
- **Known limitation (documented, not a regression):** `QuizService` reads the
  user id from `IHttpContextAccessor`, which is null after the initial Blazor
  Server render, so mid‑circuit quiz starts attribute to `"anonymous"`. Fix is
  to thread the id from `AuthenticationState`. Tracked as a TODO in
  `Tutor.Core/Services/Quiz/QuizService.cs`.

---

## 7. Course packaging & sharing *(the active design frontier)*

A `.tutor` file is a zip carrying `manifest.json`, `course.json`,
`courseStructure.json`, per‑resource JSON/text, concept maps, and `chunks.json`
(**embeddings included** so re‑imports skip the LLM pipeline). Import remaps all
GUIDs so the same bundle installs twice as two independent courses.

The roadmap is to mature the *lifecycle* — stable `CourseKey` + version,
SHA‑256 integrity, a forgiving format gate, a validate‑then‑plan step, an
installed‑courses registry with **soft unload**, and an **in‑app course
library** — modeled on the patterns proven in **MindAttic.Ideas** (`.idea`
packages), while explicitly *not* adopting its assembly‑loading machinery
(courses are pure data). See **[COURSE_PACKAGING_DESIGN.md](COURSE_PACKAGING_DESIGN.md)**
for the full comparison and phased plan, and **[USER_STORIES.md](USER_STORIES.md)**
for the acceptance‑level requirements.

---

## 8. Quality bar

- **NUnit** (`Tutor.Tests`) covers parsers, services, the concept‑map JSON shape,
  the CLI↔Blazor route, auth import/admin contracts, and the full course
  lifecycle (lock → unlock → final exam → certificate → unload).
- **Cypress** (`Tutor.Cypress`) drives the live UI: auth redirect guards and the
  deterministic course‑flow wiring. The LLM‑driven learning loop is intentionally
  *not* automated in‑browser (non‑deterministic, paid); its logic is pinned by
  `FullCourseLifecycleTests` instead.
- **Definition of done for a feature:** clean `dotnet build Tutor.slnx`, green
  `Tutor.Tests`, and — for anything user‑facing — a Cypress guard or a lifecycle
  assertion. No vendor lock‑in introduced. House versioning respected.

---

## 9. Glossary

| Term | Meaning |
|---|---|
| **Resource** | One ingested source document. |
| **Concept map** | Per‑resource graph of concepts + relationships. |
| **Knowledge base / collection** | Aggregated concept maps for a whole course. |
| **Course structure** | Ordered lessons/sections referencing concepts. |
| **Chunk** | Embedded text snippet for RAG. |
| **Bundle / `.tutor`** | A shareable zipped course (with embeddings). |
| **Load / Unload** | Install a bundle / hide it without destroying its data. |
| **Legion** | `MindAttic.Legion` — LLM transport library. |
| **Vault** | `MindAttic.Vault` — credential resolution library. |
