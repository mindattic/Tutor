namespace Tutor.Models;

/// <summary>
/// Status of the knowledge base generation process.
/// </summary>
public enum KnowledgeBaseStatus
{
    /// <summary>
    /// Knowledge base has not been started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Currently preparing the resource content for processing.
    /// </summary>
    PreparingContent,

    /// <summary>
    /// Currently extracting concepts from combined content.
    /// </summary>
    GeneratingConcepts,

    /// <summary>
    /// Currently building concept relationships.
    /// </summary>
    BuildingRelationships,

    /// <summary>
    /// Currently calculating complexity ordering.
    /// </summary>
    CalculatingComplexity,

    /// <summary>
    /// Knowledge base is ready for use.
    /// </summary>
    Ready,

    /// <summary>
    /// Knowledge base generation failed.
    /// </summary>
    Failed
}

/// <summary>
/// Represents an interconnected library of knowledge generated from a single Resource.
/// 
/// Key architectural points:
/// - Each Resource generates its own independent KnowledgeBase (1:1 relationship).
/// - A KnowledgeBase is INDEPENDENT of any Course.
/// - It contains all Concepts and their relationships for a single resource.
/// - Multiple KnowledgeBases are combined into a KnowledgeBaseCollection for a Course.
/// - The Course creates its own learning structure (Lessons/Topics) that REFERENCES
///   Concepts from the KnowledgeBaseCollection by ID (no duplication).
/// - This architecture allows dynamic composition of courses from any subset of resources.
/// - Reduces overhead since resources don't need to be merged into a single KnowledgeBase.
/// 
/// Generation pipeline:
/// 1. Resource is uploaded and stored as-is.
/// 2. KnowledgeBase is generated from the single resource.
/// 3. Concepts are extracted from the content.
/// 4. Concept relationships (prerequisites, related, complexity) are built.
/// 5. Course combines multiple KnowledgeBases into a KnowledgeBaseCollection.
/// </summary>
public class KnowledgeBase
{
    /// <summary>
    /// Unique identifier for this knowledge base.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The name of this knowledge base.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// A description of what this knowledge base covers.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// ID of the Resource used to generate this KnowledgeBase.
    /// Each KnowledgeBase is generated from exactly one Resource (1:1 relationship).
    /// </summary>
    public string ResourceId { get; set; } = "";

    /// <summary>
    /// IDs of Resources used to generate this KnowledgeBase.
    /// Maintained for backward compatibility during migration.
    /// New code should use ResourceId instead.
    /// </summary>
    [Obsolete("Use ResourceId instead. This property is maintained for backward compatibility.")]
    public List<string> ResourceIds { get; set; } = [];

    /// <summary>
    /// Current status of the knowledge base generation.
    /// </summary>
    public KnowledgeBaseStatus Status { get; set; } = KnowledgeBaseStatus.NotStarted;

    /// <summary>
    /// Error message if status is Failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Progress percentage (0-100) during generation.
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// The processed content from the source resource.
    /// Stored to allow re-processing without re-fetching the resource.
    /// </summary>
    public string? SourceContent { get; set; }

    /// <summary>
    /// The combined text from all resources.
    /// Maintained for backward compatibility during migration.
    /// New code should use SourceContent instead.
    /// </summary>
    [Obsolete("Use SourceContent instead. This property is maintained for backward compatibility.")]
    public string? CombinedContent { get; set; }

    /// <summary>
    /// All atomic Concepts in this knowledge base.
    /// Concepts are the smallest teachable units.
    /// </summary>
    public List<Concept> Concepts { get; set; } = [];

    /// <summary>
    /// Relationships between Concepts (prerequisites, related, etc.).
    /// Forms the interconnected web of knowledge.
    /// </summary>
    public List<ConceptRelationship> Relations { get; set; } = [];

    /// <summary>
    /// Complexity ordering for Concepts (foundational to advanced).
    /// Used to display Concepts by increasing complexity in the KnowledgeBase view.
    /// </summary>
    public List<ConceptComplexity> ComplexityOrder { get; set; } = [];

    /// <summary>
    /// When this knowledge base was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this knowledge base was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Version number for tracking changes and enabling safe replacement.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets the total number of concepts.
    /// </summary>
    public int TotalConcepts => Concepts.Count;

    /// <summary>
    /// Gets the total number of relations.
    /// </summary>
    public int TotalRelations => Relations.Count;

