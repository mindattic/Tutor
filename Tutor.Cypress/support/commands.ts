/// <reference types="cypress" />

declare global {
  namespace Cypress {
    interface Chainable {
      /**
       * Logs in via the home-page form. Reads credentials from
       * `Cypress.env('username')` / `Cypress.env('password')` if not provided
       * — set those when running CI against a seeded test user.
       */
      login(username?: string, password?: string): Chainable<void>;

      /**
       * Visits the URL and waits for the Blazor SignalR connection to be
       * "Connected". Use this when a spec needs interactive features.
       */
      visitInteractive(url: string): Chainable<void>;

      /**
       * Returns true if the home page is currently rendering the login form
       * (i.e., AuthService.IsAuthenticated is false). Tutor.Blazor's
       * AuthenticationService is a singleton with in-memory state seeded from
       * %LOCALAPPDATA%/Tutor/Settings/secure-preferences.json at first
       * AuthGuard navigation, so the dev's prior login may still be active
       * when Cypress connects. Use this to gate auth-form specs.
       */
      isLoggedOut(): Chainable<boolean>;
    }
  }
}

Cypress.Commands.add('login', (username?: string, password?: string) => {
  const u = username ?? (Cypress.env('username') as string | undefined);
  const p = password ?? (Cypress.env('password') as string | undefined);
  if (!u || !p) {
    throw new Error(
      'login() requires CYPRESS_username and CYPRESS_password env vars (or explicit args).'
    );
  }
  cy.visit('/');
  cy.get('input#username').clear().type(u);
  cy.get('input#password').clear().type(p, { log: false });
  cy.contains('button', /^Login$/).click();
});

Cypress.Commands.add('visitInteractive', (url: string) => {
  cy.visit(url);
  // Blazor injects this attribute on the document body once the server-side
  // circuit is established. Waiting for it removes the most common race
  // (clicks landing before SignalR is ready and being silently dropped).
  cy.get('body[blazor\\:reconnect-state="off"]', { timeout: 30_000 }).should('exist');
});

Cypress.Commands.add('isLoggedOut', () => {
  return cy.get('body', { timeout: 10_000 }).then(($body) => {
    return $body.find('input#username').length > 0;
  }) as unknown as Cypress.Chainable<boolean>;
});

export {};
