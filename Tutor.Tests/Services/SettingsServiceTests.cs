using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class SettingsServiceTests
{
    [Test]
    public void Constructor_AcceptsSettingsAndPreferences()
    {
        var sut = new SettingsService(TestLamp.Settings(), new FakeSecurePreferences());
        Assert.That(sut, Is.Not.Null);
    }

    [Test]
    public async Task GetEnterToSendAsync_DefaultsToTrue()
    {
        var sut = new SettingsService(TestLamp.Settings(), new FakeSecurePreferences());
        Assert.That(await sut.GetEnterToSendAsync(), Is.True);
    }

    [Test]
    public async Task EnterToSend_ReadsTheVaultSettingOnEveryCall()
    {
        var settings = TestLamp.Settings();
        var sut = new SettingsService(settings, new FakeSecurePreferences());

        settings.Store.Update(s => s.EnterToSend = false);   // as Vault.Desktop would save it

        Assert.That(await sut.GetEnterToSendAsync(), Is.False);
    }

    [Test]
    public async Task LessonState_StaysInSecurePreferences()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new SettingsService(TestLamp.Settings(), prefs);

        await sut.SaveLessonStateAsync(new LessonState { CurriculumId = "c1", CurrentLessonIndex = 2 });

        Assert.That(prefs.ContainsKey("LESSON_STATE"), Is.True);
        Assert.That((await sut.GetLessonStateAsync())!.CurriculumId, Is.EqualTo("c1"));
    }
}
