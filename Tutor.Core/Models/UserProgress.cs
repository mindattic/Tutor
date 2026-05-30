namespace Tutor.Core.Models;

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

    /// <summary>Id of the passing final-exam <c>QuizResult</c>, once the course is completed.</summary>
    public string? FinalExamResultId { get; set; }

    /// <summary>True once the student has passed the final exam for this course.</summary>
    public bool HasCompletedCourse { get; set; }

    /// <summary>When the course was completed (final exam passed), if it has been.</summary>
    public DateTime? CourseCompletedAt { get; set; }

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
        
        // Use long to prevent overflow when summing large values
        TotalMinutesSpent = (int)(SectionTimeSpent.Values.Sum(v => (long)v) / 60);
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

    /// <summary>
    /// Spiral-curriculum mastery per lesson topic, keyed by a normalized topic slug so it
    /// survives the live curriculum being regenerated across sessions. Drives how deeply a
    /// topic is taught on each return (see <see cref="RecordTopicTaught"/>).
    /// </summary>
    public Dictionary<string, TopicMastery> TopicMastery { get; set; } = [];

    /// <summary>
    /// Quiz score (0-100) at or above which a topic is considered mastered enough to teach
    /// deeper on the next visit. Below this, the next visit re-grounds the fundamentals.
    /// </summary>
    public const int TopicPassThreshold = 70;

    /// <summary>Highest spiral depth a topic can reach.</summary>
    public const int MaxSpiralLevel = 3;

    /// <summary>Cap on spiral depth before any quiz has validated understanding.</summary>
    public const int UnquizzedSpiralCap = 2;

    /// <summary>
    /// Derives the stable per-course mastery key for a topic from its title. Topic ids are
    /// regenerated when the curriculum is rebuilt, so a title slug is used instead so a
    /// student's mastery survives re-generation. Always pair with the topic title when
    /// recording so the display name stays current.
    /// </summary>
    public static string TopicKey(string topicTitle)
    {
        if (string.IsNullOrWhiteSpace(topicTitle)) return "topic";
        var chars = topicTitle.Trim().ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();
        var slug = new string(chars);
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        slug = slug.Trim('-');
        return slug.Length > 0 ? slug : "topic";
    }

    /// <summary>True when a topic's best quiz score meets the mastery threshold.</summary>
    public bool IsTopicMastered(string topicTitle)
        => TopicMastery.TryGetValue(TopicKey(topicTitle), out var m)
           && m.BestQuizScorePct is int best && best >= TopicPassThreshold;

    /// <summary>
    /// Gets the mastery record for a topic, creating it on first use.
    /// </summary>
    public TopicMastery GetOrCreateTopicMastery(string topicKey, string topicTitle)
    {
        if (!TopicMastery.TryGetValue(topicKey, out var mastery))
        {
            mastery = new TopicMastery { TopicKey = topicKey, TopicTitle = topicTitle };
            TopicMastery[topicKey] = mastery;
        }
        // Keep the display title fresh in case the curriculum reworded it.
        if (!string.IsNullOrWhiteSpace(topicTitle))
            mastery.TopicTitle = topicTitle;
        return mastery;
    }

    /// <summary>
    /// Records that a topic is being taught and returns the spiral level (1-3) to teach at.
    ///
    /// Level rules (mastery + revisit):
    /// - Never taught -> level 1 (introduction).
    /// - Revisit with a passing best quiz score -> advance one level (capped at MaxSpiralLevel).
    /// - Revisit with a quiz taken but below the pass threshold -> hold the current level
    ///   (the caller re-grounds the fundamentals).
    /// - Revisit with no quiz yet -> advance by visit, but capped at UnquizzedSpiralCap so
    ///   depth is never assumed without evidence of understanding.
    /// </summary>
    public int RecordTopicTaught(string topicKey, string topicTitle)
    {
        var mastery = GetOrCreateTopicMastery(topicKey, topicTitle);
        mastery.VisitCount++;
        mastery.LastTaughtAt = DateTime.UtcNow;

        int level;
        if (mastery.SpiralLevel == 0)
        {
            // First time taught.
            level = 1;
        }
        else if (mastery.BestQuizScorePct is int best && best >= TopicPassThreshold)
        {
            // Demonstrated mastery: go one level deeper.
            level = Math.Min(mastery.SpiralLevel + 1, MaxSpiralLevel);
        }
        else if (mastery.LastQuizScorePct is int)
        {
            // Quiz taken but not passed: hold depth and re-ground fundamentals.
            level = mastery.SpiralLevel;
        }
        else
        {
            // No quiz yet: deepen by revisiting, but cap until a quiz validates depth.
            level = Math.Min(mastery.SpiralLevel + 1, UnquizzedSpiralCap);
        }

        mastery.SpiralLevel = level;
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
        return level;
    }

    /// <summary>
    /// Records a quiz outcome (0-100) for a topic, updating last and best scores.
    /// </summary>
    public void RecordTopicQuiz(string topicKey, string topicTitle, int scorePct)
    {
        var clamped = Math.Clamp(scorePct, 0, 100);
        var mastery = GetOrCreateTopicMastery(topicKey, topicTitle);
        mastery.LastQuizScorePct = clamped;
        mastery.BestQuizScorePct = mastery.BestQuizScorePct is int best
            ? Math.Max(best, clamped)
            : clamped;
        mastery.LastQuizAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        LastActiveAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Spiral-curriculum mastery state for a single lesson topic.
/// </summary>
public class TopicMastery
{
    /// <summary>Normalized topic slug used as the stable per-course key.</summary>
    public string TopicKey { get; set; } = "";

    /// <summary>Last-seen human-readable topic title.</summary>
    public string TopicTitle { get; set; } = "";

    /// <summary>
    /// Depth the topic has reached: 0 = never taught, 1 = introduction,
    /// 2 = connect/deepen, 3 = advanced/synthesis.
    /// </summary>
    public int SpiralLevel { get; set; }

    /// <summary>Number of times the topic has been taught.</summary>
    public int VisitCount { get; set; }

    /// <summary>Most recent quiz score for this topic (0-100), if quizzed.</summary>
    public int? LastQuizScorePct { get; set; }

    /// <summary>Best quiz score for this topic (0-100), if quizzed.</summary>
    public int? BestQuizScorePct { get; set; }

    /// <summary>When the topic was last taught.</summary>
    public DateTime? LastTaughtAt { get; set; }

    /// <summary>When the topic was last quizzed.</summary>
    public DateTime? LastQuizAt { get; set; }
}
