namespace Tutor.Core.Models;

/// <summary>
/// Represents an atomic teachable concept - the smallest unit of learning.
/// A Concept is a single piece of knowledge that can be taught and tested.
/// This is distinct from ConceptNode which is used for the knowledge graph.
/// </summary>
public class Concept
{
    /// <summary>
    /// Unique identifier for this concept.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The title of this concept (the term or name being taught).
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// A concise summary explaining this concept (1-3 sentences).
    /// </summary>
    public string Summary { get; set; } = "";

    /// <summary>
    /// Extended content or detailed explanation of this concept.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Alternative names or terms for this concept.
    /// </summary>
    public List<string> Aliases { get; set; } = [];

    /// <summary>
    /// Tags for categorization and filtering.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// IDs of prerequisite concepts that should be learned first.
    /// </summary>
    public List<string> PrerequisiteIds { get; set; } = [];

    /// <summary>
    /// IDs of related concepts (not prerequisites, just related).
    /// </summary>
    public List<string> RelatedIds { get; set; } = [];

    /// <summary>
    /// The ID of the concept map this concept belongs to.
    /// </summary>
    public string ConceptMapId { get; set; } = "";

    /// <summary>
    /// The ID of the corresponding ConceptNode in the knowledge graph (if any).
    /// </summary>
    public string? ConceptNodeId { get; set; }

    /// <summary>
    /// IDs of source resources this concept was extracted from.
    /// </summary>
    public List<string> SourceResourceIds { get; set; } = [];

    /// <summary>
    /// Confidence score from the AI extraction (0.0 to 1.0).
    /// </summary>
    public float ConfidenceScore { get; set; } = 1.0f;

    /// <summary>
    /// When this concept was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this concept was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this concept was dynamically created through user query expansion
    /// (as opposed to being extracted during initial ConceptMap build).
    /// </summary>
    public bool IsDynamicallyExpanded { get; set; }

    /// <summary>
    /// The original user query that triggered the dynamic creation of this concept.
    /// Only set if IsDynamicallyExpanded is true.
    /// </summary>
    public string? ExpansionQuery { get; set; }

    /// <summary>
    /// Source excerpt from the resource that supports this concept's existence.
    /// Used for citation and validation that the concept is grounded in source material.
    /// </summary>
    public string? SourceExcerpt { get; set; }
}
