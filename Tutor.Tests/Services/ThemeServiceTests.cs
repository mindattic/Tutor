using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class ThemeServiceTests
{
    [Test]
    public void Constructor_DoesNotThrow()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new ThemeService(prefs);
        Assert.That(sut, Is.Not.Null);
    }

    [Test]
    public async Task GetThemeAsync_DefaultIsNotEmpty()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new ThemeService(prefs);
        var theme = await sut.GetThemeAsync();
        Assert.That(string.IsNullOrEmpty(theme), Is.False);
    }

    [Test]
    public async Task SetAndGet_Theme_Roundtrips()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new ThemeService(prefs);

        await sut.SetThemeAsync("Summer");
        var result = await sut.GetThemeAsync();
        Assert.That(result, Is.EqualTo("Summer"));
    }
}
