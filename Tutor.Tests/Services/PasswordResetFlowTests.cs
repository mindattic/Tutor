using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MindAttic.Authentication.Crypto;
using MindAttic.Authentication.Entities;
using MindAttic.Authentication.Options;
using MindAttic.Authentication.Secrets;
using MindAttic.Authentication.Services;
using Tutor.Core.Data;

namespace Tutor.Tests.Services;

/// <summary>
/// Self-service password reset over Tutor's own pieces: the PublicBaseUrl in Tutor.Blazor/appsettings.json,
/// the reset page at the library's ResetPath (Tutor.Shared), and TutorAuthDbContext as the auth store.
/// A capturing sender stands in for SMTP — nothing is sent.
/// </summary>
[TestFixture]
public class PasswordResetFlowTests
{
    private const string OldPassword = "Initial-Passphrase-1";
    private const string NewPassword = "Brand-New-Passphrase-42";

    private sealed class CapturingSender : IAuthEmailSender
    {
        public List<string> Links { get; } = new();
        public Task SendPasswordResetAsync(string toEmail, string resetLink, CancellationToken ct = default) { Links.Add(resetLink); return Task.CompletedTask; }
        public Task SendSecurityAlertAsync(string toEmail, string subject, string body, CancellationToken ct = default) => Task.CompletedTask;
    }

    private sealed class AcceptAll : IPasswordPolicy
    {
        public Task<PasswordPolicyResult> ValidateAsync(string password, Guid? userId = null, CancellationToken ct = default) =>
            Task.FromResult(new PasswordPolicyResult(true, null));
    }

    private sealed class NoAudit : IAuthAuditWriter
    {
        public Task WriteAsync(AuthAuditEntry entry, CancellationToken ct = default) => Task.CompletedTask;
    }

    private static DirectoryInfo RepoRoot()
    {
        var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Tutor.slnx"))) dir = dir.Parent;
        Assert.That(dir, Is.Not.Null, "Could not locate the repo root (Tutor.slnx).");
        return dir!;
    }

    private static Type PageAt(string route) =>
        Assembly.Load("Tutor.Shared").GetTypes()
            .Single(t => t.GetCustomAttributes<RouteAttribute>().Any(r => r.Template == route));

    private static AuthResetOptions AppResetOptions()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(RepoRoot().FullName, "Tutor.Blazor", "appsettings.json"))
            .Build();
        return config.GetSection("MindAttic:Auth:Reset").Get<AuthResetOptions>() ?? new AuthResetOptions();
    }

    [Test]
    public void ResetAndForgotPages_AreAnonymousStaticPages()
    {
        foreach (var route in new[] { new AuthResetOptions().ResetPath, "/forgot-password" })
        {
            var page = PageAt(route);
            Assert.That(page.GetCustomAttribute<AllowAnonymousAttribute>(), Is.Not.Null, route);
            // Static SSR: the library forms post with antiforgery, which an interactive circuit can't serve.
            Assert.That(page.GetCustomAttribute<ExcludeFromInteractiveRoutingAttribute>(), Is.Not.Null, route);
        }
    }

    [Test]
    public async Task RequestReset_EmailsAnAbsoluteLinkToTheResetPage_WhichResetsThePassword()
    {
        var options = AppResetOptions();
        Assert.That(options.PublicBaseUrl, Is.EqualTo("https://localhost:7200"));

        var secrets = new ConfigAuthSecrets(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{ConfigAuthSecrets.SectionPath}:pepper.v1"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            [$"{ConfigAuthSecrets.SectionPath}:reset-token-key"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
        }).Build());
        var hasher = new Argon2idPasswordHasher(secrets, Microsoft.Extensions.Options.Options.Create(new AuthCryptoOptions
        {
            MemoryKiB = AuthCryptoOptions.FloorMemoryKiB, Iterations = AuthCryptoOptions.FloorIterations,
            Parallelism = AuthCryptoOptions.FloorParallelism,
        }));
        using var db = new TutorAuthDbContext(new DbContextOptionsBuilder<TutorAuthDbContext>()
            .UseInMemoryDatabase($"reset-{Guid.NewGuid():N}").Options);
        var stored = hasher.Hash(OldPassword);
        db.AuthUsers.Add(new AuthUser
        {
            UserName = "alice", NormalizedUserName = IUserStore.Normalize("alice"),
            Email = "alice@example.com", NormalizedEmail = "ALICE@EXAMPLE.COM",
            PasswordHash = stored.Phc, PasswordPepperKeyId = stored.PepperKeyId, Role = "User", IsActive = true,
        });
        await db.SaveChangesAsync();

        var sender = new CapturingSender();
        var reset = new PasswordResetService(new UserStore(db), db, hasher, new AcceptAll(), secrets, sender, new NoAudit(),
            Microsoft.Extensions.Options.Options.Create(options), TimeProvider.System);

        await reset.RequestAsync("alice", "127.0.0.1", "agent");

        var link = new Uri(sender.Links.Single());
        Assert.That(link.GetLeftPart(UriPartial.Authority), Is.EqualTo("https://localhost:7200"));
        Assert.That(link.AbsolutePath, Is.EqualTo(options.ResetPath));
        Assert.That(PageAt(link.AbsolutePath), Is.Not.Null, "the emailed link must land on a Tutor page");
        var token = Uri.UnescapeDataString(link.Query.TrimStart('?').Split('&')
            .Single(p => p.StartsWith("token=", StringComparison.Ordinal))["token=".Length..]);

        // The page's form posts the token to /_ma-auth/reset/confirm, which calls ConfirmAsync.
        var result = await reset.ConfirmAsync(token, NewPassword);
        Assert.That(result.Ok, Is.True, result.Error);
        var user = db.AuthUsers.Single();
        Assert.That(hasher.Verify(NewPassword, user.PasswordHash, user.PasswordPepperKeyId, null).Succeeded, Is.True);
        Assert.That(hasher.Verify(OldPassword, user.PasswordHash, user.PasswordPepperKeyId, null).Succeeded, Is.False);
    }
}
