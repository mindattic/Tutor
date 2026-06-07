# Tutor - Bible Digest
> AUTHORITATIVE - full detail in docs/BIBLE.md
> generatedFrom: docs/BIBLE.md  -  generated: 2026-06-07
> Do not hand-edit; regenerate with `tools/codex.ps1 digest`.

## One sentence
**Drop a book in. Get a course out.** Tutor turns books, papers, and documents
into structured, navigable courses — with a concept graph, a learning path,
baked quizzes, and RAG‑grounded answers — and lets those courses be packaged,
shared, loaded, and unloaded.

## What it is NOT
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

## The Laws
> Tutor inherits the org‑wide laws from
> **[MindAttic.HouseRules.md](../../MindAttic.HouseRules.md)** by reference — they
> are not restated here. The project‑specific laws below extend them.

**Inherited from House Rules** (authoritative text lives there):

- Whole‑number versioning — [see HOUSE-LAW-1](../../MindAttic.HouseRules.md#HOUSE-LAW-1)
- Soft‑disable, never hard‑delete — [see HOUSE-LAW-2](../../MindAttic.HouseRules.md#HOUSE-LAW-2)
  (Tutor: `IUserAdminService` is pinned by a contract test exposing **no hard
  delete**; course load/unload follows the same instinct — unload hides, it
  doesn't erase.)
- Credentials resolve through MindAttic.Vault — [see HOUSE-LAW-3](../../MindAttic.HouseRules.md#HOUSE-LAW-3)
  (Tutor: LLM keys via `%APPDATA%\MindAttic\LLM\providers.json` or
  `IConfiguration`; auth secrets via the Vault `Security` bucket.)
- Provider‑agnostic LLMs via MindAttic.Legion — [see HOUSE-LAW-4](../../MindAttic.HouseRules.md#HOUSE-LAW-4)
  (Tutor: route through `LlmServiceRouter`; never reference a vendor SDK directly
  from pipeline code.)
- Packaging is a guarded zip with a lifecycle — [see HOUSE-LAW-5](../../MindAttic.HouseRules.md#HOUSE-LAW-5)
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
   bundle installs as its own course. Importing the same bundle twice yields two
   independent courses (all GUIDs remapped); courses are never merged into one
   another.
4. **Code style.** {#TUT-LAW-4} (from `CLAUDE.md`): private fields are
   `camelCase` **without** underscore prefix; constructors use `this.x = x`.

---

## Glossary
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

## Status index
- done: 14
- partial: 11
- planned: 9
- cut: 0

## Latest amendment
_No amendments (epoch 0)._

