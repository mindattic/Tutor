# Tutor.Cypress

End-to-end UI tests for the **Tutor.Blazor** app, run with Cypress.

## Prerequisites

- Node.js 18+
- Tutor.Blazor running locally: `dotnet run --project ../Tutor.Blazor`
  Confirm it is reachable at `http://localhost:5200` before running tests.

## Install

```powershell
cd Tutor.Cypress
npm install
```

## Run

```powershell
npm run cypress:open   # interactive runner (development)
npm run cypress:run    # headless (CI)
```

Override the base URL:

```powershell
$env:CYPRESS_BASE_URL = "https://localhost:7200"
npm run cypress:run
```

## Authenticated specs

Specs that hit auth-guarded pages use the `cy.login()` custom command. Provide credentials via env vars:

```powershell
$env:CYPRESS_username = "test-user"
$env:CYPRESS_password = "test-password"
npm run cypress:run
```

If credentials are not set, authenticated specs skip their post-login assertions. Smoke specs that only check the unauthenticated landing page still run.

## Auth state caveat

`AuthenticationService` is a singleton that seeds itself from `%LOCALAPPDATA%\Tutor\Settings\secure-preferences.json` on first `AuthGuard` navigation. If you (or any prior session on the same machine) have logged in, the app renders the welcome card instead of the login form.

Suites that test logged-out state (`cy.isLoggedOut()` check) self-skip when a session is active. To force a clean state:

```powershell
npm run reset-auth
# Then restart the app:
cd ..
dotnet run --project Tutor.Blazor --launch-profile http
cd Tutor.Cypress
npm run cypress:run
```

The `reset-auth` script strips `CURRENT_USER` from `secure-preferences.json` and stops the running Tutor.Blazor process. Clearing the file alone is not enough — the singleton's in-memory state survives until the process restarts.

## Layout

```
Tutor.Cypress/
├── cypress.config.ts     base URL, viewport, timeouts
├── tsconfig.json
├── package.json
├── support/
│   ├── e2e.ts            global setup
│   └── commands.ts       custom commands (cy.login, cy.visitInteractive, cy.isLoggedOut)
└── e2e/
    ├── smoke.cy.ts         app loads, brand link, login form visible
    ├── auth.cy.ts          auth-guarded routes redirect / hide nav
    ├── courses.cy.ts       courses + learn flows (requires login)
    └── concept-graph.cy.ts concept map / knowledge graph route
```
