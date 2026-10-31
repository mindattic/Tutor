using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class ThemeServiceTests
{
    [Test]
    public void Constructor_DoesNotThrow()
    {
        var sut = new ThemeService(TestLamp.Settings());
        Assert.That(sut, Is.Not.Null);
    }

    [Test]
    public async Task GetThemeAsync_DefaultIsLight()
    {
        var sut = new ThemeService(TestLamp.Settings());
        Assert.That(await sut.GetThemeAsync(), Is.EqualTo("Light"));
    }

    [Test]
    public async Task GetThemeAsync_ReadsTheVaultSetting()
    {
        var sut = new ThemeService(TestLamp.Settings(s => s.Theme = "Summer"));
        await sut.InitializeAsync();
        Assert.That(sut.CurrentTheme, Is.EqualTo("Summer"));
    }
}
