using Tutor.Core.Services.Packaging;

namespace Tutor.Tests.Packaging;

/// <summary>
/// Pins the pure install-plan decision: clean install, no-op on the same
/// version, refused downgrade, upgrade on a higher version, and explicit
/// side-by-side duplication — all without a database.
/// </summary>
public class CourseInstallResolverTests
{
    private static InstalledCourse Installed(string key, int version) => new()
    {
        CourseId = Guid.NewGuid().ToString(),
        CourseKey = key,
        CourseVersion = version,
        Name = key,
        InstalledUtc = DateTime.UtcNow,
    };

    [Test]
    public void UnknownKey_PlansCleanInstall()
    {
        var plan = CourseInstallResolver.Plan("dracula", 1, [Installed("frankenstein", 1)]);

        Assert.That(plan.Kind, Is.EqualTo(InstallPlanKind.Install));
        Assert.That(plan.Existing, Is.Null);
        Assert.That(plan.ShouldInstall, Is.True);
    }

    [Test]
    public void SameKeySameVersion_PlansNoOp()
    {
        var existing = Installed("dracula", 2);

        var plan = CourseInstallResolver.Plan("dracula", 2, [existing]);

        Assert.That(plan.Kind, Is.EqualTo(InstallPlanKind.NoOpAlreadyInstalled));
        Assert.That(plan.Existing, Is.SameAs(existing));
        Assert.That(plan.ShouldInstall, Is.False);
    }

    [Test]
    public void LowerVersion_PlansRejectDowngrade()
    {
        var plan = CourseInstallResolver.Plan("dracula", 1, [Installed("dracula", 2)]);

        Assert.That(plan.Kind, Is.EqualTo(InstallPlanKind.RejectDowngrade));
        Assert.That(plan.ShouldInstall, Is.False);
    }

    [Test]
    public void HigherVersion_PlansUpgrade()
    {
        var plan = CourseInstallResolver.Plan("dracula", 3, [Installed("dracula", 2)]);

        Assert.That(plan.Kind, Is.EqualTo(InstallPlanKind.Upgrade));
        Assert.That(plan.ShouldInstall, Is.True);
    }

    [Test]
    public void AllowDuplicate_PlansSideBySide_EvenForSameVersion()
    {
        var plan = CourseInstallResolver.Plan("dracula", 2, [Installed("dracula", 2)], allowDuplicate: true);

        Assert.That(plan.Kind, Is.EqualTo(InstallPlanKind.InstallSideBySide));
        Assert.That(plan.ShouldInstall, Is.True);
    }

    [Test]
    public void MultipleInstalledVersions_ComparesAgainstHighest()
    {
        var installed = new[] { Installed("dracula", 1), Installed("dracula", 3) };

        var plan = CourseInstallResolver.Plan("dracula", 2, installed);

        Assert.That(plan.Kind, Is.EqualTo(InstallPlanKind.RejectDowngrade));
        Assert.That(plan.Existing!.CourseVersion, Is.EqualTo(3));
    }
}
