using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>Inputs needed to mint a <see cref="CourseCertificate"/>.</summary>
public sealed record CertificateRequest(
    string UserId,
    string StudentName,
    string CourseId,
    string CourseName,
    int FinalScorePct,
    string FinalExamResultId,
    string CourseContentHash);

/// <summary>
/// Issues completion certificates. This is the seam for the (deferred) cryptographic signing:
/// the default <see cref="LocalCertificateAuthority"/> mints an <em>unsigned</em> certificate;
/// a future signing authority will implement this same interface and populate
/// <see cref="CourseCertificate.Signature"/>/<see cref="CourseCertificate.SigningKeyId"/> with no
/// change to callers.
/// </summary>
public interface ICertificateAuthority
{
    CourseCertificate Issue(CertificateRequest request);
}

/// <summary>
/// Default authority — produces an unsigned certificate with a unique number and the course
/// content hash. Verifiable signing is intentionally out of scope here (on the back burner).
/// </summary>
public sealed class LocalCertificateAuthority : ICertificateAuthority
{
    public CourseCertificate Issue(CertificateRequest request) => new()
    {
        CertificateNumber = GenerateNumber(),
        UserId = request.UserId,
        StudentName = request.StudentName,
        CourseId = request.CourseId,
        CourseName = request.CourseName,
        FinalScorePct = request.FinalScorePct,
        FinalExamResultId = request.FinalExamResultId,
        CourseContentHash = request.CourseContentHash,
        IssuedAt = DateTime.UtcNow,
        // Signature / SigningKeyId intentionally left null — cryptographic signing is deferred.
    };

    private static string GenerateNumber()
        => $"TUTOR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
}
