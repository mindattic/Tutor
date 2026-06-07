# Tutor — User Stories

> Acceptance‑level requirements, grouped by epic. Status legend:
> **✅ Done** (shipped & tested) · **🟡 Partial** (works, gaps noted) ·
> **⬜ Planned** (designed, not built). Where a story is verified by a test,
> the test is named.

Personas:

- **Author/Operator** — builds and curates courses (CLI today; admin in‑app).
- **Learner/Student** — consumes courses, takes quizzes, earns certificates.
- **Admin** — manages users and (planned) the course library.
- **Sharer/Recipient** — passes a `.tutor` file to someone, or receives one.

---

## Epic A — Ingest & Course Building

- **A1 ✅** As an Author, I can ingest a single source file (PDF, EPUB, HTML,
  DOCX) so it becomes a course resource.
  *Given* a supported file, *when* I run `tutor import file <path>`, *then* it is
  parsed, chunked, embedded, and a concept map is produced.
- **A2 ✅** As an Author, I can ingest legacy formats (`.doc/.rtf/.odt` via
  LibreOffice; `.mobi/.azw/.azw3` via Calibre) without the downstream pipeline
  caring which format it was.
- **A3 ✅** As an Author, I can pull a Project Gutenberg work by ID, and seed a
  starter library with `tutor gutenberg top10`.
- **A4 ✅** As an Author, the system extracts concepts, correlates them into one
  knowledge graph across resources, and reattaches orphan concepts.
- **A5 ✅** As an Author, a hierarchical course structure (Lessons → Sections →
  concepts) is generated *from the graph*, with section content filled from the
  source.
- **A6 🟡** As an Author, I can do all of the above from the Blazor UI.
  *Gap:* building is CLI‑first; in‑app build/monitoring is partial.

## Epic B — Learning Experience

- **B1 ✅** As a Learner, I see only the first lesson unlocked, and later lessons
  unlock as I demonstrate mastery. *(LearningPathService; `FullCourseLifecycleTests`.)*
- **B2 ✅** As a Learner, I take quizzes whose questions are grounded in the
  source material via RAG.
- **B3 ✅** As a Learner, once I master the course the final exam unlocks; passing
  it issues a certificate. *(FinalExamService + CertificateService;
  `FullCourseLifecycleTests`.)*
- **B4 ✅** As a Learner, my progress is tracked per‑user, per‑course and is
  isolated from other learners. *(`FullCourseLifecycleTests` — a fresh student
  starts locked.)*
- **B5 🟡** As a Learner, my quiz results are attributed to *me*.
  *Gap:* quizzes started mid‑Blazor‑circuit fall back to `"anonymous"` because
  `QuizService` reads `IHttpContextAccessor` (null post‑render). Fix: thread the
  id from `AuthenticationState`. *(TODO in `QuizService.cs`.)*

## Epic C — Authentication & Accounts  *(verified: clean build, 81/81 tests)*

- **C1 ✅** As a Learner, I must log in; visiting `/courses`, `/learn`, or
  `/settings` while logged out redirects me to login. *(`auth.cy.ts`.)*
- **C2 ✅** As a user, my password is stored with Argon2id + pepper (SQL‑backed),
  not the retired SHA‑256 JSON file.
- **C3 ✅** As an existing user, my legacy account is imported on startup and my
  password upgrades transparently on first login; weak dev seeds are forced to
  reset. *(`AuthUserImportTests` — idempotent, role mapping, GUID preservation.)*
- **C4 ✅** As a user, an idle session warns me and auto‑logs me out (30 min idle /
  8 h absolute). *(`UserTimeout` + `user-timeout.js`.)*
- **C5 ✅** As an Admin, at `/users` I can create users, set roles, reset
  passwords, and **soft‑disable** accounts — with **no hard delete** exposed.
  *(`UsersAdminContractTests.ExposesNoHardDeleteApi`.)*
- **C6 ✅** As an Admin, the Users page is reachable only with the Admin policy.
  *(`[Authorize(Policy = MaPolicies.Admin)]`.)*

## Epic D — Course Packaging  *(foundation done; lifecycle planned)*

- **D1 ✅** As an Author, I can export a course to a self‑contained `.tutor`
  bundle that includes pre‑computed embeddings, so re‑import skips the LLM
  pipeline. *(`CourseExporter`; `BundleManifest.IncludesEmbeddings`.)*
