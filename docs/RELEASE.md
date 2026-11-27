---
layer: release
status: living
---
# Tutor — Release charter

> The one finish line this repo is working toward. Every open ticket in this repo sits under one of the epics
> below, and every epic is part of the milestone. Work that does not fit is out of scope until this milestone is
> reached (owner, 2026-11-27, MAT-T319).

## 1. What this is
Tutor turns books and documents into structured courses (concept graph, learning path, grounded quizzes, final
exam, certificate) through a Blazor Server app and a CLI that share one engine, and packages each course as a
portable `.tutor` bundle.

## 2. The milestone: Tutor 1.0 Local Release {#milestone}
- **Milestone:** MS-11, "Tutor 1.0 Local Release"
- **Published means:** an internal app deployed by `run.ps1` to `R:\MindAttic\Apps\Tutor\Debug` and `\Release`,
  signing in against local SQL Server. No hosted site (Azure was decommissioned 2026-11-26).
- **The natural stopping point:** Tutor runs from R: with no Azure configuration left; several people signed in to
  the same local instance each keep their own progress, quiz, lesson and certificate; an author can upload a
  document in the web app and build a course from it without the CLI; the open correctness bugs from the last bug
  hunts are fixed; and the services most likely to lose data or stall a build have NUnit coverage. That closes the
  two gaps the BIBLE's active frontier names (per-user attribution, in-app building) on top of an engine that
  already works end to end, which is a coherent 1.0.

## 3. Done when (acceptance checks)
Each line is also a `tickets milestone check` (build | exists | harness | signoff).
1. build — `dotnet build Tutor.slnx` builds clean on main.
2. exists — `R:\MindAttic\Apps\Tutor\Release` holds a deployed Tutor that starts against local SQL.
3. signoff — the owner, signed in as two different users, uploads a document, builds a course from it in the web
   app, and works a lesson, quiz and final exam with separate progress for each user.

## 4. Epics, in priority order
| Priority | Epic (ticket) | What it delivers | Features |
|---|---|---|---|
| 900 | TUT-T157 Local runtime | Runs from R: on local SQL, no Azure config, docs match | TUT-T162 Local SQL config (990), TUT-T163 Deploy to R: (980), TUT-T164 Local docs (970) |
| 800 | TUT-T158 Open bug fixes | The concrete bugs from the last bug hunts, fixed | TUT-T165 Admin page safety (890), TUT-T166 Learner UI errors (880), TUT-T167 Storage and concurrency (870), TUT-T168 Service robustness (860) |
| 700 | TUT-T159 Per-user learning state | Each signed-in user keeps their own progress, quiz, lesson, course and certificate | TUT-T169 Per-user progress and quiz (790), TUT-T170 Per-user session state (780) |
| 600 | TUT-T160 In-app course building | Upload a document and build and monitor a course in the web app | TUT-T171 Web document upload (690), TUT-T172 Build progress in web (680) |
| 500 | TUT-T161 Engine test coverage | NUnit tests where data loss or a stalled build would hurt most | TUT-T173 Key failover tests (590), TUT-T174 Queue tests (580), TUT-T175 Learner data tests (570), TUT-T176 Bundle path tests (560) |

## 5. Out of scope (not before this milestone)
Ideas and systems deliberately left out. A new request that lands here is filed `proposed`, never approved
automatically.
- Any hosted deployment (Azure, Hetzner, custom domain, deploy workflows, health probes for a host) — Azure is
  gone and Tutor runs locally.
- Phases 1–3 of `docs/plans/tutor-improvement-plan.md` beyond what is listed above: a SQL/Blob course store,
  a vector database, telemetry, rate limits and MFA for a public site — they were for hosting.
- More Gutenberg batch builds and timing runs — `tutor gutenberg-top` already exists (TUT-T50).
- Open-ended bug hunts, "run the suite until green" loops and general clean-up sweeps — bugs are filed as concrete
  fix tasks instead.
- Test back-fill beyond the four areas in Epic 5 (graph algorithms, parsers, CLI argument parsing, concept-merge
  and correlation scoring, log storage).
- New LLM providers (Kimi), the News feed, Tutor.Mcp features, the Tutor.Ingestion NuGet for other apps.

## 6. Current verified state
- The full pipeline (parse, chunk, embed, concept maps, knowledge graph, course structure, baked quizzes, `.tutor`
  export and install) runs from the CLI; ten bundled courses ship in `Courses/`.
- Course lifecycle (mastery-gated unlock, final exam, certificate, unload) is pinned by `FullCourseLifecycleTests`.
- `QuizService` and `FinalExamService` are scoped per circuit (ccde0ba, TUT-T152).
- `tutor gutenberg-top` builds the current top Gutenberg books and reports time per stage (06efd6e, TUT-T50).
- The improvement-plan audit is written (`docs/plans/tutor-improvement-plan.md`, 76e6abd, TUT-T51).
- `run.ps1` is on the shared run menu (e7a6ba6, MAT-T306), but nothing is deployed to `R:\MindAttic\Apps\Tutor` yet.

## 7. Decision record
- 2026-11-27 — charter drafted from README/BIBLE/docs/tickets; Legion vote: **Approve** (5 of 5 answering voters, plurality quorum reached, 9 asked, 4 failed on missing keys or model errors); dissent: none.
- Triage: 57 open (plus this job ticket) → done 5, duplicate 0, wont-do 28, kept 24; 9 new gap tasks filed (TUT-T177..T185). Tree: 5 epics, 15 features, 33 tasks, all approved; MS-11 active with build, exists and signoff checks.
