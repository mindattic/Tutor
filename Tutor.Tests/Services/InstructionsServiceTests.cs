using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class InstructionsServiceTests
{
    [Fact]
    public void Constructor_DoesNotThrow()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new InstructionsService(prefs);
        Assert.NotNull(sut);
    }

    [Fact]
    public async Task GetAllInstructionsAsync_DefaultEmpty()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new InstructionsService(prefs);
        var instructions = await sut.GetAllInstructionsAsync();
        Assert.NotNull(instructions);
        Assert.Empty(instructions); // No user instructions stored yet
    }

    [Fact]
    public async Task GetCombinedInstructionsAsync_ReturnsNonEmpty()
    {
        var prefs = new FakeSecurePreferences();
        var sut = new InstructionsService(prefs);
        var combined = await sut.GetCombinedInstructionsAsync();
        Assert.NotNull(combined);
        Assert.NotEmpty(combined);
    }
}
