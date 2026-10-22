---
codex: 1
project: Tutor
code: TUT
layer: stories
status: living
updated: 2026-10-03
---
<!-- GENERATED from MindAttic.Tickets/data/Tickets.db by `tickets db render`. Do not edit: this file is rewritten from the database; change stories and tickets with the tickets CLI. -->

# Tutor — User Stories

> Acceptance‑level requirements, grouped by epic. Status legend:
> **✅ Done** (shipped & tested) · **🟡 Partial** (works, gaps noted) ·
> **⬜ Planned** (designed, not built). Every ✅ cites the test that proves it. IDs are stable (`TUT-US-<Epic><n>`); never reference line numbers.

Personas:

- **Author/Operator** — builds and curates courses (CLI; admin in‑app).
- **Learner/Student** — consumes courses, takes quizzes, earns certificates.
- **Admin** — manages users and the course library.
- **Sharer/Recipient** — passes a `.tutor` file to someone, or receives one.

---

## Epic A — Ingest & Course Building {#TUT-EPIC-A}

- **TUT-US-A1 🟡** As an Author, I can ingest a single source file (PDF, EPUB,
  HTML, DOCX) so it becomes a course resource.
  *Given* a supported file, *when* I run `tutor import <path> --course "Name"`, *then* it is
  parsed, chunked, embedded, and a concept map is produced.
  *Verified in part by parser tests* (`TxtBookParserTests`, `HtmlBookParserTests`,
  `ParserRegistryTests`, `ExtractedBookTests`) *and* `ChunkingServiceTests`; the
  end‑to‑end LLM pipeline run is not automated (paid/non‑deterministic).
- **TUT-US-A2 🟡** As an Author, I can ingest legacy formats (`.doc/.rtf/.odt` via
  LibreOffice; `.mobi/.azw/.azw3` via Calibre) without the downstream pipeline
  caring which format it was. *Gap:* shell‑out conversion is not unit‑tested
  (requires LibreOffice/Calibre on the box); downstream parsing is covered by the
  parser tests above.
- **TUT-US-A3 🟡** As an Author, I can pull a Project Gutenberg work by ID, and
  seed a starter library with `tutor gutenberg-top10`. *Gap:* the fetch path hits
  a live network and is not automated.
- **TUT-US-A4 🟡** As an Author, the system extracts concepts, correlates them
  into one knowledge graph across resources, and reattaches orphan concepts.
  *Correlation primitives verified by* `LSHServiceTests`, `SimHashServiceTests`,
  `KnowledgeGraphTests`, `CoreConceptServiceTests`; the LLM extraction step itself
  is not automated.
- **TUT-US-A5 🟡** As an Author, a hierarchical course structure (Lessons →
  Sections → concepts) is generated *from the graph*, with section content filled
  from the source. *Structure shape verified by* `CourseStructureBuildTaskHandlerTests`;
  LLM section fill is not automated.
- **TUT-US-A6 🟡** As an Author, I can do all of the above from the Blazor UI.
  *Gap:* building is CLI‑first; in‑app build/monitoring is partial.

## Epic B — Learning Experience {#TUT-EPIC-B}

- **TUT-US-B1 ✅** As a Learner, I see only the first lesson unlocked, and later
  lessons unlock as I demonstrate mastery. *(verified by `LearningPathServiceTests`
  and `FullCourseLifecycleTests.Student_WorksThroughCourse_FromLockedLessonsToCertificate`.)*
- **TUT-US-B2 ✅** As a Learner, I take quizzes whose questions are grounded in
  the source material via RAG. *(verified by `QuizGenerationServiceTests` — quizzes
  are baked from source‑grounded content — and `quiz.cy.ts` — the Quiz tab renders
  from baked questions without a live LLM call.)*
- **TUT-US-B3 ✅** As a Learner, once I master the course the final exam unlocks;
  passing it issues a certificate. *(verified by `FinalExamServiceTests`,
  `CertificateAuthorityTests`, and
  `FullCourseLifecycleTests.Student_WorksThroughCourse_FromLockedLessonsToCertificate`.)*
- **TUT-US-B4 ✅** As a Learner, my progress is tracked per‑user, per‑course and
  is isolated from other learners. *(verified by `UserProgressTests` and
  `FullCourseLifecycleTests` — a fresh student starts locked.)*
