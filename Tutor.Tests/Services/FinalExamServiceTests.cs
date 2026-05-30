using NUnit.Framework;
using Tutor.Core.Models;
using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class FinalExamServiceTests
{
    // ApplyResult only touches the progress + result; the deps aren't exercised here.
    private static FinalExamService Sut() => new(null!, null!);

    [Test]
    public void ApplyResult_PassingFinalExam_CompletesCourse()
    {
        var p = new UserProgress();
        var r = new QuizResult
        {
            Id = "exam-1", IsFinalExam = true, PassingScore = FinalExamService.PassingScore,
            TotalQuestions = 10, CorrectAnswers = 9, // 90%
        };

        Assert.That(Sut().ApplyResult(p, r), Is.True);
        Assert.That(p.HasCompletedCourse, Is.True);
        Assert.That(p.FinalExamResultId, Is.EqualTo("exam-1"));
        Assert.That(p.CourseCompletedAt, Is.Not.Null);
    }

    [Test]
    public void ApplyResult_FailingFinalExam_DoesNotComplete()
    {
        var p = new UserProgress();
        var r = new QuizResult
        {
            IsFinalExam = true, PassingScore = FinalExamService.PassingScore,
            TotalQuestions = 10, CorrectAnswers = 7, // 70% < 80
        };

        Assert.That(Sut().ApplyResult(p, r), Is.False);
        Assert.That(p.HasCompletedCourse, Is.False);
        Assert.That(p.FinalExamResultId, Is.Null);
    }

    [Test]
    public void ApplyResult_SectionQuiz_IsIgnored()
    {
        var p = new UserProgress();
        var r = new QuizResult { IsFinalExam = false, TotalQuestions = 10, CorrectAnswers = 10 };

        Assert.That(Sut().ApplyResult(p, r), Is.False);
        Assert.That(p.HasCompletedCourse, Is.False);
    }

    [Test]
    public void QuizResult_PassingScore_GatesPassed()
    {
        // 75% passes a section quiz (70) but not the final exam (80).
        var section = new QuizResult { TotalQuestions = 4, CorrectAnswers = 3 };
        Assert.That(section.Passed, Is.True);

        var exam = new QuizResult { PassingScore = 80, TotalQuestions = 4, CorrectAnswers = 3 };
        Assert.That(exam.Passed, Is.False);
    }
}
