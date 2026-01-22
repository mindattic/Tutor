namespace Tutor.Models;

/// <summary>
/// Status of the concept map generation process.
/// </summary>
public enum ConceptMapStatus
{
    /// <summary>
    /// Concept map has not been started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Currently preparing the resource content for processing.
    /// </summary>
    PreparingContent,

    /// <summary>
    /// Currently extracting concepts from combined content.
    /// </summary>
    ExtractingConcepts,

    /// <summary>
    /// Currently building concept relationships.
    /// </summary>
    BuildingRelationships,

    /// <summary>
    /// Currently calculating complexity ordering.
    /// </summary>
    CalculatingComplexity,

    /// <summary>
    /// Concept map is ready for use.
    /// </summary>
    Ready,

    /// <summary>
    /// Concept map generation failed.
    /// </summary>
    Failed
}

/// <summary>
/// Represents an interconnected map of concepts and their relationships,
/// generated from a single Resource.
/// 
/// A ConceptMap is a graph structure where:
/// - Nodes are Concepts (atomic teachable units)
/// - Edges are relationships (prerequisites, related, etc.)
/// 
/// Key architectural points:
/// - Each Resource generates its own independent ConceptMap (1:1 relationship).
/// - A ConceptMap is INDEPENDENT of any Course.
/// - Multiple ConceptMaps are combined into a ConceptMapCollection for a Course.
/// - This allows dynamic composition of courses from any subset of resources.
/// 
/// Generation pipeline:
/// 1. Resource is uploaded and AI-formatted.
/// 2. ConceptMap is generated from the formatted resource.
/// 3. Concepts are extracted from the content.
/// 4. Concept relationships (prerequisites, related, complexity) are built.
/// 5. Course combines multiple ConceptMaps into a ConceptMapCollection.
/// </summary>
public class ConceptMap
{
    /// <summary>
    /// Unique identifier for this concept map.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The name of this concept map (typically the resource title).
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// A description of what this concept map covers.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// ID of the Resource used to generate this ConceptMap.
    /// Each ConceptMap is generated from exactly one Resource (1:1 relationship).
    /// </summary>
    public string ResourceId { get; set; } = "";

    /// <summary>
    /// Current status of the concept map generation.
    /// </summary>
    public ConceptMapStatus Status { get; set; } = ConceptMapStatus.NotStarted;

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
    /// All atomic Concepts in this concept map.
    /// Concepts are the smallest teachable units (nodes in the graph).
    /// </summary>
    public List<Concept> Concepts { get; set; } = [];

    /// <summary>
    /// Relationships between Concepts (edges in the graph).
    /// Forms the interconnected web of knowledge.
    /// </summary>
    public List<ConceptRelationship> Relations { get; set; } = [];

    /// <summary>
    /// Complexity ordering for Concepts (foundational to advanced).
    /// Used to display Concepts by increasing complexity.
    /// </summary>
    public List<ConceptComplexity> ComplexityOrder { get; set; } = [];

    /// <summary>
    /// When this concept map was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this concept map was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Version number for tracking changes and enabling safe replacement.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets the total number of concepts (nodes).
    /// </summary>
    public int TotalConcepts => Concepts.Count;

    /// <summary>
    /// Gets the total number of relations (edges).
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
    /// Gets the maximum complexity level in this ConceptMap.
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

    /// <summary>
    /// Converts from legacy KnowledgeBase to ConceptMap.
    /// </summary>
    public static ConceptMap FromKnowledgeBase(KnowledgeBase kb)
    {
        return new ConceptMap
        {
            Id = kb.Id,
            Name = kb.Name,
            Description = kb.Description,
            ResourceId = kb.ResourceId,
            Status = (ConceptMapStatus)(int)kb.Status,
            ErrorMessage = kb.ErrorMessage,
            Progress = kb.Progress,
            SourceContent = kb.SourceContent,
            Concepts = kb.Concepts,
            Relations = kb.Relations,
            ComplexityOrder = kb.ComplexityOrder,
            CreatedAt = kb.CreatedAt,
            UpdatedAt = kb.UpdatedAt,
            Version = kb.Version
        };
    }

    /// <summary>
    /// Converts to legacy KnowledgeBase for backward compatibility.
    /// </summary>
    public KnowledgeBase ToKnowledgeBase()
    {
        return new KnowledgeBase
        {
            Id = Id,
            Name = Name,
            Description = Description,
            ResourceId = ResourceId,
            Status = (KnowledgeBaseStatus)(int)Status,
            ErrorMessage = ErrorMessage,
            Progress = Progress,
            SourceContent = SourceContent,
            Concepts = Concepts,
            Relations = Relations,
            ComplexityOrder = ComplexityOrder,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            Version = Version
        };
    }
}