- **TUT-US-B5 🟡** As a Learner, my quiz results are attributed to *me*.
  *Gap:* quizzes started mid‑Blazor‑circuit fall back to `"anonymous"` because
  `QuizService` reads `IHttpContextAccessor` (null post‑render). Fix: thread the
  id from `AuthenticationState`. *(TODO in `QuizService.cs`; see
  [BIBLE §6.3](BIBLE.md#TUT-§6).)*

## Epic C — Authentication & Accounts {#TUT-EPIC-C}

- **TUT-US-C1 ✅** As a Learner, I must log in; visiting `/courses`, `/learn`, or
  `/settings` while logged out redirects me to login. *(verified by `auth.cy.ts`.)*
- **TUT-US-C2 ✅** As a user, my password is stored with Argon2id + pepper
  (SQL‑backed). *(verified by `AuthUserImportTests`
  — legacy SHA‑256 accounts import into the SQL `AuthUsers` store and upgrade on
  first login; Argon2id hashing itself is owned and tested by
  MindAttic.Authentication.)*
- **TUT-US-C3 ✅** As an existing user, my legacy account is imported on startup
  and my password upgrades transparently on first login; weak dev seeds are forced
  to reset. *(verified by `AuthUserImportTests` — idempotent, role mapping, GUID
  preservation: `IsIdempotent_SecondRunImportsZero`, `Ryan_ImportsAsAdmin_WithSha256_AndForceReset`,
  `PreservesGuidId_WhenParseable`.)*
- **TUT-US-C4 🟡** As a user, an idle session warns me and auto‑logs me out
  (30 min idle / 8 h absolute). *Gap:* timing is enforced by `UserTimeout` +
  `user-timeout.js` and the session policy in MindAttic.Authentication; no
  in‑repo automated test pins the modal/timeout behavior.
- **TUT-US-C5 ✅** As an Admin, at `/users` I can create users, set roles, reset
  passwords, and **soft‑disable** accounts — with **no hard delete** exposed.
  *(verified by `UsersAdminContractTests.ExposesNoHardDeleteApi` and
  `UsersAdminContractTests.ExposesSoftDisableAndCrud`.)*
- **TUT-US-C6 ✅** As an Admin, the Users page is reachable only with the Admin
  policy. *(enforced by `[Authorize(Policy = MaPolicies.Admin)]`; redirect‑guard
  behavior covered by `auth.cy.ts`.)*
- **TUT-US-C7 ✅** As a user who forgot my password, I follow "Forgot password?" on `/login`, get an
  emailed link to `https://localhost:7200/account/reset` (`MindAttic:Auth:Reset:PublicBaseUrl` in
  `Tutor.Blazor/appsettings.json` + the library's `ResetPath`), and setting a new password there replaces
  the old one. Mail is sent only when the Vault `Notifications` SMTP settings are configured. *(verified by
  `PasswordResetFlowTests.RequestReset_EmailsAnAbsoluteLinkToTheResetPage_WhichResetsThePassword` and
  `PasswordResetFlowTests.ResetAndForgotPages_AreAnonymousStaticPages`.)*

## Epic D — Course Packaging {#TUT-EPIC-D}

> Built in `Tutor.Core/Services/Packaging` ([BIBLE §4.4](BIBLE.md#TUT-§4)). Each
> story names its covering test; the suite is green ([BIBLE §6](BIBLE.md#TUT-§6)).

- **TUT-US-D1 ✅** As an Author, I can export a course to a self‑contained
  `.tutor` bundle that includes pre‑computed embeddings, so re‑import skips the
  LLM pipeline. *(verified by
  `BundleRoundTripTests.Import_RoundTrips_ContentStructureAndEmbeddings`.)*
- **TUT-US-D2 ✅** As a Recipient, I can install a bundle with `tutor install
  <file>`; installing an explicit duplicate yields two independent courses (all
  IDs remapped). *(verified by
  `BundleRoundTripTests.ImportingTwice_YieldsTwoIndependentCourses` and
  `CourseLifecycleRegistryTests.ExplicitDuplicate_InstallsSideBySide_AsIndependentCourse`.)*
- **TUT-US-D3 ✅** As an Author, I can remove a course and all its derivatives
  with `tutor delete <id>` (cascades resources, structure, concept maps,
  embeddings, registry row and retained blob). *(verified by
  `CourseLifecycleRegistryTests.Remove_HardCascades_DataRegistryRowAndBlob` and
  `DeleteService_DryRunPlan_DoesNotDeleteAnything`.)*
- **TUT-US-D4 ✅** As a Recipient, a bundle carries a **stable course key +
  whole‑number version**, so the system recognizes "you already have *Dracula*":
  same version is a no‑op, a newer one upgrades, an older one is refused unless I
  ask for a duplicate. *(verified by `CourseInstallResolverTests` and
  `CourseLifecycleRegistryTests.ReinstallingSameVersion_NoOps_WithoutDuplicating`,
  `NewerVersion_InstallsAsUpgrade`, `Downgrade_IsRefused`.)*
- **TUT-US-D5 ✅** As a Recipient, a corrupted or tampered `.tutor` is rejected up
  front with a clear reason, via a **SHA‑256 integrity check** and a manifest‑first,
  IO‑free **validation pass** with explicit error codes. *(verified by
  `CourseManifestValidatorTests`,
  `BundleRoundTripTests.TamperedBundle_IsRejected_WithShaMismatch` and
  `CourseLifecycleRegistryTests.InvalidBundle_NeverTouchesRegistryOrStore`.)*
- **TUT-US-D6 ✅** As an Author on a newer build, I can still read **older** bundle
  formats; only formats *newer than my build* are refused, and unknown manifest
  fields round‑trip. *(verified by `BundleRoundTripTests.FormatNewerThanBuild_IsRefused`,
  `LegacyBundle_WithoutKeyShaOrNewFields_StillImports`,
  `UnknownManifestFields_RoundTripThroughExtra`.)*

## Epic E — Load / Unload & Sharing {#TUT-EPIC-E}

- **TUT-US-E1 🟡** As an Admin, I can **load** a course from a `.tutor` file **in
  the Blazor app** at `/library` (upload → validate → plan → confirm → install).
  *Gap:* no automated UI test; the shared install path is covered by
  `CourseLifecycleRegistryTests`.
- **TUT-US-E2 ✅** As an Admin, I can **unload** a course so it disappears from the
  learning view **without destroying** its data or progress (soft‑disable,
  `InstalledCourse.Enabled = false`), per
  [HOUSE-LAW-2](../../MindAttic.HouseRules.md#HOUSE-LAW-2); a separate explicit
  "Remove" performs the hard cascade. *(verified by
  `CourseLifecycleRegistryTests.Unload_SoftDisables_WithoutDestroyingDataOrProgress`.)*
- **TUT-US-E7 ✅** As an Operator, only an Admin can install, load, unload or remove a
  course on `/library`: the server checks the Admin policy on every one of those
  actions, so a non‑admin (or anonymous) call is refused and changes nothing, even
  though the page also hides the controls. *(verified by
  `LibraryAdminActionsTests.NonAdmin_LoadUnloadRemove_AreRefused_AndChangeNothing`,
  `NonAdmin_PreviewAndInstall_AreRefused_AndInstallNothing`,
  `Admin_CanUnloadLoadAndRemove`.)*
- **TUT-US-E3 🟡** As a signed‑in user, I can see **what is installed** — name, key,
  version, install date, integrity hash, enabled state — in one list on `/library`.
  *(registry covered by
  `CourseLifecycleRegistryTests.Install_RecordsRegistryRow_AndRetainsVerbatimBlob`;
  the list UI is not automated.)*
- **TUT-US-E4 ✅** As a Sharer, I can **re‑share** exactly the bundle I installed,
  because the verbatim `.tutor` is retained at
  `{AppData}\courses\{key}\{version}.tutor` in the Tutor data folder and downloadable from `/library`.
  *(verified by
  `CourseLifecycleRegistryTests.Install_RecordsRegistryRow_AndRetainsVerbatimBlob`.)*
- **TUT-US-E5 🟡** As a Recipient, I see a course's **provenance** before
  installing — author, license, source attribution, description. *(manifest fields
  covered by `BundleRoundTripTests.Export_WritesManifest_WithIdentityIntegrityAndProvenance`;
  the `/library` preview is not automated.)*
- **TUT-US-E6 ✅** As an Operator, a bundle with unsafe entry paths (rooted,
  drive‑letter, `..` escape) is rejected before install, preventing zip‑slip.
  *(verified by `BundleArchiveSafetyTests.UnsafePaths_AreRejected` and
  `CourseManifestValidatorTests.UnsafeEntryPath_FailsWithUnsafeEntryPath`.)*

## Epic F — Cross‑cutting quality (always‑on constraints)

- **TUT-US-F1 ✅** As any contributor, the solution builds clean
  (`dotnet build Tutor.slnx`) and `Tutor.Tests` is green before merge.
  *(verified 2026-10-03: build 0 errors; `dotnet test Tutor.Tests` 454 passed, 0 failed;
  see [BIBLE §6](BIBLE.md#TUT-§6).)*
- **TUT-US-F2 ✅** As any contributor, no code path hard‑codes an LLM vendor; all
  calls route through `LlmServiceRouter` over Legion. *(verified by
  `LlmServiceRouterTests`; see [TUT-LAW](BIBLE.md#TUT-§5) /
  [HOUSE-LAW-4](../../MindAttic.HouseRules.md#HOUSE-LAW-4).)*
- **TUT-US-F3 ✅** As any contributor, a CLI‑built course is readable by the
  Blazor app with zero translation (shared DI graph). *(verified by
  `DependencyInjectionTests`.)*
- **TUT-US-F4 ✅** As the org, versions bump by whole numbers only.
  *([HOUSE-LAW-1](../../MindAttic.HouseRules.md#HOUSE-LAW-1).)*
- **TUT-US-F5 ✅** As the org, credentials resolve through Vault, never embedded
  in code. *(verified by `LlmCredentialTests`;
  [HOUSE-LAW-3](../../MindAttic.HouseRules.md#HOUSE-LAW-3).)*

---

## Priority backlog (toward the "share learning" goal)

1. **TUT-US-F1** — fix the compile errors and record a green `Tutor.Tests` run;
   that promotes D1–D6, E2, E4 and E6 to ✅.
2. **TUT-US-E1 / E3 / E5** — automated UI coverage for `/library`.
3. **TUT-US-B5** — real per‑user quiz attribution.
4. **TUT-US-C4** — automated test for the idle‑timeout modal.
5. **TUT-US-A6** — in‑app course building and monitoring.

## Tickets
> Permanent log of every change and fix — this repo's ticket system. Rows are append-only: never
> delete or reword one; only move its Status (open → closed / wont-do) and fill in Ref. Status is
> a word, not a glyph, so tickets don't count toward the story index. Keep this section last.

| ID | Date | Type | Status | Summary | Ref |
|---|---|---|---|---|---|
| TUT-T1 | 2026-10-12 | docs | closed | Start the permanent ticket log in USER_STORIES.md | — |
| TUT-T2 | 2026-10-12 | chore | closed | /commit now logs every commit as a ticket in this table | `.claude/skills/commit/SKILL.md` |
| TUT-T3 | 2026-10-14 | chore | open | NUnit tests for LLM key-pool failover: KeyPoolFailover.ExecuteAsync (empty pool throws InvalidOperationException, first success returns without trying later keys, 401/408/429/5xx/null-status HttpRequestException and TaskCanceledException fail over to the next key, 400-class and non-HTTP exceptions rethrow immediately, last key's exception rethrows, caller cancellation stops failover) and IsKeyLevelFailure status mapping, plus ApiKeyPoolService Add/Remove/Clear/GetPool and IsSupportedProvider against FakeSecurePreferences (dedup, remaining count, unknown provider); this is what keeps every LLM adapter alive when a key is revoked or rate-limited | — |
| TUT-T4 | 2026-10-14 | chore | open | NUnit tests for background queue state: BackgroundQueueState GetReadyItems/GetActiveItems ordering by Priority then CreatedAt, terminal vs in-progress vs rate-limited filters, PruneHistory keeping the newest N completed/permanently-failed items and dropping items older than maxAge; RateLimitState CanMakeApiCall before/after RetryAfter, RecordRateLimitError exponential backoff 60s doubling capped at 300s or explicit retry-after, RecordSuccess reset; plus BackgroundQueueService Enqueue/CancelTask/RetryTask/Pause/Resume state transitions with a fake IBackgroundTaskHandler; untested queue logic silently loses or stalls course builds | — |
| TUT-T5 | 2026-10-14 | chore | open | NUnit tests for learning-order graph algorithms: KnowledgeGraph GetFoundationalConcepts/GetLeafConcepts, GetPrerequisites/GetDependents and transitive GetAllPrerequisites/GetAllDependents, GetLearningPathTo/From, TopologicalSort (prereqs first, stable on ties), RecalculateDepths, RebuildAdjacencyLists, FindByTerm case-insensitivity, GetStats; ConceptCorrelationService.RemoveCycles breaking a 2-node and 3-node cycle while leaving acyclic edges; and CourseStructure navigation (GetLessonsInOrder, GetAllConceptIdsInOrder, GetNext/GetPreviousConceptId at boundaries, GetConceptPosition, FindLessonForConcept/FindTopicForConcept, FindLessonForSection, GetAllSectionsFlattened); existing model tests only cover property defaults | — |
| TUT-T6 | 2026-10-14 | chore | open | NUnit tests for ConceptMergeService duplicate detection and merging: FindDuplicates/FindDuplicatesInCollection/FindDuplicatesInMergedMap flagging exact normalized-title matches and SimHash-near pairs (hamming at most 15), NeedsUserConfirmation below 0.95 confidence, MergeExactDuplicates/AutoMergeExactDuplicatesInMergedMap/MergeInMergedMap choosing the canonical concept, rewiring relationships and complexity order to the survivor, dropping self-loops and duplicate edges, and AnalyzeSimilarityWithAIAsync parsing (valid, malformed, partial JSON) via FakeLlmService; wrong merges corrupt every downstream course | — |
| TUT-T7 | 2026-10-14 | chore | open | NUnit tests for ConceptCorrelationService relationship scoring: ComputePairScore weighting (semantic 0.50, LSH 0.25, lexical 0.15, co-occurrence 0.10) with hand-built ConceptNode embeddings/signatures, FindRelatedPairs threshold filtering and descending sort, BuildRelationshipsWithoutLlm producing no self-edges or duplicates, and InferPrerequisitesAsync batching of 20 plus prerequisite-response parsing (unknown ids, malformed JSON) via FakeLlmService; this decides the prerequisite edges learners follow | — |
| TUT-T8 | 2026-10-14 | chore | open | NUnit tests for concept map collections: ConceptMapCollection Add/Remove/ContainsConceptMap (no duplicate ids, false on missing), LoadedConceptMapCollection GetAllConcepts/GetAllRelations/GetAllComplexityOrder aggregation, GetConcept/GetConceptByTitle (case-insensitive, across maps), GetConceptsByComplexity ordering, GetForceLayoutData node/link counts excluding dangling links, and MergedCourseConceptMap.InitializeFromConceptMaps de-duplication across source maps; the course concept view is built on these | — |
| TUT-T9 | 2026-10-14 | chore | open | NUnit tests for search and similarity helpers: ConceptAutoCompleteService.GetSuggestions (under 3 chars or whitespace returns empty, exact match scores as ExactMatch, starts-with on title or alias beats contains fallback, shorter titles win ties, maxResults cap, case-insensitive trimming) and IsValidQuery, plus EmbeddingService.CosineSimilarity (identical=1, orthogonal=0, opposite=-1, zero vector and mismatched lengths handled); these drive concept search and every similarity threshold | — |
| TUT-T10 | 2026-10-14 | chore | open | NUnit tests for ConceptMapService and OrphanConceptLinkerService non-network logic: ConceptMapService.CalculateComplexityOrder (prereqs rank before dependents, unrelated concepts still included, cycles do not hang), CombineResources/PrepareResourceContent formatting and empty input; OrphanConceptLinkerService.ApplySuggestedLinks (skips unknown ids, self-links and existing edges) and AnalyzeOrphansAsync/LinkAllOrphansIterativelyAsync orphan detection and iteration stop with FakeLlmService; also ExtractConceptsAsync/BuildRelationshipsAsync parsing of malformed LLM JSON | — |
| TUT-T11 | 2026-10-14 | chore | open | NUnit tests for the remaining book parsers: PdfBookParser with a fake IPdfOcrService (text PDF extracts pages, text-light page under 50 chars triggers OCR only when IsAvailable, OCR warning counts, image-only PDF with OCR unavailable yields the install-Tesseract warning), DocxBookParser and EpubBookParser on small generated fixtures (title, chapter/paragraph order, empty document), and ExternalToolBookParser/MobiBookParser/LibreOfficeBookParser reporting a clear error when the external tool is missing and cleaning up temp files; only Txt/Html parsers and ParserRegistry are tested today | — |
| TUT-T12 | 2026-10-14 | chore | open | NUnit tests for the Tutor.Cli pure logic (test project needs a Tutor.Cli reference): Args.Parse positionals vs --key value vs bare --flag and Get lookup, QuizMode.Parse (null defaults to both, case/whitespace, unknown values) and ShouldBake, GutenbergFetcher.StripHeaderFooter on text with and without the START/END OF THE PROJECT GUTENBERG markers, CoursePackage.Open on a folder and a zip manifest plus ResolveItemPath rejecting paths that escape the package, and GutenbergCatalog lookups; none of the CLI is tested | — |
| TUT-T13 | 2026-10-14 | chore | open | NUnit tests for per-user persistence services: UserProgressService GetProgressAsync creating a fresh record, Save/Get round-trip, MarkConceptLearned/Visited, SetPosition, ClearProgressAsync and GetAllProgressAsync keeping users isolated by userId over FakeSecurePreferences; CertificateService IssueAsync (one certificate per user and course, re-issue behavior), GetForCourseAsync/GetAllAsync, and ComputeCourseContentHashAsync being stable for identical structures, changing when the structure changes, and empty when none exists; this is learner progress and credential data | — |
| TUT-T14 | 2026-10-14 | chore | open | NUnit tests for log storage: LogStore Add trimming to MaxEntries oldest-first, Clear, GetFiltered by severity set, GetAllDescending newest-first, LoadEntries respecting MaxEntries, and LogStorageService save/load round-trip, corrupt or missing log file tolerance and ClearPersistedLogs using a temp app-data path; only the MindAttic log bridge touches LogStore today | — |
| TUT-T15 | 2026-10-14 | chore | open | Tutor.Mcp logs only to stderr; bridge it to MindAttic.Log like Tutor.Blazor (MindAtticLogBridge) | — |
| TUT-T16 | 2026-10-15 | chore | closed | Remove duplicate command copies (/quickload, /quicksave) now centralized at the user level (~/.claude/commands) | M.FLOW-T14 |
| TUT-T17 | 2026-10-16 | chore | closed | install.ps1 adds this repo's 8 .prose/commands stubs for the shared global commands, so any LLM client finds them | mindattic-agent-standard/install.ps1 run 2026-10-16 |
| TUT-T18 | 2026-10-16 | chore | open | Bug hunt: run an exhaustive /bughunt over the whole repo and fix every real bug found (each fix under its own fix ticket), then hunt again; close this only when a full pass finds nothing | user request 2026-10-16: approved repo sweep |
| TUT-T19 | 2026-10-16 | chore | open | Run the full unit test suite and fix every failure (each under its own fix ticket) until it passes; put the passing run in --ref. No test project: close this wont-do saying so | user request 2026-10-16: approved repo sweep |
| TUT-T20 | 2026-10-16 | chore | open | Clean up code and docs: remove dead code, stale comments and outdated or duplicated docs, and simplify where behaviour doesn't change (/simplify); tests stay green | user request 2026-10-16: approved repo sweep |
| TUT-T21 | 2026-10-16 | docs | open | Bring README.md up to date with the code as it is now (what it is, setup, usage, commands, current state), following codex-standard/README-STYLE.md; README.htm regenerated if the repo has one | user request 2026-10-16: approved repo sweep |
| TUT-T22 | 2026-10-16 | docs | closed | Update USER_STORIES.md | — |
| TUT-T23 | 2026-10-19 | chore | wont-do | Convert tracked source files to LF line endings via .gitattributes, per HOUSE-LAW-13 (low priority) | — |
| TUT-T24 | 2026-10-20 | docs | open | Publish Tutor docs to GitHub Pages via the shared Toolkit DocFX pipeline: run adopt-docs-site.ps1 (README.md as home page, docs/*.md as articles, API reference only where the code builds in CI), enable Pages with build_type=workflow, set the repo homepage URL, retire tools/build-readme.ps1 and README.htm, confirm the docs Actions run is green and https://mindattic.github.io/Tutor/ returns 200. Depends on MAT-T8. | — |
| TUT-T25 | 2026-10-22 | docs | closed | Update USER_STORIES.md | — |
