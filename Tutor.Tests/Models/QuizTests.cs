using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class QuizTests
{
    [Test]
    public void QuizResult_ScorePercentage_CalculatesCorrectly()
    {
        var result = new QuizResult
        {
            TotalQuestions = 10,
            CorrectAnswers = 7
        };
        Assert.That(result.ScorePercentage, Is.EqualTo(70.0));
    }

    [Test]
    public void QuizResult_ScorePercentage_ZeroQuestions_ReturnsZero()
    {
        var result = new QuizResult { TotalQuestions = 0 };
        Assert.That(result.ScorePercentage, Is.EqualTo(0.0));
    }

    [Test]
    public void QuizResult_Passed_AtThreshold()
    {
        var result = new QuizResult { TotalQuestions = 10, CorrectAnswers = 7 };
        Assert.That(result.Passed, Is.True);
    }

    [Test]
    public void QuizResult_NotPassed_BelowThreshold()
    {
        var result = new QuizResult { TotalQuestions = 10, CorrectAnswers = 6 };
        Assert.That(result.Passed, Is.False);
    }

    [Test]
    public void QuizSession_CurrentQuestion_ReturnsCorrectQuestion()
    {
        var q1 = new QuizQuestion { Id = "q1", QuestionText = "What is 2+2?" };
        var q2 = new QuizQuestion { Id = "q2", QuestionText = "What is 3+3?" };
        var session = new QuizSession
        {
            Questions = [q1, q2],
            CurrentQuestionIndex = 0
        };

        Assert.That(session.CurrentQuestion?.Id, Is.EqualTo("q1"));
    }

    [Test]
    public void QuizSession_CurrentQuestion_NullWhenComplete()
    {
        var session = new QuizSession
        {
            Questions = [new QuizQuestion()],
            CurrentQuestionIndex = 1
        };

        Assert.That(session.CurrentQuestion, Is.Null);
    }

    [Test]
    public void QuizSession_IsComplete_TrueWhenAllAnswered()
    {
        var session = new QuizSession
        {
            Questions = [new QuizQuestion(), new QuizQuestion()],
            CurrentQuestionIndex = 2
        };
        Assert.That(session.IsComplete, Is.True);
    }

    [Test]
    public void QuizSession_IsComplete_FalseWhenInProgress()
    {
        var session = new QuizSession
        {
            Questions = [new QuizQuestion(), new QuizQuestion()],
            CurrentQuestionIndex = 1
        };
        Assert.That(session.IsComplete, Is.False);
    }

    [Test]
    public void QuizSession_ToResult_ConvertsCorrectly()
    {
        var q1 = new QuizQuestion { Id = "q1", Points = 10 };
        var q2 = new QuizQuestion { Id = "q2", Points = 5 };
        var a1 = new QuizAnswer { QuestionId = "q1", IsCorrect = true };
        var a2 = new QuizAnswer { QuestionId = "q2", IsCorrect = false };

        var session = new QuizSession
        {
            UserId = "user1",
            CourseId = "course1",
            SectionIds = ["s1"],
            Questions = [q1, q2],
            Answers = [a1, a2]
        };

        var result = session.ToResult();
        Assert.That(result.UserId, Is.EqualTo("user1"));
        Assert.That(result.CourseId, Is.EqualTo("course1"));
        Assert.That(result.TotalQuestions, Is.EqualTo(2));
        Assert.That(result.CorrectAnswers, Is.EqualTo(1));
        Assert.That(result.TotalPoints, Is.EqualTo(15));
        Assert.That(result.EarnedPoints, Is.EqualTo(10));
    }

    [Test]
    public void QuizQuestion_DefaultValues()
    {
        var q = new QuizQuestion();
        Assert.That(q.Difficulty, Is.EqualTo(1));
        Assert.That(q.Points, Is.EqualTo(1));
        Assert.That(q.RelatedSectionIds, Is.Empty);
        Assert.That(q.RelatedConceptIds, Is.Empty);
    }
}
