# Courses

Redistributable **`.tutor`** course bundles that ship with the repo. Each file is a
self-contained zip — course metadata, the learning structure (lessons → topics →
sections), concept maps, baked quiz questions, and the RAG embeddings — so it installs
without re-running the (slow, paid) LLM build pipeline.

One file per course; bundles are **independent** and are never merged.

## Install a course

```
tutor install "Courses/Moby Dick; Or, The Whale.tutor"
```

(`tutor install` is an alias of `tutor import-bundle`.) All IDs are rewritten on import,
so installing the same bundle twice yields two distinct courses rather than overwriting.
Once installed, the course appears in the Tutor Blazor UI automatically.

## Remove a course

```
tutor list                 # find the course id
tutor delete <course-id>   # cascades resources, structure, concept maps, embeddings
```

## Regenerate / add bundles

These were produced by the CLI, e.g.:

```
tutor gutenberg-top10 --export-dir Courses --quiz-mode both
tutor build-course <dir-or-zip> --export "Courses/My Course.tutor"
```

> Note: bundles carry embeddings and can be several MB each — they are intentionally
> committed here so courses travel with the repo.
