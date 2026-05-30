using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Issues and persists course-completion certificates. Mirrors the per-user JSON storage used by
/// <see cref="LocalQuizController"/> for quiz results (one file per user under
/// <c>{AppData}/Certificates/</c>). Signing is delegated to <see cref="ICertificateAuthority"/>
/// (unsigned today).
/// </summary>
public sealed class CertificateService
{
    private readonly ICertificateAuthority authority;
    private readonly CourseStructureStorageService structureStorage;
    private readonly string certFolder;
    private readonly SemaphoreSlim fileLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public CertificateService(
        ICertificateAuthority authority,
        CourseStructureStorageService structureStorage,
        IAppDataPathProvider pathProvider)
    {
        this.authority = authority;
        this.structureStorage = structureStorage;
        certFolder = Path.Combine(pathProvider.AppDataDirectory, "Certificates");
        Directory.CreateDirectory(certFolder);
    }

    /// <summary>
    /// Issues (and persists) a completion certificate for a passing final-exam result, binding it
    /// to the course's content hash. One certificate per course per user — re-issuing replaces.
    /// </summary>
    public async Task<CourseCertificate> IssueAsync(
        string userId, string studentName, Course course, QuizResult finalExam, CancellationToken ct = default)
    {
        var hash = await ComputeCourseContentHashAsync(course.Id, ct);
        var cert = authority.Issue(new CertificateRequest(
            UserId: userId,
            StudentName: studentName,
            CourseId: course.Id,
            CourseName: course.Name,
            FinalScorePct: (int)Math.Round(finalExam.ScorePercentage),
            FinalExamResultId: finalExam.Id,
            CourseContentHash: hash));
        await SaveAsync(cert);
        return cert;
    }

    /// <summary>The student's certificate for a course, or null if none has been issued.</summary>
    public async Task<CourseCertificate?> GetForCourseAsync(string userId, string courseId)
        => (await LoadAsync(userId)).FirstOrDefault(c => c.CourseId == courseId);

    /// <summary>All certificates the student has earned.</summary>
    public Task<List<CourseCertificate>> GetAllAsync(string userId) => LoadAsync(userId);

    /// <summary>
    /// Stable SHA-256 (hex) of the course structure — the content binding (and future signature
    /// payload). Returns empty when no structure exists.
    /// </summary>
    public async Task<string> ComputeCourseContentHashAsync(string courseId, CancellationToken ct = default)
    {
        var structure = await structureStorage.LoadByCourseIdAsync(courseId, ct);
        if (structure == null) return "";
        var json = JsonSerializer.Serialize(structure);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private string PathFor(string userId) => Path.Combine(certFolder, $"Certificates-{userId}.json");

    private async Task SaveAsync(CourseCertificate cert)
    {
        await fileLock.WaitAsync();
        try
        {
            var all = await LoadAsync(cert.UserId);
            all.RemoveAll(c => c.CourseId == cert.CourseId); // one per course; latest wins
            all.Add(cert);
            await File.WriteAllTextAsync(PathFor(cert.UserId), JsonSerializer.Serialize(all, JsonOptions));
        }
        finally
        {
            fileLock.Release();
        }
    }

    private async Task<List<CourseCertificate>> LoadAsync(string userId)
    {
        var path = PathFor(userId);
        if (!File.Exists(path)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<CourseCertificate>>(await File.ReadAllTextAsync(path)) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
