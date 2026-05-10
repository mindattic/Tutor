namespace Tutor.Tests.Fakes;

public class FakeSecurePreferencesTests
{
    [Test]
    public async Task GetAsync_MissingKey_ReturnsNull()
    {
        var sut = new FakeSecurePreferences();
        Assert.That(await sut.GetAsync("nonexistent"), Is.Null);
    }

    [Test]
    public async Task SetAndGet_Roundtrips()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "value");
        Assert.That(await sut.GetAsync("key"), Is.EqualTo("value"));
    }

    [Test]
    public async Task Remove_DeletesKey()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "value");
        sut.Remove("key");
        Assert.That(await sut.GetAsync("key"), Is.Null);
    }

    [Test]
    public async Task Clear_RemovesAll()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("a", "1");
        await sut.SetAsync("b", "2");
        sut.Clear();
        Assert.That(sut.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task ContainsKey_Works()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "val");
        Assert.That(sut.ContainsKey("key"), Is.True);
        Assert.That(sut.ContainsKey("other"), Is.False);
    }

    [Test]
    public async Task SetAsync_OverwritesExisting()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "v1");
        await sut.SetAsync("key", "v2");
        Assert.That(await sut.GetAsync("key"), Is.EqualTo("v2"));
        Assert.That(sut.Count, Is.EqualTo(1));
    }
}
