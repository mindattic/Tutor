using NUnit.Framework;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

/// <summary>
/// End-to-end lifecycle of a course through the real wired services (no UI, no LLM): a student
/// works through a "Moby Dick"-shaped course lesson by lesson, lessons unlock as their sections
/// are mastered, the final exam unlocks only when the whole course is mastered, a passing exam
/// completes the course, and a completion certificate is issued and persisted — then "unloaded".
/// </summary>
public class FullCourseLifecycleTests
{
    private FakeAppDataPathProvider paths = null!;
    private CourseStructureStorageService structureStorage = null!;
    private CertificateService certificates = null!;
    private LearningPathService path = null!;
    private FinalExamService exam = null!;
    private Course course = null!;
    private CourseStructure structure = null!;

    [SetUp]
    public async Task SetUp()
    {
        paths = new FakeAppDataPathProvider();
        structureStorage = new CourseStructureStorageService(paths);
        certificates = new CertificateService(new LocalCertificateAuthority(), structureStorage, paths);
        path = new LearningPathService();
        exam = new FinalExamService(null!, path); // StartAsync (LLM) not exercised here

        course = new Course { Id = "moby", Name = "Moby Dick; Or, The Whale" };
        structure = new CourseStructure
        {
            Id = "moby-structure", CourseId = "moby", Status = CourseStructureStatus.Ready,
            Lessons =
            {
                new Lesson { Id = "L1", Order = 0, Title = "Loomings",
                    Sections =
                    {
                        new Section { Id = "s1a", Order = 0, Title = "Call me Ishmael", LessonId = "L1", HasQuiz = true, ConceptIds = { "c1" } },
                        new Section { Id = "s1b", Order = 1, Title = "The Lee Shore",   LessonId = "L1", HasQuiz = true, ConceptIds = { "c2" } },
                    } },
                new Lesson { Id = "L2", Order = 1, Title = "The Whale",
                    Sections =
                    {
                        new Section { Id = "s2a", Order = 0, Title = "Cetology", LessonId = "L2", HasQuiz = true, ConceptIds = { "c3" } },
                    } },
            },
        };
        await structureStorage.SaveAsync(structure);
    }

    [TearDown]
    public void TearDown() => paths.Dispose();

    private static QuizResult PassedFinalExam() => new()
    {
        Id = "final-1", CourseId = "moby", IsFinalExam = true,
        PassingScore = FinalExamService.PassingScore,
        TotalQuestions = 10, CorrectAnswers = 9, // 90% >= 80%
        CompletedAt = DateTime.UtcNow,
    };

    [Test]
    public async Task Student_WorksThroughCourse_FromLockedLessonsToCertificate()
    {
        var progress = new UserProgress { CourseId = "moby", UserId = "student-1" };

        // 1. Fresh start: only the first lesson is open; exam locked; no certificate.
        Assert.That(path.IsLessonUnlocked(structure, progress, "L1"), Is.True);
        Assert.That(path.IsLessonUnlocked(structure, progress, "L2"), Is.False);
        Assert.That(exam.IsEligible(structure, progress), Is.False);
        Assert.That(await certificates.GetForCourseAsync("student-1", "moby"), Is.Null);
        Assert.That(path.NextRecommendedSection(structure, progress)?.Section.Id, Is.EqualTo("s1a"));

        // 2. Master lesson 1 (passing each section quiz marks it complete) -> lesson 2 unlocks.
        progress.MarkSectionComplete("s1a", "q-s1a");
        progress.MarkSectionComplete("s1b", "q-s1b");
        Assert.That(path.IsLessonComplete(structure.GetLesson("L1")!, progress), Is.True);
        Assert.That(path.IsLessonUnlocked(structure, progress, "L2"), Is.True);
        Assert.That(exam.IsEligible(structure, progress), Is.False, "lesson 2 not done yet");

        // 3. Master lesson 2 -> the whole course is mastered, final exam unlocks.
        progress.MarkSectionComplete("s2a", "q-s2a");
        Assert.That(exam.IsEligible(structure, progress), Is.True);
        Assert.That(path.NextRecommendedSection(structure, progress), Is.Null);
        Assert.That(path.CompletionPercent(structure, progress), Is.EqualTo(100));

        // 4. Pass the final exam -> course completion is recorded.
        var examResult = PassedFinalExam();
        Assert.That(examResult.Passed, Is.True);
        Assert.That(exam.ApplyResult(progress, examResult), Is.True);
        Assert.That(progress.HasCompletedCourse, Is.True);
        Assert.That(progress.FinalExamResultId, Is.EqualTo("final-1"));
        Assert.That(progress.CourseCompletedAt, Is.Not.Null);

        // 5. Issue the certificate -> bound to the student, course, score, and course content hash.
        var cert = await certificates.IssueAsync("student-1", "Ada Lovelace", course, examResult);
        Assert.That(cert.StudentName, Is.EqualTo("Ada Lovelace"));
        Assert.That(cert.CourseName, Is.EqualTo("Moby Dick; Or, The Whale"));
        Assert.That(cert.FinalScorePct, Is.EqualTo(90));
        Assert.That(cert.CertificateNumber, Does.StartWith("TUTOR-"));
        Assert.That(cert.CourseContentHash, Is.Not.Empty, "bound to the saved structure");
        Assert.That(cert.IsSigned, Is.False, "cryptographic signing is deferred");

        // 6. The certificate persists and is retrievable for the course.
        var loaded = await certificates.GetForCourseAsync("student-1", "moby");
        Assert.That(loaded, Is.Not.Null);
        Assert.That(loaded!.CertificateNumber, Is.EqualTo(cert.CertificateNumber));

        // 7. "Unload": removing the course structure breaks future content-hashing, and a fresh
        //    student starts locked again — state is per-student, so unloading is clean.
        await structureStorage.DeleteByCourseIdAsync("moby");
        Assert.That(await structureStorage.LoadByCourseIdAsync("moby"), Is.Null);
        var newStudent = new UserProgress { CourseId = "moby", UserId = "student-2" };
        Assert.That(exam.IsEligible(structure, newStudent), Is.False);
        Assert.That(await certificates.GetForCourseAsync("student-2", "moby"), Is.Null);
    }

    [Test]
    public async Task FailingFinalExam_DoesNotCompleteOrCertify()
    {
        var progress = new UserProgress { CourseId = "moby", UserId = "student-3" };
        foreach (var id in new[] { "s1a", "s1b", "s2a" }) progress.MarkSectionComplete(id);
        Assert.That(exam.IsEligible(structure, progress), Is.True);

        var failed = new QuizResult
        {
            Id = "final-fail", CourseId = "moby", IsFinalExam = true,
            PassingScore = FinalExamService.PassingScore,
            TotalQuestions = 10, CorrectAnswers = 7, // 70% < 80%
        };
        Assert.That(exam.ApplyResult(progress, failed), Is.False);
        Assert.That(progress.HasCompletedCourse, Is.False);
        Assert.That(await certificates.GetForCourseAsync("student-3", "moby"), Is.Null);
    }
}
