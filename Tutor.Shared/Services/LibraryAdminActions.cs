using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MindAttic.Authentication;
using Tutor.Core.Services.Packaging;

namespace Tutor.Shared.Services;

/// <summary>
/// The Course Library's state-changing actions (preview/install a bundle, load, unload, remove), each
/// authorized on the server against <see cref="MaPolicies.Admin"/> before it touches anything. The page
/// hides these controls from non-admins, but hiding is presentation: a Blazor Server event handler runs
/// with whatever principal the circuit carries, so the check lives here, where the work is done.
/// </summary>
public sealed class LibraryAdminActions
{
    private readonly IAuthorizationService authorization;
    private readonly CourseInstallService installService;

    public LibraryAdminActions(IAuthorizationService authorization, CourseInstallService installService)
    {
        this.authorization = authorization;
        this.installService = installService;
    }

    /// <summary>True when <paramref name="user"/> satisfies the Admin policy.</summary>
    public async Task<bool> IsAdminAsync(ClaimsPrincipal user) =>
        (await authorization.AuthorizeAsync(user, resource: null, MaPolicies.Admin)).Succeeded;

    /// <summary>Throws <see cref="UnauthorizedAccessException"/> unless <paramref name="user"/> is an admin.</summary>
    public async Task EnsureAdminAsync(ClaimsPrincipal user)
    {
        if (!await IsAdminAsync(user))
            throw new UnauthorizedAccessException("Only an admin can change the course library.");
    }

    public async Task<CourseInstallPreview> PreviewAsync(ClaimsPrincipal user, string bundlePath, bool allowDuplicate, CancellationToken ct = default)
    {
        await EnsureAdminAsync(user);
        return await installService.PreviewAsync(bundlePath, allowDuplicate, ct);
    }

    public async Task<CourseInstallOutcome> InstallAsync(ClaimsPrincipal user, string bundlePath, bool allowDuplicate, CancellationToken ct = default)
    {
        await EnsureAdminAsync(user);
        return await installService.InstallAsync(bundlePath, overrideCourseName: null, allowDuplicate, ct);
    }

    public async Task<bool> LoadAsync(ClaimsPrincipal user, string courseId, CancellationToken ct = default)
    {
        await EnsureAdminAsync(user);
        return await installService.LoadAsync(courseId, ct);
    }

    public async Task<bool> UnloadAsync(ClaimsPrincipal user, string courseId, CancellationToken ct = default)
    {
        await EnsureAdminAsync(user);
        return await installService.UnloadAsync(courseId, ct);
    }

    public async Task<CourseDeletePlan?> RemoveAsync(ClaimsPrincipal user, string courseId, CancellationToken ct = default)
    {
        await EnsureAdminAsync(user);
        return await installService.RemoveAsync(courseId, ct);
    }
}
