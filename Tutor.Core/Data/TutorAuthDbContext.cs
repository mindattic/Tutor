using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MindAttic.Authentication.Data;
using MindAttic.Authentication.Entities;

namespace Tutor.Core.Data;

/// <summary>
/// Tutor's FIRST SQL database — auth only. Dedicated, EF-migration-managed context for the
/// MindAttic.Authentication identity tables (the isolated <c>auth</c> schema). Course content and
/// per-user progress stay in JSON; only identity moves to SQL Server. Mirrors the StreetSamurai/Ideas
/// adoption pattern.
/// </summary>
public sealed class TutorAuthDbContext(DbContextOptions<TutorAuthDbContext> options)
    : DbContext(options), IAuthDataContext
{
    public DbSet<AuthUser>               AuthUsers               => Set<AuthUser>();
    public DbSet<AuthUserMfa>            AuthUserMfa             => Set<AuthUserMfa>();
    public DbSet<AuthRecoveryCode>       AuthRecoveryCodes       => Set<AuthRecoveryCode>();
    public DbSet<AuthSession>            AuthSessions            => Set<AuthSession>();
    public DbSet<AuthLoginThrottle>      AuthLoginThrottles      => Set<AuthLoginThrottle>();
    public DbSet<AuthAuditLog>           AuthAuditLog            => Set<AuthAuditLog>();
    public DbSet<AuthPasswordHistory>    AuthPasswordHistory     => Set<AuthPasswordHistory>();
    public DbSet<AuthPasswordResetToken> AuthPasswordResetTokens => Set<AuthPasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.ApplyMindAtticAuthConfiguration();   // all 8 tables in the 'auth' schema
    }
}

/// <summary>Design-time factory so <c>dotnet ef migrations add … --context TutorAuthDbContext</c> works.</summary>
public sealed class TutorAuthDbContextFactory : IDesignTimeDbContextFactory<TutorAuthDbContext>
{
    public TutorAuthDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("ConnectionStrings__TutorAuth")
                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=TutorAuth;Trusted_Connection=True;TrustServerCertificate=True;";
        var options = new DbContextOptionsBuilder<TutorAuthDbContext>().UseSqlServer(conn).Options;
        return new TutorAuthDbContext(options);
    }
}
