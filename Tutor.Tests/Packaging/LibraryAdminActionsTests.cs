using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MindAttic.Authentication;
using Tutor.Core.Services.Packaging;
using Tutor.Shared.Services;

namespace Tutor.Tests.Packaging;

/// <summary>
/// The Course Library's Load / Unload / Remove / Install handlers are authorized on the server: a call
/// from a non-admin principal is refused before it changes anything, whatever the page renders. The
/// Admin policy is registered exactly as MindAttic.Authentication does with MFA off (role only), which
/// is Tutor's configuration.
/// </summary>
public class LibraryAdminActionsTests
{
    private PackagingTestHost host = null!;
    private ServiceProvider authz = null!;
    private LibraryAdminActions actions = null!;

    [SetUp]
    public void SetUp()
    {
        host = new PackagingTestHost();
        authz = new ServiceCollection()
            .AddLogging()
            .AddAuthorizationCore(o => o.AddPolicy(MaPolicies.Admin, p => p.RequireRole(MaRoles.Admin)))
            .BuildServiceProvider();
        actions = new LibraryAdminActions(authz.GetRequiredService<IAuthorizationService>(), host.Get<CourseInstallService>());
    }

    [TearDown]
    public void TearDown()
    {
        authz.Dispose();
        host.Dispose();
    }

    private static ClaimsPrincipal Principal(string? role) => role == null
        ? new ClaimsPrincipal(new ClaimsIdentity())   // anonymous
        : new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "someone"), new Claim(ClaimTypes.Role, role)], "test"));

    private string BundlePath => Path.Combine(host.Sandbox, "bundle.tutor");

    private async Task<string> InstallSeededCourseAsync()
    {
        var course = await host.SeedCourseAsync("Dracula");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath, new BundleExportOptions { CourseVersion = 1 });
        var outcome = await host.Get<CourseInstallService>().InstallAsync(BundlePath, "Dracula (installed)");
        Assert.That(outcome.DidInstall, Is.True);
        return outcome.Installed!.CourseId;
    }

    [TestCase("User")]
    [TestCase(null)]
    public async Task NonAdmin_LoadUnloadRemove_AreRefused_AndChangeNothing(string? role)
    {
        var courseId = await InstallSeededCourseAsync();
        var registry = host.Get<InstalledCourseRegistry>();
        var user = Principal(role);

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => actions.UnloadAsync(user, courseId));
        Assert.That((await registry.GetByCourseIdAsync(courseId))!.Enabled, Is.True, "unload must not run");

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => actions.RemoveAsync(user, courseId));
        Assert.That(await registry.GetByCourseIdAsync(courseId), Is.Not.Null, "remove must not run");

        await host.Get<CourseInstallService>().UnloadAsync(courseId);
        Assert.ThrowsAsync<UnauthorizedAccessException>(() => actions.LoadAsync(user, courseId));
        Assert.That((await registry.GetByCourseIdAsync(courseId))!.Enabled, Is.False, "load must not run");
    }

    [Test]
    public async Task NonAdmin_PreviewAndInstall_AreRefused_AndInstallNothing()
    {
        var course = await host.SeedCourseAsync("Carmilla");
        await host.Get<CourseExporter>().ExportAsync(course.Id, BundlePath, new BundleExportOptions { CourseVersion = 1 });
        var user = Principal("User");

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => actions.PreviewAsync(user, BundlePath, allowDuplicate: false));
        Assert.ThrowsAsync<UnauthorizedAccessException>(() => actions.InstallAsync(user, BundlePath, allowDuplicate: false));
        Assert.That(await host.Get<InstalledCourseRegistry>().GetAllAsync(), Is.Empty);
    }

    [Test]
    public async Task Admin_CanUnloadLoadAndRemove()
    {
        var courseId = await InstallSeededCourseAsync();
        var registry = host.Get<InstalledCourseRegistry>();
        var admin = Principal(MaRoles.Admin);

        Assert.That(await actions.IsAdminAsync(admin), Is.True);
        Assert.That(await actions.UnloadAsync(admin, courseId), Is.True);
        Assert.That((await registry.GetByCourseIdAsync(courseId))!.Enabled, Is.False);
        Assert.That(await actions.LoadAsync(admin, courseId), Is.True);
        Assert.That((await registry.GetByCourseIdAsync(courseId))!.Enabled, Is.True);
        await actions.RemoveAsync(admin, courseId);
        Assert.That(await registry.GetByCourseIdAsync(courseId), Is.Null);
    }
}
