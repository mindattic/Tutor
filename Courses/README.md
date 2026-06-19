# Courses

Pre-built **`.tutor`** course bundles that ship with the repo. Each file is a self-contained zip containing course metadata, the learning structure (lessons → topics → sections), concept maps, baked quiz questions, and RAG embeddings — so it installs without re-running the (slow, paid) LLM build pipeline.

One file per course. Bundles are independent and are never merged.

## Included courses

| Bundle | Source |
| --- | --- |
| `Alice's Adventures in Wonderland.tutor` | Project Gutenberg |
| `Dracula.tutor` | Project Gutenberg |
| `Frankenstein.tutor` | Project Gutenberg |
| `Gulliver's Travels.tutor` | Project Gutenberg |
| `Moby Dick; Or, The Whale.tutor` | Project Gutenberg |
| `Pride and Prejudice.tutor` | Project Gutenberg |
| `The Adventures of Sherlock Holmes.tutor` | Project Gutenberg |
| `The Adventures of Tom Sawyer.tutor` | Project Gutenberg |
| `The Picture of Dorian Gray.tutor` | Project Gutenberg |
| `Treasure Island.tutor` | Project Gutenberg |

## Install a course

```powershell
tutor install "Courses/Moby Dick; Or, The Whale.tutor"
```

(`tutor install` is an alias of `tutor import-bundle`.) All IDs are rewritten on import, so installing the same bundle twice yields two distinct courses rather than overwriting. Once installed, the course appears in the Tutor Blazor UI automatically.

## Remove a course

```powershell
tutor list                 # find the course id
tutor delete <course-id>   # cascades resources, structure, concept maps, embeddings
```

## Regenerate or add bundles

Bundles were produced by the CLI:

```powershell
tutor gutenberg-top10 --export-dir Courses --quiz-mode both
tutor build-course <dir-or-zip> --export "Courses/My Course.tutor"
```

Bundles carry embeddings and can be several MB each — they are committed here so courses travel with the repo.
