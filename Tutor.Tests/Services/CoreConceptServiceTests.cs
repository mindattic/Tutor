using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class CoreConceptServiceTests
{
    [Fact]
    public async Task AddConceptAsync_AddsAndPersists()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new CoreConceptService(prefs);
        await sut.LoadForUserAsync("alice");

        var added = await sut.AddConceptAsync("Photosynthesis", "Plants make food from light");

        Assert.True(added);
        Assert.Equal(1, sut.Count);
        Assert.NotNull(sut.GetConcept("photosynthesis"));
    }

    [Fact]
    public async Task AddConceptAsync_DuplicateTermRejected()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new CoreConceptService(prefs);
        await sut.LoadForUserAsync("alice");

        await sut.AddConceptAsync("Term", "first");
        var second = await sut.AddConceptAsync("term", "duplicate, different case");

        Assert.False(second);
        Assert.Equal(1, sut.Count);
    }

    [Fact]
    public async Task AddConceptAsync_BlankInputRejected()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());

        Assert.False(await sut.AddConceptAsync("", "desc"));
        Assert.False(await sut.AddConceptAsync("term", "  "));
        Assert.Equal(0, sut.Count);
    }

    [Fact]
    public async Task UpdateConceptAsync_UpdatesDescription()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        await sut.LoadForUserAsync("alice");
        await sut.AddConceptAsync("Term", "old");

        var ok = await sut.UpdateConceptAsync("Term", "new");

        Assert.True(ok);
        Assert.Equal("new", sut.GetConcept("term")!.Description);
    }

    [Fact]
    public async Task UpdateConceptAsync_UnknownTermReturnsFalse()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        Assert.False(await sut.UpdateConceptAsync("missing", "x"));
    }

    [Fact]
    public async Task RemoveConceptAsync_RemovesByCaseInsensitiveTerm()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        await sut.LoadForUserAsync("alice");
        await sut.AddConceptAsync("Term", "desc");

        Assert.True(await sut.RemoveConceptAsync("TERM"));
        Assert.Equal(0, sut.Count);
        Assert.False(await sut.RemoveConceptAsync("TERM"));
    }

    [Fact]
    public async Task LoadForUserAsync_PerUserIsolation()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new CoreConceptService(prefs);

        await sut.LoadForUserAsync("alice");
        await sut.AddConceptAsync("AliceConcept", "for alice");

        await sut.LoadForUserAsync("bob");
        Assert.Equal(0, sut.Count);

        await sut.AddConceptAsync("BobConcept", "for bob");
        await sut.LoadForUserAsync("alice");
        Assert.Equal(1, sut.Count);
        Assert.NotNull(sut.GetConcept("aliceconcept"));
        Assert.Null(sut.GetConcept("bobconcept"));
    }

    [Fact]
    public async Task ClearAllAsync_EmptiesList()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        await sut.LoadForUserAsync("u");
        await sut.AddConceptAsync("a", "1");
        await sut.AddConceptAsync("b", "2");

        await sut.ClearAllAsync();

        Assert.Equal(0, sut.Count);
    }

    [Fact]
    public async Task OnConceptsChanged_FiresOnMutations()
    {
        // LoadForUserAsync skips raising the event when there's no stored data
        // (early return on empty JSON); the event covers Add/Update/Remove/Clear.
        var sut = new CoreConceptService(new FakeSecurePreferences());
        await sut.LoadForUserAsync("u");

        var fires = 0;
        sut.OnConceptsChanged += () => fires++;

        await sut.AddConceptAsync("a", "1");
        await sut.UpdateConceptAsync("a", "1b");
        await sut.RemoveConceptAsync("a");
        await sut.ClearAllAsync();

        Assert.Equal(4, fires);
    }
}
