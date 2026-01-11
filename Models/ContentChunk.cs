namespace Tutor.Models;

/// <summary>
/// Represents a chunk of content from a learning resource with its embedding vector.
/// Used for RAG (Retrieval-Augmented Generation) to find relevant content.
/// </summary>
public class ContentChunk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// The ID of the learning resource this chunk belongs to.
    /// </summary>
    public string ResourceId { get; set; } = "";
    
    /// <summary>
    /// The ID of the curriculum this chunk belongs to (for efficient filtering).
    /// </summary>
    public string CurriculumId { get; set; } = "";
    
    /// <summary>
    /// The index of this chunk within the resource (for ordering).
    /// </summary>
    public int ChunkIndex { get; set; }
    
    /// <summary>
    /// The actual text content of this chunk.
    /// </summary>
    public string Content { get; set; } = "";
    
    /// <summary>
    /// The embedding vector for this chunk (1536 dimensions for text-embedding-3-small).
    /// </summary>
    public float[] Embedding { get; set; } = [];

    /// <summary>
    /// LSH signature for fast approximate nearest neighbor search (semantic similarity).
    /// Computed from the embedding vector using random hyperplane projections.
    /// </summary>
    public byte[] SemanticSignature { get; set; } = [];

    /// <summary>
    /// SimHash signature for lexical similarity comparison.
    /// Computed from the text content using FNV-1a hashing.
    /// </summary>
    public ulong LexicalSignature { get; set; }
    
    /// <summary>
    /// Title of the source resource (for context when retrieved).
    /// </summary>
    public string SourceTitle { get; set; } = "";
    
    /// <summary>
    /// When this chunk was created/embedded.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
