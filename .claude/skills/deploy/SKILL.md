---
name: deploy
description: Deploy Tutor via MindAttic.Deploy (sibling repo). Fires the GitHub Actions workflow that targets the tutor Azure App Service. Currently DISABLED in MindAttic.Deploy -- no workflow exists yet, and no Azure infrastructure is provisioned.
---

When invoked, run:

```
powershell -NoProfile -ExecutionPolicy Bypass -Command "cd D:\Projects\MindAttic\MindAttic.Deploy; npm run deploy -- --app tutor"
```

Report the result. Today this prints the "disabled" note and exits 0. To enable:
1. Add `.github/workflows/azure-deploy.yml` mirroring StreetSamurai's pattern (push-to-main trigger).
2. Provision a `tutor` Azure App Service.
3. Add `AZURE_WEBAPP_PUBLISH_PROFILE` secret to `mindattic/Tutor`.
4. Flip `apps[].disabled` from `true` to `false` in `MindAttic.Deploy/projects.json`.

Notes:
- The legacy `scripts/cli/deploy.{bat,ps1}` + `build-html.js` in this repo only deployed the FTP **landing page** (`mindattic.com/tutor.htm`) -- not the Blazor app. The landing page is now deployed centrally from MindAttic.Deploy (`npm run deploy -- --only tutor`); this `/deploy` command is for the APP.
