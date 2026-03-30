using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class ChunkingServiceTests
{
    private readonly ChunkingService sut = new();

    [Fact]
    public void ChunkContent_NullOrEmpty_ReturnsEmpty()
    {
        Assert.Empty(sut.ChunkContent(null!));
        Assert.Empty(sut.ChunkContent(""));
        Assert.Empty(sut.ChunkContent("   "));
    }

    [Fact]
    public void ChunkContent_ShortText_ReturnsSingleChunk()
    {
        var text = "This is a short paragraph about learning.";
        var chunks = sut.ChunkContent(text);
        Assert.Single(chunks);
        Assert.Contains("short paragraph", chunks[0]);
    }

    [Fact]
    public void ChunkContent_LongText_SplitsIntoMultipleChunks()
    {
        // Generate text longer than MaxChunkSize (2500)
        var paragraphs = Enumerable.Range(1, 20)
            .Select(i => $"Paragraph {i}: " + new string('x', 200))
            .ToList();
        var text = string.Join("\n\n", paragraphs);

        var chunks = sut.ChunkContent(text);
        Assert.True(chunks.Count > 1, "Long text should produce multiple chunks");
    }

    [Fact]
    public void ChunkContent_NoChunkExceedsMaxSize()
    {
        var text = string.Join("\n\n",
            Enumerable.Range(1, 30).Select(i => $"Section {i}: " + new string('a', 300)));

        var chunks = sut.ChunkContent(text);
        // Allow some tolerance for overlap and boundary effects
        foreach (var chunk in chunks)
        {
            Assert.True(chunk.Length <= 3500,
                $"Chunk length {chunk.Length} exceeds reasonable max");
        }
    }

    [Fact]
    public void ChunkContent_PreservesAllContent()
    {
        var keywords = new[] { "Alpha", "Bravo", "Charlie", "Delta", "Echo" };
        var text = string.Join("\n\n",
            keywords.Select(k => $"{k}: " + new string('z', 600)));

        var chunks = sut.ChunkContent(text);
        var combined = string.Join(" ", chunks);

        foreach (var keyword in keywords)
        {
            Assert.Contains(keyword, combined);
        }
    }
}
