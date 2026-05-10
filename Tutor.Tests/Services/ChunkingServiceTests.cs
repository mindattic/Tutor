using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class ChunkingServiceTests
{
    private readonly ChunkingService sut = new();

    [Test]
    public void ChunkContent_NullOrEmpty_ReturnsEmpty()
    {
        Assert.That(sut.ChunkContent(null!), Is.Empty);
        Assert.That(sut.ChunkContent(""), Is.Empty);
        Assert.That(sut.ChunkContent("   "), Is.Empty);
    }

    [Test]
    public void ChunkContent_ShortText_ReturnsSingleChunk()
    {
        var text = "This is a short paragraph about learning.";
        var chunks = sut.ChunkContent(text);
        Assert.That(chunks, Has.Count.EqualTo(1));
        Assert.That(chunks[0], Does.Contain("short paragraph"));
    }

    [Test]
    public void ChunkContent_LongText_SplitsIntoMultipleChunks()
    {
        // Generate text longer than MaxChunkSize (2500)
        var paragraphs = Enumerable.Range(1, 20)
            .Select(i => $"Paragraph {i}: " + new string('x', 200))
            .ToList();
        var text = string.Join("\n\n", paragraphs);

        var chunks = sut.ChunkContent(text);
        Assert.That(chunks.Count > 1, Is.True, "Long text should produce multiple chunks");
    }

    [Test]
    public void ChunkContent_NoChunkExceedsMaxSize()
    {
        var text = string.Join("\n\n",
            Enumerable.Range(1, 30).Select(i => $"Section {i}: " + new string('a', 300)));

        var chunks = sut.ChunkContent(text);
        // Allow some tolerance for overlap and boundary effects
        foreach (var chunk in chunks)
        {
            Assert.That(chunk.Length <= 3500, Is.True,
                $"Chunk length {chunk.Length} exceeds reasonable max");
        }
    }

    [Test]
    public void ChunkContent_PreservesAllContent()
    {
        var keywords = new[] { "Alpha", "Bravo", "Charlie", "Delta", "Echo" };
        var text = string.Join("\n\n",
            keywords.Select(k => $"{k}: " + new string('z', 600)));

        var chunks = sut.ChunkContent(text);
        var combined = string.Join(" ", chunks);

        foreach (var keyword in keywords)
        {
            Assert.That(combined, Does.Contain(keyword));
        }
    }
}
