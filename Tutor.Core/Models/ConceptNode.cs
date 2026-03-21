namespace Tutor.Core.Models;

/// <summary>
/// Represents a core concept as a node in a hierarchical knowledge graph.
/// Concepts form a directed acyclic graph where edges represent prerequisite relationships.
/// Example: "Emperor" ? "Primarchs" ? "Horus Heresy" means you must understand
/// the Emperor before Primarchs, and Primarchs before the Horus Heresy.
/// </summary>
public class ConceptNode
{
    /// <summary>
    /// Unique identifier for this concept.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The canonical name/term for this concept.
    /// </summary>
    public string Term { get; set; } = "";

    /// <summary>
    /// A concise description of the concept.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Extended explanation with examples and context.
    /// </summary>
    public string ExtendedContent { get; set; } = "";

    /// <summary>
    /// Alternative names or synonyms for this concept.
    /// Used for matching during concept discovery.
    /// </summary>
    public List<string> Aliases { get; set; } = [];

    /// <summary>
    /// The embedding vector for semantic similarity comparisons.
    /// Generated from the term + description for dense retrieval.
    /// </summary>
    public float[] Embedding { get; set; } = [];

    /// <summary>
    /// LSH signature for fast approximate nearest neighbor search.
    /// </summary>
    public byte[] SemanticSignature { get; set; } = [];

    /// <summary>
    /// SimHash signature for lexical similarity.
    /// </summary>
    public ulong LexicalSignature { get; set; }

    /// <summary>
    /// IDs of concepts that must be understood BEFORE this one (prerequisites).
    /// These are the incoming edges in the knowledge graph.
    /// </summary>
    public List<string> PrerequisiteIds { get; set; } = [];

    /// <summary>
    /// IDs of concepts that depend on this one (this is their prerequisite).
    /// These are the outgoing edges in the knowledge graph.
    /// </summary>
    public List<string> DependentIds { get; set; } = [];

    /// <summary>
    /// Computed depth in the knowledge hierarchy.
    /// Concepts with no prerequisites have depth 0 (foundational).
    /// Higher depth = more advanced/dependent concepts.
    /// </summary>
    public int HierarchyDepth { get; set; }

    /// <summary>
    /// IDs of resources where this concept was discovered or is discussed.
    /// </summary>
    public List<string> SourceResourceIds { get; set; } = [];

    /// <summary>
    /// IDs of content chunks most relevant to this concept.
    /// Used for retrieval when explaining the concept.
    /// </summary>
    public List<string> RelevantChunkIds { get; set; } = [];

    /// <summary>
    /// Tags/categories for grouping related concepts.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Confidence score (0-1) indicating how well-established this concept is.
    /// Higher scores indicate more supporting evidence from resources.
    /// </summary>
    public float ConfidenceScore { get; set; } = 1.0f;

    /// <summary>
    /// Whether this concept has been reviewed/confirmed by the user.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// When this concept was first discovered/added.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this concept was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates the text representation used for generating embeddings.
    /// </summary>
    public string GetEmbeddingText()
    {
        var parts = new List<string> { Term };
        
        if (!string.IsNullOrWhiteSpace(Description))
            parts.Add(Description);
            
        if (Aliases.Count > 0)
            parts.Add($"Also known as: {string.Join(", ", Aliases)}");

        return string.Join(". ", parts);
    }

    /// <summary>
    /// Checks if this concept is foundational (no prerequisites).
    /// </summary>
    public bool IsFoundational => PrerequisiteIds.Count == 0;

    /// <summary>
    /// Checks if this concept is a leaf (no dependents).
    /// </summary>
    public bool IsLeaf => DependentIds.Count == 0;
}
