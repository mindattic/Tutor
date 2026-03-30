using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class SimHashServiceTests
{
    private readonly SimHashService sut = new();

    [Fact]
    public void GetSignature64_EmptyText_ReturnsZero()
    {
        Assert.Equal(0UL, sut.GetSignature64(""));
        Assert.Equal(0UL, sut.GetSignature64(null!));
    }

    [Fact]
    public void GetSignature64_IsDeterministic()
    {
        var text = "The quick brown fox jumps over the lazy dog";
        var a = sut.GetSignature64(text);
        var b = sut.GetSignature64(text);
        Assert.Equal(a, b);
    }

    [Fact]
    public void GetSignature64_DifferentTexts_DifferentSignatures()
    {
        var a = sut.GetSignature64("machine learning algorithms");
        var b = sut.GetSignature64("underwater basket weaving");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void HammingDistance_Identical_Zero()
    {
        Assert.Equal(0, SimHashService.HammingDistance(42UL, 42UL));
    }

    [Fact]
    public void HammingDistance_SingleBitDifference()
    {
        Assert.Equal(1, SimHashService.HammingDistance(0UL, 1UL));
    }

    [Fact]
    public void HammingDistance_AllBitsDifferent()
    {
        Assert.Equal(64, SimHashService.HammingDistance(0UL, ulong.MaxValue));
    }

    [Fact]
    public void SimilarTexts_LowHammingDistance()
    {
        var sigA = sut.GetSignature64("introduction to machine learning algorithms");
        var sigB = sut.GetSignature64("an introduction to machine learning algorithms and models");
        var sigC = sut.GetSignature64("the history of ancient Roman architecture");

        var distAB = SimHashService.HammingDistance(sigA, sigB);
        var distAC = SimHashService.HammingDistance(sigA, sigC);

        Assert.True(distAB < distAC,
            $"Similar texts should have lower distance (AB={distAB} vs AC={distAC})");
    }

    [Fact]
    public void BitCount_Is64()
    {
        Assert.Equal(64, SimHashService.BitCount);
    }
}
