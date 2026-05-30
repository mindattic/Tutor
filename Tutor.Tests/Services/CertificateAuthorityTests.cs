using NUnit.Framework;
using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class CertificateAuthorityTests
{
    private static CertificateRequest Request() => new(
        UserId: "u1",
        StudentName: "Ada Lovelace",
        CourseId: "c1",
        CourseName: "Moby Dick",
        FinalScorePct: 92,
        FinalExamResultId: "exam-1",
        CourseContentHash: "abc123");

    [Test]
    public void Issue_ProducesUnsignedCertificate_WithRequestData()
    {
        var cert = new LocalCertificateAuthority().Issue(Request());

        Assert.That(cert.StudentName, Is.EqualTo("Ada Lovelace"));
        Assert.That(cert.CourseName, Is.EqualTo("Moby Dick"));
        Assert.That(cert.FinalScorePct, Is.EqualTo(92));
        Assert.That(cert.FinalExamResultId, Is.EqualTo("exam-1"));
        Assert.That(cert.CourseContentHash, Is.EqualTo("abc123"));
        Assert.That(cert.CertificateNumber, Does.StartWith("TUTOR-"));

        // Signing is deferred — the cert is unsigned, and the seam fields are null.
        Assert.That(cert.IsSigned, Is.False);
        Assert.That(cert.Signature, Is.Null);
        Assert.That(cert.SigningKeyId, Is.Null);
    }

    [Test]
    public void Issue_GivesEachCertificate_AUniqueNumber()
    {
        var ca = new LocalCertificateAuthority();
        var a = ca.Issue(Request());
        var b = ca.Issue(Request());
        Assert.That(a.CertificateNumber, Is.Not.EqualTo(b.CertificateNumber));
        Assert.That(a.Id, Is.Not.EqualTo(b.Id));
    }
}
