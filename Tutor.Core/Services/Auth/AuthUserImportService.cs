using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MindAttic.Authentication;
using MindAttic.Authentication.Entities;
using Tutor.Core.Data;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services.Auth;

/// <summary>
/// One-time, idempotent migration of the legacy JSON user store (<see cref="UsersStore"/> at
/// <c>%APPDATA%/Tutor/Users/Users.json</c>) into the MindAttic.Authentication <see cref="AuthUser"/>
/// table. The legacy unsalted-SHA256 hash (base64 of SHA256(utf8(password))) is carried verbatim with
/// <c>LegacyHashScheme="sha256"</c>, so it transparently upgrades to Argon2id+pepper on next login.
/// Tutor logs in by USERNAME (not email), so UserName = the legacy username. A profile with the Admin
/// role maps to the canonical <c>Admin</c>; everyone else is <c>Student</c>. The well-known weak dev
/// seeds (ryan/erin, password "aaa") are force-reset (MustChangePassword) rather than trusted.
/// Idempotency key = NormalizedUserName.
/// </summary>
public sealed class AuthUserImportService(IAppDataPathProvider paths, TutorAuthDbContext authDb)
{
    private static readonly HashSet<string> WellKnownWeak = new(StringComparer.OrdinalIgnoreCase) { "ryan", "erin" };
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<int> ImportAsync(CancellationToken ct = default)
    {
        var usersFile = Path.Combine(paths.AppDataDirectory, "Users", "Users.json");
        if (!File.Exists(usersFile)) return 0;

        UsersStore? store;
        try { store = JsonSerializer.Deserialize<UsersStore>(await File.ReadAllTextAsync(usersFile, ct), JsonOptions); }
        catch { return 0; }
        if (store is null || store.Credentials.Count == 0) return 0;

        var imported = 0;
        foreach (var (username, cred) in store.Credentials)
        {
            if (string.IsNullOrWhiteSpace(username)) continue;
            var normalized = Normalize(username);
            if (await authDb.AuthUsers.AnyAsync(a => a.NormalizedUserName == normalized, ct)) continue;

            store.Profiles.TryGetValue(username, out var profile);
            var isAdmin = profile?.IsAdmin ?? false;
            var created = cred.CreatedAt == default ? DateTime.UtcNow : cred.CreatedAt;

            authDb.AuthUsers.Add(new AuthUser
            {
                Id = Guid.TryParse(profile?.Id, out var g) ? g : Guid.NewGuid(),
                UserName = username,
                NormalizedUserName = normalized,
                Email = profile?.Email,
                NormalizedEmail = string.IsNullOrWhiteSpace(profile?.Email) ? null : Normalize(profile!.Email!),
                EmailVerified = false,
                PasswordHash = cred.PasswordHash,
                LegacyHashScheme = "sha256",            // upgrade-on-login to Argon2id+pepper
                PasswordPepperKeyId = null,
                PasswordUpdatedUtc = created,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                Role = isAdmin ? MaRoles.Admin : "Student",
                MfaEnabled = false,
                MustChangePassword = WellKnownWeak.Contains(username),   // decommission weak 'aaa' dev seeds
                MustEnrollMfa = false,                  // MFA off for now (owner directive)
                IsActive = true,
                LastLoginUtc = cred.LastLoginAt,
                CreatedUtc = created,
            });
            imported++;
        }

        if (imported > 0) await authDb.SaveChangesAsync(ct);
        return imported;
    }

    private static string Normalize(string s) => s.Normalize(NormalizationForm.FormKC).Trim().ToUpperInvariant();
}
