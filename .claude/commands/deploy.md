Deploy the Tutor landing page (`mindattic.com/tutor.htm`) via **MindAttic.Deploy** (sibling repo at `D:\Projects\MindAttic\MindAttic.Deploy`).

Renders this repo's `README.md` through the catalog template (`template/index.template.htm`, Cyberspace theme, MindAttic.UiUx components loaded via jsDelivr) and FTPS-uploads the single-file result. One repo owns the whole FTP pipeline — there is no per-project deploy state in this folder.

Run this command and report the result:

```
powershell -NoProfile -ExecutionPolicy Bypass -Command "cd D:\Projects\MindAttic\MindAttic.Deploy; npm run deploy -- --only tutor"
```

It will:

1. Render `D:\Projects\MindAttic\Tutor\README.md` through the catalog template.
2. FTPS-upload `out/tutor.htm` to `/mindattic.com/tutor.htm`.

After running, summarize the result and flag any failures.

Notes:
- Catalog entry: `MindAttic.Deploy/projects.json` -> `projects[]` slug `tutor` (theme: Cyberspace).
- Credentials: MindAttic.Vault at `%APPDATA%\MindAttic\Deploy\ftp.json` (transitional fallback: `MindAttic.Deploy/secrets/ftp.json`, gitignored).
- A Blazor app deploy also exists in `apps[]` (`--app tutor`) but is **disabled** pending a workflow + Azure infra. Until that's provisioned, `/deploy` ships the landing page only.
