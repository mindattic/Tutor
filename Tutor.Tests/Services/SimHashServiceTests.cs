using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class SimHashServiceTests
{
    private readonly SimHashService sut = new();

    [Test]
    public void GetSignature64_EmptyText_ReturnsZero()
    {
        Assert.That(sut.GetSignature64(""), Is.EqualTo(0UL));
        Assert.That(sut.GetSignature64(null!), Is.EqualTo(0UL));
    }

    [Test]
    public void GetSignature64_IsDeterministic()
    {
        var text = "The quick brown fox jumps over the lazy dog";
        var a = sut.GetSignature64(text);
        var b = sut.GetSignature64(text);
        Assert.That(b, Is.EqualTo(a));
    }

    [Test]
    public void GetSignature64_DifferentTexts_DifferentSignatures()
    {
        var a = sut.GetSignature64("machine learning algorithms");
        var b = sut.GetSignature64("underwater basket weaving");
        Assert.That(b, Is.Not.EqualTo(a));
    }

    [Test]
    public void HammingDistance_Identical_Zero()
    {
        Assert.That(SimHashService.HammingDistance(42UL, 42UL), Is.EqualTo(0));
    }

    [Test]
    public void HammingDistance_SingleBitDifference()
    {
        Assert.That(SimHashService.HammingDistance(0UL, 1UL), Is.EqualTo(1));
    }

    [Test]
    public void HammingDistance_AllBitsDifferent()
    {
        Assert.That(SimHashService.HammingDistance(0UL, ulong.MaxValue), Is.EqualTo(64));
    }

    [Test]
    public void SimilarTexts_LowHammingDistance()
    {
        var sigA = sut.GetSignature64("introduction to machine learning algorithms");
        var sigB = sut.GetSignature64("an introduction to machine learning algorithms and models");
        var sigC = sut.GetSignature64("the history of ancient Roman architecture");

        var distAB = SimHashService.HammingDistance(sigA, sigB);
        var distAC = SimHashService.HammingDistance(sigA, sigC);

        Assert.That(distAB < distAC, Is.True,
            $"Similar texts should have lower distance (AB={distAB} vs AC={distAC})");
    }

    [Test]
    public void BitCount_Is64()
    {
        Assert.That(SimHashService.BitCount, Is.EqualTo(64));
    }
}
