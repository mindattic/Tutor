using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class VectorStoreServiceTests
{
    [Test]
    public void Constructor_AcceptsDependencies()
    {
        var lsh = new LSHService(embeddingDimension: 8, bitCount: 16, seed: 1, paths: new TutorIngestionPaths());
        var simHash = new SimHashService();
        var sut = new VectorStoreService(lsh, simHash, new TutorIngestionPaths());
        Assert.That(sut, Is.Not.Null);
    }
}