    /// <summary>
    /// Gets a Concept by ID.
    /// </summary>
    public Concept? GetConcept(string conceptId)
    {
        return Concepts.FirstOrDefault(c => c.Id == conceptId);
    }

    /// <summary>
    /// Gets a Concept by title (case-insensitive).
    /// </summary>
    public Concept? GetConceptByTitle(string title)
    {
        return Concepts.FirstOrDefault(c => 
            c.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all Concepts ordered by complexity (foundational first).
    /// </summary>
    public IEnumerable<Concept> GetConceptsByComplexity()
    {
        var orderedIds = ComplexityOrder
            .OrderBy(c => c.Level)
            .ThenBy(c => c.PrerequisiteCount)
            .Select(c => c.ConceptId)
            .ToList();

        foreach (var id in orderedIds)
        {
            var concept = GetConcept(id);
            if (concept != null)
                yield return concept;
        }

        // Include any concepts not in complexity order
        var orderedSet = orderedIds.ToHashSet();
        foreach (var concept in Concepts.Where(c => !orderedSet.Contains(c.Id)))
        {
            yield return concept;
        }
    }

    /// <summary>
    /// Gets Concepts at a specific complexity level.
    /// </summary>
    public IEnumerable<Concept> GetConceptsAtLevel(int level)
    {
        var idsAtLevel = ComplexityOrder
            .Where(c => c.Level == level)
            .Select(c => c.ConceptId)
            .ToHashSet();

        return Concepts.Where(c => idsAtLevel.Contains(c.Id));
    }

    /// <summary>
    /// Gets the maximum complexity level in this KnowledgeBase.
    /// </summary>
    public int MaxComplexityLevel => ComplexityOrder.Count > 0 
        ? ComplexityOrder.Max(c => c.Level) 
        : 0;

    /// <summary>
    /// Gets prerequisite Concepts for a given Concept.
    /// </summary>
    public IEnumerable<Concept> GetPrerequisites(string conceptId)
    {
        var prereqIds = Relations
            .Where(r => r.TargetConceptId == conceptId && r.RelationType == ConceptRelationType.Prerequisite)
            .Select(r => r.SourceConceptId);

        return prereqIds.Select(GetConcept).Where(c => c != null).Cast<Concept>();
    }

    /// <summary>
    /// Gets Concepts that depend on a given Concept (it is their prerequisite).
    /// </summary>
    public IEnumerable<Concept> GetDependents(string conceptId)
    {
        var depIds = Relations
            .Where(r => r.SourceConceptId == conceptId && r.RelationType == ConceptRelationType.Prerequisite)
            .Select(r => r.TargetConceptId);

        return depIds.Select(GetConcept).Where(c => c != null).Cast<Concept>();
    }

    /// <summary>
    /// Gets Concepts related to a given Concept (non-prerequisite relationships).
    /// </summary>
    public IEnumerable<Concept> GetRelatedConcepts(string conceptId)
    {
        var relatedIds = Relations
            .Where(r => (r.SourceConceptId == conceptId || r.TargetConceptId == conceptId) 
                        && r.RelationType != ConceptRelationType.Prerequisite)
            .Select(r => r.SourceConceptId == conceptId ? r.TargetConceptId : r.SourceConceptId)
            .Distinct();

        return relatedIds.Select(GetConcept).Where(c => c != null).Cast<Concept>();
    }

    /// <summary>
    /// Gets the complexity info for a Concept.
    /// </summary>
    public ConceptComplexity? GetComplexity(string conceptId)
    {
        return ComplexityOrder.FirstOrDefault(c => c.ConceptId == conceptId);
    }
}

/// <summary>
/// Represents a Concept's complexity level within the KnowledgeBase.
/// Used to order concepts from simple to complex for progressive learning.
/// </summary>
public class ConceptComplexity
{
    /// <summary>
    /// ID of the Concept.
    /// </summary>
    public string ConceptId { get; set; } = "";

    /// <summary>
    /// Complexity level (0 = foundational, higher = more complex).
    /// Determined by the number of prerequisites and depth in the knowledge graph.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Number of direct prerequisites for this concept.
    /// </summary>
    public int PrerequisiteCount { get; set; }

    /// <summary>
    /// Number of concepts that depend on this one.
    /// </summary>
    public int DependentCount { get; set; }

    /// <summary>
    /// Centrality score indicating how connected this concept is (0.0 to 1.0).
    /// Higher values mean the concept is more central to the knowledge base.
    /// </summary>
    public float Centrality { get; set; }
}
