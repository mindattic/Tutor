# Resources

This folder contains resources for the Tutor application courses.

## Folder Structure

- **Original/** - Original uploaded files (txt, docx, etc.)
- **Formatted/** - AI-formatted markdown versions of resources
- **Courses/** - Course definitions and metadata

## How It Works

When you import a resource with "Auto-format with AI" enabled:

1. The original file is saved to `Original/`
2. The AI-formatted version is saved to `Formatted/` as a `.md` file
3. The course metadata is saved to `Courses/`

## File Naming

- Original files keep their original names
- Formatted files are named: `{original-name}_formatted.md`
- Course files are named: `{course-name}_{id}.json`

## Checking Into GitHub

All files in this folder are designed to be checked into version control,
allowing you to share resources across machines and with others.
