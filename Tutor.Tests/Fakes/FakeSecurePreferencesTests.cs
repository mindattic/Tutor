namespace Tutor.Tests.Fakes;

public class FakeSecurePreferencesTests
{
    [Fact]
    public async Task GetAsync_MissingKey_ReturnsNull()
    {
        var sut = new FakeSecurePreferences();
        Assert.Null(await sut.GetAsync("nonexistent"));
    }

    [Fact]
    public async Task SetAndGet_Roundtrips()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "value");
        Assert.Equal("value", await sut.GetAsync("key"));
    }

    [Fact]
    public async Task Remove_DeletesKey()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "value");
        sut.Remove("key");
        Assert.Null(await sut.GetAsync("key"));
    }

    [Fact]
    public async Task Clear_RemovesAll()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("a", "1");
        await sut.SetAsync("b", "2");
        sut.Clear();
        Assert.Equal(0, sut.Count);
    }

    [Fact]
    public async Task ContainsKey_Works()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "val");
        Assert.True(sut.ContainsKey("key"));
        Assert.False(sut.ContainsKey("other"));
    }

    [Fact]
    public async Task SetAsync_OverwritesExisting()
    {
        var sut = new FakeSecurePreferences();
        await sut.SetAsync("key", "v1");
        await sut.SetAsync("key", "v2");
        Assert.Equal("v2", await sut.GetAsync("key"));
        Assert.Equal(1, sut.Count);
    }
}
