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


## Epic I — Shared ingestion package
- **TUT-US-I1 ⬜** As another MindAttic app, I parse, chunk and embed documents with the same pipeline Tutor uses, from a NuGet package.

## Tickets
> Permanent log of every change and fix — this repo's ticket system. Rows are append-only: never
> delete or reword one; only move its Status (open → closed / wont-do) and fill in Ref. Status is
> a word, not a glyph, so tickets don't count toward the story index. Keep this section last.

| ID | Date | Type | Status | Summary | Ref |
|---|---|---|---|---|---|
| TUT-T1 | 2026-10-12 | docs | closed | Start the permanent ticket log in USER_STORIES.md | — |
| TUT-T2 | 2026-10-12 | chore | closed | /commit now logs every commit as a ticket in this table | `.claude/skills/commit/SKILL.md` |
| TUT-T3 | 2026-10-14 | chore | wont-do | NUnit tests for LLM key-pool failover: KeyPoolFailover.ExecuteAsync (empty pool throws InvalidOperationException, first success returns without trying later keys, 401/408/429/5xx/null-status HttpRequestException and TaskCanceledException fail over to the next key, 400-class and non-HTTP exceptions rethrow immediately, last key's exception rethrows, caller cancellation stops failover) and IsKeyLevelFailure status mapping, plus ApiKeyPoolService Add/Remove/Clear/GetPool and IsSupportedProvider against FakeSecurePreferences (dedup, remaining count, unknown provider); this is what keeps every LLM adapter alive when a key is revoked or rate-limited | portfolio reset: container; kept children re-parented under MS-11 features |
| TUT-T4 | 2026-10-14 | chore | wont-do | NUnit tests for background queue state: BackgroundQueueState GetReadyItems/GetActiveItems ordering by Priority then CreatedAt, terminal vs in-progress vs rate-limited filters, PruneHistory keeping the newest N completed/permanently-failed items and dropping items older than maxAge; RateLimitState CanMakeApiCall before/after RetryAfter, RecordRateLimitError exponential backoff 60s doubling capped at 300s or explicit retry-after, RecordSuccess reset; plus BackgroundQueueService Enqueue/CancelTask/RetryTask/Pause/Resume state transitions with a fake IBackgroundTaskHandler; untested queue logic silently loses or stalls course builds | portfolio reset: container; kept children re-parented under MS-11 features |
| TUT-T5 | 2026-10-14 | chore | wont-do | NUnit tests for learning-order graph algorithms: KnowledgeGraph GetFoundationalConcepts/GetLeafConcepts, GetPrerequisites/GetDependents and transitive GetAllPrerequisites/GetAllDependents, GetLearningPathTo/From, TopologicalSort (prereqs first, stable on ties), RecalculateDepths, RebuildAdjacencyLists, FindByTerm case-insensitivity, GetStats; ConceptCorrelationService.RemoveCycles breaking a 2-node and 3-node cycle while leaving acyclic edges; and CourseStructure navigation (GetLessonsInOrder, GetAllConceptIdsInOrder, GetNext/GetPreviousConceptId at boundaries, GetConceptPosition, FindLessonForConcept/FindTopicForConcept, FindLessonForSection, GetAllSectionsFlattened); existing model tests only cover property defaults | children done |
| TUT-T6 | 2026-10-14 | chore | wont-do | NUnit tests for ConceptMergeService duplicate detection and merging: FindDuplicates/FindDuplicatesInCollection/FindDuplicatesInMergedMap flagging exact normalized-title matches and SimHash-near pairs (hamming at most 15), NeedsUserConfirmation below 0.95 confidence, MergeExactDuplicates/AutoMergeExactDuplicatesInMergedMap/MergeInMergedMap choosing the canonical concept, rewiring relationships and complexity order to the survivor, dropping self-loops and duplicate edges, and AnalyzeSimilarityWithAIAsync parsing (valid, malformed, partial JSON) via FakeLlmService; wrong merges corrupt every downstream course | portfolio reset: audit-generated test nice-to-have |
| TUT-T7 | 2026-10-14 | chore | wont-do | NUnit tests for ConceptCorrelationService relationship scoring: ComputePairScore weighting (semantic 0.50, LSH 0.25, lexical 0.15, co-occurrence 0.10) with hand-built ConceptNode embeddings/signatures, FindRelatedPairs threshold filtering and descending sort, BuildRelationshipsWithoutLlm producing no self-edges or duplicates, and InferPrerequisitesAsync batching of 20 plus prerequisite-response parsing (unknown ids, malformed JSON) via FakeLlmService; this decides the prerequisite edges learners follow | portfolio reset: audit-generated test nice-to-have |
| TUT-T8 | 2026-10-14 | chore | wont-do | NUnit tests for concept map collections: ConceptMapCollection Add/Remove/ContainsConceptMap (no duplicate ids, false on missing), LoadedConceptMapCollection GetAllConcepts/GetAllRelations/GetAllComplexityOrder aggregation, GetConcept/GetConceptByTitle (case-insensitive, across maps), GetConceptsByComplexity ordering, GetForceLayoutData node/link counts excluding dangling links, and MergedCourseConceptMap.InitializeFromConceptMaps de-duplication across source maps; the course concept view is built on these | portfolio reset: audit-generated test nice-to-have |
| TUT-T9 | 2026-10-14 | chore | wont-do | NUnit tests for search and similarity helpers: ConceptAutoCompleteService.GetSuggestions (under 3 chars or whitespace returns empty, exact match scores as ExactMatch, starts-with on title or alias beats contains fallback, shorter titles win ties, maxResults cap, case-insensitive trimming) and IsValidQuery, plus EmbeddingService.CosineSimilarity (identical=1, orthogonal=0, opposite=-1, zero vector and mismatched lengths handled); these drive concept search and every similarity threshold | portfolio reset: audit-generated test nice-to-have |
| TUT-T10 | 2026-10-14 | chore | wont-do | NUnit tests for ConceptMapService and OrphanConceptLinkerService non-network logic: ConceptMapService.CalculateComplexityOrder (prereqs rank before dependents, unrelated concepts still included, cycles do not hang), CombineResources/PrepareResourceContent formatting and empty input; OrphanConceptLinkerService.ApplySuggestedLinks (skips unknown ids, self-links and existing edges) and AnalyzeOrphansAsync/LinkAllOrphansIterativelyAsync orphan detection and iteration stop with FakeLlmService; also ExtractConceptsAsync/BuildRelationshipsAsync parsing of malformed LLM JSON | portfolio reset: audit-generated test nice-to-have |
| TUT-T11 | 2026-10-14 | chore | wont-do | NUnit tests for the remaining book parsers: PdfBookParser with a fake IPdfOcrService (text PDF extracts pages, text-light page under 50 chars triggers OCR only when IsAvailable, OCR warning counts, image-only PDF with OCR unavailable yields the install-Tesseract warning), DocxBookParser and EpubBookParser on small generated fixtures (title, chapter/paragraph order, empty document), and ExternalToolBookParser/MobiBookParser/LibreOfficeBookParser reporting a clear error when the external tool is missing and cleaning up temp files; only Txt/Html parsers and ParserRegistry are tested today | children done |
| TUT-T12 | 2026-10-14 | chore | wont-do | NUnit tests for the Tutor.Cli pure logic (test project needs a Tutor.Cli reference): Args.Parse positionals vs --key value vs bare --flag and Get lookup, QuizMode.Parse (null defaults to both, case/whitespace, unknown values) and ShouldBake, GutenbergFetcher.StripHeaderFooter on text with and without the START/END OF THE PROJECT GUTENBERG markers, CoursePackage.Open on a folder and a zip manifest plus ResolveItemPath rejecting paths that escape the package, and GutenbergCatalog lookups; none of the CLI is tested | portfolio reset: container; kept children re-parented under MS-11 features |
| TUT-T13 | 2026-10-14 | chore | wont-do | NUnit tests for per-user persistence services: UserProgressService GetProgressAsync creating a fresh record, Save/Get round-trip, MarkConceptLearned/Visited, SetPosition, ClearProgressAsync and GetAllProgressAsync keeping users isolated by userId over FakeSecurePreferences; CertificateService IssueAsync (one certificate per user and course, re-issue behavior), GetForCourseAsync/GetAllAsync, and ComputeCourseContentHashAsync being stable for identical structures, changing when the structure changes, and empty when none exists; this is learner progress and credential data | portfolio reset: container; kept children re-parented under MS-11 features |
| TUT-T14 | 2026-10-14 | chore | wont-do | NUnit tests for log storage: LogStore Add trimming to MaxEntries oldest-first, Clear, GetFiltered by severity set, GetAllDescending newest-first, LoadEntries respecting MaxEntries, and LogStorageService save/load round-trip, corrupt or missing log file tolerance and ClearPersistedLogs using a temp app-data path; only the MindAttic log bridge touches LogStore today | portfolio reset: audit-generated test nice-to-have |
| TUT-T15 | 2026-10-14 | chore | open | Tutor.Mcp logs only to stderr; bridge it to MindAttic.Log like Tutor.Blazor (MindAtticLogBridge) | — |
| TUT-T16 | 2026-10-15 | chore | closed | Remove duplicate command copies (/quickload, /quicksave) now centralized at the user level (~/.claude/commands) | M.FLOW-T14 |
| TUT-T17 | 2026-10-16 | chore | closed | install.ps1 adds this repo's 8 .prose/commands stubs for the shared global commands, so any LLM client finds them | mindattic-agent-standard/install.ps1 run 2026-10-16 |
| TUT-T18 | 2026-10-16 | chore | closed | Bug hunt: run an exhaustive /bughunt over the whole repo and fix every real bug found (each fix under its own fix ticket), then hunt again; close this only when a full pass finds nothing | children done |
| TUT-T19 | 2026-10-16 | chore | wont-do | Run the full unit test suite and fix every failure (each under its own fix ticket) until it passes; put the passing run in --ref. No test project: close this wont-do saying so | portfolio reset: run-suite-until-green loop |
| TUT-T20 | 2026-10-16 | chore | wont-do | Clean up code and docs: remove dead code, stale comments and outdated or duplicated docs, and simplify where behaviour doesn't change (/simplify); tests stay green | portfolio reset: general cleanup sweep, outside the 1.0 milestone |
| TUT-T21 | 2026-10-16 | docs | open | Bring README.md up to date with the code as it is now (what it is, setup, usage, commands, current state), following codex-standard/README-STYLE.md; README.htm regenerated if the repo has one | user request 2026-10-16: approved repo sweep |
| TUT-T22 | 2026-10-16 | docs | closed | Update USER_STORIES.md | — |
| TUT-T23 | 2026-10-19 | chore | wont-do | Convert tracked source files to LF line endings via .gitattributes, per HOUSE-LAW-13 (low priority) | — |
| TUT-T24 | 2026-10-20 | docs | closed | Publish Tutor docs to GitHub Pages via the shared Agency DocFX pipeline: run adopt-docs-site.ps1 (README.md as home page, docs/*.md as articles, API reference only where the code builds in CI), enable Pages with build_type=workflow, set the repo homepage URL, retire tools/build-readme.ps1 and README.htm, confirm the docs Actions run is green and https://mindattic.github.io/Tutor/ returns 200. Depends on MAT-T8. | — |
| TUT-T25 | 2026-10-22 | docs | closed | Update USER_STORIES.md | — |
| TUT-T26 | 2026-10-24 | docs | closed | Update USER_STORIES.md | — |
| TUT-T27 | 2026-10-30 | feature | wont-do | Extract Tutor's settings into a Vault Dashboard settings-tab package | superseded by Epic E redesign: target MindAttic.Vault.Desktop, not Vault Dashboard |
| TUT-T28 | 2026-10-30 | chore | wont-do | Remove Tutor's own settings page and wire its cog to open Vault Dashboard instead | superseded by Epic E redesign: target MindAttic.Vault.Desktop, not Vault Dashboard |
| TUT-T29 | 2026-10-30 | feature | wont-do | Adapt Tutor's settings component to the Components settings-tab contract | withdrawn: plan rejected by owner 2026-10-31, see M.VALT-T26 |
| TUT-T30 | 2026-10-30 | chore | wont-do | Remove Tutor's standalone settings route, wire its cog to MindAttic.Vault.Desktop | withdrawn: plan rejected by owner 2026-10-31, see M.VALT-T26 |
| TUT-T31 | 2026-10-31 | feature | closed | LAMP: move Tutor's app-level settings and keys to MindAttic.Vault.Desktop (manifest, LoadOrSeed, shared named keys, cog opens Vault) | 194968f TutorSettingTests+TutorLampTests (81/81; full suite 536/536) |
| TUT-T32 | 2026-10-31 | fix | closed | Saved paragraph spacing is never applied on startup: MainLayout applies theme and border radius but not paragraph spacing, so the setting only took effect in the session it was changed | 95d7b8a |
| TUT-T33 | 2026-10-31 | fix | closed | Tutor.Blazor dev startup fails with PendingModelChangesWarning: MindAttic.Authentication's auth-v2 model (AuthUser DisplayName/NormalizedDisplayName + unique filtered index) has no Tutor migration | d52f321 Tutor.Blazor starts (migration applied, listening) |
| TUT-T34 | 2026-10-31 | fix | closed | TutorSettingsSource keeps builder.Configuration, which AddMindAtticVaultConfiguration later fills with Tutor's own local Vault file, so Current treats the local file as deployed config (no live re-read of Vault edits, Update ignored) (found by M.VALT-T73) | 3b7e9d6 TutorLampTests.Local_Vault_file_projected_into_configuration_later_stays_the_live_editable_source |
| TUT-T35 | 2026-10-31 | fix | closed | Tutor.Blazor adds AddMindAtticVaultConfiguration() after the environment variables, so on a machine with Tutor's Vault file an env var/App Service setting MindAttic__Settings__Tutor__* (and the auth options) is shadowed by the local file instead of overriding it (found by M.VALT-T73) | 1356409 TutorLampTests.Environment_variable_overrides_the_local_Vault_file_in_the_hosts_configuration |
| TUT-T36 | 2026-10-31 | docs | closed | Update USER_STORIES.md | — |
| TUT-T37 | 2026-11-02 | docs | closed | Adopt MindAttic.Agency docs-site pipeline, publish README to GitHub Pages (HOUSE-LAW-14) | — |
| TUT-T38 | 2026-11-02 | fix | closed | Bughunt sweep, two passes: find and fix real bugs across tracked source (user request 2026-11-02) | — |
| TUT-T39 | 2026-11-02 | fix | closed | Command pointer in .agent/commands said a literal $source instead of its legacy path (install.ps1 escaping bug fixed in MAT-T84); point it at the real source, or the global command when the project copy is gone | — |
| TUT-T40 | 2026-11-02 | fix | closed | Fix the open items the 2026-11-02 bughunt left for decision (unloading active course, CourseService/storage locking) | — |
| TUT-T41 | 2026-11-10 | chore | closed | Crash recovery: commit the command-catalog sync (.agent/commands/*.md) and regenerated docs/USER_STORIES.md left uncommitted by the session crash | — |
| TUT-T42 | 2026-11-10 | chore | closed | Crash recovery: commit the command-catalog sync (.agent/commands/*.md) and regenerated docs/USER_STORIES.md left uncommitted by the session crash | — |
| TUT-T43 | 2026-11-10 | chore | closed | Crash recovery / fleet sync: commit the regenerated docs/USER_STORIES.md left uncommitted after other tickets closed elsewhere | — |
| TUT-T44 | 2026-11-10 | chore | closed | Adopt the shared docs-site pipeline: turn on GitHub Pages | — |
| TUT-T45 | 2026-11-17 | docs | closed | Update USER_STORIES.md | — |
| TUT-T46 | 2026-11-17 | chore | closed | Azure SQL migration: connection resiliency and Entra auth so TutorAuth runs against the Azure SQL free-offer database | — |
| TUT-T47 | 2026-11-17 | chore | closed | Deploy Tutor.Blazor to Azure App Service through GitHub Actions (OIDC), on the Azure SQL TutorAuth database, restoring without C:\LocalNuGet | — |
| TUT-T48 | 2026-11-18 | chore | closed | Development defaults point at Azure SQL (sql-mindattic-f65744) instead of LocalDB, so no local SQL Server is needed to develop | — |
| TUT-T49 | 2026-11-18 | docs | closed | Update USER_STORIES.md | — |
| TUT-T50 | 2026-11-19 | feature | closed | Build courses from the top 10 Project Gutenberg books and record how long each stage takes per book | already done: 06efd6e tutor gutenberg-top |
| TUT-T51 | 2026-11-19 | docs | closed | Audit Tutor against current web-app standards and write the prioritized improvement plan (docs/plans) for making it an impressive hosted app on Azure | already done: 76e6abd docs/plans/tutor-improvement-plan.md |
| TUT-T52 | 2026-11-19 | feature | closed | Tutor requires sign-in everywhere it can spend real API keys: MindAttic.Authentication plus MindAttic.Vault for keys, verified on the hosted app, with the owner told which accounts exist and where every secret lives so they can be backed up | — |
| TUT-T53 | 2026-11-19 | fix | wont-do | Fails on main: Tutor.Tests.Lamp.TutorLampTests.Manifest_builds_with_every_field_labeled_grouped_and_helped — Assert.That(m.Fields.Select(f => f.Group).Distinct(), Is.EquivalentTo(new[] { "General", "Appearance", "Storage", "Learning", "Quiz", "Logging", "Sign-in security" })) Expected: equivalent to < "General", "Appearance", "Storage", "Learning", "Quiz", "Logging", "Sign-in security" > But was: < "Gen... (landed on main since the last green run: tickets TUT-T50 in 06efd6e tutor gutenberg-top: build a course from each of Project Gutenberg's current top books and report time per stage, LLM calls with estimated tokens and cost, and course size per book (TUT-T50)) | superseded by TUT-T56: bisected to 06efd6e |
| TUT-T54 | 2026-11-19 | fix | wont-do | Fails on main: Tutor.Tests.Lamp.TutorSettingTests.EverySettingHasItsOwnTest — theme, activeModel, enterToSend, userDisplayName, userAvatarIcon, userCustomAvatar, assistantDisplayName, assistantAvatarIcon, assistantCustomAvatar, borderRadius, paragraphSpacing, dateFormat, learningResourcesDirectory, saveFormattedAsMarkdown, preserveOriginalFiles, readingTimeSeconds, useCalc... (landed on main since the last green run: tickets TUT-T50 in 06efd6e tutor gutenberg-top: build a course from each of Project Gutenberg's current top books and report time per stage, LLM calls with estimated tokens and cost, and course size per book (TUT-T50)) | superseded by TUT-T57: bisected to 06efd6e |
| TUT-T55 | 2026-11-19 | fix | wont-do | Fails on main: Tutor.Tests.Lamp.TutorLampTests.Manifest_has_no_secret_fields_and_declares_every_provider_Tutor_calls — Assert.That(m.AllFields().Where(f => f.Kind is not SettingKind.Object && f.Kind is not SettingKind.Integer && f.Property != "currentPepperKeyId" // the pepper's id, not the pepper && secretish.Any(w => f.Property.Contains(w, StringComparison.OrdinalIgnoreCase))), Is.Empty) Expected: <empty> But w... (landed on main since the last green run: tickets TUT-T50 in 06efd6e tutor gutenberg-top: build a course from each of Project Gutenberg's current top books and report time per stage, LLM calls with estimated tokens and cost, and course size per book (TUT-T50)) | superseded by TUT-T58: bisected to 06efd6e |
| TUT-T56 | 2026-11-19 | fix | closed | Fails on main: Tutor.Tests.Lamp.TutorLampTests.Manifest_builds_with_every_field_labeled_grouped_and_helped — Assert.That(m.Fields.Select(f => f.Group).Distinct(), Is.EquivalentTo(new[] { "General", "Appearance", "Storage", "Learning", "Quiz", "Logging", "Sign-in security" })) Expected: equivalent to < "General", "Appearance", "Storage", "Learning", "Quiz", "Logging", "Sign-in security" > But was: < "Gen... (bisected to 06efd6e "tutor gutenberg-top: build a course from each of Project Gutenberg's current top books and report time per stage, LLM calls with estimated tokens and cost, a..." on main, reopening TUT-T50's work: fix what that commit changed so this passes again; bisected from the last green 37cddad in 0 steps) | 62f4446 |
| TUT-T57 | 2026-11-19 | fix | closed | Fails on main: Tutor.Tests.Lamp.TutorSettingTests.EverySettingHasItsOwnTest — theme, activeModel, enterToSend, userDisplayName, userAvatarIcon, userCustomAvatar, assistantDisplayName, assistantAvatarIcon, assistantCustomAvatar, borderRadius, paragraphSpacing, dateFormat, learningResourcesDirectory, saveFormattedAsMarkdown, preserveOriginalFiles, readingTimeSeconds, useCalc... (bisected to 06efd6e "tutor gutenberg-top: build a course from each of Project Gutenberg's current top books and report time per stage, LLM calls with estimated tokens and cost, a..." on main, reopening TUT-T50's work: fix what that commit changed so this passes again; bisected from the last green 37cddad in 0 steps) | 4598926 |
| TUT-T58 | 2026-11-19 | fix | closed | Fails on main: Tutor.Tests.Lamp.TutorLampTests.Manifest_has_no_secret_fields_and_declares_every_provider_Tutor_calls — Assert.That(m.AllFields().Where(f => f.Kind is not SettingKind.Object && f.Kind is not SettingKind.Integer && f.Property != "currentPepperKeyId" // the pepper's id, not the pepper && secretish.Any(w => f.Property.Contains(w, StringComparison.OrdinalIgnoreCase))), Is.Empty) Expected: <empty> But w... (bisected to 06efd6e "tutor gutenberg-top: build a course from each of Project Gutenberg's current top books and report time per stage, LLM calls with estimated tokens and cost, a..." on main, reopening TUT-T50's work: fix what that commit changed so this passes again; bisected from the last green 37cddad in 0 steps) | 18bae5180936d0ed79f60cfc314ebb3c0675f05d |
| TUT-T59 | 2026-11-19 | feature | closed | Tutor plan 0.2: Author policy on authoring pages and actions enforced server-side, per-user LLM rate and daily budget at the LLM choke point (Vault settings), chat no longer writes shared concept maps, rate limiter on /_ma-auth and bundle download, TOTP for Admin, break-glass doc | — |
| TUT-T60 | 2026-11-19 | feature | closed | Tutor plan 1.3+1.4: LLM keys on the host via Key Vault references after spending is gated, seed the ten bundled courses at startup; owner wants hosted Tutor verified working end to end online at least once | a55e5b2 2351425 |
| TUT-T61 | 2026-11-19 | fix | closed | EmbeddingService batches by an estimated 4 chars/token up to 250k tokens, so dense text (Chambers's Dictionary, PG 37683: 1,435 chunks in 3 batches) goes over OpenAI's 300,000-token per-request cap; the request 400s and the whole course ships without RAG vectors | — |
| TUT-T62 | 2026-11-19 | fix | closed | Quiz tab Start Quiz silently does nothing for installed bundles (collection id loaded as a ConceptMap); lesson chat shows placeholder suggestions and the raw spiral prompt. Found online in TUT-T60 | — |
| TUT-T63 | 2026-11-19 | fix | closed | tutor delete (CourseDeleteService) leaves the course's merged concept map ConceptMaps\collection_<courseId>.json and collections\collection_<courseId>.json.bak behind | — |
| TUT-T64 | 2026-11-20 | feature | closed | Extract Tutor's parsers, chunking, embedding and vector-store abstraction into a Tutor.Ingestion NuGet package (C:\LocalNuGet) that Tutor itself consumes | 815814f |
| TUT-T65 | 2026-11-22 | chore | open | NUnit tests for KeyPoolFailover.ExecuteAsync failover rules and IsKeyLevelFailure status mapping | — |
| TUT-T66 | 2026-11-22 | chore | open | NUnit tests for ApiKeyPoolService Add/Remove/Clear/GetPool and IsSupportedProvider against FakeSecurePreferences | — |
| TUT-T67 | 2026-11-22 | chore | open | NUnit tests for BackgroundQueueState ordering, filters and PruneHistory, and RateLimitState backoff/reset | — |
| TUT-T68 | 2026-11-22 | chore | wont-do | NUnit tests for KnowledgeGraph queries, TopologicalSort, depths, adjacency, FindByTerm and GetStats | portfolio reset: test back-fill outside the 1.0 test areas |
| TUT-T69 | 2026-11-22 | chore | wont-do | NUnit tests for PdfBookParser with a fake IPdfOcrService covering OCR triggers and warnings | portfolio reset: test back-fill outside the 1.0 test areas |
| TUT-T70 | 2026-11-22 | chore | wont-do | NUnit tests for DocxBookParser and EpubBookParser on small generated fixtures | portfolio reset: test back-fill outside the 1.0 test areas |
| TUT-T71 | 2026-11-22 | chore | wont-do | NUnit tests for ExternalToolBookParser/MobiBookParser/LibreOfficeBookParser missing-tool errors and temp cleanup | portfolio reset: test back-fill outside the 1.0 test areas |
| TUT-T72 | 2026-11-22 | chore | open | NUnit tests for UserProgressService over FakeSecurePreferences: fresh record, Save/Get round-trip, MarkConceptLearned/Visited, SetPosition, Clear, GetAll with per-user isolation | — |
| TUT-T73 | 2026-11-22 | chore | open | NUnit tests for CertificateService IssueAsync (one per user and course, re-issue), GetForCourseAsync/GetAllAsync, ComputeCourseContentHashAsync stability/change/empty | — |
| TUT-T74 | 2026-11-22 | chore | open | NUnit tests for BackgroundQueueService Enqueue/CancelTask/RetryTask/Pause/Resume with a fake IBackgroundTaskHandler | — |
| TUT-T75 | 2026-11-22 | chore | wont-do | NUnit tests for ConceptCorrelationService.RemoveCycles and CourseStructure navigation methods | portfolio reset: test back-fill outside the 1.0 test areas |
| TUT-T76 | 2026-11-22 | chore | wont-do | NUnit tests for Tutor.Cli Args.Parse/Get and QuizMode.Parse/ShouldBake; add the Tutor.Cli reference to the test project | portfolio reset: test back-fill outside the 1.0 test areas |
| TUT-T77 | 2026-11-22 | chore | wont-do | NUnit tests for GutenbergFetcher.StripHeaderFooter with and without START/END markers, and GutenbergCatalog lookups | portfolio reset: test back-fill outside the 1.0 test areas |
| TUT-T78 | 2026-11-22 | chore | open | NUnit tests for CoursePackage.Open on a folder and a zip manifest, and ResolveItemPath rejecting paths that escape the package | — |
| TUT-T79 | 2026-11-22 | feature | closed | Add a way to list/fetch the current top 10 Project Gutenberg books in Tutor.Cli | already done: 06efd6e tutor gutenberg-top |
| TUT-T80 | 2026-11-22 | feature | closed | Build a course from each of the top 10 books through the existing build pipeline | already done: 06efd6e tutor gutenberg-top |
| TUT-T81 | 2026-11-22 | feature | closed | Record and report how long each build stage takes per book | already done: 06efd6e tutor gutenberg-top |
| TUT-T82 | 2026-11-22 | docs | closed | 2026-11-22 morning | — |
| TUT-T83 | 2026-11-24 | fix | wont-do | Hunt the whole repo for real bugs with no time limit and fix every confirmed one | portfolio reset: open-ended bughunt loop; bugs are filed as concrete fix tasks |
| TUT-T84 | 2026-11-25 | feature | wont-do | Move Tutor off Azure to the Hetzner server | portfolio reset: Tutor runs locally (Azure decommissioned); hosting is out of scope |
| TUT-T85 | 2026-11-25 | chore | wont-do | Move TutorAuth to Hetzner SQL with the DB move script, verify, and repoint Tutor's connection strings (Vault + server env file) | portfolio reset: Tutor runs locally (Azure decommissioned); hosting is out of scope |
| TUT-T86 | 2026-11-25 | feature | wont-do | Add Tutor to the reusable deploy workflow and a systemd/Caddy entry, deploy on push to main, verify /health and the main pages; if Tutor only runs locally, record that and close | portfolio reset: Tutor runs locally (Azure decommissioned); hosting is out of scope |
| TUT-T87 | 2026-11-26 | fix | closed | ReadAllEntriesAsync buffers every entry with no cap, so a hostile .tutor (zip bomb) exhausts memory before validation; resources/conceptMaps ToDictionary throws an unfriendly ArgumentException on duplicate ids. | 1f4464c |
| TUT-T88 | 2026-11-26 | chore | closed | Local development reads the local SQL copy from its own Development config (owner 2026-11-25: all local development runs off local SQL Server, MAT-T255); the User-level override and deployed settings still win. | — |
| TUT-T89 | 2026-11-26 | fix | closed | CourseService caches an empty list when loading resources/courses fails, so the next save overwrites stored data | 9ddd411 |
| TUT-T90 | 2026-11-26 | fix | closed | ConceptMapService staggers chunk calls by index*200ms while holding a throttle slot, so large resources wait minutes for nothing | bf8ed84 |
| TUT-T91 | 2026-11-26 | fix | closed | BackgroundQueueService: a task cancelled while in progress is overwritten back to Completed/Failed and can be retried | 12117a0 |
| TUT-T92 | 2026-11-26 | fix | closed | BackgroundQueueService: negative rate-limit delay throws ArgumentOutOfRangeException and kills the processing loop | bdab2a6 |
| TUT-T93 | 2026-11-26 | fix | closed | ConceptMergeService: applying an approved CanonicalTitle overwrote the canonical's old title, losing it instead of keeping it as an alias | 157c380 |
| TUT-T94 | 2026-11-26 | fix | closed | OrphanConceptLinkerService: empty AI term partial-matched the first concept, and a link could resolve to a self-relationship | f4fd73c |
| TUT-T95 | 2026-11-26 | fix | closed | DynamicConceptExpansionService: a blank AI term or repeated term created empty/duplicate concepts or threw, and self-relationships were saved | a014205 |
| TUT-T96 | 2026-11-26 | fix | closed | ConceptMapStorageService: concurrent saves of one map shared the same .tmp file and could fail or corrupt the write | cb82baa |
| TUT-T97 | 2026-11-26 | fix | closed | KnowledgeGraphBuildService: failed and cancelled builds had no CompletedAt so ClearCompletedTasks never removed them | 5c1e389 |
| TUT-T98 | 2026-11-26 | fix | closed | KnowledgeGraphService: loadedGraphs was a plain Dictionary shared across threads and graph files were written in place | 53e1baf |
| TUT-T99 | 2026-11-26 | fix | closed | SideNavService: Learn nav loaded the course's collection id as a single ConceptMap so it always showed 'Concept maps not ready' | 2d6b6eb |
| TUT-T100 | 2026-11-26 | fix | closed | CourseStructureService: a concept the AI listed in two topics was added twice, and a null term threw | 8592b25 |
| TUT-T101 | 2026-11-26 | fix | closed | ResourceProcessingService: failed or cancelled tasks never got CompletedAt so their elapsed time kept growing | 05a24a6 |
| TUT-T102 | 2026-11-26 | fix | closed | LocalQuizController.PickPreGeneratedForSections takes the first N baked questions in section order, so the final exam (all sections, 20 questions) only covers the earliest lesson(s); interleave across sections | a3cc5a7 |
| TUT-T103 | 2026-11-26 | fix | closed | CoursePackage.Open leaks its temp extraction dir when the zip extract or the manifest parse throws (malformed manifest.json, null Items) | d9f2dbd |
| TUT-T104 | 2026-11-26 | fix | closed | CoursePackage.ResolveItemPath accepts manifest File paths that escape the package dir (../ or rooted), reading arbitrary files into a course | d9f2dbd |
| TUT-T105 | 2026-11-26 | fix | closed | VectorStoreService treats an unreadable chunks file as empty and overwrites it on the next save; keep a corrupt copy aside | 8b1fe2b |
| TUT-T106 | 2026-11-26 | fix | closed | LocalQuizController treats an unreadable quiz-results file as empty and overwrites it on the next save; keep a corrupt copy aside | b883890 |
| TUT-T107 | 2026-11-26 | fix | closed | UserStorageService treats an unreadable user-data file as missing and the next save overwrites it; keep a corrupt copy aside | e8c084252b49a68a5c852d9b21478e22f77087c0 |
| TUT-T108 | 2026-11-26 | fix | closed | gutenberg-top report shows run and stage times wrapped at 24 hours (h:mm:ss drops days) | ae2e09b635c85013684225119c6bca14d96824de |
| TUT-T109 | 2026-11-26 | fix | closed | GutenbergFetcher falls back to the mirror when the first address times out | 50eb1dc |
| TUT-T110 | 2026-11-26 | fix | closed | ExternalToolBookParser kills a --version probe that outlives its 5s wait | a05cce7 |
| TUT-T111 | 2026-11-26 | fix | closed | Chat reading timer double-counts seconds and can mark sections read from a stale start time | f855738 |
| TUT-T112 | 2026-11-26 | fix | closed | Chat: clicking a term link or Enter-send while a reply is in flight starts a concurrent send that clobbers cts/isSending | a5727ca |
| TUT-T113 | 2026-11-26 | fix | closed | CourseManagement: cached resource KB details never refresh, so a resource expanded before its concept map finished shows 0 concepts forever | bf74af4 |
| TUT-T114 | 2026-11-26 | fix | closed | LearnTabs: finishing or leaving a section quiz cleared the Section's own ConceptIds list (aliased), wiping its concepts | c344832 |
| TUT-T115 | 2026-11-26 | fix | closed | ConceptMapCollectionGraph: graph initialised twice (render re-entry during the 50ms delay, and after Auto-Link Orphans), leaking DotNetObjectReferences and re-registering the drag handler | fc69672 |
| TUT-T116 | 2026-11-26 | fix | closed | Course key regex accepts a trailing newline ($ matches before final \n), so a bundle key like 'abc\n' passes validation and then fails in CourseBlobStore after the course is already imported | dc8b3cd |
| TUT-T117 | 2026-11-26 | fix | closed | VectorStoreService writes content_chunks.json in place, so a crash mid-write corrupts every course's chunks; write to a temp file and move it over | f181ac2 |
| TUT-T118 | 2026-11-26 | chore | closed | Bug hunt pass 2: Tutor.Shared components (Chat.razor, CourseManagement.razor, ConceptMapCollectionGraph.razor) and tutor-interop.js; fix each real bug under its own fix ticket | children done |
| TUT-T119 | 2026-11-26 | chore | closed | Bug hunt pass 3: Tutor.Core services (ConceptMapService, ConceptMergeService, CourseService, BackgroundQueueService, DynamicConceptExpansionService, OrphanConceptLinkerService) plus Tutor.Cli and Tutor.Ingestion parsers; fix each real bug under its own fix ticket | ae5f55f32b9c4ce4ebd5a295430418fe64f73bed |
| TUT-T120 | 2026-11-26 | chore | wont-do | Final bug hunt: full pass over the whole repo; close only when it finds nothing | portfolio reset: container; kept children re-parented under MS-11 features |
| TUT-T121 | 2026-11-26 | fix | closed | BackgroundQueueService.ProcessTaskAsync sleeps out NextRetryAt while holding a taskThrottle slot, so backed-off retries starve other tasks | ae5f55f32b9c4ce4ebd5a295430418fe64f73bed |
| TUT-T122 | 2026-11-26 | chore | wont-do | Bug hunt pass 3b: Tutor.Core services TUT-T119 left unread (ConceptMapService, ConceptMergeService, CourseService, DynamicConceptExpansionService, OrphanConceptLinkerService), Tutor.Cli and the Tutor.Ingestion parsers | portfolio reset: open-ended bughunt loop; bugs are filed as concrete fix tasks |
| TUT-T123 | 2026-11-26 | fix | closed | tutor-interop.js: context menu Copy throws TypeError because contextMenuNode is nulled before clipboard promise resolves | 72d503c |
| TUT-T124 | 2026-11-26 | fix | closed | tutor-interop.js: resetGraphFilter and filterGraphByNodes release pinned nodes' fixed positions while they stay marked pinned | 3a9efed |
| TUT-T125 | 2026-11-26 | fix | closed | tutor-interop.js: pressing P while typing in an input toggles the selected graph node's pin | a4dcfc1 |
| TUT-T126 | 2026-11-26 | chore | closed | Bug hunt pass 2: read Tutor.Shared/Components/Chat.razor end to end and fix each real bug under its own fix ticket | c05fd86 |
| TUT-T127 | 2026-11-26 | chore | closed | Bug hunt pass 2: read Tutor.Shared/Components/CourseManagement.razor end to end and fix each real bug under its own fix ticket | bf55714 |
| TUT-T128 | 2026-11-26 | chore | closed | Bug hunt pass 2: read Tutor.Shared/Components/ConceptMapCollectionGraph.razor end to end and fix each real bug under its own fix ticket | bcd2749 |
| TUT-T129 | 2026-11-26 | fix | closed | ConceptMapCollectionGraph: a failed initForceGraph is marked initialized anyway, so the retry never happens | b42847d |
| TUT-T130 | 2026-11-26 | fix | closed | ConceptMapCollectionGraph: Auto-Link Orphans result message is hidden unless the details panel is open or any orphans remain | 2df1ff1 |
| TUT-T131 | 2026-11-26 | fix | closed | ConceptMapCollectionGraph: collection mode never computes connectivity, so no orphan banner or Auto-Link | bcd2749 |
| TUT-T132 | 2026-11-26 | fix | closed | CourseManagement: status and auth-denied messages only render on the Add tab, so Library-tab actions fail silently | 1ba39e1 |
| TUT-T133 | 2026-11-26 | fix | closed | CourseManagement: per-resource min confidence parsed and shown with current culture, breaks on comma-decimal locales | bf55714 |
| TUT-T134 | 2026-11-26 | fix | closed | Chat.razor: Begin and Next record the spiral teaching twice, so a first lesson teaches at depth 2 | 89d078f |
| TUT-T135 | 2026-11-26 | fix | closed | Chat.razor: Restart and course change leave the reading timer running, so Begin books the old elapsed time on section 1 | 071faff |
| TUT-T136 | 2026-11-26 | fix | closed | Chat.razor: lesson-complete message and reading timer show a literal ?? and ? instead of text | c05fd86 |
| TUT-T137 | 2026-11-26 | chore | open | Remove Azure leftovers after the 2026-11-26 decommission (MAT-T297): appsettings.json and the TutorAuthDbContext fallback still name the deleted Azure SQL server, and .github/workflows/azure-deploy.yml and kv-tutor references remain; make every environment use local SQL Server and delete the Azure-only config and workflow | — |
| TUT-T138 | 2026-11-26 | feature | open | Tutor: adopt (or confirm) the shared run menu over deploy-lib.ps1, run Redeploy, and confirm its exe lands and starts at R:\MindAttic\Apps\Tutor\Debug and \Release; repoint anything in the repo still naming an old deploy path. Outcome: both builds exist under R: and launch | — |
| TUT-T139 | 2026-11-27 | chore | open | LlmServiceRouter.GetCurrentService (Tutor.Core/Services/LlmServiceRouter.cs:59-69) bare catch swallows real DI/config resolution failures and silently reroutes the user to OpenAIService with no log entry, hiding a real misconfiguration; only catch the expected resolution failure and log before falling back. | — |
| TUT-T140 | 2026-11-27 | chore | open | LogStorageService's save-every-10-entries debounce (Tutor.Core/Services/Logging/LogStorageService.cs:41-55) is checked against LogStore.Count which plateaus at MaxEntries=5000 (divisible by 10) once the ring buffer fills, so every single log call after that persists the full 5000-entry store; debounce on an entries-since-last-save counter instead. | — |
| TUT-T141 | 2026-11-27 | chore | open | UserProgressService (Tutor.Core/Services/UserProgressService.cs:17) is registered AddSingleton in Tutor.Blazor/Program.cs but caches progress in a plain Dictionary, not thread-safe across concurrent circuits; change cachedProgress to ConcurrentDictionary like its siblings. | — |
| TUT-T142 | 2026-11-27 | chore | open | ConceptMapCollectionService (Tutor.Core/Services/ConceptMapCollectionService.cs:18) is registered AddSingleton but its cache field is a plain Dictionary, unlike sibling ConceptMapStorageService which uses ConcurrentDictionary for the exact same documented hazard; change cache to ConcurrentDictionary. | — |
| TUT-T143 | 2026-11-27 | chore | open | CourseConceptMapService.MergedMapsDirectory (Tutor.Core/Services/CourseConceptMapService.cs:24-25) is a static readonly field evaluated once from DataStorageSettings.GetKnowledgeBasesDirectory(), so a later change to the mutable KnowledgeBasesPath setting (e.g. between tests, or runtime reconfig) is silently ignored; make it a computed property like sibling storage services. | — |
| TUT-T144 | 2026-11-27 | chore | open | AppUiState (Tutor.Core/Services/AppUiState.cs) is registered AddSingleton in Tutor.Blazor/Program.cs:139 but holds per-user activeTab/sideNavNodes/selectedNodeId as plain instance fields, so every connected user shares and overwrites the same navigation state; scope it per-circuit (AddScoped) or key state by user/session id. | — |
| TUT-T145 | 2026-11-27 | chore | open | FinalExamTab.StartExam (Tutor.Shared/Components/FinalExamTab.razor:223) discards the Task<QuizSession?> result of FinalExamService.StartAsync and only catches LlmLimitReachedException, so a null session or any other exception leaves the UI silently stuck on the Begin button with no error shown; port the null-check + general catch + error message pattern already used in QuizTab.StartQuiz (TUT-T62). | — |
| TUT-T146 | 2026-11-27 | chore | open | Users.razor's mutating handlers (SaveUser, DeactivateUser, etc.) call IUserAdminService directly with no server-side re-check of MaPolicies.Admin, unlike the CourseAuthoringActions/LibraryAdminActions pattern this codebase otherwise follows for Blazor Server circuits; add the same policy re-check before each UserAdmin.* call on this, the most sensitive admin page. | — |
| TUT-T147 | 2026-11-27 | chore | open | Library.razor's ToggleAsync/RemoveAsync (lines 228-242) discard the bool/CourseDeletePlan? result of LibraryAdminActions.LoadAsync/UnloadAsync/RemoveAsync and always report success; inspect the returned value and surface a Fail(...) message when it indicates a no-op (e.g. a concurrent admin already acted on the same row). | — |
| TUT-T148 | 2026-11-27 | chore | open | Library.razor's AsAdminAsync (lines 146-158) only catches UnauthorizedAccessException; any other exception from AdminActions.LoadAsync/UnloadAsync/RemoveAsync propagates out of the Blazor Server event handler and tears down the user's circuit; broaden to a general catch with a friendly error, matching OnBundlePicked/ConfirmInstallAsync in the same file. | — |
| TUT-T149 | 2026-11-27 | chore | open | VectorStoreService.SaveChunksAsync (Tutor.Ingestion/Storage/VectorStoreService.cs:334-349) sets cachedChunks = chunks before the file write, then catches and only logs any write failure (full disk, locked file, AV); callers like StoreChunksAsync/RemoveChunksForResourceAsync/RemoveChunksForCurriculumAsync believe the operation succeeded while content_chunks.json silently keeps the stale on-disk state. Propagate the failure instead of catch-and-log-only. | — |
| TUT-T150 | 2026-11-27 | chore | open | BlazorSecurePreferences.Remove and CliSecurePreferences.Remove use a blocking @lock.Wait() while their own SetAsync uses await @lock.WaitAsync() on the same SemaphoreSlim; in Blazor Server's SynchronizationContext this is a dormant deadlock risk if Remove is ever called synchronously while a SetAsync is outstanding on the same circuit. Make Remove use WaitAsync like SetAsync. | — |
| TUT-T151 | 2026-11-27 | chore | open | CourseBuildPipeline (Tutor.Cli/Pipeline/CourseBuildPipeline.cs:149-162) catches OperationCanceledException inside its generic 'catch (Exception ex)' around AddResourceToCourseAsync and only logs a warning, defeating cooperative cancellation for that stage once a CancellationTokenSource is ever wired in; add catch (Exception ex) when (ex is not OperationCanceledException) or rethrow cancellation explicitly. | — |
| TUT-T152 | 2026-11-27 | chore | closed | QuizService (Tutor.Core/Services/Quiz/QuizService.cs) is registered AddSingleton in Tutor.Blazor/Program.cs:273 but keeps the in-progress quiz as mutable instance fields (currentSession/currentResult); two users quizzing concurrently stomp each other's session, so one user's answers can grade against another user's questions. Register QuizService as AddScoped or move the session into circuit-scoped state. | ccde0ba |
| TUT-T153 | 2026-11-27 | chore | closed | BundleImporter (Tutor.Core/Services/Packaging/BundleImporter.cs:155-229) builds resourceIdMap/conceptMapIdMap via GroupBy(r => r.Id).ToDictionary, collapsing any resources/concept maps in a .tutor bundle that share a duplicate internal Id into one remapped entry; since CourseService.SaveResourceAsync upserts by Id, the second save silently destroys the first with no error. A tampered/corrupt bundle passes SHA-256/path/format validation but still loses data on import. Have CourseManifestValidator (or the dictionary-build step) reject bundles with duplicate internal Ids. | c211b25 |
| TUT-T154 | 2026-11-27 | chore | closed | LocalQuizController.SaveQuizResultAsync writes under fileLock but GetQuizResultsAsync/LoadQuizResultsAsync (Tutor.Core/Services/Quiz/LocalQuizController.cs:241-291) read the same file WITHOUT taking fileLock, and the write is non-atomic (no temp-file+rename); a concurrent read can see a half-written file, get treated as 'corrupted', get backed up as .corrupt-<timestamp>, and silently return an empty quiz history. Take fileLock on read too, or make saves atomic like InstalledCourseRegistry.SaveAsync already does. | 77f35a5 |
| TUT-T155 | 2026-11-27 | chore | closed | Tutor.Core/Services/Queue/Tasks/KnowledgeBaseBuildTaskHandler.cs is a completely empty (0-byte) file; no code currently references the type and nothing enqueues the task types it would presumably handle, so it has no observable effect today, but needs a human sanity check for whether this is lost source rather than an intentional stub. | c42f7a1 |
| TUT-T156 | 2026-11-27 | chore | open | Portfolio reset (MAT-T324): release charter (docs/RELEASE.md, one milestone), bible, and triage of every open ticket into epic>feature>task for: Tutor | — |
| TUT-T157 | 2026-11-27 | feature | open | Tutor runs from R:\MindAttic\Apps\Tutor against local SQL Server with no Azure configuration, and its README says how | — |
| TUT-T158 | 2026-11-27 | feature | open | Fix the concrete bugs the last bug hunts found: admin pages, learner UI errors, storage and concurrency, service robustness | — |
| TUT-T159 | 2026-11-27 | feature | open | Several signed-in users on one instance each keep their own progress, quiz, lesson state, active course and certificate | — |
| TUT-T160 | 2026-11-27 | feature | open | An author uploads a document in the web app and builds and monitors a course from it without the CLI | — |
| TUT-T161 | 2026-11-27 | feature | open | NUnit coverage for key-pool failover, the background queue, learner data and bundle path safety | — |
| TUT-T162 | 2026-11-27 | feature | open | Production and Development both use local SQL Server; no Azure server, Key Vault or deploy workflow left in config or code | — |
| TUT-T163 | 2026-11-27 | feature | open | run.ps1 Redeploy puts Debug and Release builds under R:\MindAttic\Apps\Tutor and both start | — |
| TUT-T164 | 2026-11-27 | feature | open | README and BIBLE describe the local-only setup as it is | — |
| TUT-T165 | 2026-11-27 | feature | open | Users and Library admin actions re-check the Admin policy server-side, report real results and never kill the circuit | — |
| TUT-T166 | 2026-11-27 | feature | open | Learner tabs show a clear error instead of hanging when a start fails | — |
| TUT-T167 | 2026-11-27 | feature | open | Singleton caches are thread-safe, storage paths follow settings, failed writes surface and no sync-over-async lock waits | — |
| TUT-T168 | 2026-11-27 | feature | open | LLM routing, logging and the CLI build pipeline log real failures, debounce saves and honour cancellation | — |
| TUT-T169 | 2026-11-27 | feature | open | Progress, quiz results, the final exam and certificates use the signed-in user's id from AuthenticationState, never a shared default | — |
| TUT-T170 | 2026-11-27 | feature | open | Navigation state, lesson state (chat transcript) and the active course are per user or per circuit, not process-wide | — |
| TUT-T171 | 2026-11-27 | feature | open | The Blazor host parses uploaded PDF, EPUB, DOCX, HTML and TXT files through the same ParserRegistry the CLI uses | — |
| TUT-T172 | 2026-11-27 | feature | open | The courses page shows a course build's stage progress and errors from the background queue | — |
| TUT-T173 | 2026-11-27 | feature | open | NUnit tests for KeyPoolFailover and ApiKeyPoolService | — |
| TUT-T174 | 2026-11-27 | feature | open | NUnit tests for BackgroundQueueState, RateLimitState and BackgroundQueueService transitions | — |
| TUT-T175 | 2026-11-27 | feature | open | NUnit tests for UserProgressService and CertificateService | — |
| TUT-T176 | 2026-11-27 | feature | open | NUnit tests for CoursePackage.Open and ResolveItemPath path-escape rejection | — |
| TUT-T177 | 2026-11-27 | fix | open | UserProgressService: remove the shared DefaultUserId 'default' fallback and make userId required; Chat.razor, LearnTabs.razor, QuizTab and FinalExamTab pass MaClaims.UserId from AuthenticationState on every progress call (improvement plan S1) | — |
| TUT-T178 | 2026-11-27 | fix | open | QuizService and FinalExamService take the user id from the calling component's AuthenticationState instead of IHttpContextAccessor, so mid-circuit quizzes are no longer attributed to 'anonymous'; remove the TODO (story B5, plan S2) | — |
| TUT-T179 | 2026-11-27 | fix | open | FinalExamTab issues the certificate from the signed-in user's own progress and exam result, never shared progress (plan S6) | — |
| TUT-T180 | 2026-11-27 | fix | open | SettingsService lesson state (chat transcript) and CourseService ACTIVE_COURSE are keyed by the signed-in user id instead of one global key (plan S3, S4) | — |
| TUT-T181 | 2026-11-27 | feature | open | Tutor.Blazor/Program.cs registers the same IBookParser set and ParserRegistry as Tutor.Cli/Program.cs (HOUSE-LAW-6), so the web app can parse documents | — |
| TUT-T182 | 2026-11-27 | feature | open | CourseManagement adds a resource from an InputFile upload (PDF, EPUB, DOCX, HTML, TXT) parsed through ParserRegistry; remove the PDF placeholder sentence and the always-null BlazorFilePickerService path | — |
| TUT-T183 | 2026-11-27 | feature | open | The courses page shows the running build's current stage, percent and any failure message from BackgroundQueueService for that course, refreshing while it runs (story A6) | — |
| TUT-T184 | 2026-11-27 | feature | open | The courses page can cancel a running course build and retry a failed one through BackgroundQueueService CancelTask/RetryTask, with the result shown | — |
| TUT-T185 | 2026-11-27 | chore | open | docs/BIBLE.md: replace Azure SQL references in 6.1 and the active frontier with the local-SQL setup and the MS-11 epics; regenerate BIBLE.digest.md | — |
