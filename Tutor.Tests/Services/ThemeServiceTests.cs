using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class ThemeServiceTests
{
    [Fact]
    public void Constructor_DoesNotThrow()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new ThemeService(prefs);
        Assert.NotNull(sut);
    }

    [Fact]
    public async Task GetThemeAsync_DefaultIsNotEmpty()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new ThemeService(prefs);
        var theme = await sut.GetThemeAsync();
        Assert.False(string.IsNullOrEmpty(theme));
    }

    [Fact]
    public async Task SetAndGet_Theme_Roundtrips()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new ThemeService(prefs);

        await sut.SetThemeAsync("Summer");
        var result = await sut.GetThemeAsync();
        Assert.Equal("Summer", result);
    }
}
