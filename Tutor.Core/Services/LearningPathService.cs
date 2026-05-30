using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Computes mastery-gated, spiral progression over a <see cref="CourseStructure"/> from a
/// student's <see cref="UserProgress"/>. Pure (stateless) logic — no I/O — so it is trivially
/// unit-testable and safe to register as a singleton.
///
/// <para>Mastery is tracked per <see cref="Section"/> (the unit that actually carries a quiz):
/// a quiz pass calls <c>UserProgress.MarkSectionComplete</c> and records the score against the
/// section's spiral mastery (see <c>LearnTabs.HandleSectionQuizCompleted</c>). Gating reads from
/// there.</para>
///
/// <para>Lessons are taught in order; lesson N unlocks once lesson N-1 is <em>complete</em> (every
/// quiz-bearing section passed; lessons without quizzes complete when every section is read).
/// The first lesson is always unlocked. A failed/unfinished section stays the
/// <see cref="NextRecommendedSection"/> until passed, which is how weak material resurfaces. The
/// whole course must be complete before the final exam is eligible.</para>
/// </summary>
public sealed class LearningPathService
{
    /// <summary>True when a section's quiz has been passed (status Complete).</summary>
    public bool IsSectionComplete(UserProgress progress, string sectionId)
        => progress.GetSectionStatus(sectionId) == SectionStatus.Complete;

    /// <summary>
    /// True when a lesson is finished: every quiz-bearing section in it is Complete. A lesson with
    /// no quizzes is complete once every section has at least been read.
    /// </summary>
    public bool IsLessonComplete(Lesson lesson, UserProgress progress)
    {
        var sections = lesson.GetAllSectionsFlattened().ToList();
        if (sections.Count == 0) return false; // nothing generated yet → keep the gate closed

        var quizSections = sections.Where(s => s.HasQuiz).ToList();
        if (quizSections.Count > 0)
            return quizSections.All(s => progress.GetSectionStatus(s.Id) == SectionStatus.Complete);

        return sections.All(s => progress.GetSectionStatus(s.Id) >= SectionStatus.Read);
    }

    /// <summary>
    /// True when the lesson is open to the student: the first lesson in order, or any lesson whose
    /// immediate predecessor (by <see cref="Lesson.Order"/>) is complete.
    /// </summary>
    public bool IsLessonUnlocked(CourseStructure structure, UserProgress progress, string lessonId)
    {
        var ordered = structure.GetLessonsInOrder().ToList();
        var index = ordered.FindIndex(l => l.Id == lessonId);
        if (index <= 0) return index == 0; // not found → locked; first lesson → unlocked
        return IsLessonComplete(ordered[index - 1], progress);
    }

    /// <summary>
    /// Spiral depth a section is at (0 = never taught, 1 = intro, 2 = deepen, 3 = synthesis),
    /// as assigned by <see cref="UserProgress.RecordTopicTaught"/> keyed on the section id.
    /// </summary>
    public int GetSectionSpiralLevel(UserProgress progress, string sectionId)
        => progress.TopicMastery.TryGetValue(sectionId, out var m) ? m.SpiralLevel : 0;

    /// <summary>
    /// The next section the student should study: the earliest unfinished section in the unlocked
    /// portion of the course. A failed quiz section stays "next" until passed (resurfacing weak
    /// material). Returns null when everything reachable is complete.
    /// </summary>
    public (Lesson Lesson, Section Section)? NextRecommendedSection(CourseStructure structure, UserProgress progress)
    {
        foreach (var lesson in structure.GetLessonsInOrder())
        {
            if (!IsLessonUnlocked(structure, progress, lesson.Id)) break; // stop at first locked lesson
            foreach (var section in lesson.GetAllSectionsFlattened())
            {
                var status = progress.GetSectionStatus(section.Id);
                if (section.HasQuiz && status != SectionStatus.Complete) return (lesson, section);
                if (!section.HasQuiz && status < SectionStatus.Read) return (lesson, section);
            }
        }
        return null;
    }

    /// <summary>Fraction (0-100) of the course's quiz-bearing sections that are complete.</summary>
    public double CompletionPercent(CourseStructure structure, UserProgress progress)
    {
        var quizSections = structure.GetLessonsInOrder()
            .SelectMany(l => l.GetAllSectionsFlattened())
            .Where(s => s.HasQuiz)
            .ToList();
        if (quizSections.Count == 0) return 0;
        var done = quizSections.Count(s => progress.GetSectionStatus(s.Id) == SectionStatus.Complete);
        return (double)done / quizSections.Count * 100;
    }

    /// <summary>True when every lesson in the course is complete (the final-exam gate).</summary>
    public bool IsExamEligible(CourseStructure structure, UserProgress progress)
    {
        var lessons = structure.GetLessonsInOrder().ToList();
        return lessons.Count > 0 && lessons.All(l => IsLessonComplete(l, progress));
    }
}
