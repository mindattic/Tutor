using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class VectorStoreServiceTests
{
    [Fact]
    public void Constructor_AcceptsDependencies()
    {
        var lsh = new LSHService(embeddingDimension: 8, bitCount: 16, seed: 1);
        var simHash = new SimHashService();
        var sut = new VectorStoreService(lsh, simHash);
        Assert.NotNull(sut);
    }
}
