using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MindAttic.Authentication;
using Tutor.Core.Data;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Auth;

namespace Tutor.Tests;

/// <summary>
/// The legacy Users.json (unsalted SHA256) → AuthUser importer: role mapping, sha256 carry +
/// force-reset of the well-known weak dev seeds, Guid id preservation, and idempotency.
/// </summary>
[TestFixture]
public class AuthUserImportTests
{
    private string _tempDir = null!;

    private sealed class TempPathProvider(string dir) : IAppDataPathProvider
    {
        public string AppDataDirectory => dir;
    }

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "tutor_import_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempDir, "Users"));
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, recursive: true);
    }

    private void WriteStore(UsersStore store) =>
        File.WriteAllText(Path.Combine(_tempDir, "Users", "Users.json"), JsonSerializer.Serialize(store));

    private static TutorAuthDbContext NewDb() =>
        new(new DbContextOptionsBuilder<TutorAuthDbContext>()
            .UseInMemoryDatabase("import_" + Guid.NewGuid().ToString("N")).Options);

    private AuthUserImportService Importer(TutorAuthDbContext db) => new(new TempPathProvider(_tempDir), db);

    private static UsersStore SeedStore()
    {
        var store = new UsersStore();
        store.Credentials["ryan"] = new UserCredentials { Username = "ryan", PasswordHash = "RYANHASH==", CreatedAt = DateTime.UtcNow };
        store.Profiles["ryan"] = new UserProfile { Id = Guid.NewGuid().ToString(), Username = "ryan", DisplayName = "Ryan", Roles = [UserRole.Admin, UserRole.Student] };
        store.Credentials["erin"] = new UserCredentials { Username = "erin", PasswordHash = "ERINHASH==", CreatedAt = DateTime.UtcNow };
        store.Profiles["erin"] = new UserProfile { Id = Guid.NewGuid().ToString(), Username = "erin", DisplayName = "Erin", Roles = [UserRole.Student] };
        return store;
    }

    [Test]
    public async Task Ryan_ImportsAsAdmin_WithSha256_AndForceReset()
    {
        WriteStore(SeedStore());
        using var db = NewDb();
        var count = await Importer(db).ImportAsync();

        Assert.That(count, Is.EqualTo(2));
        var ryan = db.AuthUsers.Single(u => u.NormalizedUserName == "RYAN");
        Assert.That(ryan.Role, Is.EqualTo(MaRoles.Admin));
        Assert.That(ryan.LegacyHashScheme, Is.EqualTo("sha256"));
        Assert.That(ryan.PasswordHash, Is.EqualTo("RYANHASH=="));   // carried verbatim
        Assert.That(ryan.MustChangePassword, Is.True);              // well-known weak seed -> force reset
        Assert.That(ryan.IsActive, Is.True);
    }

    [Test]
    public async Task Erin_ImportsAsStudent()
    {
        WriteStore(SeedStore());
        using var db = NewDb();
        await Importer(db).ImportAsync();

        var erin = db.AuthUsers.Single(u => u.NormalizedUserName == "ERIN");
        Assert.That(erin.Role, Is.EqualTo("Student"));
        Assert.That(erin.MustChangePassword, Is.True);
    }

    [Test]
    public async Task PreservesGuidId_WhenParseable()
    {
        var store = SeedStore();
        var id = Guid.NewGuid();
        store.Profiles["ryan"].Id = id.ToString();
        WriteStore(store);
        using var db = NewDb();
        await Importer(db).ImportAsync();

        Assert.That(db.AuthUsers.Single(u => u.NormalizedUserName == "RYAN").Id, Is.EqualTo(id));
    }

    [Test]
    public async Task IsIdempotent_SecondRunImportsZero()
    {
        WriteStore(SeedStore());
        using var db = NewDb();
        var importer = Importer(db);
        await importer.ImportAsync();
        var second = await importer.ImportAsync();

        Assert.That(second, Is.EqualTo(0));
        Assert.That(db.AuthUsers.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task SkipsWhenIdAlreadyPresent_EvenIfUsernameDiverged()
    {
        // Repro of PK_AuthUsers violation: a prior import landed ryan under Id X, then the username was
        // changed to an email. The legacy store still keys him as "ryan" (Id X). The importer must skip
        // on the matching Id rather than insert a duplicate primary key.
        var store = SeedStore();
        var ryanId = Guid.NewGuid();
        store.Profiles["ryan"].Id = ryanId.ToString();
        WriteStore(store);

        using var db = NewDb();
        db.AuthUsers.Add(new MindAttic.Authentication.Entities.AuthUser
        {
            Id = ryanId,
            UserName = "ryandebraal@mindattic.com",
            NormalizedUserName = "RYANDEBRAAL@MINDATTIC.COM",
            Role = MaRoles.Admin,
            PasswordHash = "EXISTING",
            IsActive = true,
        });
        await db.SaveChangesAsync();

        var count = await Importer(db).ImportAsync();

        Assert.That(count, Is.EqualTo(1));                                   // only erin is new
        Assert.That(db.AuthUsers.Count(u => u.Id == ryanId), Is.EqualTo(1)); // no duplicate row
        Assert.That(db.AuthUsers.Single(u => u.Id == ryanId).UserName, Is.EqualTo("ryandebraal@mindattic.com"));
    }

    [Test]
    public async Task NoUsersFile_ImportsZero()
    {
        using var db = NewDb();
        Assert.That(await Importer(db).ImportAsync(), Is.EqualTo(0));
    }
}
