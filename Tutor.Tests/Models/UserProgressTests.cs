using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class UserProgressTests
{
    [Test]
    public void GetOverallProgress_ZeroTotal_Zero()
    {
        var p = new UserProgress();
        Assert.That(p.GetOverallProgress(0), Is.EqualTo(0));
    }

    [Test]
    public void GetOverallProgress_PartialCompletion()
    {
        var p = new UserProgress();
        p.MarkConceptLearned("a");
        p.MarkConceptLearned("b");
        Assert.That(p.GetOverallProgress(totalConcepts: 4), Is.EqualTo(50));
    }

    [Test]
    public void MarkConceptLearned_AddsToBothListsOnce()
    {
        var p = new UserProgress();
        p.MarkConceptLearned("c1");
        p.MarkConceptLearned("c1");

        Assert.That(p.LearnedConceptIds, Has.Count.EqualTo(1));
        Assert.That(p.VisitedConceptIds, Has.Count.EqualTo(1));
        Assert.That(p.IsConceptLearned("c1"), Is.True);
        Assert.That(p.IsConceptVisited("c1"), Is.True);
    }

    [Test]
    public void MarkConceptVisited_DoesNotMarkLearned()
    {
        var p = new UserProgress();
        p.MarkConceptVisited("c1");

        Assert.That(p.IsConceptVisited("c1"), Is.True);
        Assert.That(p.IsConceptLearned("c1"), Is.False);
    }

    [Test]
    public void SetPosition_TracksCurrent()
    {
        var p = new UserProgress();
        p.SetPosition(chapterId: "ch1", conceptId: "c1");

        Assert.That(p.CurrentChapterId, Is.EqualTo("ch1"));
        Assert.That(p.CurrentConceptId, Is.EqualTo("c1"));
        Assert.That(p.LastActiveAt, Is.Not.Null);
    }

    [Test]
    public void GetSectionStatus_DefaultsToNotStarted()
    {
        var p = new UserProgress();
        Assert.That(p.GetSectionStatus("missing"), Is.EqualTo(SectionStatus.NotStarted));
    }

    [Test]
    public void MarkSectionVisited_MovesToVisitedFromNotStarted()
    {
        var p = new UserProgress();
        p.MarkSectionVisited("s1");
        Assert.That(p.GetSectionStatus("s1"), Is.EqualTo(SectionStatus.Visited));
    }

    [Test]
    public void MarkSectionVisited_DoesNotDowngradeFromRead()
    {
        var p = new UserProgress();
        p.MarkSectionRead("s1");
        p.MarkSectionVisited("s1");
        Assert.That(p.GetSectionStatus("s1"), Is.EqualTo(SectionStatus.Read));
    }

    [Test]
    public void MarkSectionRead_AdvancesFromVisited()
    {
        var p = new UserProgress();
        p.MarkSectionVisited("s1");
        p.MarkSectionRead("s1");
        Assert.That(p.GetSectionStatus("s1"), Is.EqualTo(SectionStatus.Read));
        Assert.That(p.SectionProgress["s1"].MarkedReadAt, Is.Not.Null);
    }

    [Test]
    public void MarkSectionComplete_RecordsQuizResultId()
    {
        var p = new UserProgress();
        p.MarkSectionComplete("s1", quizResultId: "qr-7");

        Assert.That(p.GetSectionStatus("s1"), Is.EqualTo(SectionStatus.Complete));
        Assert.That(p.SectionProgress["s1"].LastQuizResultId, Is.EqualTo("qr-7"));
        Assert.That(p.SectionProgress["s1"].CompletedAt, Is.Not.Null);
    }

    [Test]
    public void RecordSectionTime_AccumulatesAndUpdatesTotalMinutes()
    {
        var p = new UserProgress();
        p.RecordSectionTime("s1", 90);   // 90 sec
        p.RecordSectionTime("s1", 30);   // +30 sec → 120 sec total
        p.RecordSectionTime("s2", 180);  // → 300 sec across all sections

        Assert.That(p.SectionTimeSpent["s1"], Is.EqualTo(120));
        Assert.That(p.SectionTimeSpent["s2"], Is.EqualTo(180));
        Assert.That(p.TotalMinutesSpent, Is.EqualTo(5)); // 300 / 60
    }

    [Test]
    public void UpdateChapterProgress_MarksCompleteWhenAllSectionsDone()
    {
        var p = new UserProgress();
        p.UpdateChapterProgress("L1", completedSections: 3, totalSections: 3);

        var chapter = p.ChapterProgressTracking["L1"];
        Assert.That(chapter.Status, Is.EqualTo(SectionStatus.Complete));
        Assert.That(chapter.CompletedAt, Is.Not.Null);
    }

    [Test]
    public void UpdateChapterProgress_MarksVisitedWhenPartial()
    {
        var p = new UserProgress();
        p.UpdateChapterProgress("L1", completedSections: 1, totalSections: 3);

        Assert.That(p.ChapterProgressTracking["L1"].Status, Is.EqualTo(SectionStatus.Visited));
    }

    [Test]
    public void GetSectionCompletionPercentage_OnlyCountsComplete()
    {
        var p = new UserProgress();
        p.MarkSectionComplete("a");
        p.MarkSectionRead("b");          // not complete
        p.MarkSectionVisited("c");       // not complete

        Assert.That(p.GetSectionCompletionPercentage(totalSections: 4), Is.EqualTo(25.0));
    }

    // --- Spiral curriculum: TopicMastery state machine ---

    [Test]
    public void RecordTopicTaught_FirstTime_IsLevelOne()
    {
        var p = new UserProgress();
        var level = p.RecordTopicTaught("intro", "Introduction");

        Assert.That(level, Is.EqualTo(1));
        Assert.That(p.TopicMastery["intro"].VisitCount, Is.EqualTo(1));
        Assert.That(p.TopicMastery["intro"].TopicTitle, Is.EqualTo("Introduction"));
    }

    [Test]
    public void RecordTopicTaught_NoQuiz_CapsAtUnquizzedLevel()
    {
        var p = new UserProgress();
        var l1 = p.RecordTopicTaught("k", "T");
        var l2 = p.RecordTopicTaught("k", "T");
        var l3 = p.RecordTopicTaught("k", "T");

        // Without a quiz to validate understanding, depth never exceeds the unquizzed cap.
        Assert.That(l1, Is.EqualTo(1));
        Assert.That(l2, Is.EqualTo(UserProgress.UnquizzedSpiralCap));
        Assert.That(l3, Is.EqualTo(UserProgress.UnquizzedSpiralCap));
    }

    [Test]
    public void RecordTopicTaught_PassingQuiz_AdvancesAndCapsAtMax()
    {
        var p = new UserProgress();
        var l1 = p.RecordTopicTaught("k", "T");      // 1
        p.RecordTopicQuiz("k", "T", 80);
        var l2 = p.RecordTopicTaught("k", "T");      // 2
        p.RecordTopicQuiz("k", "T", 90);
        var l3 = p.RecordTopicTaught("k", "T");      // 3
        var l4 = p.RecordTopicTaught("k", "T");      // still 3 (max)

        Assert.That(new[] { l1, l2, l3, l4 }, Is.EqualTo(new[] { 1, 2, 3, 3 }));
    }

    [Test]
    public void RecordTopicTaught_FailingQuiz_HoldsLevel()
    {
        var p = new UserProgress();
        var l1 = p.RecordTopicTaught("k", "T");      // 1
        p.RecordTopicQuiz("k", "T", 50);             // below pass threshold
        var l2 = p.RecordTopicTaught("k", "T");      // held at 1

        Assert.That(l1, Is.EqualTo(1));
        Assert.That(l2, Is.EqualTo(1));
    }

    [Test]
    public void RecordTopicTaught_FailThenPass_Advances()
    {
        var p = new UserProgress();
        p.RecordTopicTaught("k", "T");               // 1
        p.RecordTopicQuiz("k", "T", 50);
        var held = p.RecordTopicTaught("k", "T");    // held at 1
        p.RecordTopicQuiz("k", "T", 85);             // best now passing
        var advanced = p.RecordTopicTaught("k", "T");

        Assert.That(held, Is.EqualTo(1));
        Assert.That(advanced, Is.EqualTo(2));
    }

    [Test]
    public void RecordTopicQuiz_TracksLastAndBest()
    {
        var p = new UserProgress();
        p.RecordTopicQuiz("k", "T", 80);
        p.RecordTopicQuiz("k", "T", 60);

        var m = p.TopicMastery["k"];
        Assert.That(m.LastQuizScorePct, Is.EqualTo(60));
        Assert.That(m.BestQuizScorePct, Is.EqualTo(80));
    }

    [Test]
    public void RecordTopicQuiz_ClampsToValidRange()
    {
        var p = new UserProgress();
        p.RecordTopicQuiz("hi", "T", 150);
        p.RecordTopicQuiz("lo", "T", -10);

        Assert.That(p.TopicMastery["hi"].LastQuizScorePct, Is.EqualTo(100));
        Assert.That(p.TopicMastery["lo"].LastQuizScorePct, Is.EqualTo(0));
    }
}
