// These specs need a real authenticated session because the routes are behind
// AuthGuard. Provide credentials via CYPRESS_username / CYPRESS_password and
// they will run; otherwise they are skipped so CI doesn't fail on a fresh
// checkout where no test user is seeded.
const username = Cypress.env('username') as string | undefined;
const password = Cypress.env('password') as string | undefined;
const haveCreds = Boolean(username && password);
const ifAuthed = haveCreds ? describe : describe.skip;

ifAuthed('courses — authenticated flows', () => {
  beforeEach(() => {
    cy.login();
    // login() lands on /learn after a successful submit.
    cy.location('pathname', { timeout: 15_000 }).should('eq', '/learn');
  });

  it('exposes Courses and Learn nav links once logged in', () => {
    cy.contains('header.top-menu .top-nav-link', 'Courses').should('be.visible');
    cy.contains('header.top-menu .top-nav-link', 'Learn').should('be.visible');
  });

  it('navigates to /courses', () => {
    cy.contains('header.top-menu .top-nav-link', 'Courses').click();
    cy.location('pathname').should('eq', '/courses');
  });

  it('navigates to /learn', () => {
    cy.contains('header.top-menu .top-nav-link', 'Courses').click();
    cy.contains('header.top-menu .top-nav-link', 'Learn').click();
    cy.location('pathname').should('eq', '/learn');
  });
});

export {};
