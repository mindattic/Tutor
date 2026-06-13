using System.Text;
using Tutor.Core.Services.Packaging;

namespace Tutor.Tests.Packaging;

/// <summary>
/// Pins the zip-slip guard (a shared .tutor is untrusted input) and the
/// canonical payload hash used for bundle integrity.
/// </summary>
public class BundleArchiveSafetyTests
{
    [TestCase("manifest.json")]
    [TestCase("course.json")]
    [TestCase("resources/abc.original.txt")]
    [TestCase("conceptMaps/some-guid.json")]
    public void SafePaths_AreAccepted(string entry)
    {
        Assert.That(BundleArchiveSafety.IsSafeEntryPath(entry), Is.True);
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("../outside.json")]
    [TestCase("resources/../../outside.json")]
    [TestCase("/rooted.json")]
    [TestCase("C:/windows/system32/evil.dll")]
    [TestCase(@"resources\windows-separator.json")]
    [TestCase("resources//double-slash.json")]
    [TestCase("resources/./dot-segment.json")]
    [TestCase("trailing/")]
    public void UnsafePaths_AreRejected(string entry)
    {
        Assert.That(BundleArchiveSafety.IsSafeEntryPath(entry), Is.False);
    }

    [Test]
    public void PayloadSha_IsIndependentOfEntryOrder()
    {
        var a = ("a.json", Encoding.UTF8.GetBytes("alpha"));
        var b = ("b.json", Encoding.UTF8.GetBytes("beta"));

        var forward = BundleArchiveSafety.ComputePayloadSha256([a, b]);
        var reversed = BundleArchiveSafety.ComputePayloadSha256([b, a]);

        Assert.That(reversed, Is.EqualTo(forward));
        Assert.That(forward, Has.Length.EqualTo(64));
    }

    [Test]
    public void PayloadSha_ChangesWhenContentChanges()
    {
        var original = BundleArchiveSafety.ComputePayloadSha256([("a.json", Encoding.UTF8.GetBytes("alpha"))]);
        var tampered = BundleArchiveSafety.ComputePayloadSha256([("a.json", Encoding.UTF8.GetBytes("alphb"))]);

        Assert.That(tampered, Is.Not.EqualTo(original));
    }

    [Test]
    public void PayloadSha_ChangesWhenEntryRenamed()
    {
        var original = BundleArchiveSafety.ComputePayloadSha256([("a.json", Encoding.UTF8.GetBytes("alpha"))]);
        var renamed = BundleArchiveSafety.ComputePayloadSha256([("b.json", Encoding.UTF8.GetBytes("alpha"))]);

        Assert.That(renamed, Is.Not.EqualTo(original));
    }
}
