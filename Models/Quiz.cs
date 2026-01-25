namespace Tutor.Models;

/// <summary>
/// A quiz question generated based on learned material.
/// </summary>
public class QuizQuestion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>The question text to display</summary>
    public string QuestionText { get; set; } = "";
    
    /// <summary>The correct answer</summary>
    public string CorrectAnswer { get; set; } = "";
    
    /// <summary>Explanation shown after answering</summary>
    public string? Explanation { get; set; }
    
    /// <summary>Related section IDs this question covers</summary>
    public List<string> RelatedSectionIds { get; set; } = [];
    
    /// <summary>Related concept IDs this question covers</summary>
    public List<string> RelatedConceptIds { get; set; } = [];
    
    /// <summary>Difficulty level 1-5</summary>
    public int Difficulty { get; set; } = 1;
    
    /// <summary>Points awarded for correct answer</summary>
    public int Points { get; set; } = 1;
}

/// <summary>
/// A user's answer to a quiz question.
/// </summary>
public class QuizAnswer
{
    public string QuestionId { get; set; } = "";
    public string UserAnswer { get; set; } = "";
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Result of a completed quiz session.
/// </summary>
public class QuizResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string CourseId { get; set; } = "";
    
    /// <summary>Section IDs this quiz covered</summary>
    public List<string> SectionIds { get; set; } = [];
    
    public List<QuizAnswer> Answers { get; set; } = [];
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalPoints { get; set; }
    public int EarnedPoints { get; set; }
    public double ScorePercentage => TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;
    public bool Passed => ScorePercentage >= 70;
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
}

/// <summary>
/// An active quiz session being taken by a user.
/// </summary>
public class QuizSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string CourseId { get; set; } = "";
    public List<string> SectionIds { get; set; } = [];
    public List<QuizQuestion> Questions { get; set; } = [];
    public List<QuizAnswer> Answers { get; set; } = [];
    public int CurrentQuestionIndex { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public QuizQuestion? CurrentQuestion => 
        CurrentQuestionIndex < Questions.Count ? Questions[CurrentQuestionIndex] : null;
    
    public bool IsComplete => CurrentQuestionIndex >= Questions.Count;
    
    public QuizResult ToResult() => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserId = UserId,
        CourseId = CourseId,
        SectionIds = SectionIds,
        Answers = Answers,
        TotalQuestions = Questions.Count,
        CorrectAnswers = Answers.Count(a => a.IsCorrect),
        TotalPoints = Questions.Sum(q => q.Points),
        EarnedPoints = Answers.Where(a => a.IsCorrect).Sum(a => 
            Questions.FirstOrDefault(q => q.Id == a.QuestionId)?.Points ?? 0),
        StartedAt = StartedAt,
        CompletedAt = DateTime.UtcNow
    };
}
