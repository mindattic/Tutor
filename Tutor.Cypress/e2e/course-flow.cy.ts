// E2E coverage for the learning-experience flow: mastery-gated lessons, the Final
// Exam tab, and the certificate surface inside the Learn page.
//
// Scope note: the learning loop is LLM-driven (the Chat teaches and quizzes are
// graded by the model), so a *fully automated* in-browser run that answers Moby
// Dick questions correctly and reaches the certificate is non-deterministic and
// slow/paid — not something Cypress can assert reliably. The deterministic
// end-to-end of the lifecycle logic (lessons unlock → final exam → certificate →
// unload) lives in Tutor.Tests/Services/FullCourseLifecycleTests. These specs
// therefore assert the UI is correctly wired and every state renders without
// throwing, matching the structural approach used by quiz.cy.ts.

const username = Cypress.env('username') as string | undefined;
const password = Cypress.env('password') as string | undefined;
const haveCreds = Boolean(username && password);
const ifAuthed = haveCreds ? describe : describe.skip;

describe('course flow — public surface', () => {
  beforeEach(function () {
    cy.visit('/');
    cy.isLoggedOut().then((loggedOut) => {
      if (!loggedOut) this.skip();
    });
  });

  it('does not expose Learn (and therefore the Final Exam tab) when logged out', () => {
    cy.get('header.top-menu .top-nav-link').then(($links) => {
      const labels = [...$links].map((el) => el.textContent?.trim() ?? '');
      expect(labels).to.not.include('Learn');
    });
  });
});

ifAuthed('course flow — authenticated', () => {
  beforeEach(() => {
    cy.login();
    cy.location('pathname', { timeout: 15_000 }).should('eq', '/learn');
  });

  it('shows the Final Exam tab inside the Learn page', () => {
    cy.get('#learn-tab-finalexam').should('be.visible');
    cy.get('#learn-tab-finalexam').should('contain.text', 'Final Exam');
  });

  it('switches to the Final Exam tab and reveals its panel without throwing', () => {
    cy.get('#learn-tab-finalexam').click();
    cy.get('#learn-tab-finalexam').should('have.attr', 'aria-selected', 'true');
    cy.get('#learn-panel-finalexam, [aria-labelledby="final-exam-heading"]').should('exist');
  });

  it('renders a coherent final-exam state (locked / start / no-course)', () => {
    cy.get('#learn-tab-finalexam').click();
    // The panel shows exactly one of: no-course empty state, "Final Exam Locked"
    // (course loaded but not yet mastered), or the start screen (mastered). Any of
    // these is valid depending on the dev session's active course + progress.
    cy.get('[aria-labelledby="final-exam-heading"]').within(() => {
      cy.get('h2, h3').should('exist');
    });
  });

  it('marks locked lessons in the side nav as non-navigable when present', () => {
    // Mastery gating renders locked lessons with the .locked class. If the active
    // course has no locked lessons (none selected, or all mastered), self-skip.
    cy.get('body').then(($body) => {
      const locked = $body.find('.side-nav-item.locked');
      if (locked.length === 0) {
        cy.log('No locked lessons in the current session — skipping gating assertion.');
        return;
      }
      cy.get('.side-nav-item.locked').first().should('have.attr', 'aria-disabled', 'true');
    });
  });
});

export {};
