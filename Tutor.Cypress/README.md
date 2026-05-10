# Tutor.Cypress

End-to-end UI tests for the **Tutor.Blazor** app, run with [Cypress](https://www.cypress.io/).

## Prerequisites

- Node.js 18+ (Cypress 13.x)
- The Tutor.Blazor app running locally — start it with `dotnet run --project ../Tutor.Blazor` and confirm it's reachable at `http://localhost:5200`.

## Install

```powershell
cd Tutor.Cypress
npm install
```

## Run the tests

Open the interactive runner (good for development):

```powershell
npm run cypress:open
```

Run all specs headlessly (good for CI):

```powershell
npm run cypress:run
```

## Configuration

`cypress.config.ts` defaults the base URL to `http://localhost:5200`. Override with an env var:

```powershell
$env:CYPRESS_BASE_URL = "https://localhost:7200"
npm run cypress:run
```

## Authenticated specs

Specs that hit pages behind `AuthGuard` use the `cy.login()` custom command. Provide credentials via env vars:

```powershell
$env:CYPRESS_username = "your-test-user"
$env:CYPRESS_password = "your-test-password"
npm run cypress:run
```

If `CYPRESS_username` / `CYPRESS_password` are not set, authenticated specs will log a warning and skip the post-login assertions; smoke specs that only check the unauthenticated landing page still run.

## Auth state caveat

`AuthenticationService` is a singleton that holds `currentUser` in memory and seeds itself from `%LOCALAPPDATA%\Tutor\Settings\secure-preferences.json` on first AuthGuard navigation. That file persists the developer's login across server restarts, so when you run Cypress against a Blazor app that you (or any prior session on the same machine) have logged into, the home page renders the welcome card instead of the login form.

**Effect on the specs.** The "logged-out landing", "auth — guarded routes", and "concept graph route — unauthenticated" suites all `beforeEach` self-skip via `cy.isLoggedOut()` — they only run when no session is active. App-shell assertions (`smoke — app shell`) always run regardless.

**To force a clean state**, run:

```powershell
npm run reset-auth
```

That script (a) strips the `CURRENT_USER` key from `%LOCALAPPDATA%\Tutor\Settings\secure-preferences.json` and (b) stops any running `Tutor.Blazor` process. Restart the app and re-run Cypress:

```powershell
cd ..
dotnet run --project Tutor.Blazor --launch-profile http
cd Tutor.Cypress
npm run cypress:run
```

Clearing the file alone is not enough — the singleton's in-memory state survives until the process restarts.

## Layout

```
Tutor.Cypress/
├── cypress.config.ts     # base URL, viewport, timeouts
├── tsconfig.json
├── package.json
├── support/
│   ├── e2e.ts            # global setup
│   └── commands.ts       # custom commands (cy.login, cy.visitInteractive)
└── e2e/
    ├── smoke.cy.ts        # app loads, brand link, login form visible
    ├── auth.cy.ts         # auth-guarded routes redirect/hide nav
    ├── courses.cy.ts      # courses + learn flows (requires login)
    └── concept-graph.cy.ts # concept map / knowledge graph route
```

This is starter scaffolding — expand each spec as features stabilize.
