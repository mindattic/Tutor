---
codex: 1
project: Tutor
code: TUT
layer: rfc
status: planned
updated: 2026-06-07
---

# RFC 0001 — Course Packaging & Sharing

> **Goal:** let learners *load and unload courses* in‑app so that learning can be
> shared, the same way **MindAttic.Ideas** lets you install/uninstall `.idea`
> packages. This document compares the two packaging designs and lays out a
> concrete upgrade path for Tutor's `.tutor` bundles.

## Problem

Tutor already has a real bundle format, but it lacks the *lifecycle* layers that
make "load / unload / share" possible for a non‑technical user: stable identity,
integrity, a forgiving version gate, a validation/plan step, an installed
registry with soft unload, and an in‑app surface. This RFC decides which patterns
to port from `.idea` and which to deliberately leave behind.

---

## 1. Where Tutor is today

Tutor already has a real bundle format — this is a *starting point*, not a blank
slate.

| Piece | Location |
|---|---|
| Bundle manifest | `Tutor.Cli/Export/TutorCourseBundle.cs` → `BundleManifest` |
| Exporter (pack) | `Tutor.Cli/Export/CourseExporter.cs` |
| Importer (unpack) | `Tutor.Cli/Export/BundleImporter.cs` |
| CLI surface | `Tutor.Cli/Commands/ExportCommand.cs`, `ImportBundleCommand.cs` |
| Shipped bundles | `Courses/*.tutor` (11 books, ~40 MB) |

**Archive layout** (a `.tutor` file is a plain zip):

```
manifest.json            FormatVersion, CourseId, CourseName, counts, IncludesEmbeddings
course.json              Course metadata (references only)
courseStructure.json     Lessons → Sections → ConceptIds + baked quizzes (optional)
resources/{id}.json      CourseResource metadata
resources/{id}.original.txt
resources/{id}.formatted.md
conceptMaps/{id}.json     One ConceptMap per resource
chunks.json              BundleChunkSet — RAG chunks WITH pre-computed embeddings
```

**What already works well — keep it:**

- **Embeddings ride along** in `chunks.json`, so re-importing a course skips the
  slow/paid LLM pipeline. This is the single best decision in the current design
  and `.idea` has no equivalent (its packages carry assemblies, not derived data).
- **ID remapping on import** — every cross-entity GUID is rewritten so importing
  the same bundle twice yields two independent courses instead of clobbering.
- **`FormatVersion` is the compatibility source of truth**, decoupled from the
  `.tutor` extension. Right instinct.
- **Same services as the live app.** `BundleImporter` persists through
  `CourseService` / `ConceptMapStorageService` / `CourseStructureStorageService` /
  `VectorStoreService`, so an imported course is indistinguishable from a built one.

**What's missing for the "load / unload / share" goal:**

1. **It's CLI-only.** Install is `tutor import-bundle`; there is no in-app
   "Install a course" / "Remove a course" surface. Sharing learning means a
   *user* (not an operator at a terminal) loads a `.tutor` someone sent them.
2. **No stable identity.** Import always mints fresh GUIDs, so the system can
   never recognize "this is *Dracula*, which you already have" or "this is a
   newer version of a course you installed." Every import is a blind duplicate.
3. **No integrity check.** A truncated or tampered `.tutor` is detected only when
   deserialization happens to throw. No checksum.
4. **Brittle version gate.** `if (manifest.FormatVersion != 1) throw` rejects
   *everything* that isn't exactly 1 — including older bundles a newer CLI should
   still read. Strict equality, not "accept ≤ host max."
5. **No installed registry.** Nothing records *what is installed, from which
   bundle, when*. "Unload" is `tutor delete <guid>` with a cascade; there is no
   enable/disable, no re-export, no provenance.
6. **No validation pass.** Manifest fields are trusted as-is; malformed input
   fails deep inside persistence rather than up-front with a clear reason.
7. **Thin provenance.** `ExportedBy = "tutor-cli"` and a timestamp — no author,
   license, source attribution, or description for the person receiving a shared
   course.

---

## 2. Options compared — what MindAttic.Ideas does that's worth stealing

The `.idea` system (`MindAttic.Ideas/src/MindAttic.Ideas.Packaging` +
`…Core/Services`) is a more mature take on the *same* "zip a thing, install /
uninstall it" problem. The transferable ideas:

