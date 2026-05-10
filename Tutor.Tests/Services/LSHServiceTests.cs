using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class LSHServiceTests
{
    [Test]
    public void Constructor_SetsProperties()
    {
        var sut = new LSHService(embeddingDimension: 512, bitCount: 128, seed: 42);
        Assert.That(sut.EmbeddingDimension, Is.EqualTo(512));
        Assert.That(sut.BitCount, Is.EqualTo(128));
        Assert.That(sut.Seed, Is.EqualTo(42));
    }

    [Test]
    public void DefaultConstructor_Uses1536Dims_256Bits()
    {
        var sut = new LSHService();
        Assert.That(sut.EmbeddingDimension, Is.EqualTo(1536));
        Assert.That(sut.BitCount, Is.EqualTo(256));
        Assert.That(sut.Seed, Is.EqualTo(1337));
    }

    [Test]
    public void GetSignature_EmptyEmbedding_ReturnsEmpty()
    {
        var sut = new LSHService();
        var sig = sut.GetSignature([]);
        Assert.That(sig, Is.Empty);
    }

    [Test]
    public void GetSignature_NullEmbedding_ReturnsEmpty()
    {
        var sut = new LSHService();
        var sig = sut.GetSignature(null!);
        Assert.That(sig, Is.Empty);
    }

    [Test]
    public void GetSignature_ProducesCorrectLength()
    {
        var sut = new LSHService(embeddingDimension: 8, bitCount: 16, seed: 1);
        var embedding = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f };
        var sig = sut.GetSignature(embedding);
        Assert.That(sig.Length, Is.EqualTo(2)); // 16 bits = 2 bytes
    }

    [Test]
    public void GetSignature_IsDeterministic()
    {
        var sut = new LSHService(embeddingDimension: 8, bitCount: 16, seed: 1);
        var embedding = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f };
        var sig1 = sut.GetSignature(embedding);
        var sig2 = sut.GetSignature(embedding);
        Assert.That(sig2, Is.EqualTo(sig1));
    }

    [Test]
    public void HammingDistance_IdenticalSignatures_Zero()
    {
        var a = new byte[] { 0xFF, 0x00, 0xAA };
        Assert.That(LSHService.HammingDistance(a, a), Is.EqualTo(0));
    }

    [Test]
    public void HammingDistance_OppositeSignatures_AllBits()
    {
        var a = new byte[] { 0x00 };
        var b = new byte[] { 0xFF };
        Assert.That(LSHService.HammingDistance(a, b), Is.EqualTo(8));
    }

    [Test]
    public void HammingDistance_DifferentLengths_CountsExtraBits()
    {
        var a = new byte[] { 0x00 };
        var b = new byte[] { 0x00, 0xFF };
        Assert.That(LSHService.HammingDistance(a, b), Is.EqualTo(8));
    }

    [Test]
    public void HammingDistance_NullInputs_ReturnsMaxValue()
    {
        Assert.That(LSHService.HammingDistance(null!, new byte[] { 0x00 }), Is.EqualTo(int.MaxValue));
        Assert.That(LSHService.HammingDistance(new byte[] { 0x00 }, null!), Is.EqualTo(int.MaxValue));
    }

    [Test]
    public void SimilarEmbeddings_ProduceLowHammingDistance()
    {
        var sut = new LSHService(embeddingDimension: 8, bitCount: 64, seed: 42);
        var a = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var b = new float[] { 1.01f, 2.01f, 3.01f, 4.01f, 5.01f, 6.01f, 7.01f, 8.01f };
        var c = new float[] { -8, -7, -6, -5, -4, -3, -2, -1 };

        var sigA = sut.GetSignature(a);
        var sigB = sut.GetSignature(b);
        var sigC = sut.GetSignature(c);

        var distAB = LSHService.HammingDistance(sigA, sigB);
        var distAC = LSHService.HammingDistance(sigA, sigC);

        Assert.That(distAB < distAC, Is.True,
            $"Similar vectors should have lower distance (AB={distAB} vs AC={distAC})");
    }
}
