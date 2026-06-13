using System.Text.RegularExpressions;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// Pure, IO-free validation of a bundle manifest + its archive entry listing.
/// Runs before any persistence is touched so a malformed or tampered .tutor is
/// rejected up front with an explicit code, not deep inside a service call.
/// Hard errors block the install; warnings (legacy bundles missing key/hash)
/// don't.
/// </summary>
public static class CourseManifestValidator
{
    /// <summary>Pattern a stable course key must match: lowercase slug, max 120 chars.</summary>
    public static readonly Regex CourseKeyPattern = new("^[a-z0-9][a-z0-9._-]{0,119}$", RegexOptions.Compiled);

    public static ValidationResult Validate(
        BundleManifest? manifest,
        IReadOnlyCollection<string> entryNames,
        int hostMaxFormat = BundleManifest.HostMaxFormatVersion,
        string? computedPayloadSha256 = null)
    {
        var result = new ValidationResult();

        if (manifest == null)
        {
            result.AddError(ValidationCodes.NoManifest, "Bundle is missing manifest.json.");
            return result;
        }

        if (manifest.FormatVersion > hostMaxFormat)
        {
            result.AddError(ValidationCodes.FormatTooNew,
                $"Bundle format {manifest.FormatVersion} is newer than this build supports ({hostMaxFormat}). Update Tutor.");
        }

        if (string.IsNullOrWhiteSpace(manifest.CourseName))
            result.AddError(ValidationCodes.NoName, "Manifest has no course name.");

        if (string.IsNullOrEmpty(manifest.CourseKey))
        {
            result.AddWarning(ValidationCodes.LegacyNoKey,
                "Legacy bundle: manifest carries no stable CourseKey; one will be derived from the course name.");
        }
        else if (!CourseKeyPattern.IsMatch(manifest.CourseKey))
        {
            result.AddError(ValidationCodes.BadKey,
                $"CourseKey '{manifest.CourseKey}' is not a valid slug (expected {CourseKeyPattern}).");
        }

        if (manifest.CourseVersion < 1)
            result.AddError(ValidationCodes.BadVersion,
                $"CourseVersion {manifest.CourseVersion} is invalid; versions are whole numbers starting at 1.");

        if (!entryNames.Contains("course.json"))
            result.AddError(ValidationCodes.MissingCourseJson, "Bundle is missing course.json.");

        foreach (var name in entryNames)
        {
            if (!BundleArchiveSafety.IsSafeEntryPath(name))
                result.AddError(ValidationCodes.UnsafeEntryPath,
                    $"Bundle entry '{name}' has an unsafe path (rooted, drive-letter, or '..' escape).");
        }

        if (string.IsNullOrEmpty(manifest.Sha256))
        {
            result.AddWarning(ValidationCodes.LegacyNoSha,
                "Legacy bundle: manifest carries no SHA-256, so payload integrity cannot be verified.");
        }
        else if (computedPayloadSha256 != null &&
                 !string.Equals(manifest.Sha256, computedPayloadSha256, StringComparison.OrdinalIgnoreCase))
        {
            result.AddError(ValidationCodes.ShaMismatch,
                "Bundle payload does not match its manifest SHA-256 — the file is corrupted or has been tampered with.");
        }

        return result;
    }
}

public static class ValidationCodes
{
    public const string NoManifest = "NO_MANIFEST";
    public const string BadKey = "BAD_KEY";
    public const string BadVersion = "BAD_VERSION";
    public const string NoName = "NO_NAME";
    public const string FormatTooNew = "FORMAT_TOO_NEW";
    public const string ShaMismatch = "SHA_MISMATCH";
    public const string MissingCourseJson = "MISSING_COURSE_JSON";
    public const string UnsafeEntryPath = "UNSAFE_ENTRY_PATH";
    public const string LegacyNoKey = "LEGACY_NO_KEY";
    public const string LegacyNoSha = "LEGACY_NO_SHA";
}

public sealed class ValidationIssue
{
    public ValidationIssue(string code, string message)
    {
        this.Code = code;
        this.Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public override string ToString() => $"[{Code}] {Message}";
}

public sealed class ValidationResult
{
    public List<ValidationIssue> Errors { get; } = new();
    public List<ValidationIssue> Warnings { get; } = new();
    public bool IsValid => Errors.Count == 0;

    public void AddError(string code, string message) => Errors.Add(new ValidationIssue(code, message));
    public void AddWarning(string code, string message) => Warnings.Add(new ValidationIssue(code, message));
}
