using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class LSHServiceTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var sut = new LSHService(embeddingDimension: 512, bitCount: 128, seed: 42);
        Assert.Equal(512, sut.EmbeddingDimension);
        Assert.Equal(128, sut.BitCount);
        Assert.Equal(42, sut.Seed);
    }

    [Fact]
    public void DefaultConstructor_Uses1536Dims_256Bits()
    {
        var sut = new LSHService();
        Assert.Equal(1536, sut.EmbeddingDimension);
        Assert.Equal(256, sut.BitCount);
        Assert.Equal(1337, sut.Seed);
    }

    [Fact]
    public void GetSignature_EmptyEmbedding_ReturnsEmpty()
    {
        var sut = new LSHService();
        var sig = sut.GetSignature([]);
        Assert.Empty(sig);
    }

    [Fact]
    public void GetSignature_NullEmbedding_ReturnsEmpty()
    {
        var sut = new LSHService();
        var sig = sut.GetSignature(null!);
        Assert.Empty(sig);
    }

    [Fact]
    public void GetSignature_ProducesCorrectLength()
    {
        var sut = new LSHService(embeddingDimension: 8, bitCount: 16, seed: 1);
        var embedding = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f };
        var sig = sut.GetSignature(embedding);
        Assert.Equal(2, sig.Length); // 16 bits = 2 bytes
    }

    [Fact]
    public void GetSignature_IsDeterministic()
    {
        var sut = new LSHService(embeddingDimension: 8, bitCount: 16, seed: 1);
        var embedding = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f };
        var sig1 = sut.GetSignature(embedding);
        var sig2 = sut.GetSignature(embedding);
        Assert.Equal(sig1, sig2);
    }

    [Fact]
    public void HammingDistance_IdenticalSignatures_Zero()
    {
        var a = new byte[] { 0xFF, 0x00, 0xAA };
        Assert.Equal(0, LSHService.HammingDistance(a, a));
    }

    [Fact]
    public void HammingDistance_OppositeSignatures_AllBits()
    {
        var a = new byte[] { 0x00 };
        var b = new byte[] { 0xFF };
        Assert.Equal(8, LSHService.HammingDistance(a, b));
    }

    [Fact]
    public void HammingDistance_DifferentLengths_CountsExtraBits()
    {
        var a = new byte[] { 0x00 };
        var b = new byte[] { 0x00, 0xFF };
        Assert.Equal(8, LSHService.HammingDistance(a, b));
    }

    [Fact]
    public void HammingDistance_NullInputs_ReturnsMaxValue()
    {
        Assert.Equal(int.MaxValue, LSHService.HammingDistance(null!, new byte[] { 0x00 }));
        Assert.Equal(int.MaxValue, LSHService.HammingDistance(new byte[] { 0x00 }, null!));
    }

    [Fact]
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

        Assert.True(distAB < distAC,
            $"Similar vectors should have lower distance (AB={distAB} vs AC={distAC})");
    }
}
