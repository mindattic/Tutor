describe('smoke — app shell', () => {
  // The shell is visible regardless of auth state, so these assertions don't
  // need an explicit logout.
  beforeEach(() => {
    cy.visit('/');
  });

  it('renders the top header and brand', () => {
    cy.get('header.top-menu').should('be.visible');
    cy.contains('header.top-menu .brand-text', 'Tutor').should('be.visible');
  });

  it('Home nav link routes to /', () => {
    cy.contains('header.top-menu .top-nav-link', 'Home').click();
    cy.location('pathname').should('eq', '/');
  });
});

describe('smoke — logged-out landing', () => {
  // The dev's prior login may persist in the running app (in-memory singleton
  // seeded from %LOCALAPPDATA%/Tutor). Each test self-skips if the login form
  // isn't on screen.
  beforeEach(function () {
    cy.visit('/');
    cy.isLoggedOut().then((loggedOut) => {
      if (!loggedOut) this.skip();
    });
  });

  it('shows the welcome heading and login form', () => {
    cy.contains('h1', /Welcome to Tutor/i).should('be.visible');
    cy.get('input#username').should('be.visible');
    cy.get('input#password').should('be.visible');
    cy.contains('button', /^Login$/).should('be.enabled');
  });

  it('hides authenticated nav items', () => {
    cy.get('header.top-menu .top-nav-link').then(($links) => {
      const labels = [...$links].map((el) => el.textContent?.trim() ?? '');
      // Courses / Learn / Settings are gated on AuthService.IsAuthenticated.
      expect(labels).to.not.include.members(['Courses', 'Learn', 'Settings']);
    });
  });

  it('blocks login submission with empty credentials via native validation', () => {
    // The form has `required` attributes on both inputs, so the browser halts
    // submission before C# server-side validation runs. The observable
    // outcome is that the page stays at "/" and the input is :invalid.
    cy.contains('button', /^Login$/).click();
    cy.location('pathname').should('eq', '/');
    cy.get('input#username:invalid').should('exist');
  });

  it('shows an error for an obviously bad credential pair', () => {
    cy.get('input#username').type('does-not-exist');
    cy.get('input#password').type('does-not-exist');
    cy.contains('button', /^Login$/).click();
    // Wait for the error text itself rather than visibility — the alert div is
    // present but empty (height 0) until LoginAsync completes and sets the
    // error message.
    cy.get('[role="alert"]', { timeout: 15_000 })
      .invoke('text')
      .should('match', /failed|invalid|incorrect|error|not found/i);
  });
});

export {};
