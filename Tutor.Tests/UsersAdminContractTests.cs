using System.Reflection;
using MindAttic.Authentication.Services;

namespace Tutor.Tests;

/// <summary>
/// The admin Users page drives user management through <see cref="IUserAdminService"/>. Owner rule:
/// users are SOFT-DISABLED (IsActive=false), never hard-deleted — otherwise per-user progress/data
/// keyed on the user would orphan. These tests pin that contract at the app boundary. (The
/// last-active-admin guard behavior itself is unit-tested inside MindAttic.Authentication.)
/// </summary>
[TestFixture]
public class UsersAdminContractTests
{
    private static readonly string[] Methods =
        typeof(IUserAdminService).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.Name).ToArray();

    [Test]
    public void ExposesNoHardDeleteApi()
    {
        var offenders = Methods.Where(n =>
            n.Contains("Delete", StringComparison.OrdinalIgnoreCase) ||
            n.Contains("Remove", StringComparison.OrdinalIgnoreCase) ||
            n.Contains("Purge", StringComparison.OrdinalIgnoreCase)).ToArray();
        Assert.That(offenders, Is.Empty,
            "IUserAdminService must not expose a hard-delete; deactivation is SetActiveAsync(IsActive=false).");
    }

    [Test]
    public void ExposesSoftDisableAndCrud()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Methods, Does.Contain(nameof(IUserAdminService.SetActiveAsync)));
            Assert.That(Methods, Does.Contain(nameof(IUserAdminService.CreateAsync)));
            Assert.That(Methods, Does.Contain(nameof(IUserAdminService.SetRoleAsync)));
            Assert.That(Methods, Does.Contain(nameof(IUserAdminService.UpdateProfileAsync)));
            Assert.That(Methods, Does.Contain(nameof(IUserAdminService.ResetPasswordAsync)));
            Assert.That(Methods, Does.Contain(nameof(IUserAdminService.ListAsync)));
        });
    }
}
