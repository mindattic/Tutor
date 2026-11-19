using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;
using MindAttic.Authentication;
using MindAttic.Authentication.Web;
using Tutor.Blazor.Components;
using Tutor.Core.Data;
using Tutor.Core.Services;
using Tutor.Core.Services.Packaging;
using Tutor.Shared.Services;

namespace Tutor.Blazor.Hosting;

/// <summary>
/// Default-deny access for the hosted site (TUT-T52, plan item 0.1). Every endpoint and every routable
/// component requires a signed-in user through the fallback policy; only the paths on
/// <see cref="AnonymousRoutes"/> opt out, and each of those is marked <c>[AllowAnonymous]</c> where it is
/// declared. The Blazor hub gets no exemption, so an anonymous <c>POST /_blazor/negotiate</c> is refused.
/// <c>EndpointAuthorizationTests</c> fails if an anonymous endpoint or page appears that is not listed here.
/// </summary>
public static class TutorAccess
{
    /// <summary>The anonymous /_ma-auth posts (the library's change-password post stays signed-in only).</summary>
    public static readonly IReadOnlyList<string> AnonymousAuthPosts =
        ["login", "mfa-challenge", "logout", "reset/request", "reset/confirm"];

    /// <summary>Every route a signed-out visitor may reach (route templates, no leading slash).</summary>
    public static readonly IReadOnlyList<string> AnonymousRoutes =
    [
        "login", "forgot-password", "account/reset", "not-found", "mfa",
        "healthz", "healthz/db",
        .. AnonymousAuthPosts.Select(p => "_ma-auth/" + p),
    ];

    /// <summary>Route prefixes of framework static assets that may be served as endpoints (blazor.web.js).</summary>
    public static readonly IReadOnlyList<string> AnonymousRoutePrefixes = ["_framework/"];

    /// <summary>True when <paramref name="routeTemplate"/> is on the anonymous allow-list.</summary>
    public static bool IsAllowListed(string? routeTemplate)
    {
        var route = (routeTemplate ?? "").Trim('/');
        return AnonymousRoutes.Contains(route, StringComparer.OrdinalIgnoreCase)
            || AnonymousRoutePrefixes.Any(p => route.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Rate-limit policy on the /_ma-auth group: per client address (TUT-T59).</summary>
    public const string SignInRateLimitPolicy = "tutor-sign-in";

    /// <summary>Rate-limit policy on the bundle download: per signed-in user (TUT-T59).</summary>
    public const string BundleDownloadRateLimitPolicy = "tutor-bundle-download";

    /// <summary>The app's authorization policies: the signed-in fallback plus Tutor's own named policies.</summary>
    public static void ConfigurePolicies(AuthorizationBuilder authz)
    {
        authz.SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        // Author (TUT-T59): course content changes and the spending that goes with them. Admin only to start.
        authz.AddPolicy(TutorPolicies.Author, p => p.RequireAuthenticatedUser().RequireRole(TutorPolicies.AuthorRoles));
    }

    /// <summary>
    /// ASP.NET rate limits (TUT-T59, plan item 0.2(d)) on sign-in and the bundle download. Permits and windows are
    /// Vault settings, read when a client's window is first created. Rejections answer 429.
    /// </summary>
    public static IServiceCollection AddTutorRateLimits(this IServiceCollection services, TutorSettingsSource settings)
    {
        services.AddRateLimiter(o =>
        {
            o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            o.AddPolicy(SignInRateLimitPolicy, http => RateLimitPartition.GetFixedWindowLimiter(
                http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ =>
                {
                    var s = settings.Current;
                    return new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = Math.Max(1, s.SignInRateLimitPermits),
                        Window = TimeSpan.FromSeconds(Math.Max(1, s.SignInRateLimitWindowSeconds)),
                        QueueLimit = 0,
                    };
                }));
            o.AddPolicy(BundleDownloadRateLimitPolicy, http => RateLimitPartition.GetFixedWindowLimiter(
                http.User.FindFirst(MaClaims.UserId)?.Value ?? http.User.Identity?.Name
                    ?? http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ =>
                {
                    var s = settings.Current;
                    return new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = Math.Max(1, s.BundleDownloadPermits),
                        Window = TimeSpan.FromSeconds(Math.Max(1, s.BundleDownloadWindowSeconds)),
                        QueueLimit = 0,
                    };
                }));
        });
        return services;
    }

