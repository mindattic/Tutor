namespace Tutor.Core.Services.Packaging;

/// <summary>
/// Pure decision step for "what should installing this bundle do?", given what
/// is already installed. No IO, no persistence — trivially unit-testable. The
/// downgrade rule: a lower CourseVersion for an existing CourseKey is rejected
/// unless the caller explicitly asks for a side-by-side duplicate.
/// </summary>
public static class CourseInstallResolver
{
    public static InstallPlan Plan(
        string courseKey,
        int courseVersion,
        IEnumerable<InstalledCourse> installed,
        bool allowDuplicate = false)
    {
        var existing = installed
            .Where(c => string.Equals(c.CourseKey, courseKey, StringComparison.Ordinal))
            .OrderByDescending(c => c.CourseVersion)
            .FirstOrDefault();

        if (existing == null)
            return new InstallPlan(InstallPlanKind.Install, null,
                $"'{courseKey}' v{courseVersion} is not installed; clean install.");

        if (allowDuplicate)
            return new InstallPlan(InstallPlanKind.InstallSideBySide, existing,
                $"'{courseKey}' v{existing.CourseVersion} is already installed; installing v{courseVersion} side-by-side as requested.");

        if (courseVersion == existing.CourseVersion)
            return new InstallPlan(InstallPlanKind.NoOpAlreadyInstalled, existing,
                $"'{courseKey}' v{courseVersion} is already installed; nothing to do.");

        if (courseVersion < existing.CourseVersion)
            return new InstallPlan(InstallPlanKind.RejectDowngrade, existing,
                $"'{courseKey}' v{existing.CourseVersion} is installed; refusing downgrade to v{courseVersion}. " +
                "Install as a duplicate if you really want the older version side-by-side.");

        return new InstallPlan(InstallPlanKind.Upgrade, existing,
            $"'{courseKey}' v{existing.CourseVersion} is installed; v{courseVersion} installs as an upgrade alongside it.");
    }
}

public enum InstallPlanKind
{
    /// <summary>Key not present — clean install.</summary>
    Install,

    /// <summary>Same key, higher version — install the newer course.</summary>
    Upgrade,

    /// <summary>Same key, same version — skip.</summary>
    NoOpAlreadyInstalled,

    /// <summary>Same key, lower version — refused unless explicitly duplicated.</summary>
    RejectDowngrade,

    /// <summary>Caller explicitly opted into a duplicate of an installed key.</summary>
    InstallSideBySide,
}

public sealed record InstallPlan(InstallPlanKind Kind, InstalledCourse? Existing, string Reason)
{
    /// <summary>True when the plan calls for actually importing the bundle.</summary>
    public bool ShouldInstall => Kind is InstallPlanKind.Install or InstallPlanKind.Upgrade or InstallPlanKind.InstallSideBySide;
}
