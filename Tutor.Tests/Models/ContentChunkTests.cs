using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ContentChunkTests
{
    [Test]
    public void Defaults_AreSensible()
    {
        var chunk = new ContentChunk();

        Assert.That(string.IsNullOrWhiteSpace(chunk.Id), Is.False);
        Assert.That(Guid.TryParse(chunk.Id, out _), Is.True);
        Assert.That(chunk.ResourceId, Is.EqualTo(""));
        Assert.That(chunk.CurriculumId, Is.EqualTo(""));
        Assert.That(chunk.ChunkIndex, Is.EqualTo(0));
        Assert.That(chunk.Content, Is.EqualTo(""));
        Assert.That(chunk.Embedding, Is.Not.Null);
        Assert.That(chunk.Embedding, Is.Empty);
        Assert.That(chunk.SemanticSignature, Is.Not.Null);
        Assert.That(chunk.SemanticSignature, Is.Empty);
        Assert.That(chunk.LexicalSignature, Is.EqualTo(0UL));
        Assert.That(chunk.SourceTitle, Is.EqualTo(""));
        Assert.That(chunk.CreatedAt > DateTime.MinValue, Is.True);
    }

    [Test]
    public void Id_IsUniquePerInstance()
    {
        var a = new ContentChunk();
        var b = new ContentChunk();
        Assert.That(b.Id, Is.Not.EqualTo(a.Id));
    }

    [Test]
    public void Properties_AreMutable()
    {
        var chunk = new ContentChunk
        {
            Id = "fixed-id",
            ResourceId = "r1",
            CurriculumId = "c1",
            ChunkIndex = 7,
            Content = "hello",
            Embedding = new[] { 0.1f, 0.2f },
            SemanticSignature = new byte[] { 1, 2, 3 },
            LexicalSignature = 0xDEADBEEFUL,
            SourceTitle = "Title"
        };

        Assert.That(chunk.Id, Is.EqualTo("fixed-id"));
        Assert.That(chunk.ResourceId, Is.EqualTo("r1"));
        Assert.That(chunk.ChunkIndex, Is.EqualTo(7));
        Assert.That(chunk.Embedding.Length, Is.EqualTo(2));
        Assert.That(chunk.SemanticSignature.Length, Is.EqualTo(3));
        Assert.That(chunk.LexicalSignature, Is.EqualTo(0xDEADBEEFUL));
    }
}
