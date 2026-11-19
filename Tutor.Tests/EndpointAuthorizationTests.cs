using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MindAttic.Authentication.Web;
using Tutor.Blazor.Hosting;
using Tutor.Core.Data;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Packaging;
using Tutor.Tests.Fakes;

namespace Tutor.Tests;

/// <summary>
/// Default-deny guard (TUT-T52, plan item 0.1(d)). Builds the real endpoint map (<see cref="TutorAccess.MapTutorEndpoints"/>,
/// the same call Program makes) without starting the server or touching a database, then walks every mapped
/// endpoint and every <c>@page</c> component. A route that a signed-out visitor could reach and that is not on
/// <see cref="TutorAccess.AnonymousRoutes"/> fails the suite.
/// </summary>
[TestFixture]
public sealed class EndpointAuthorizationTests
{
    private WebApplication _app = null!;
    private IReadOnlyList<RouteEndpoint> _endpoints = [];

    [OneTimeSetUp]
    public void BuildEndpointMap()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>());

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddDbContext<TutorAuthDbContext>(o => o.UseInMemoryDatabase("endpoint-auth-" + Guid.NewGuid().ToString("N")));
        builder.Services.AddMindAtticAuthentication<TutorAuthDbContext>(builder.Configuration, o =>
        {
            o.AppName = "Tutor.Tests";
            o.IsProduction = false;
            o.DevKeyRingPath = Path.Combine(TestVaultRoots.Root, "dp-keys");
            o.ConfigureAdditionalPolicies = TutorAccess.ConfigurePolicies;
        });
        // Only registered so minimal-API parameter inference sees them as services; never constructed here.
        builder.Services.AddSingleton<IAppDataPathProvider, FakeAppDataPathProvider>();
        builder.Services.AddSingleton<InstalledCourseRegistry>();

        _app = builder.Build();
        _app.MapTutorEndpoints();

        _endpoints = ((IEndpointRouteBuilder)_app).DataSources
            .SelectMany(d => d.Endpoints)
            .OfType<RouteEndpoint>()
            .ToList();
    }

    [OneTimeTearDown]
    public async Task DisposeApp() => await _app.DisposeAsync();

    private static string Route(RouteEndpoint e) => (e.RoutePattern.RawText ?? "").Trim('/');

    private static bool IsAnonymous(Endpoint e) => e.Metadata.GetMetadata<IAllowAnonymous>() is not null;

    [Test]
    public async Task FallbackPolicy_RequiresASignedInUser()
    {
        var provider = _app.Services.GetRequiredService<IAuthorizationPolicyProvider>();
        var fallback = await provider.GetFallbackPolicyAsync();

        Assert.That(fallback, Is.Not.Null, "No fallback policy: every page and endpoint would be public unless it opted in.");
        Assert.That(fallback!.Requirements.OfType<DenyAnonymousAuthorizationRequirement>(), Is.Not.Empty);
    }

    [Test]
    public void EndpointMap_IsNotEmpty()
    {
        Assert.That(_endpoints.Select(Route), Does.Contain("login"));
        Assert.That(_endpoints.Select(Route), Does.Contain("healthz"));
        Assert.That(_endpoints.Select(Route), Does.Contain("_ma-auth/login"));
        Assert.That(_endpoints.Any(e => Route(e).StartsWith("_blazor", StringComparison.OrdinalIgnoreCase)), Is.True,
            "The Blazor hub endpoints were not found in the map.");
    }

    [Test]
    public void EveryAnonymousEndpoint_IsOnTheAllowList()
    {
        var violations = _endpoints
            .Where(IsAnonymous)
            .Select(Route)
            .Where(r => !TutorAccess.IsAllowListed(r))
            .Distinct()
            .ToList();

        Assert.That(violations, Is.Empty,
            "Anonymous endpoints not on TutorAccess.AnonymousRoutes: " + string.Join(", ", violations));
    }

    [Test]
    public void BlazorHub_RefusesAnonymousConnections()
    {
        var hub = _endpoints.Where(e => Route(e).StartsWith("_blazor", StringComparison.OrdinalIgnoreCase)).ToList();

        Assert.That(hub.Select(Route), Does.Contain("_blazor/negotiate"));
        Assert.That(hub.Where(IsAnonymous).Select(Route), Is.Empty, "A Blazor hub endpoint allows anonymous access.");
    }

    [Test]
    public void AuthEndpoints_OnlyTheNamedPostsAreAnonymous()
    {
        foreach (var post in TutorAccess.AnonymousAuthPosts)
        {
            var endpoint = _endpoints.SingleOrDefault(e => Route(e) == "_ma-auth/" + post);
            Assert.That(endpoint, Is.Not.Null, $"/_ma-auth/{post} is not mapped.");
            Assert.That(IsAnonymous(endpoint!), Is.True, $"/_ma-auth/{post} must be reachable signed out.");
        }

        var change = _endpoints.Single(e => Route(e) == "_ma-auth/change-password");
        Assert.That(IsAnonymous(change), Is.False);
    }

    [TestCase("healthz")]
    [TestCase("healthz/db")]
    public void HealthChecks_AreAnonymous(string route)
    {
        Assert.That(IsAnonymous(_endpoints.Single(e => Route(e) == route)), Is.True);
    }

    [Test]
    public void BundleDownload_RequiresAuthorization()
    {
        var bundle = _endpoints.Single(e => Route(e) == "api/library/{courseId}/bundle");
        Assert.That(IsAnonymous(bundle), Is.False);
    }

    /// <summary>Every routable component in the app's assemblies, with its routes.</summary>
    private static IEnumerable<(Type Page, string Route)> Pages() =>
        new[] { typeof(Tutor.Shared.Components.Pages.Home).Assembly, typeof(Tutor.Blazor.Components.App).Assembly }
            .Distinct()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IComponent).IsAssignableFrom(t))
            .SelectMany(t => t.GetCustomAttributes<RouteAttribute>(inherit: false).Select(r => (t, r.Template.Trim('/'))));

    [Test]
    public void EveryPage_IsSignedInOnlyOrOnTheAllowList()
    {
        var pages = Pages().ToList();
        Assert.That(pages, Is.Not.Empty);

        var violations = pages
            .Where(p => p.Page.GetCustomAttribute<AllowAnonymousAttribute>() is not null && !TutorAccess.IsAllowListed(p.Route))
            .Select(p => $"{p.Page.Name} (/{p.Route})")
            .ToList();

        Assert.That(violations, Is.Empty, "[AllowAnonymous] pages not on TutorAccess.AnonymousRoutes: " + string.Join(", ", violations));
    }

    [Test]
    public void EveryPage_IsMappedAsAnEndpoint()
    {
        var mapped = _endpoints.Select(Route).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = Pages().Where(p => !mapped.Contains(p.Route)).Select(p => p.Page.Name + " /" + p.Route).ToList();

        Assert.That(missing, Is.Empty, "Pages the endpoint walk did not see: " + string.Join(", ", missing));
    }

    // Author pages (TUT-T59, plan item 0.2(a)): a Student gets a 403 at the endpoint, not just a hidden link.
    [TestCase("courses")]
    [TestCase("instructions")]
    [TestCase("knowledge-graph")]
    [TestCase("knowledge-bases")]
    public void AuthoringPages_RequireTheAuthorPolicy(string route)
    {
        var endpoint = _endpoints.First(e => Route(e).Equals(route, StringComparison.OrdinalIgnoreCase));
        var policies = endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Select(a => a.Policy);

        Assert.That(policies, Does.Contain(Tutor.Shared.Services.TutorPolicies.Author));
        Assert.That(IsAnonymous(endpoint), Is.False);
    }

    [TestCase("_ma-auth/login")]
    [TestCase("api/library/{courseId}/bundle")]
    public void SignInAndBundleDownload_AreRateLimited(string route)
    {
        var endpoint = _endpoints.Single(e => Route(e) == route);
        Assert.That(endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.RateLimiting.EnableRateLimitingAttribute>(), Is.Not.Null);
    }

    [Test]
    public void AllowListedPages_AreActuallyAnonymous()
    {
        // A sign-in page behind the fallback policy would redirect /login to /login forever.
        foreach (var (page, route) in Pages().Where(p => TutorAccess.IsAllowListed(p.Route)))
            Assert.That(page.GetCustomAttribute<AllowAnonymousAttribute>(), Is.Not.Null, $"{page.Name} (/{route}) must be [AllowAnonymous].");
    }
}
