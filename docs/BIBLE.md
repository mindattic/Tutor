---
codex: 1
project: Tutor
code: TUT
layer: bible
status: living
updated: 2026-10-03
---

# Tutor — Project Bible

> The single source of truth for *what Tutor is, what it is not, and the rules
> that keep it coherent*. The `README.md` tells you how to build and run; this
> tells you how to think about the system and why it's shaped the way it is.

---

## 1. The one sentence {#TUT-§1}

**Drop a book in. Get a course out.** Tutor turns books, papers, and documents
into structured, navigable courses — with a concept graph, a learning path,
baked quizzes, and RAG‑grounded answers — and lets those courses be packaged,
shared, loaded, and unloaded.

## 2. The product promise {#TUT-§2}

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

## 3. What Tutor is NOT {#TUT-§3}

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

## 4. Architecture canon {#TUT-§4}

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
| **Tutor.Core** | Parsers, the full pipeline, domain models, storage services, and course packaging (`Tutor.Core/Services/Packaging`). *Everything substantive lives here.* |
| **Tutor.Shared** | Razor components shared by the host (and any sibling shell), including the `/library` page. |
| **Tutor.Blazor** | Blazor Server host. Composition root (`Program.cs`), DI wiring, middleware, the bundle re‑share endpoint. |
| **Tutor.Cli** | `tutor` headless binary. Mirrors Blazor's DI graph; the terminal front door for build, export, install and delete. |
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
- **BundleManifest** — the `manifest.json` of a `.tutor` bundle: `FormatVersion`,
  stable `CourseKey` + whole‑number `CourseVersion`, payload `Sha256`, counts,
  `IncludesEmbeddings`, provenance (`Author`, `License`, `Description`,
  `SourceAttribution`) and an `Extra` dictionary that round‑trips unknown fields.
- **InstalledCourse** — one registry row per installed bundle (`CourseId`,
  `CourseKey`, `CourseVersion`, `Name`, `Sha256`, `BlobPath`, `Enabled`,
  `InstalledUtc`), kept in `{AppData}\InstalledCourses\installed-courses.json`.

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

### 4.4 Course packaging lifecycle

A `.tutor` file is a plain zip:

```
manifest.json            BundleManifest (identity, integrity, provenance, counts)
course.json              Course metadata (references only)
courseStructure.json     Lessons → Sections → ConceptIds + baked quizzes (optional)
resources/{id}.json      CourseResource metadata
resources/{id}.original.txt
resources/{id}.formatted.md
conceptMaps/{id}.json    One ConceptMap per resource
chunks.json              RAG chunks WITH pre-computed embeddings
```

Both front doors go through one code path, `CourseInstallService`:

- **Export** — `CourseExporter` writes the bundle, derives `CourseKey` from the
  course name when none is given, and stores the SHA‑256 of the payload
  (everything but the manifest, independent of entry order).
- **Inspect / validate** — `CourseManifestValidator` is pure and IO‑free. Hard
  errors block (no manifest, format newer than `HostMaxFormatVersion`, bad key,
  non‑positive version, no name, missing `course.json`, SHA mismatch, unsafe entry
  path); a missing key or SHA is a legacy warning, so older bundles still install.
- **Plan** — `CourseInstallResolver` decides a clean install, a no‑op (same key
  and version already installed), an upgrade (higher version), a refused
  downgrade, or a side‑by‑side duplicate (only with an explicit allow‑duplicate),
  comparing against the highest installed version of the same key.
- **Install** — `BundleImporter` remaps every GUID and persists through the same
  storage services the live app uses; `InstalledCourseRegistry` records the row
  and `CourseBlobStore` keeps the verbatim bundle at
  `{AppData}\courses\{key}\{version}.tutor` for re‑share.
- **Load / unload** — flips `InstalledCourse.Enabled`; the learning view filters
  out disabled courses, and no data or progress is touched.
- **Remove** — `CourseDeleteService` hard‑cascades resources, structure, concept
  maps and embeddings, then drops the registry row and blob (`tutor delete`, or
  Remove on `/library`).

The Blazor `/library` page (`[Authorize]`) lists installed courses with a
re‑share download (`/api/library/{courseId}/bundle`); install, load/unload and
remove are admin‑only. Install previews the manifest, provenance, validation
messages and the plan before asking to confirm.

---

## 5. The Laws (non‑negotiable rules) {#TUT-§5}

> Tutor inherits the org‑wide laws from
> **[MindAttic.HouseRules.md](../../MindAttic.HouseRules.md)** by reference — they
> are not restated here. The project‑specific laws below extend them.

