namespace Tutor.Core.Models;

/// <summary>
/// A certificate of completion issued when a student passes a course's final exam.
///
/// <para>It carries a <see cref="CourseContentHash"/> (a hash of the exact course structure) so
/// the credential is bound to the specific course content, and reserves <see cref="Signature"/>
/// / <see cref="SigningKeyId"/> for the (currently deferred) cryptographic signature. Until a
/// signing authority is wired in, certificates are issued unsigned — the data model and the
/// content binding are already in place so signing drops in without a schema change.</para>
/// </summary>
public class CourseCertificate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>Human-friendly unique certificate number (e.g. TUTOR-20260530-AB12CD34).</summary>
    public string CertificateNumber { get; set; } = "";

    public string UserId { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string CourseId { get; set; } = "";
    public string CourseName { get; set; } = "";

    /// <summary>Final-exam score (0-100) the certificate was awarded for.</summary>
    public int FinalScorePct { get; set; }

    /// <summary>The passing final-exam result this certificate attests to.</summary>
    public string FinalExamResultId { get; set; } = "";

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// SHA-256 (hex) of the course structure at issue time — binds the certificate to the exact
    /// course content so it can't be transplanted to a different course, and is the payload a
    /// future signature will cover.
    /// </summary>
    public string CourseContentHash { get; set; } = "";

    /// <summary>Cryptographic signature over the certificate payload. Null until signing is added (deferred).</summary>
    public string? Signature { get; set; }

    /// <summary>Identifier of the key that produced <see cref="Signature"/>, once signing exists.</summary>
    public string? SigningKeyId { get; set; }

    /// <summary>True once a cryptographic signature has been applied (verifiable credential).</summary>
    public bool IsSigned => !string.IsNullOrEmpty(Signature);
}
