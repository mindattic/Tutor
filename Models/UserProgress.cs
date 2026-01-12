namespace Tutor.Models;

/// <summary>
/// Tracks a user's learning progress for a specific course.
/// </summary>
public class UserProgress
{
    /// <summary>
    /// Unique identifier for this progress record.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The ID of the user this progress belongs to.
    /// </summary>
    public string UserId { get; set; } = "";

    /// <summary>
    /// The ID of the course this progress is for.
    /// </summary>
    public string CourseId { get; set; } = "";

    /// <summary>
    /// The ID of the current chapter the user is on.
    /// </summary>
    public string? CurrentChapterId { get; set; }

    /// <summary>
    /// The ID of the current concept the user is on.
    /// </summary>
    public string? CurrentConceptId { get; set; }

    /// <summary>
    /// IDs of concepts the user has learned (marked as understood).
    /// </summary>
    public List<string> LearnedConceptIds { get; set; } = [];

    /// <summary>
    /// IDs of concepts the user has visited but not completed.
    /// </summary>
    public List<string> VisitedConceptIds { get; set; } = [];

    /// <summary>
    /// Progress through each chapter (chapter ID to completion percentage).
    /// </summary>
    public Dictionary<string, int> ChapterProgress { get; set; } = [];

    /// <summary>
    /// When this progress record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this progress was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Total time spent learning in this course (in minutes).
    /// </summary>
    public int TotalMinutesSpent { get; set; }

    /// <summary>
    /// The last time the user was active in this course.
    /// </summary>
    public DateTime? LastActiveAt { get; set; }

    /// <summary>
    /// Calculates overall course completion percentage.
    /// </summary>
    public int GetOverallProgress(int totalConcepts)
    {
        if (totalConcepts == 0) return 0;
        return (int)((double)LearnedConceptIds.Count / totalConcepts * 100);
    }

    /// <summary>
    /// Marks a concept as learned.
    /// </summary>
    public void MarkConceptLearned(string conceptId)
    {
        if (!LearnedConceptIds.Contains(conceptId))
        {
            LearnedConceptIds.Add(conceptId);
        }
        if (!VisitedConceptIds.Contains(conceptId))
        {
            VisitedConceptIds.Add(conceptId);
        }
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks a concept as visited (but not necessarily learned).
    /// </summary>
    public void MarkConceptVisited(string conceptId)
    {
        if (!VisitedConceptIds.Contains(conceptId))
        {
            VisitedConceptIds.Add(conceptId);
        }
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the current position in the course.
    /// </summary>
    public void SetPosition(string? chapterId, string? conceptId)
    {
        CurrentChapterId = chapterId;
        CurrentConceptId = conceptId;
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if a concept has been learned.
    /// </summary>
    public bool IsConceptLearned(string conceptId)
    {
        return LearnedConceptIds.Contains(conceptId);
    }

    /// <summary>
    /// Checks if a concept has been visited.
    /// </summary>
    public bool IsConceptVisited(string conceptId)
    {
        return VisitedConceptIds.Contains(conceptId);
    }
}
