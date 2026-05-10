import './commands';

// Blazor Server reports SignalR reconnect attempts as console errors during
// page-load races. They aren't actionable for E2E assertions and would make
// every spec brittle, so swallow uncaught exceptions and let the test
// determine pass/fail from real DOM assertions.
Cypress.on('uncaught:exception', () => false);
