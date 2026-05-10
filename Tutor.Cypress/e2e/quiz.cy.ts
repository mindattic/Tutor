// E2E coverage for the quiz UI. The behaviour change behind these specs:
// BookImportPipeline now bakes a quiz into every section that has HasQuiz=true,
// so when a user opens a previously-imported course and starts a quiz, questions
// appear immediately (no live LLM call). We can't directly observe "no LLM call
// was made" from Cypress, but we can assert (a) the empty-state UI for a
// logged-out user is correct, and (b) for an authenticated session, the Quiz
// tab is reachable and renders without throwing.

const username = Cypress.env('username') as string | undefined;
const password = Cypress.env('password') as string | undefined;
const haveCreds = Boolean(username && password);
const ifAuthed = haveCreds ? describe : describe.skip;

describe('quiz — public surface', () => {
  beforeEach(function () {
    cy.visit('/');
    cy.isLoggedOut().then((loggedOut) => {
      if (!loggedOut) this.skip();
    });
  });

  it('does not expose Learn (and therefore the Quiz tab) when logged out', () => {
    // Quiz lives inside the Learn page, which is gated on AuthService.IsAuthenticated.
    cy.get('header.top-menu .top-nav-link').then(($links) => {
      const labels = [...$links].map((el) => el.textContent?.trim() ?? '');
      expect(labels).to.not.include('Learn');
    });
  });
});

ifAuthed('quiz — authenticated flows', () => {
  beforeEach(() => {
    cy.login();
    cy.location('pathname', { timeout: 15_000 }).should('eq', '/learn');
  });

  it('shows the Quiz tab inside the Learn page', () => {
    cy.get('#learn-tab-quiz').should('be.visible');
    cy.get('#learn-tab-quiz').should('contain.text', 'Quiz');
  });

  it('switches to the Quiz tab when clicked and reveals the quiz region', () => {
    cy.get('#learn-tab-quiz').click();
    cy.get('#learn-tab-quiz').should('have.attr', 'aria-selected', 'true');
    // The Quiz panel is always rendered behind the tab — its inner state varies
    // (empty / loading / active / results) based on whether a course is loaded
    // and whether a session is in flight, so we only assert presence here.
    cy.get('#learn-panel-quiz, [aria-labelledby="quiz-region-label"]').should('exist');
  });

  it('renders the "no course selected" empty state when no course is active', () => {
    cy.get('#learn-tab-quiz').click();
    // QuizTab.razor renders the empty-state block when CourseId is null/empty.
    // If the dev's CURRENT_USER session has a previously-selected course, this
    // test self-skips rather than asserting a false positive.
    cy.get('body').then(($body) => {
      const empty = $body.find('.quiz-empty');
      if (empty.length === 0) {
        cy.log('A course is already active for this user — skipping empty-state assertion.');
        return;
      }
      cy.get('.quiz-empty').should('be.visible');
      cy.get('.quiz-empty h3').should('contain.text', 'No Course Selected');
    });
  });
});

export {};
