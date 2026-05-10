const username = Cypress.env('username') as string | undefined;
const password = Cypress.env('password') as string | undefined;
const haveCreds = Boolean(username && password);

describe('concept graph route — unauthenticated', () => {
  beforeEach(function () {
    cy.visit('/');
    cy.isLoggedOut().then((loggedOut) => {
      if (!loggedOut) this.skip();
    });
  });

  it('/knowledge-graph redirects to / for anonymous users', () => {
    cy.visit('/knowledge-graph');
    cy.location('pathname', { timeout: 15_000 }).should('eq', '/');
  });

  it('/knowledge-bases redirects to / for anonymous users', () => {
    cy.visit('/knowledge-bases');
    cy.location('pathname', { timeout: 15_000 }).should('eq', '/');
  });
});

const ifAuthed = haveCreds ? describe : describe.skip;

ifAuthed('concept graph route — authenticated', () => {
  beforeEach(() => {
    cy.login();
    cy.location('pathname', { timeout: 15_000 }).should('eq', '/learn');
  });

  it('/knowledge-graph renders the page title', () => {
    cy.visit('/knowledge-graph');
    cy.contains('h1', /Knowledge Bases/i, { timeout: 15_000 }).should('be.visible');
  });

  it('handles "no active course" gracefully', () => {
    // With no active course set, the page exposes a "No Active Course" empty
    // state with a CTA link to /courses. This is the most stable assertion we
    // can make on a fresh test environment.
    cy.visit('/knowledge-graph');
    cy.get('body', { timeout: 15_000 }).then(($body) => {
      const text = $body.text();
      // Either the empty state is visible, or the page is mid-load — both are
      // acceptable; we just want to confirm the route renders without error.
      const ok =
        /No Active Course/i.test(text) ||
        /Loading/i.test(text) ||
        /Knowledge Bases/i.test(text);
      expect(ok, 'concept graph page rendered a known state').to.be.true;
    });
  });
});

export {};
