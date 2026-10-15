---
codex: 1
project: Tutor
code: TUT
layer: stories
status: living
updated: 2026-10-03
---
<!-- workflow-db: this repo's stories and tickets live in the MindAttic.Workflow database -->

# Tutor — User Stories

This repo's stories and tickets (TUT-US-…, TUT-T…) live in the MindAttic.Workflow database
(`MindAttic.Workflow/data/workflow.db`), not in this file. Use the `workflow` CLI:

- Read: `workflow story list --repo Tutor`, `workflow ticket list --repo Tutor`, `workflow ticket show <ID>`
- Change: `workflow ticket new` / `close`, `workflow story status <ID> done|partial|planned`, `workflow ticket link <A> requires <B>`
- This file as it was, rebuilt from the database: `workflow db export --repo Tutor`
