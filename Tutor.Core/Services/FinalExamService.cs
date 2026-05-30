using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Orchestrates the course-wide final exam: eligibility, starting a comprehensive assessment
/// that spans every section/concept, and recording course completion on a pass.
///
/// <para>Reuses the existing <see cref="QuizService"/> lifecycle — the final-exam UI drives
/// answer submission through <see cref="QuizService"/> exactly like a section quiz. The only
/// differences are breadth (all concepts), the higher <see cref="PassingScore"/>, and the
/// <see cref="QuizResult.IsFinalExam"/> flag, which this service stamps onto the session.</para>
/// </summary>
public sealed class FinalExamService
{
    private readonly QuizService quizService;
    private readonly LearningPathService learningPath;

    /// <summary>Score (0-100) required to pass the final exam — a higher bar than a section quiz.</summary>
    public const int PassingScore = 80;

    /// <summary>Target number of questions for the comprehensive exam.</summary>
    public const int QuestionCount = 20;

    public FinalExamService(QuizService quizService, LearningPathService learningPath)
    {
        this.quizService = quizService;
        this.learningPath = learningPath;
    }

    /// <summary>True when every lesson in the course is complete — the gate to sit the exam.</summary>
    public bool IsEligible(CourseStructure structure, UserProgress progress)
        => learningPath.IsExamEligible(structure, progress);

    /// <summary>
    /// Starts the final exam: a comprehensive quiz drawn from every section and concept in the
    /// course. Returns null if no questions could be sourced. The returned session is flagged as
    /// the final exam with the higher passing score, so the resulting <see cref="QuizResult"/>
    /// carries both through <see cref="QuizSession.ToResult"/>.
    /// </summary>
    public async Task<QuizSession?> StartAsync(CourseStructure structure)
    {
        var sectionIds = structure.GetLessonsInOrder()
            .SelectMany(l => l.GetAllSectionsFlattened())
            .Select(s => s.Id)
            .Distinct()
            .ToList();
        var conceptIds = structure.GetAllConceptIdsInOrder().Distinct().ToList();

        var session = await quizService.StartQuizAsync(structure.CourseId, sectionIds, conceptIds, QuestionCount);
        if (session != null)
        {
            session.IsFinalExam = true;
            session.PassingScore = PassingScore;
        }
        return session;
    }

    /// <summary>
    /// Applies a completed final-exam result to the student's progress: on a pass, records the
    /// course as completed. Returns true when this result completed the course (used to trigger
    /// certificate issuance). No-op for section-quiz or failing results.
    /// </summary>
    public bool ApplyResult(UserProgress progress, QuizResult result)
    {
        if (!result.IsFinalExam || !result.Passed) return false;
        progress.FinalExamResultId = result.Id;
        progress.HasCompletedCourse = true;
        progress.CourseCompletedAt = DateTime.UtcNow;
        return true;
    }
}
