using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class SettingsServiceTests
{
    [Test]
    public void Constructor_AcceptsSecurePreferences()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new SettingsService(prefs);
        Assert.That(sut, Is.Not.Null);
    }

    [Test]
    public async Task GetEnterToSendAsync_DefaultsToTrue()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new SettingsService(prefs);
        var value = await sut.GetEnterToSendAsync();
        Assert.That(value, Is.True);
    }

    [Test]
    public async Task SetAndGet_EnterToSend_Roundtrips()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new SettingsService(prefs);

        await sut.SetEnterToSendAsync(false);
        var result = await sut.GetEnterToSendAsync();
        Assert.That(result, Is.False);
    }
}
