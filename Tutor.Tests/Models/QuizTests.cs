using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class QuizTests
{
    [Fact]
    public void QuizResult_ScorePercentage_CalculatesCorrectly()
    {
        var result = new QuizResult
        {
            TotalQuestions = 10,
            CorrectAnswers = 7
        };
        Assert.Equal(70.0, result.ScorePercentage);
    }

    [Fact]
    public void QuizResult_ScorePercentage_ZeroQuestions_ReturnsZero()
    {
        var result = new QuizResult { TotalQuestions = 0 };
        Assert.Equal(0.0, result.ScorePercentage);
    }

    [Fact]
    public void QuizResult_Passed_AtThreshold()
    {
        var result = new QuizResult { TotalQuestions = 10, CorrectAnswers = 7 };
        Assert.True(result.Passed);
    }

    [Fact]
    public void QuizResult_NotPassed_BelowThreshold()
    {
        var result = new QuizResult { TotalQuestions = 10, CorrectAnswers = 6 };
        Assert.False(result.Passed);
    }

    [Fact]
    public void QuizSession_CurrentQuestion_ReturnsCorrectQuestion()
    {
        var q1 = new QuizQuestion { Id = "q1", QuestionText = "What is 2+2?" };
        var q2 = new QuizQuestion { Id = "q2", QuestionText = "What is 3+3?" };
        var session = new QuizSession
        {
            Questions = [q1, q2],
            CurrentQuestionIndex = 0
        };

        Assert.Equal("q1", session.CurrentQuestion?.Id);
    }

    [Fact]
    public void QuizSession_CurrentQuestion_NullWhenComplete()
    {
        var session = new QuizSession
        {
            Questions = [new QuizQuestion()],
            CurrentQuestionIndex = 1
        };

        Assert.Null(session.CurrentQuestion);
    }

    [Fact]
    public void QuizSession_IsComplete_TrueWhenAllAnswered()
    {
        var session = new QuizSession
        {
            Questions = [new QuizQuestion(), new QuizQuestion()],
            CurrentQuestionIndex = 2
        };
        Assert.True(session.IsComplete);
    }

    [Fact]
    public void QuizSession_IsComplete_FalseWhenInProgress()
    {
        var session = new QuizSession
        {
            Questions = [new QuizQuestion(), new QuizQuestion()],
            CurrentQuestionIndex = 1
        };
        Assert.False(session.IsComplete);
    }

    [Fact]
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
        Assert.Equal("user1", result.UserId);
        Assert.Equal("course1", result.CourseId);
        Assert.Equal(2, result.TotalQuestions);
        Assert.Equal(1, result.CorrectAnswers);
        Assert.Equal(15, result.TotalPoints);
        Assert.Equal(10, result.EarnedPoints);
    }

    [Fact]
    public void QuizQuestion_DefaultValues()
    {
        var q = new QuizQuestion();
        Assert.Equal(1, q.Difficulty);
        Assert.Equal(1, q.Points);
        Assert.Empty(q.RelatedSectionIds);
        Assert.Empty(q.RelatedConceptIds);
    }
}
