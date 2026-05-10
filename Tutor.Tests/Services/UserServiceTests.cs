using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class UserServiceTests
{
    [Test]
    public async Task GetCurrentUserAsync_NoStoredUser_CreatesAndPersistsDefault()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new UserService(prefs);

        var user = await sut.GetCurrentUserAsync();

        Assert.That(user.Id, Is.EqualTo("default_user"));
        Assert.That(user.Name, Is.EqualTo("Default User"));
        Assert.That(prefs.ContainsKey("ACTIVE_USER"), Is.True);
    }

    [Test]
    public async Task GetCurrentUserAsync_CachesUserAcrossCalls()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new UserService(prefs);

        var first = await sut.GetCurrentUserAsync();
        var second = await sut.GetCurrentUserAsync();

        Assert.That(second, Is.SameAs(first));
    }

    [Test]
    public async Task SaveCurrentUserAsync_PersistsAsJson()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new UserService(prefs);
        var user = new User { Id = "u1", Name = "Alice", Email = "a@example.com" };

        await sut.SaveCurrentUserAsync(user);

        var json = await prefs.GetAsync("ACTIVE_USER");
        Assert.That(json, Is.Not.Null);
        var parsed = JsonSerializer.Deserialize<User>(json!);
        Assert.That(parsed, Is.Not.Null);
        Assert.That(parsed!.Id, Is.EqualTo("u1"));
        Assert.That(parsed.Name, Is.EqualTo("Alice"));
        Assert.That(parsed.Email, Is.EqualTo("a@example.com"));
    }

    [Test]
    public async Task GetCurrentUserAsync_ReadsExistingJsonFromStore()
    {
        var prefs = new FakeSecurePreferences();
        var stored = new User { Id = "u42", Name = "Stored" };
        await prefs.SetAsync("ACTIVE_USER", JsonSerializer.Serialize(stored));
        var sut = new UserService(prefs);

        var user = await sut.GetCurrentUserAsync();

        Assert.That(user.Id, Is.EqualTo("u42"));
        Assert.That(user.Name, Is.EqualTo("Stored"));
    }

    [Test]
    public async Task GetCurrentUserAsync_CorruptJson_FallsBackToDefault()
    {
        var prefs = new FakeSecurePreferences();
        await prefs.SetAsync("ACTIVE_USER", "{ not valid json");
        var sut = new UserService(prefs);

        var user = await sut.GetCurrentUserAsync();

        Assert.That(user.Id, Is.EqualTo("default_user"));
    }
}
