using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class CoreConceptServiceTests
{
    [Test]
    public async Task AddConceptAsync_AddsAndPersists()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new CoreConceptService(prefs);
        await sut.LoadForUserAsync("alice");

        var added = await sut.AddConceptAsync("Photosynthesis", "Plants make food from light");

        Assert.That(added, Is.True);
        Assert.That(sut.Count, Is.EqualTo(1));
        Assert.That(sut.GetConcept("photosynthesis"), Is.Not.Null);
    }

    [Test]
    public async Task AddConceptAsync_DuplicateTermRejected()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new CoreConceptService(prefs);
        await sut.LoadForUserAsync("alice");

        await sut.AddConceptAsync("Term", "first");
        var second = await sut.AddConceptAsync("term", "duplicate, different case");

        Assert.That(second, Is.False);
        Assert.That(sut.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task AddConceptAsync_BlankInputRejected()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());

        Assert.That(await sut.AddConceptAsync("", "desc"), Is.False);
        Assert.That(await sut.AddConceptAsync("term", "  "), Is.False);
        Assert.That(sut.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task UpdateConceptAsync_UpdatesDescription()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        await sut.LoadForUserAsync("alice");
        await sut.AddConceptAsync("Term", "old");

        var ok = await sut.UpdateConceptAsync("Term", "new");

        Assert.That(ok, Is.True);
        Assert.That(sut.GetConcept("term")!.Description, Is.EqualTo("new"));
    }

    [Test]
    public async Task UpdateConceptAsync_UnknownTermReturnsFalse()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        Assert.That(await sut.UpdateConceptAsync("missing", "x"), Is.False);
    }

    [Test]
    public async Task RemoveConceptAsync_RemovesByCaseInsensitiveTerm()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        await sut.LoadForUserAsync("alice");
        await sut.AddConceptAsync("Term", "desc");

        Assert.That(await sut.RemoveConceptAsync("TERM"), Is.True);
        Assert.That(sut.Count, Is.EqualTo(0));
        Assert.That(await sut.RemoveConceptAsync("TERM"), Is.False);
    }

    [Test]
    public async Task LoadForUserAsync_PerUserIsolation()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new CoreConceptService(prefs);

        await sut.LoadForUserAsync("alice");
        await sut.AddConceptAsync("AliceConcept", "for alice");

        await sut.LoadForUserAsync("bob");
        Assert.That(sut.Count, Is.EqualTo(0));

        await sut.AddConceptAsync("BobConcept", "for bob");
        await sut.LoadForUserAsync("alice");
        Assert.That(sut.Count, Is.EqualTo(1));
        Assert.That(sut.GetConcept("aliceconcept"), Is.Not.Null);
        Assert.That(sut.GetConcept("bobconcept"), Is.Null);
    }

    [Test]
    public async Task ClearAllAsync_EmptiesList()
    {
        var sut = new CoreConceptService(new FakeSecurePreferences());
        await sut.LoadForUserAsync("u");
        await sut.AddConceptAsync("a", "1");
        await sut.AddConceptAsync("b", "2");

        await sut.ClearAllAsync();

        Assert.That(sut.Count, Is.EqualTo(0));
    }

    [Test]
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

        Assert.That(fires, Is.EqualTo(4));
    }
}