**Inherited from House Rules** (authoritative text lives there):

- Whole‑number versioning — [see HOUSE-LAW-1](../../MindAttic.HouseRules.md#HOUSE-LAW-1)
- Soft‑disable, never hard‑delete — [see HOUSE-LAW-2](../../MindAttic.HouseRules.md#HOUSE-LAW-2)
  (Tutor: `IUserAdminService` is pinned by a contract test exposing **no hard
  delete**; course unload hides a course without erasing it, and only an explicit
  Remove cascades.)
- Credentials resolve through MindAttic.Vault — [see HOUSE-LAW-3](../../MindAttic.HouseRules.md#HOUSE-LAW-3)
  (Tutor: LLM keys via `%APPDATA%\MindAttic\LLM\providers.json` or
  `IConfiguration`; auth secrets via the Vault `Security` bucket.)
- Provider‑agnostic LLMs via MindAttic.Legion — [see HOUSE-LAW-4](../../MindAttic.HouseRules.md#HOUSE-LAW-4)
  (Tutor: route through `LlmServiceRouter`; never reference a vendor SDK directly
  from pipeline code.)
- Packaging is a guarded zip with a lifecycle — [see HOUSE-LAW-5](../../MindAttic.HouseRules.md#HOUSE-LAW-5)
  (Tutor: the `.tutor` lifecycle in [§4.4](#TUT-§4). Courses are pure data, so
  nothing in a bundle is ever loaded as code.)
- One engine, many front doors — [see HOUSE-LAW-6](../../MindAttic.HouseRules.md#HOUSE-LAW-6)
  (Tutor: a service registered for Blazor must be registered identically for the
  CLI; a course built by one is readable by the other with **zero translation**.)
- Authentication via MindAttic.Authentication — [see HOUSE-LAW-7](../../MindAttic.HouseRules.md#HOUSE-LAW-7)
- Definition of done is verified, not asserted — [see HOUSE-LAW-8](../../MindAttic.HouseRules.md#HOUSE-LAW-8)

**Project‑specific laws:**

1. **Grounding over generation.** {#TUT-LAW-1} Content shown to a learner is
   retrieved from source via RAG, not invented. Quizzes and section fills cite
   the page. This is Tutor's defining constraint: the source is retained for
   grounding, never replayed verbatim, and never substituted by free‑floating
   generation.
2. **The graph is the product, not the source bytes.** {#TUT-LAW-2} Courses are
   generated *from* the correlated knowledge graph, and a `Course` never embeds
   content — it references resources, structure, concept maps, and chunks. This
   is what keeps bundles composable and storage deduplicated (see the §4.2
   invariant).
3. **Bundles are independent; never merge courses.** {#TUT-LAW-3} A `.tutor`
   bundle installs as its own course; courses are never merged into one another.
   Re‑installing the same key and version is a no‑op, a lower version is refused,
   and only an explicit allow‑duplicate installs a second, independent copy (all
   GUIDs remapped).
4. **Code style.** {#TUT-LAW-4} (from `CLAUDE.md`): private fields are
   `camelCase` **without** underscore prefix; constructors use `this.x = x`.

---

## 6. Verified state {#TUT-§6}

> What's proven working, with the build/test evidence. Status legend:
> ✅ verified · 🟡 partial · ⬜ planned.

**Build/test evidence:** the last green run on record is `dotnet test Tutor.Tests`
→ **Passed: 380, Failed: 0, Skipped: 0** (2026-06-07). The packaging tests in
`Tutor.Tests/Packaging` were added after that run and have no green run on record.

**Current build: broken (checked 2026-10-03).** `dotnet test Tutor.Tests` does not
compile at HEAD:

- `ClaudeService`, `DeepSeekService`, `GeminiService` and `OpenAIService` are
  declared in the global namespace and call `KeyPoolFailover`, which lives in
  `Tutor.Core.Services`, without a `using` (CS0103).
- `Tutor.Tests/Fakes/FakeSecurePreferences.cs` does not implement
  `ISecurePreferences.GetApiKeysAsync` / `SetApiKeysAsync` (CS0535).

Until the suite compiles again, ✅ stories rest on the 2026‑06‑07 run and the
packaging stories stay 🟡.

### 6.1 Authentication canon ✅

Tutor authenticates through **MindAttic.Authentication**
([HOUSE-LAW-7](../../MindAttic.HouseRules.md#HOUSE-LAW-7)), package 2.0.0 (Debug
builds also reference the sibling source project). The shape:

- **Storage:** SQL Server (`TutorAuthDbContext`). `AuthUsers` (Argon2id + pepper)
  and `AuthSessions` (idle 30 min / absolute 8 h, IP+UA bound, revocable).
  Connection string `TutorAuth`, LocalDB fallback in dev. SQL holds auth only;
  courses and progress stay file‑based.
- **Wiring:** `AddMindAtticAuthentication<TutorAuthDbContext>` +
  `UseMindAtticAuthentication()` + `MapMindAtticAuthEndpoints()` (`/_ma-auth/*`).
- **Bootstrap:** dev migrate → `AuthUserImportService` imports legacy
  `Users.json` accounts (SHA‑256 carried forward, upgraded transparently on first
  login, weak dev seeds flagged `MustChangePassword`) →
  `AuthBootstrapper.SeedAdminAsync` (operator‑provided Vault
  `Security:bootstrap-token`, fail‑closed).
- **UI:** shared `UserCircle` (avatar/role menu + logout), `UserTimeout`
  (idle‑logout modal), `UserLogin` (styles `MaLogin`). Admin **Users** page at
  `/users` behind `[Authorize(Policy = MaPolicies.Admin)]` — create / edit role /
  reset password / **soft‑disable** (no hard delete).
- **Verified by:** `AuthUserImportTests`, `UsersAdminContractTests`,
  `auth.cy.ts`. See [stories C1–C6](USER_STORIES.md#TUT-EPIC-C).

### 6.2 Course lifecycle ✅

Build → lock → unlock by mastery → final exam → certificate → unload, pinned by
`FullCourseLifecycleTests` (`Student_WorksThroughCourse_FromLockedLessonsToCertificate`,
`FailingFinalExam_DoesNotCompleteOrCertify`). See
[stories B1–B4](USER_STORIES.md#TUT-EPIC-B).

### 6.3 Course packaging 🟡

The lifecycle in [§4.4](#TUT-§4) is built and covered by `BundleRoundTripTests`,
`CourseManifestValidatorTests`, `CourseInstallResolverTests`,
`CourseLifecycleRegistryTests` and `BundleArchiveSafetyTests`, none of which has a
green run on record (see above). The `/library` page has no automated UI test.
See [Epic D](USER_STORIES.md#TUT-EPIC-D) and [Epic E](USER_STORIES.md#TUT-EPIC-E).

### 6.4 Known limitation 🟡

`QuizService` reads the user id from `IHttpContextAccessor`, which is null after
the initial Blazor Server render, so mid‑circuit quiz starts attribute to
`"anonymous"`. Fix is to thread the id from `AuthenticationState`. Tracked as a
TODO in `Tutor.Core/Services/Quiz/QuizService.cs` and as
[story B5](USER_STORIES.md#TUT-EPIC-B).

---

## 7. Active frontier {#TUT-§7}

There are no open RFCs. The open work, in order (see the
[priority backlog](USER_STORIES.md)):

1. **Get the build green again** ([§6](#TUT-§6)) and record a full
   `Tutor.Tests` run, which promotes the packaging stories (Epics D and E).
2. **Per‑user quiz attribution** ([story B5](USER_STORIES.md#TUT-EPIC-B)).
3. **In‑app course building and monitoring** ([story A6](USER_STORIES.md#TUT-EPIC-A)).
4. **Automated UI coverage** for the idle‑timeout modal (C4) and the `/library`
   page (E1).

---

## 8. Quality bar {#TUT-§8}

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
  (See [HOUSE-LAW-8](../../MindAttic.HouseRules.md#HOUSE-LAW-8).)

---

## 9. Glossary {#TUT-§9}

| Term | Meaning |
|---|---|
| **Resource** | One ingested source document. |
| **Concept map** | Per‑resource graph of concepts + relationships. |
| **Knowledge base / collection** | Aggregated concept maps for a whole course. |
| **Course structure** | Ordered lessons/sections referencing concepts. |
| **Chunk** | Embedded text snippet for RAG. |
| **Bundle / `.tutor`** | A shareable zipped course (with embeddings). |
| **Course key / version** | A bundle's stable slug identity and whole‑number version; what install plans compare. |
| **Install** | Validate, plan and import a bundle, recording an `InstalledCourse` row and keeping the verbatim blob. |
| **Load / Unload** | Enable / disable an installed course in the learning view without touching its data. |
| **Remove** | Hard cascade delete of a course, its registry row and its retained blob. |
| **Legion** | `MindAttic.Legion` — LLM transport library. |
| **Vault** | `MindAttic.Vault` — credential resolution library. |
