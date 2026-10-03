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
- There is no landing page to deploy, and MindAttic.Deploy rejects `--only tutor`. This repo's README on GitHub (https://github.com/mindattic/Tutor) is the project page. This `/deploy` command is for the APP only.
