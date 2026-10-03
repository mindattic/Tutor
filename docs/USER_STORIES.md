---
codex: 1
project: Tutor
code: TUT
layer: stories
status: living
updated: 2026-10-03
---

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

## Epic D — Course Packaging {#TUT-EPIC-D}

> Built in `Tutor.Core/Services/Packaging` ([BIBLE §4.4](BIBLE.md#TUT-§4)). Each
> story names its covering test; they stay 🟡 until the suite compiles and a
> green run is recorded ([BIBLE §6](BIBLE.md#TUT-§6)).

- **TUT-US-D1 🟡** As an Author, I can export a course to a self‑contained
  `.tutor` bundle that includes pre‑computed embeddings, so re‑import skips the
  LLM pipeline. *(covered by
  `BundleRoundTripTests.Import_RoundTrips_ContentStructureAndEmbeddings`.)*
- **TUT-US-D2 🟡** As a Recipient, I can install a bundle with `tutor install
  <file>`; installing an explicit duplicate yields two independent courses (all
  IDs remapped). *(covered by
  `BundleRoundTripTests.ImportingTwice_YieldsTwoIndependentCourses` and
  `CourseLifecycleRegistryTests.ExplicitDuplicate_InstallsSideBySide_AsIndependentCourse`.)*
- **TUT-US-D3 🟡** As an Author, I can remove a course and all its derivatives
  with `tutor delete <id>` (cascades resources, structure, concept maps,
  embeddings, registry row and retained blob). *(covered by
  `CourseLifecycleRegistryTests.Remove_HardCascades_DataRegistryRowAndBlob` and
  `DeleteService_DryRunPlan_DoesNotDeleteAnything`.)*
- **TUT-US-D4 🟡** As a Recipient, a bundle carries a **stable course key +
  whole‑number version**, so the system recognizes "you already have *Dracula*":
  same version is a no‑op, a newer one upgrades, an older one is refused unless I
  ask for a duplicate. *(covered by `CourseInstallResolverTests` and
  `CourseLifecycleRegistryTests.ReinstallingSameVersion_NoOps_WithoutDuplicating`,
  `NewerVersion_InstallsAsUpgrade`, `Downgrade_IsRefused`.)*
- **TUT-US-D5 🟡** As a Recipient, a corrupted or tampered `.tutor` is rejected up
  front with a clear reason, via a **SHA‑256 integrity check** and a manifest‑first,
  IO‑free **validation pass** with explicit error codes. *(covered by
  `CourseManifestValidatorTests`,
  `BundleRoundTripTests.TamperedBundle_IsRejected_WithShaMismatch` and
  `CourseLifecycleRegistryTests.InvalidBundle_NeverTouchesRegistryOrStore`.)*
- **TUT-US-D6 🟡** As an Author on a newer build, I can still read **older** bundle
  formats; only formats *newer than my build* are refused, and unknown manifest
  fields round‑trip. *(covered by `BundleRoundTripTests.FormatNewerThanBuild_IsRefused`,
  `LegacyBundle_WithoutKeyShaOrNewFields_StillImports`,
  `UnknownManifestFields_RoundTripThroughExtra`.)*

## Epic E — Load / Unload & Sharing {#TUT-EPIC-E}

- **TUT-US-E1 🟡** As an Admin, I can **load** a course from a `.tutor` file **in
  the Blazor app** at `/library` (upload → validate → plan → confirm → install).
  *Gap:* no automated UI test; the shared install path is covered by
  `CourseLifecycleRegistryTests`.
- **TUT-US-E2 🟡** As an Admin, I can **unload** a course so it disappears from the
  learning view **without destroying** its data or progress (soft‑disable,
  `InstalledCourse.Enabled = false`), per
  [HOUSE-LAW-2](../../MindAttic.HouseRules.md#HOUSE-LAW-2); a separate explicit
  "Remove" performs the hard cascade. *(covered by
  `CourseLifecycleRegistryTests.Unload_SoftDisables_WithoutDestroyingDataOrProgress`.)*
- **TUT-US-E3 🟡** As a signed‑in user, I can see **what is installed** — name, key,
  version, install date, integrity hash, enabled state — in one list on `/library`.
  *(registry covered by
  `CourseLifecycleRegistryTests.Install_RecordsRegistryRow_AndRetainsVerbatimBlob`;
  the list UI is not automated.)*
- **TUT-US-E4 🟡** As a Sharer, I can **re‑share** exactly the bundle I installed,
  because the verbatim `.tutor` is retained at
  `{AppData}\courses\{key}\{version}.tutor` in the Tutor data folder and downloadable from `/library`.
  *(covered by
  `CourseLifecycleRegistryTests.Install_RecordsRegistryRow_AndRetainsVerbatimBlob`.)*
- **TUT-US-E5 🟡** As a Recipient, I see a course's **provenance** before
  installing — author, license, source attribution, description. *(manifest fields
  covered by `BundleRoundTripTests.Export_WritesManifest_WithIdentityIntegrityAndProvenance`;
  the `/library` preview is not automated.)*
- **TUT-US-E6 🟡** As an Operator, a bundle with unsafe entry paths (rooted,
  drive‑letter, `..` escape) is rejected before install, preventing zip‑slip.
  *(covered by `BundleArchiveSafetyTests.UnsafePaths_AreRejected` and
  `CourseManifestValidatorTests.UnsafeEntryPath_FailsWithUnsafeEntryPath`.)*

## Epic F — Cross‑cutting quality (always‑on constraints)

- **TUT-US-F1 🟡** As any contributor, the solution builds clean
  (`dotnet build Tutor.slnx`) and `Tutor.Tests` is green before merge.
  *Gap:* HEAD does not compile (see [BIBLE §6](BIBLE.md#TUT-§6)); last green run
  2026-06-07, 380 passed.
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
