using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MindAttic.Authentication;
using Tutor.Blazor.Hosting;
using Tutor.Core.Services;
using Tutor.Shared.Services;

namespace Tutor.Tests.Services;

/// <summary>
/// Author policy enforced on the server (TUT-T59, plan item 0.2(a)/(c)): a Student is refused every authoring
/// action, including Auto-Link Orphans and chat-driven concept expansion; an Admin is allowed.
/// </summary>
[TestFixture]
public sealed class CourseAuthoringActionsTests
{
    private ServiceProvider _sp = null!;
    private CourseAuthoringActions _actions = null!;

    [OneTimeSetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        TutorAccess.ConfigurePolicies(services.AddAuthorizationBuilder());
        _sp = services.BuildServiceProvider();

        // The linker and storage are never reached for a refused user; the allowed path is covered with no maps.
        _actions = new CourseAuthoringActions(
            _sp.GetRequiredService<IAuthorizationService>(),
            orphanLinker: null!,
            conceptMapStorage: null!,
            TestLamp.Settings());
    }

    [OneTimeTearDown]
    public void TearDown() => _sp.Dispose();

    private static ClaimsPrincipal User(string role) =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.Name, role.ToLowerInvariant()), new Claim(ClaimTypes.Role, role)], "test"));

    [Test]
    public async Task Student_IsNotAnAuthor()
    {
        Assert.That(await _actions.IsAuthorAsync(User("Student")), Is.False);
        Assert.That(await _actions.CanExpandSharedConceptMapsAsync(User("Student")), Is.False);
        Assert.That(await _actions.IsAuthorAsync(null), Is.False);
    }

    [Test]
    public async Task Admin_IsAnAuthor()
    {
        Assert.That(await _actions.IsAuthorAsync(User(MaRoles.Admin)), Is.True);
        Assert.That(await _actions.CanExpandSharedConceptMapsAsync(User(MaRoles.Admin)), Is.True);
    }

    [Test]
    public void Student_IsRefusedAutoLinkOrphans()
    {
        Assert.ThrowsAsync<UnauthorizedAccessException>(() => _actions.LinkOrphansAsync(User("Student"), []));
        Assert.ThrowsAsync<UnauthorizedAccessException>(() => _actions.EnsureAuthorAsync(User("Student")));
    }

    [Test]
    public async Task Admin_MayRunAutoLinkOrphans()
    {
        Assert.That(await _actions.LinkOrphansAsync(User(MaRoles.Admin), []), Is.EqualTo(0));
    }
}
