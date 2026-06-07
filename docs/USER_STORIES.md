---
codex: 1
project: Tutor
code: TUT
layer: stories
status: living
updated: 2026-06-07
---

# Tutor — User Stories

> Acceptance‑level requirements, grouped by epic. Status legend:
> **✅ Done** (shipped & tested) · **🟡 Partial** (works, gaps noted) ·
> **⬜ Planned** (designed, not built) · **🗑️ Cut**. Every ✅ cites the test that
> proves it. IDs are stable (`TUT-US-<Epic><n>`); never reference line numbers.

Personas:

- **Author/Operator** — builds and curates courses (CLI today; admin in‑app).
- **Learner/Student** — consumes courses, takes quizzes, earns certificates.
- **Admin** — manages users and (planned) the course library.
- **Sharer/Recipient** — passes a `.tutor` file to someone, or receives one.

---

## Epic A — Ingest & Course Building {#TUT-EPIC-A}

- **TUT-US-A1 🟡** As an Author, I can ingest a single source file (PDF, EPUB,
  HTML, DOCX) so it becomes a course resource.
  *Given* a supported file, *when* I run `tutor import file <path>`, *then* it is
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
  seed a starter library with `tutor gutenberg top10`. *Gap:* the fetch path hits
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

## Epic C — Authentication & Accounts  *(verified: clean build, 81/81 auth‑subset tests)* {#TUT-EPIC-C}

- **TUT-US-C1 ✅** As a Learner, I must log in; visiting `/courses`, `/learn`, or
  `/settings` while logged out redirects me to login. *(verified by `auth.cy.ts`.)*
