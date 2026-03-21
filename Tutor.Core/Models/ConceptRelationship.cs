namespace Tutor.Core.Models;

/// <summary>
/// Represents a directed edge in the knowledge graph between two concepts.
/// The edge indicates that the source concept is a prerequisite for the target concept.
/// </summary>
public class ConceptRelationship
{
    /// <summary>
    /// Unique identifier for this relationship.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ID of the prerequisite concept (the concept that must be understood first).
    /// </summary>
    public string SourceConceptId { get; set; } = "";

    /// <summary>
    /// ID of the dependent concept (the concept that requires the prerequisite).
    /// </summary>
    public string TargetConceptId { get; set; } = "";

    /// <summary>
    /// The type of relationship between concepts.
    /// </summary>
    public ConceptRelationType RelationType { get; set; } = ConceptRelationType.Prerequisite;

    /// <summary>
    /// Confidence score (0-1) for this relationship.
    /// Higher scores indicate stronger evidence for the relationship.
    /// Computed from semantic similarity, co-occurrence, and other signals.
    /// </summary>
    public float ConfidenceScore { get; set; }

    /// <summary>
    /// Cosine similarity between the concept embeddings.
    /// </summary>
    public float SemanticSimilarity { get; set; }

    /// <summary>
    /// LSH Hamming distance between semantic signatures.
    /// Lower values indicate higher similarity.
    /// </summary>
    public int SemanticDistance { get; set; }

    /// <summary>
    /// SimHash Hamming distance for lexical similarity.
    /// Lower values indicate higher similarity.
    /// </summary>
    public int LexicalDistance { get; set; }

    /// <summary>
    /// How often these concepts appear together in the source material.
    /// Higher values suggest stronger relationship.
    /// </summary>
    public int CoOccurrenceCount { get; set; }

    /// <summary>
    /// IDs of resources where this relationship was discovered.
    /// </summary>
    public List<string> EvidenceResourceIds { get; set; } = [];

    /// <summary>
    /// IDs of chunks that provide evidence for this relationship.
    /// </summary>
    public List<string> EvidenceChunkIds { get; set; } = [];

    /// <summary>
    /// Optional explanation of why this relationship exists.
    /// </summary>
    public string Justification { get; set; } = "";

    /// <summary>
    /// Whether this relationship has been confirmed by the user or LLM analysis.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// When this relationship was discovered.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this relationship was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a weighted combined score from all similarity metrics.
    /// </summary>
    public float ComputeCombinedScore(float semanticWeight = 0.5f, float distanceWeight = 0.3f, float coOccurrenceWeight = 0.2f)
    {
        // Normalize LSH distance (0-256 typically) to 0-1 similarity
        var normalizedSemanticDist = 1.0f - Math.Clamp(SemanticDistance / 128f, 0f, 1f);
        
        // Normalize lexical distance to 0-1 similarity
        var normalizedLexicalDist = 1.0f - Math.Clamp(LexicalDistance / 32f, 0f, 1f);
        
        // Average the distance-based scores
        var distanceScore = (normalizedSemanticDist + normalizedLexicalDist) / 2f;
        
        // Normalize co-occurrence (assuming max of ~100 is very strong)
        var normalizedCoOccurrence = Math.Clamp(CoOccurrenceCount / 50f, 0f, 1f);

        return (SemanticSimilarity * semanticWeight) +
               (distanceScore * distanceWeight) +
               (normalizedCoOccurrence * coOccurrenceWeight);
    }
}

/// <summary>
/// Types of relationships between concepts in the knowledge graph.
/// </summary>
public enum ConceptRelationType
{
    /// <summary>
    /// Source must be understood before target (foundational dependency).
    /// Example: "Addition" is a prerequisite for "Multiplication"
    /// </summary>
    Prerequisite,

    /// <summary>
    /// Concepts are strongly related but neither is strictly required first.
    /// Example: "Velocity" and "Speed" are related
    /// </summary>
    Related,

    /// <summary>
    /// Target is a specific instance or example of the source.
    /// Example: "Horus" is an instance of "Primarch"
    /// </summary>
    Instance,

    /// <summary>
    /// Target is a part or component of the source.
    /// Example: "Engine" is part of "Car"
    /// </summary>
    PartOf,


    /// <summary>
    /// Target is a specialization or subtype of the source.
    /// Example: "Traitor Primarch" specializes "Primarch"
    /// </summary>
    Specialization,

    /// <summary>
    /// Concepts are frequently mentioned together but relationship type is unclear.
    /// </summary>
    CoOccurs,

    /// <summary>
    /// Source concept is a broader category containing the target.
    /// Example: "Mammals" contains "Dogs"
    /// </summary>
    Contains,

    /// <summary>
    /// Source concept is a specific instance of the target.
    /// Example: "Golden Retriever" is an instance of "Dogs"
    /// </summary>
    InstanceOf,

    /// <summary>
    /// Concepts are similar or easily confused.
    /// Example: "Mitosis" is similar to "Meiosis"
    /// </summary>
    SimilarTo,

    /// <summary>
    /// Concepts are opposites or contrasting ideas.
    /// Example: "Oxidation" contrasts with "Reduction"
    /// </summary>
    ContrastsWith
}
