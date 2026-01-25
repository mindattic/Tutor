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

    /// <summary>
    /// Section-level progress tracking.
    /// </summary>
    public Dictionary<string, SectionProgress> SectionProgress { get; set; } = [];

    /// <summary>
    /// Chapter-level progress tracking.
    /// </summary>
    public Dictionary<string, ChapterProgress> ChapterProgressTracking { get; set; } = [];

    /// <summary>
    /// Quiz results for this course.
    /// </summary>
    public List<string> QuizResultIds { get; set; } = [];

    /// <summary>
    /// Time spent on each section (sectionId -> seconds).
    /// </summary>
    public Dictionary<string, int> SectionTimeSpent { get; set; } = [];

    /// <summary>
    /// Gets the status of a section.
    /// </summary>
    public SectionStatus GetSectionStatus(string sectionId)
    {
        if (SectionProgress.TryGetValue(sectionId, out var progress))
            return progress.Status;
        return SectionStatus.NotStarted;
    }

    /// <summary>
    /// Marks a section as visited.
    /// </summary>
    public void MarkSectionVisited(string sectionId)
    {
        if (!SectionProgress.ContainsKey(sectionId))
        {
            SectionProgress[sectionId] = new SectionProgress
            {
                SectionId = sectionId,
                Status = SectionStatus.Visited,
                FirstVisitedAt = DateTime.UtcNow
            };
        }
        else if (SectionProgress[sectionId].Status == SectionStatus.NotStarted)
        {
            SectionProgress[sectionId].Status = SectionStatus.Visited;
            SectionProgress[sectionId].FirstVisitedAt = DateTime.UtcNow;
        }
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks a section as read (user spent enough time reading).
    /// </summary>
    public void MarkSectionRead(string sectionId)
    {
        if (!SectionProgress.ContainsKey(sectionId))
        {
            SectionProgress[sectionId] = new SectionProgress
            {
                SectionId = sectionId,
                Status = SectionStatus.Read,
                FirstVisitedAt = DateTime.UtcNow,
                MarkedReadAt = DateTime.UtcNow
            };
        }
        else if (SectionProgress[sectionId].Status < SectionStatus.Read)
        {
            SectionProgress[sectionId].Status = SectionStatus.Read;
            SectionProgress[sectionId].MarkedReadAt = DateTime.UtcNow;
        }
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks a section as complete (quiz passed).
    /// </summary>
    public void MarkSectionComplete(string sectionId, string? quizResultId = null)
    {
        if (!SectionProgress.ContainsKey(sectionId))
        {
            SectionProgress[sectionId] = new SectionProgress
            {
                SectionId = sectionId,
                Status = SectionStatus.Complete,
                FirstVisitedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                LastQuizResultId = quizResultId
            };
        }
        else
        {
            SectionProgress[sectionId].Status = SectionStatus.Complete;
            SectionProgress[sectionId].CompletedAt = DateTime.UtcNow;
            SectionProgress[sectionId].LastQuizResultId = quizResultId;
        }
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records time spent on a section.
    /// </summary>
    public void RecordSectionTime(string sectionId, int seconds)
    {
        if (!SectionTimeSpent.ContainsKey(sectionId))
            SectionTimeSpent[sectionId] = 0;
        
        SectionTimeSpent[sectionId] += seconds;
        
        if (SectionProgress.ContainsKey(sectionId))
            SectionProgress[sectionId].TimeSpentSeconds = SectionTimeSpent[sectionId];
        
        TotalMinutesSpent = SectionTimeSpent.Values.Sum() / 60;
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates chapter progress based on section completion.
    /// </summary>
    public void UpdateChapterProgress(string lessonId, int completedSections, int totalSections)
    {
        if (!ChapterProgressTracking.ContainsKey(lessonId))
        {
            ChapterProgressTracking[lessonId] = new ChapterProgress
            {
                LessonId = lessonId,
                TotalSections = totalSections
            };
        }

        var chapter = ChapterProgressTracking[lessonId];
        chapter.CompletedSections = completedSections;
        chapter.TotalSections = totalSections;
        
        if (completedSections >= totalSections)
        {
            chapter.Status = SectionStatus.Complete;
            chapter.CompletedAt = DateTime.UtcNow;
        }
        else if (completedSections > 0)
        {
            chapter.Status = SectionStatus.Visited;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets overall course completion percentage based on sections.
    /// </summary>
    public double GetSectionCompletionPercentage(int totalSections)
    {
        if (totalSections == 0) return 0;
        var completedCount = SectionProgress.Values.Count(p => p.Status == SectionStatus.Complete);
        return (double)completedCount / totalSections * 100;
    }
}
