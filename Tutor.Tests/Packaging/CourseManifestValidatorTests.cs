using Tutor.Core.Services.Packaging;

namespace Tutor.Tests.Packaging;

/// <summary>
/// Pins the IO-free validation pass: a malformed, tampered, or too-new bundle
/// is rejected up front with an explicit code, while legacy bundles (no key,
/// no hash) pass with warnings instead of errors.
/// </summary>
public class CourseManifestValidatorTests
{
    private static BundleManifest ValidManifest() => new()
    {
        FormatVersion = 1,
        CourseId = Guid.NewGuid().ToString(),
        CourseName = "Dracula",
        CourseKey = "dracula",
        CourseVersion = 1,
        Sha256 = new string('a', 64),
    };

    private static readonly string[] ValidEntries = ["manifest.json", "course.json", "chunks.json"];

    [Test]
    public void ValidManifest_PassesWithoutErrors()
    {
        var result = CourseManifestValidator.Validate(ValidManifest(), ValidEntries,
            computedPayloadSha256: new string('a', 64));

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Warnings, Is.Empty);
    }

    [Test]
    public void MissingManifest_FailsWithNoManifest()
    {
        var result = CourseManifestValidator.Validate(null, ValidEntries);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.NoManifest));
    }

    [Test]
    public void FormatNewerThanHost_FailsWithFormatTooNew()
    {
        var manifest = ValidManifest();
        manifest.FormatVersion = BundleManifest.HostMaxFormatVersion + 1;

        var result = CourseManifestValidator.Validate(manifest, ValidEntries);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.FormatTooNew));
    }

    [Test]
    public void FormatOlderThanHost_IsAccepted()
    {
        // The forgiving gate: only formats NEWER than the build are refused.
        var manifest = ValidManifest();
        manifest.FormatVersion = 0;

        var result = CourseManifestValidator.Validate(manifest, ValidEntries);

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase("UPPERCASE")]
    [TestCase("-starts-with-dash")]
    [TestCase("has spaces")]
    [TestCase("nope/slash")]
    public void MalformedKey_FailsWithBadKey(string key)
    {
        var manifest = ValidManifest();
        manifest.CourseKey = key;

        var result = CourseManifestValidator.Validate(manifest, ValidEntries);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.BadKey));
    }

    [Test]
    public void EmptyKey_IsLegacyWarning_NotError()
    {
        var manifest = ValidManifest();
        manifest.CourseKey = "";

        var result = CourseManifestValidator.Validate(manifest, ValidEntries,
            computedPayloadSha256: manifest.Sha256);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Warnings.Select(w => w.Code), Does.Contain(ValidationCodes.LegacyNoKey));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositiveVersion_FailsWithBadVersion(int version)
    {
        var manifest = ValidManifest();
        manifest.CourseVersion = version;

        var result = CourseManifestValidator.Validate(manifest, ValidEntries);

        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.BadVersion));
    }

    [Test]
    public void MissingName_FailsWithNoName()
    {
        var manifest = ValidManifest();
        manifest.CourseName = " ";

        var result = CourseManifestValidator.Validate(manifest, ValidEntries);

        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.NoName));
    }

    [Test]
    public void MissingCourseJson_FailsWithMissingCourseJson()
    {
        var result = CourseManifestValidator.Validate(ValidManifest(), ["manifest.json", "chunks.json"]);

        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.MissingCourseJson));
    }

    [Test]
    public void ShaMismatch_FailsWithShaMismatch()
    {
        var result = CourseManifestValidator.Validate(ValidManifest(), ValidEntries,
            computedPayloadSha256: new string('b', 64));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.ShaMismatch));
    }

    [Test]
    public void MissingSha_IsLegacyWarning_NotError()
    {
        var manifest = ValidManifest();
        manifest.Sha256 = "";

        var result = CourseManifestValidator.Validate(manifest, ValidEntries,
            computedPayloadSha256: new string('b', 64));

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Warnings.Select(w => w.Code), Does.Contain(ValidationCodes.LegacyNoSha));
    }

    [TestCase("../escape.json")]
    [TestCase("/rooted.json")]
    [TestCase("C:/windows/evil.json")]
    [TestCase(@"resources\backslash.json")]
    public void UnsafeEntryPath_FailsWithUnsafeEntryPath(string entry)
    {
        var entries = new[] { "manifest.json", "course.json", entry };

        var result = CourseManifestValidator.Validate(ValidManifest(), entries);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Select(e => e.Code), Does.Contain(ValidationCodes.UnsafeEntryPath));
    }
}