| `.idea` capability | Class | Why it matters for courses |
|---|---|---|
| **Stable string `Key` + whole‑number `Version`** | `IdeaManifest` | Identity that survives re‑install. Lets you say "*Dracula* v2 supersedes v1" instead of duplicating. |
| **Manifest‑first, IO‑free validation** | `ManifestValidator` → `ValidationResult` with explicit codes (`BAD_KEY`, `MANIFEST_TOO_NEW`, `SHA_MISMATCH`, …) | Fail fast with a *reason* before touching disk. |
| **Install plan / version collision logic** | `PackageVersionResolver` → `InstallPlan` (`Install` / `NoOpAlreadyInstalled` / `RejectDowngrade` / `Blocked`) | Pure decision step: upgrade, no‑op, or refuse a downgrade — testable without a DB. |
| **SHA‑256 integrity** | computed in `PackageInstallService`, checked by validator | Detect corruption/tampering on a shared file. |
| **Forward/back compat** | `ManifestVersion <= HostMax` + `Extra` dict round‑trips unknown fields | Newer host reads older packages; older fields aren't lost. |
| **Installed registry (soft‑delete)** | `InstalledPackage` row: `Enabled`, `IsActiveVersion`, `BlobPath`, `Sha256`, `InstalledUtc` | Real load/**unload**: disable without destroying, re‑export, history. |
| **Verbatim blob store** | `LocalFilePackageBlobStore` → `{category}/{key}/{version}.idea` | Keep the original package to re‑share or roll back. |
| **Zip‑slip‑guarded extraction** | `IdeaArchiveReader.IsSafeEntryPath` + double‑checked `ExtractTo` | A shared file is *untrusted input* — Tutor extracts none today, but will the moment a course carries arbitrary assets. |

## Decision

Adopt the *identity, validation, integrity, and registry* layers from `.idea`;
keep Tutor's embeddings‑in‑bundle and ID‑remap strengths; ignore the code‑loading
machinery. This realizes
[HOUSE-LAW-5](../../../MindAttic.HouseRules.md#HOUSE-LAW-5) (packaging is a guarded
zip with a lifecycle) for `.tutor`. Concretely:

### 3.1 Give a course a stable identity

Add to `BundleManifest`:

```csharp
public string CourseKey { get; set; } = "";   // stable slug, e.g. "dracula"  (^[a-z0-9][a-z0-9._-]{0,119}$)
public int CourseVersion { get; set; } = 1;    // whole-number, forward-only (matches house versioning rule)
public string Sha256 { get; set; } = "";       // integrity of the payload (everything but manifest)
// provenance for shared courses:
public string? Author { get; set; }
public string? License { get; set; }
public string? Description { get; set; }
public string? SourceAttribution { get; set; } // e.g. "Project Gutenberg #345"
public IDictionary<string, JsonElement> Extra { get; set; } = new Dictionary<string, JsonElement>(); // forward-compat
```

`CourseKey` is what import dedupes on. The internal GUID remap **stays** — it's
how two side‑by‑side installs of the same key avoid storage collisions — but the
*key* lets the UI say "you already have this" and offer upgrade vs. duplicate.

### 3.2 Loosen the version gate + verify integrity

Replace the strict equality check in `BundleImporter`:

```csharp
// before:  if (manifest.FormatVersion != 1) throw …
const int HostMaxFormat = 1;
if (manifest.FormatVersion > HostMaxFormat)
    throw new InvalidOperationException(
        $"Bundle format {manifest.FormatVersion} is newer than this build supports ({HostMaxFormat}). Update Tutor.");
// older formats fall through and are read with documented field defaults.
```

Compute SHA‑256 of the payload on export, store it in the manifest, recompute and
compare on import (mirrors `PackageInstallService`).

### 3.3 Add a pure validation + plan step (port the pattern, not the code)

Introduce `Tutor.Core/Courses/Packaging/` with two **pure, IO‑free** classes
modeled on `.idea`:

- `CourseManifestValidator.Validate(manifest, entryNames, hostMaxFormat)` →
  `ValidationResult` with codes (`BAD_KEY`, `NO_NAME`, `FORMAT_TOO_NEW`,
  `SHA_MISMATCH`, `MISSING_COURSE_JSON`). Hard errors block; warnings don't.
- `CourseInstallResolver.Plan(candidate, installed)` → `InstallPlan`
  (`Install` / `NoOpAlreadyInstalled` / `RejectDowngrade` / `InstallSideBySide`).
  Downgrade rule: a lower `CourseVersion` for an existing `CourseKey` is rejected
  unless the caller explicitly asks to duplicate.

Pure classes = trivially unit‑testable, exactly as `.idea` tests
`PackageVersionResolver` with no database.

### 3.4 Add an installed‑courses registry → real load/**unload**

Today "installed" is implicit (a course exists in secure storage). Add an
explicit record, mirroring `InstalledPackage`:

```csharp
public sealed class InstalledCourse
{
    public string CourseId { get; set; } = "";     // the remapped GUID actually persisted
    public string CourseKey { get; set; } = "";     // stable identity from the bundle
    public int CourseVersion { get; set; }
    public string Name { get; set; } = "";
    public string Sha256 { get; set; } = "";
    public string? BlobPath { get; set; }            // kept verbatim .tutor for re-share/rollback
    public bool Enabled { get; set; } = true;        // UNLOAD = Enabled:false, not destroy
    public DateTime InstalledUtc { get; set; }
}
```

- **Load** = install bundle → `Enabled = true`.
- **Unload** = `Enabled = false` (course hidden from the learner, progress and
  data retained). This is softer and friendlier than today's `tutor delete`
  cascade, and matches `.idea`'s soft‑disable‑never‑hard‑delete contract that
  Tutor's own `IUserAdminService` already follows
  ([HOUSE-LAW-2](../../../MindAttic.HouseRules.md#HOUSE-LAW-2)).
- **Remove** = the existing cascade delete, now also dropping the registry row.

Persist the verbatim `.tutor` in a blob store (`%APPDATA%\Tutor\courses\{key}\{version}.tutor`)
so a learner can **re‑share** exactly what they installed.

### 3.5 Bring install/unload into the Blazor UI

The whole point of "share learning" is that a non‑technical user does it. Add a
**Courses** admin/library page (parallel to the existing Admin Users page) that:

- lists `InstalledCourse` rows with enable/disable toggles,
- accepts a `.tutor` upload → runs validate → plan → (confirm upgrade/duplicate) → import,
- surfaces validation errors and the install plan decision in plain language,
- offers "Export / Share" (re‑download the verbatim blob).

Wrap the existing `BundleImporter`/`CourseExporter` so the CLI and UI share one
code path — the same "same engine, two front doors" principle the README already
states for the build pipeline
([HOUSE-LAW-6](../../../MindAttic.HouseRules.md#HOUSE-LAW-6)).

### 3.6 Guard extraction *if/when* courses carry binary assets

Today the importer only reads JSON/text entries by name, so zip‑slip isn't yet
exploitable. The moment a course bundles images/audio extracted to disk, port
`IdeaArchiveReader.IsSafeEntryPath` (reject rooted paths, drive letters, `..`
escapes) before writing any entry.

---

## What NOT to do

Do **not** copy `.idea`'s code‑loading machinery. `.idea` ships **executable
assemblies** loaded into an `AssemblyLoadContext`, with SDK‑version gating,
`EntryType` discovery, and a "don't ship host assemblies" audit. Courses are
**pure data** (JSON + text + float vectors). Skip the ALC machinery,
`Kind=code/data`, `EntryType`, `Sdk`, and the `DependsOn`/`Uses` dependency
graph. Pulling that in would be cargo‑cult complexity — a course never executes.

---

## Phased plan (with risk)

| Phase | Scope | Risk |
|---|---|---|
| **1 — Identity & integrity** | Add `CourseKey`, `CourseVersion`, `Sha256`, provenance to `BundleManifest`; compute/verify SHA; loosen the format gate to `≤ HostMax`. Back‑fill `CourseKey` for the 11 shipped bundles. | Low — additive manifest fields, `Extra` keeps old readers working. |
| **2 — Validate + plan** | Port `CourseManifestValidator` + `CourseInstallResolver` as pure classes with unit tests. Wire into `BundleImporter`. | Low — pure logic, fully testable. |
| **3 — Registry & soft unload** | `InstalledCourse` store + verbatim blob store; redefine load/unload/remove; keep `tutor delete` as hard‑remove. | Medium — touches persistence + the lifecycle test. |
| **4 — In‑app library** | Blazor Courses page: upload‑install, enable/disable, re‑share, behind `[Authorize]` (admin for install, learner for browse). | Medium — UI + upload handling + antiforgery. |

Each phase is shippable on its own and leaves the CLI working throughout.

---

## One‑paragraph summary

Tutor's `.tutor` format already nails the hard part `.idea` can't help with —
bundling pre‑computed embeddings so re‑imports are free — and it remaps IDs
correctly. What it lacks is everything around the *lifecycle*: stable identity,
integrity, a forgiving version gate, a validation/plan step, an installed
registry with soft unload, and an in‑app surface. Those are precisely the layers
`.idea` has matured (`IdeaManifest` key+version, `ManifestValidator`,
`PackageVersionResolver`/`InstallPlan`, `InstalledPackage` soft‑disable,
SHA‑256, zip‑slip guards). Port the *pattern* of those layers — not the
assembly‑loading machinery, which courses, being pure data, never need.

---

## Graduates into

- **Bible:** [TUT-§7 Active frontier](../BIBLE.md#TUT-§7);
  [TUT-LAW-3 Bundles are independent](../BIBLE.md#TUT-§5).
- **Stories:** [Epic D (D4–D6)](../USER_STORIES.md#TUT-EPIC-D)
  and [Epic E (E1–E6)](../USER_STORIES.md#TUT-EPIC-E).

When each phase ships and is test‑pinned, promote the corresponding story to ✅,
fold the decision into the Bible, and mark the relevant section of this RFC
*superseded*.
