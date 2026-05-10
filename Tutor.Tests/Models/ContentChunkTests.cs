using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ContentChunkTests
{
    [Fact]
    public void Defaults_AreSensible()
    {
        var chunk = new ContentChunk();

        Assert.False(string.IsNullOrWhiteSpace(chunk.Id));
        Assert.True(Guid.TryParse(chunk.Id, out _));
        Assert.Equal("", chunk.ResourceId);
        Assert.Equal("", chunk.CurriculumId);
        Assert.Equal(0, chunk.ChunkIndex);
        Assert.Equal("", chunk.Content);
        Assert.NotNull(chunk.Embedding);
        Assert.Empty(chunk.Embedding);
        Assert.NotNull(chunk.SemanticSignature);
        Assert.Empty(chunk.SemanticSignature);
        Assert.Equal(0UL, chunk.LexicalSignature);
        Assert.Equal("", chunk.SourceTitle);
        Assert.True(chunk.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public void Id_IsUniquePerInstance()
    {
        var a = new ContentChunk();
        var b = new ContentChunk();
        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
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

        Assert.Equal("fixed-id", chunk.Id);
        Assert.Equal("r1", chunk.ResourceId);
        Assert.Equal(7, chunk.ChunkIndex);
        Assert.Equal(2, chunk.Embedding.Length);
        Assert.Equal(3, chunk.SemanticSignature.Length);
        Assert.Equal(0xDEADBEEFUL, chunk.LexicalSignature);
    }
}