    /// <summary>
    /// Maps every Tutor endpoint: the components (with the Blazor hub), the auth endpoints, health checks and
    /// the bundle download. Shared by Program and EndpointAuthorizationTests so the test walks the real map.
    /// </summary>
    public static void MapTutorEndpoints(this WebApplication app)
    {
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(Tutor.Shared.Components.Pages.Home).Assembly)
            .Add(endpoint =>
            {
                // Framework scripts the signed-out login page needs, when served as endpoints rather than files.
                if (endpoint is RouteEndpointBuilder route
                    && AnonymousRoutePrefixes.Any(p => (route.RoutePattern.RawText ?? "").TrimStart('/').StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    endpoint.Metadata.Add(new AllowAnonymousAttribute());
            });

        // MindAttic.Authentication HTTP endpoints - /_ma-auth/{login,mfa-challenge,logout,change-password,reset/*}.
        // Only the named anonymous posts opt out of the fallback; a post the library adds later stays signed-in only.
        app.MapMindAtticAuthEndpoints(group => ConfigureAuthGroup(group));

        // Liveness and database checks for the App Service deploy (TUT-T47). Anonymous, and they return no data:
        // /healthz answers once startup is through; /healthz/db reads the auth database (503 when it cannot).
        app.MapGet("/healthz", () => Results.Text("ok")).AllowAnonymous();
        app.MapGet("/healthz/db", async (TutorAuthDbContext db, CancellationToken ct) =>
        {
            try
            {
                await db.AuthUsers.AnyAsync(ct);
                return Results.Text("ok");
            }
            catch (Exception)
            {
                return Results.Text("database unavailable", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }).AllowAnonymous();

        // Re-share: download the verbatim .tutor a course was installed from (the blob
        // store keeps it byte-for-byte, so what you share is exactly what you received).
        app.MapGet("/api/library/{courseId}/bundle", async (
            string courseId,
            InstalledCourseRegistry registry,
            CancellationToken ct) =>
        {
            var row = await registry.GetByCourseIdAsync(courseId, ct);
            if (row?.BlobPath == null || !File.Exists(row.BlobPath))
                return Results.NotFound();

            return Results.File(row.BlobPath, "application/zip", $"{row.CourseKey}-v{row.CourseVersion}.tutor");
        }).RequireAuthorization().RequireRateLimiting(BundleDownloadRateLimitPolicy);
    }

    private static void ConfigureAuthGroup(RouteGroupBuilder group)
    {
        // Sign-in, MFA, reset and sign-out posts: a per-address ceiling in front of the library's own backoff.
        group.RequireRateLimiting(SignInRateLimitPolicy);

        // Finally runs after each endpoint's own conventions, so the route pattern is complete here.
        ((IEndpointConventionBuilder)group).Finally(endpoint =>
        {
            if (endpoint is RouteEndpointBuilder route && IsAnonymousAuthPost(route.RoutePattern))
                endpoint.Metadata.Add(new AllowAnonymousAttribute());
        });
    }

    private static bool IsAnonymousAuthPost(RoutePattern pattern)
    {
        var raw = (pattern.RawText ?? "").Trim('/');
        return AnonymousAuthPosts.Any(p =>
            raw.Equals(p, StringComparison.OrdinalIgnoreCase)
            || raw.Equals("_ma-auth/" + p, StringComparison.OrdinalIgnoreCase));
    }
}
