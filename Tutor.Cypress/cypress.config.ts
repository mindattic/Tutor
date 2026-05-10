import { defineConfig } from 'cypress';

// Tutor.Blazor's launchSettings.json maps to https://localhost:7200 (HTTPS) and
// http://localhost:5200 (HTTP). HTTP is fine for local dev and avoids the dev
// cert dance, so we default there. Override with CYPRESS_BASE_URL if needed.
const baseUrl = process.env.CYPRESS_BASE_URL ?? 'http://localhost:5200';

export default defineConfig({
  e2e: {
    baseUrl,
    specPattern: 'e2e/**/*.cy.ts',
    supportFile: 'support/e2e.ts',
    viewportWidth: 1280,
    viewportHeight: 800,
    video: false,
    screenshotOnRunFailure: true,
    defaultCommandTimeout: 10_000,
    requestTimeout: 15_000,
    // Blazor Server pushes UI updates over a SignalR WebSocket. The first paint
    // can be slower than a typical SSR app on a cold start; bumping the page-
    // load timeout keeps CI stable.
    pageLoadTimeout: 60_000,
    setupNodeEvents(on, config) {
      return config;
    }
  }
});
