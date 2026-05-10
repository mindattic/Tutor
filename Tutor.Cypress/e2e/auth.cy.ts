describe('auth — guarded routes', () => {
  // The dev's prior login may persist in the running app — each test
  // self-skips if the login form isn't on screen.
  beforeEach(function () {
    cy.visit('/');
    cy.isLoggedOut().then((loggedOut) => {
      if (!loggedOut) this.skip();
    });
  });

  // AuthGuard redirects unauthenticated visitors back to "/".
  const guarded = ['/courses', '/learn', '/settings'];

  guarded.forEach((path) => {
    it(`${path} redirects unauthenticated visitors to /`, () => {
      cy.visit(path);
      cy.location('pathname', { timeout: 15_000 }).should('eq', '/');
      cy.contains('h1', /Welcome to Tutor/i).should('be.visible');
    });
  });

  it('login form requires both fields', () => {
    // Browser-native `required` halts submission when password is blank — the
    // page stays at "/" and the password input is in the :invalid state.
    cy.get('input#username').type('only-username');
    cy.contains('button', /^Login$/).click();
    cy.location('pathname').should('eq', '/');
    cy.get('input#password:invalid').should('exist');
  });

  it('login form clears the password after a failed attempt', () => {
    cy.get('input#username').type('does-not-exist');
    cy.get('input#password').type('does-not-exist');
    cy.contains('button', /^Login$/).click();
    cy.get('[role="alert"]', { timeout: 15_000 })
      .invoke('text')
      .should('match', /failed|invalid|incorrect|error|not found/i);
    cy.get('input#password').should('have.value', '');
  });
});

export {};
