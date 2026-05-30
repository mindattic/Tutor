using NUnit.Framework;
using Tutor.Core.Models;
using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class LearningPathServiceTests
{
    private LearningPathService sut = null!;

    [SetUp]
    public void SetUp() => sut = new LearningPathService();

    // Two lessons; lesson 1 has two quiz sections, lesson 2 has one.
    private static CourseStructure TwoLessonCourse() => new()
    {
        Id = "struct-1",
        CourseId = "course-1",
        Lessons =
        {
            new Lesson
            {
                Id = "L1", Order = 0, Title = "Lesson One",
                Sections =
                {
                    new Section { Id = "s1", Order = 0, Title = "Intro",   LessonId = "L1", HasQuiz = true },
                    new Section { Id = "s2", Order = 1, Title = "Deeper",  LessonId = "L1", HasQuiz = true },
                },
            },
            new Lesson
            {
                Id = "L2", Order = 1, Title = "Lesson Two",
                Sections =
                {
                    new Section { Id = "s3", Order = 0, Title = "Advanced", LessonId = "L2", HasQuiz = true },
                },
            },
        },
    };

    [Test]
    public void FirstLessonUnlocked_SecondLocked_Initially()
    {
        var s = TwoLessonCourse();
        var p = new UserProgress();

        Assert.That(sut.IsLessonUnlocked(s, p, "L1"), Is.True);
        Assert.That(sut.IsLessonUnlocked(s, p, "L2"), Is.False);
    }

    [Test]
    public void SecondLesson_Unlocks_WhenFirstLessonComplete()
    {
        var s = TwoLessonCourse();
        var p = new UserProgress();

        p.MarkSectionComplete("s1");
        Assert.That(sut.IsLessonUnlocked(s, p, "L2"), Is.False, "one section still incomplete");

        p.MarkSectionComplete("s2");
        Assert.That(sut.IsLessonComplete(s.GetLesson("L1")!, p), Is.True);
        Assert.That(sut.IsLessonUnlocked(s, p, "L2"), Is.True);
    }

    [Test]
    public void NextRecommendedSection_ReturnsEarliestIncomplete_AndResurfaces()
    {
        var s = TwoLessonCourse();
        var p = new UserProgress();

        Assert.That(sut.NextRecommendedSection(s, p)?.Section.Id, Is.EqualTo("s1"));

        p.MarkSectionComplete("s1");
        Assert.That(sut.NextRecommendedSection(s, p)?.Section.Id, Is.EqualTo("s2"));

        // s3 is in a locked lesson, so it is never recommended until L1 is done.
        Assert.That(sut.NextRecommendedSection(s, p)?.Section.Id, Is.Not.EqualTo("s3"));
    }

    [Test]
    public void ExamEligible_OnlyWhenEveryLessonComplete()
    {
        var s = TwoLessonCourse();
        var p = new UserProgress();
        Assert.That(sut.IsExamEligible(s, p), Is.False);

        p.MarkSectionComplete("s1");
        p.MarkSectionComplete("s2");
        Assert.That(sut.IsExamEligible(s, p), Is.False, "Lesson Two still incomplete");

        p.MarkSectionComplete("s3");
        Assert.That(sut.IsExamEligible(s, p), Is.True);
        Assert.That(sut.NextRecommendedSection(s, p), Is.Null);
        Assert.That(sut.CompletionPercent(s, p), Is.EqualTo(100));
    }

    [Test]
    public void LessonWithoutQuizzes_CompletesWhenAllSectionsRead()
    {
        var s = new CourseStructure
        {
            Id = "x", CourseId = "c",
            Lessons =
            {
                new Lesson { Id = "L1", Order = 0, Title = "Reading-only",
                    Sections = { new Section { Id = "r1", Order = 0, Title = "R1", HasQuiz = false } } },
            },
        };
        var p = new UserProgress();
        Assert.That(sut.IsLessonComplete(s.GetLesson("L1")!, p), Is.False);

        p.MarkSectionRead("r1");
        Assert.That(sut.IsLessonComplete(s.GetLesson("L1")!, p), Is.True);
    }

    [Test]
    public void TopicKey_IsStableSlug()
    {
        Assert.That(UserProgress.TopicKey("The Water Cycle!"), Is.EqualTo("the-water-cycle"));
        Assert.That(UserProgress.TopicKey("  Spaces   &  Symbols  "), Is.EqualTo("spaces-symbols"));
        Assert.That(UserProgress.TopicKey(""), Is.EqualTo("topic"));
    }
}
