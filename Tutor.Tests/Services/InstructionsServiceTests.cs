using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class InstructionsServiceTests
{
    [Test]
    public void Constructor_DoesNotThrow()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new InstructionsService(prefs);
        Assert.That(sut, Is.Not.Null);
    }

    [Test]
    public async Task GetAllInstructionsAsync_DefaultEmpty()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new InstructionsService(prefs);
        var instructions = await sut.GetAllInstructionsAsync();
        Assert.That(instructions, Is.Not.Null);
        Assert.That(instructions, Is.Empty); // No user instructions stored yet
    }

    [Test]
    public async Task GetCombinedInstructionsAsync_ReturnsNonEmpty()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new InstructionsService(prefs);
        var combined = await sut.GetCombinedInstructionsAsync();
        Assert.That(combined, Is.Not.Null);
        Assert.That(combined, Is.Not.Empty);
    }
}