- **D2 ✅** As a Recipient, I can install a bundle with `tutor install <file>`,
  and installing the same bundle twice yields two independent courses (all IDs
  remapped). *(`BundleImporter`.)*
- **D3 ✅** As an Author, I can remove a course and all its derivatives with
  `tutor delete <id>` (cascades resources, structure, concept maps, embeddings).
- **D4 ⬜** As a Recipient, a bundle carries a **stable course key + whole‑number
  version**, so the system recognizes "you already have *Dracula*" and offers
  *upgrade vs. duplicate* instead of blindly duplicating.
  *(Add `CourseKey`/`CourseVersion` to the manifest; `CourseInstallResolver`.)*
- **D5 ⬜** As a Recipient, a corrupted or tampered `.tutor` is rejected up front
  with a clear reason, via a **SHA‑256 integrity check** and a **validation pass**
  (manifest‑first, IO‑free, explicit error codes). *(`CourseManifestValidator`.)*
- **D6 ⬜** As an Author on a newer build, I can still read **older** bundle
  formats; only formats *newer than my build* are refused. *(Replace
  `FormatVersion != 1` with `> HostMax`; add an `Extra` round‑trip dict.)*

## Epic E — Load / Unload & Sharing  *(the headline goal — planned)*

- **E1 ⬜** As a Learner/Admin, I can **load** a course from a `.tutor` file
  **in the Blazor app** (upload → validate → plan → confirm → install), not only
  from the CLI. *(New Courses library page, behind `[Authorize]`.)*
- **E2 ⬜** As a Learner/Admin, I can **unload** a course so it disappears from my
  learning view **without destroying** its data or my progress (soft‑disable,
  `InstalledCourse.Enabled = false`) — mirroring the no‑hard‑delete rule auth
  already follows. A separate explicit "Remove" performs the hard cascade.
- **E3 ⬜** As an Admin, I can see **what is installed** — name, key, version,
  install date, integrity hash, enabled state — in one list. *(`InstalledCourse`
  registry.)*
- **E4 ⬜** As a Sharer, I can **re‑share** exactly the bundle I installed, because
  the verbatim `.tutor` is retained in a blob store. *(`%APPDATA%\Tutor\courses\{key}\{version}.tutor`.)*
- **E5 ⬜** As a Recipient receiving a shared course, I see its **provenance**
  before installing — author, license, source attribution, description.
  *(Provenance fields on `BundleManifest`.)*
- **E6 ⬜** As an Operator, when a course bundle extracts binary assets to disk,
  the extractor rejects unsafe entry paths (rooted, drive‑letter, `..` escape) to
  prevent zip‑slip. *(Port `IdeaArchiveReader.IsSafeEntryPath` — only needed once
  bundles carry extracted assets.)*

## Epic F — Cross‑cutting quality (always‑on constraints)

- **F1 ✅** As any contributor, the solution builds clean (`dotnet build Tutor.slnx`)
  and `Tutor.Tests` is green before merge.
- **F2 ✅** As any contributor, no code path hard‑codes an LLM vendor; all calls
  route through `LlmServiceRouter` over Legion.
- **F3 ✅** As any contributor, a CLI‑built course is readable by the Blazor app
  with zero translation (shared DI graph).
- **F4 ✅** As the org, versions bump by whole numbers only.
- **F5 ✅** As the org, credentials resolve through Vault, never embedded in code.

---

## Priority for the "share learning" goal

The user‑facing ask — *load and unload courses so learning can be shared* — is
**Epic E**, which depends on **D4–D6**. Recommended order:

1. **D4 + D5 + D6** (identity, integrity, forgiving version gate) — additive,
   low‑risk, fully unit‑testable.
2. **E3 + E2** (installed registry + soft unload) — turns "delete" into
   "load/unload."
3. **E1 + E4 + E5** (in‑app library, re‑share, provenance) — the visible payoff.
4. **B5** (real per‑user quiz attribution) and **E6** (zip‑slip guard, when
   assets are bundled) as they become relevant.

See **[COURSE_PACKAGING_DESIGN.md](COURSE_PACKAGING_DESIGN.md)** for the phased
implementation plan and the MindAttic.Ideas comparison.
