using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetCurrentUserAsync_NoStoredUser_CreatesAndPersistsDefault()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new UserService(prefs);

        var user = await sut.GetCurrentUserAsync();

        Assert.Equal("default_user", user.Id);
        Assert.Equal("Default User", user.Name);
        Assert.True(prefs.ContainsKey("ACTIVE_USER"));
    }

    [Fact]
    public async Task GetCurrentUserAsync_CachesUserAcrossCalls()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new UserService(prefs);

        var first = await sut.GetCurrentUserAsync();
        var second = await sut.GetCurrentUserAsync();

        Assert.Same(first, second);
    }

    [Fact]
    public async Task SaveCurrentUserAsync_PersistsAsJson()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new UserService(prefs);
        var user = new User { Id = "u1", Name = "Alice", Email = "a@example.com" };

        await sut.SaveCurrentUserAsync(user);

        var json = await prefs.GetAsync("ACTIVE_USER");
        Assert.NotNull(json);
        var parsed = JsonSerializer.Deserialize<User>(json!);
        Assert.NotNull(parsed);
        Assert.Equal("u1", parsed!.Id);
        Assert.Equal("Alice", parsed.Name);
        Assert.Equal("a@example.com", parsed.Email);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReadsExistingJsonFromStore()
    {
        var prefs = new FakeSecurePreferences();
        var stored = new User { Id = "u42", Name = "Stored" };
        await prefs.SetAsync("ACTIVE_USER", JsonSerializer.Serialize(stored));
        var sut = new UserService(prefs);

        var user = await sut.GetCurrentUserAsync();

        Assert.Equal("u42", user.Id);
        Assert.Equal("Stored", user.Name);
    }

    [Fact]
    public async Task GetCurrentUserAsync_CorruptJson_FallsBackToDefault()
    {
        var prefs = new FakeSecurePreferences();
        await prefs.SetAsync("ACTIVE_USER", "{ not valid json");
        var sut = new UserService(prefs);

        var user = await sut.GetCurrentUserAsync();

        Assert.Equal("default_user", user.Id);
    }
}