- **TUT-US-C2 ✅** As a user, my password is stored with Argon2id + pepper
  (SQL‑backed), not the retired SHA‑256 JSON file. *(verified by `AuthUserImportTests`
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

## Epic D — Course Packaging  *(foundation built in production code; not yet test‑pinned)* {#TUT-EPIC-D}

- **TUT-US-D1 🟡** As an Author, I can export a course to a self‑contained
  `.tutor` bundle that includes pre‑computed embeddings, so re‑import skips the
  LLM pipeline. *Built:* `CourseExporter` + `BundleManifest.IncludesEmbeddings`
  (11 bundles shipped in `Courses/`). *Gap:* no automated export test in
  `Tutor.Tests` — pinned only by manual use and the shipped bundles.
- **TUT-US-D2 🟡** As a Recipient, I can install a bundle with `tutor install
  <file>`, and installing the same bundle twice yields two independent courses
  (all IDs remapped). *Built:* `BundleImporter` (`Tutor.Cli/Commands/ImportBundleCommand.cs`).
  *Gap:* the ID‑remap independence is not yet pinned by an automated test.
- **TUT-US-D3 🟡** As an Author, I can remove a course and all its derivatives
  with `tutor delete <id>` (cascades resources, structure, concept maps,
  embeddings). *Built* in the CLI delete command. *Gap:* the cascade is not
  pinned by an automated test.
- **TUT-US-D4 ⬜** As a Recipient, a bundle carries a **stable course key +
  whole‑number version**, so the system recognizes "you already have *Dracula*"
  and offers *upgrade vs. duplicate* instead of blindly duplicating.
  *(Add `CourseKey`/`CourseVersion` to the manifest; `CourseInstallResolver`.)*
- **TUT-US-D5 ⬜** As a Recipient, a corrupted or tampered `.tutor` is rejected up
  front with a clear reason, via a **SHA‑256 integrity check** and a **validation
  pass** (manifest‑first, IO‑free, explicit error codes). *(`CourseManifestValidator`.)*
- **TUT-US-D6 ⬜** As an Author on a newer build, I can still read **older** bundle
  formats; only formats *newer than my build* are refused. *(Replace
  `FormatVersion != 1` with `> HostMax`; add an `Extra` round‑trip dict.)*

## Epic E — Load / Unload & Sharing  *(the headline goal — planned)* {#TUT-EPIC-E}

- **TUT-US-E1 ⬜** As a Learner/Admin, I can **load** a course from a `.tutor`
  file **in the Blazor app** (upload → validate → plan → confirm → install), not
  only from the CLI. *(New Courses library page, behind `[Authorize]`.)*
- **TUT-US-E2 ⬜** As a Learner/Admin, I can **unload** a course so it disappears
  from my learning view **without destroying** its data or my progress
  (soft‑disable, `InstalledCourse.Enabled = false`) — mirroring the no‑hard‑delete
  rule auth already follows ([HOUSE-LAW-2](../../MindAttic.HouseRules.md#HOUSE-LAW-2)).
  A separate explicit "Remove" performs the hard cascade.
- **TUT-US-E3 ⬜** As an Admin, I can see **what is installed** — name, key,
  version, install date, integrity hash, enabled state — in one list.
  *(`InstalledCourse` registry.)*
- **TUT-US-E4 ⬜** As a Sharer, I can **re‑share** exactly the bundle I installed,
  because the verbatim `.tutor` is retained in a blob store.
  *(`%APPDATA%\Tutor\courses\{key}\{version}.tutor`.)*
- **TUT-US-E5 ⬜** As a Recipient receiving a shared course, I see its
  **provenance** before installing — author, license, source attribution,
  description. *(Provenance fields on `BundleManifest`.)*
- **TUT-US-E6 ⬜** As an Operator, when a course bundle extracts binary assets to
  disk, the extractor rejects unsafe entry paths (rooted, drive‑letter, `..`
  escape) to prevent zip‑slip. *(Port `IdeaArchiveReader.IsSafeEntryPath` — only
  needed once bundles carry extracted assets.)*

## Epic F — Cross‑cutting quality (always‑on constraints)

- **TUT-US-F1 ✅** As any contributor, the solution builds clean
  (`dotnet build Tutor.slnx`) and `Tutor.Tests` is green before merge.
  *(verified 2026-06-07: `dotnet test Tutor.Tests` → 380 passed, 0 failed.)*
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

The user‑facing ask — *load and unload courses so learning can be shared* — is
**Epic E**, which depends on **D4–D6**. Recommended order:

1. **TUT-US-D4 + D5 + D6** (identity, integrity, forgiving version gate) —
   additive, low‑risk, fully unit‑testable.
2. **TUT-US-E3 + E2** (installed registry + soft unload) — turns "delete" into
   "load/unload."
3. **TUT-US-E1 + E4 + E5** (in‑app library, re‑share, provenance) — the visible
   payoff.
4. **TUT-US-B5** (real per‑user quiz attribution) and **TUT-US-E6** (zip‑slip
   guard, when assets are bundled) as they become relevant.

See **[RFC 0001 — Course Packaging & Sharing](rfc/0001-course-packaging.md)** for
the phased implementation plan and the MindAttic.Ideas comparison.

---

### Audit log

Stories whose **status changed** during the 2026-06-07 Codex conform pass. The
original ask is preserved verbatim; only the *status* and the *test citation* were
corrected to match what an automated test in `Tutor.Tests` actually proves (per
[HOUSE-LAW-8](../../MindAttic.HouseRules.md#HOUSE-LAW-8), `✅` requires a verifying
test). No requirement wording was changed.

- **A1–A5** were marked **✅**; downgraded to **🟡** because the end‑to‑end LLM
  pipeline (paid/non‑deterministic) is not automated. The deterministic
  primitives (parsers, chunking, LSH/SimHash, structure shape) *are* tested and
  are now cited. *(original spec — audit log: A1–A5 stated ✅ "shipped & tested".)*
- **B2** kept **✅**; test citation added (`QuizGenerationServiceTests` +
  `quiz.cy.ts`) where the original named none.
- **C2** kept **✅**; clarified that Argon2id hashing is owned/tested by
  MindAttic.Authentication and the import path is pinned by `AuthUserImportTests`.
- **C4** was marked **✅**; downgraded to **🟡** because no in‑repo automated test
  pins the idle‑timeout modal behavior. *(original spec — audit log: C4 stated ✅
  "(`UserTimeout` + `user-timeout.js`)".)*
- **D1, D2, D3** were marked **✅**; downgraded to **🟡** because no test in
  `Tutor.Tests` pins course export/import/delete — the originals cited class names
  (`CourseExporter`, `BundleImporter`, `BundleManifest.IncludesEmbeddings`), not
  tests. *(original spec — audit log: D1 "✅ … `CourseExporter`;
  `BundleManifest.IncludesEmbeddings`", D2 "✅ … `BundleImporter`", D3 "✅ … cascade".)*
