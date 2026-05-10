using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class UserProgressTests
{
    [Fact]
    public void GetOverallProgress_ZeroTotal_Zero()
    {
        var p = new UserProgress();
        Assert.Equal(0, p.GetOverallProgress(0));
    }

    [Fact]
    public void GetOverallProgress_PartialCompletion()
    {
        var p = new UserProgress();
        p.MarkConceptLearned("a");
        p.MarkConceptLearned("b");
        Assert.Equal(50, p.GetOverallProgress(totalConcepts: 4));
    }

    [Fact]
    public void MarkConceptLearned_AddsToBothListsOnce()
    {
        var p = new UserProgress();
        p.MarkConceptLearned("c1");
        p.MarkConceptLearned("c1");

        Assert.Single(p.LearnedConceptIds);
        Assert.Single(p.VisitedConceptIds);
        Assert.True(p.IsConceptLearned("c1"));
        Assert.True(p.IsConceptVisited("c1"));
    }

    [Fact]
    public void MarkConceptVisited_DoesNotMarkLearned()
    {
        var p = new UserProgress();
        p.MarkConceptVisited("c1");

        Assert.True(p.IsConceptVisited("c1"));
        Assert.False(p.IsConceptLearned("c1"));
    }

    [Fact]
    public void SetPosition_TracksCurrent()
    {
        var p = new UserProgress();
        p.SetPosition(chapterId: "ch1", conceptId: "c1");

        Assert.Equal("ch1", p.CurrentChapterId);
        Assert.Equal("c1", p.CurrentConceptId);
        Assert.NotNull(p.LastActiveAt);
    }

    [Fact]
    public void GetSectionStatus_DefaultsToNotStarted()
    {
        var p = new UserProgress();
        Assert.Equal(SectionStatus.NotStarted, p.GetSectionStatus("missing"));
    }

    [Fact]
    public void MarkSectionVisited_MovesToVisitedFromNotStarted()
    {
        var p = new UserProgress();
        p.MarkSectionVisited("s1");
        Assert.Equal(SectionStatus.Visited, p.GetSectionStatus("s1"));
    }

    [Fact]
    public void MarkSectionVisited_DoesNotDowngradeFromRead()
    {
        var p = new UserProgress();
        p.MarkSectionRead("s1");
        p.MarkSectionVisited("s1");
        Assert.Equal(SectionStatus.Read, p.GetSectionStatus("s1"));
    }

    [Fact]
    public void MarkSectionRead_AdvancesFromVisited()
    {
        var p = new UserProgress();
        p.MarkSectionVisited("s1");
        p.MarkSectionRead("s1");
        Assert.Equal(SectionStatus.Read, p.GetSectionStatus("s1"));
        Assert.NotNull(p.SectionProgress["s1"].MarkedReadAt);
    }

    [Fact]
    public void MarkSectionComplete_RecordsQuizResultId()
    {
        var p = new UserProgress();
        p.MarkSectionComplete("s1", quizResultId: "qr-7");

        Assert.Equal(SectionStatus.Complete, p.GetSectionStatus("s1"));
        Assert.Equal("qr-7", p.SectionProgress["s1"].LastQuizResultId);
        Assert.NotNull(p.SectionProgress["s1"].CompletedAt);
    }

    [Fact]
    public void RecordSectionTime_AccumulatesAndUpdatesTotalMinutes()
    {
        var p = new UserProgress();
        p.RecordSectionTime("s1", 90);   // 90 sec
        p.RecordSectionTime("s1", 30);   // +30 sec → 120 sec total
        p.RecordSectionTime("s2", 180);  // → 300 sec across all sections

        Assert.Equal(120, p.SectionTimeSpent["s1"]);
        Assert.Equal(180, p.SectionTimeSpent["s2"]);
        Assert.Equal(5, p.TotalMinutesSpent); // 300 / 60
    }

    [Fact]
    public void UpdateChapterProgress_MarksCompleteWhenAllSectionsDone()
    {
        var p = new UserProgress();
        p.UpdateChapterProgress("L1", completedSections: 3, totalSections: 3);

        var chapter = p.ChapterProgressTracking["L1"];
        Assert.Equal(SectionStatus.Complete, chapter.Status);
        Assert.NotNull(chapter.CompletedAt);
    }

    [Fact]
    public void UpdateChapterProgress_MarksVisitedWhenPartial()
    {
        var p = new UserProgress();
        p.UpdateChapterProgress("L1", completedSections: 1, totalSections: 3);

        Assert.Equal(SectionStatus.Visited, p.ChapterProgressTracking["L1"].Status);
    }

    [Fact]
    public void GetSectionCompletionPercentage_OnlyCountsComplete()
    {
        var p = new UserProgress();
        p.MarkSectionComplete("a");
        p.MarkSectionRead("b");          // not complete
        p.MarkSectionVisited("c");       // not complete

        Assert.Equal(25.0, p.GetSectionCompletionPercentage(totalSections: 4));
    }
}
