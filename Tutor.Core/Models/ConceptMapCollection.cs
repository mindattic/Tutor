namespace Tutor.Core.Models;

/// <summary>
/// Represents a collection of ConceptMaps that are combined for a Course.
/// 
/// A ConceptMapCollection aggregates multiple ConceptMaps (one per resource)
/// to form a unified knowledge graph that can be visualized using D3.js force layout.
/// 
/// Key architectural points:
/// - Each Resource generates its own ConceptMap independently
/// - A Course combines multiple Resources' ConceptMaps into a ConceptMapCollection
/// - This allows dynamic composition of courses from any subset of resources
/// - The collection can be rendered as a single force-directed graph
/// 
/// Example:
/// - Course "Intro to Science" has Resources: Physics Textbook, Chemistry Guide
/// - Physics Textbook → ConceptMap A (physics concepts)
/// - Chemistry Guide → ConceptMap B (chemistry concepts)
/// - Course.ConceptMapCollection contains [A, B]
/// - D3.js visualization shows combined graph from both ConceptMaps
/// </summary>
public class ConceptMapCollection
{
    /// <summary>
    /// Unique identifier for this collection.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name of the collection (usually matches Course name).
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Description of what this collection covers.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// IDs of ConceptMaps in this collection.
    /// Each ConceptMap corresponds to a single Resource.
    /// </summary>
    public List<string> ConceptMapIds { get; set; } = [];

    /// <summary>
    /// When this collection was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this collection was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the total number of ConceptMaps in this collection.
    /// </summary>
    public int Count => ConceptMapIds.Count;

    /// <summary>
    /// Whether the collection has any ConceptMaps.
    /// </summary>
    public bool HasConceptMaps => ConceptMapIds.Count > 0;

    /// <summary>
    /// Adds a ConceptMap to the collection if not already present.
    /// </summary>
    public bool AddConceptMap(string conceptMapId)
    {
        if (string.IsNullOrEmpty(conceptMapId) || ConceptMapIds.Contains(conceptMapId))
            return false;

        ConceptMapIds.Add(conceptMapId);
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Removes a ConceptMap from the collection.
    /// </summary>
    public bool RemoveConceptMap(string conceptMapId)
    {
        var removed = ConceptMapIds.Remove(conceptMapId);
        if (removed)
            UpdatedAt = DateTime.UtcNow;
        return removed;
    }

    /// <summary>
    /// Checks if a ConceptMap is in this collection.
    /// </summary>
    public bool ContainsConceptMap(string conceptMapId)
    {
        return ConceptMapIds.Contains(conceptMapId);
    }
}

/// <summary>
/// Represents an aggregated view of multiple ConceptMaps loaded in memory.
/// Provides unified query methods across all ConceptMaps in a collection.
/// 
/// This is the in-memory representation used for:
/// - D3.js force-directed graph visualization
/// - Aggregated concept queries
/// - Cross-resource relationship analysis
/// </summary>
public class LoadedConceptMapCollection
{
    /// <summary>
    /// The collection metadata.
    /// </summary>
    public ConceptMapCollection Collection { get; }

    /// <summary>
    /// The loaded ConceptMaps.
    /// </summary>
    public List<ConceptMap> ConceptMaps { get; }

    public LoadedConceptMapCollection(ConceptMapCollection collection, List<ConceptMap> conceptMaps)
    {
        Collection = collection;
        ConceptMaps = conceptMaps;
    }

    /// <summary>
    /// Gets all concepts across all ConceptMaps in the collection.
    /// These become the nodes in the D3.js force layout.
    /// </summary>
    public IEnumerable<Concept> GetAllConcepts()
    {
        return ConceptMaps.SelectMany(cm => cm.Concepts);
    }

    /// <summary>
    /// Gets all relationships across all ConceptMaps in the collection.
    /// These become the links/edges in the D3.js force layout.
    /// </summary>
    public IEnumerable<ConceptRelationship> GetAllRelations()
    {
        return ConceptMaps.SelectMany(cm => cm.Relations);
    }

    /// <summary>
    /// Gets all complexity orderings across all ConceptMaps in the collection.
    /// </summary>
    public IEnumerable<ConceptComplexity> GetAllComplexityOrder()
    {
        return ConceptMaps.SelectMany(cm => cm.ComplexityOrder);
    }

    /// <summary>
    /// Gets the total number of concepts across all ConceptMaps.
    /// </summary>
    public int TotalConcepts => ConceptMaps.Sum(cm => cm.TotalConcepts);

    /// <summary>
    /// Gets the total number of relationships across all ConceptMaps.
    /// </summary>
    public int TotalRelations => ConceptMaps.Sum(cm => cm.TotalRelations);

    /// <summary>
    /// Gets a concept by ID from any ConceptMap in the collection.
    /// </summary>
    public Concept? GetConcept(string conceptId)
    {
        foreach (var cm in ConceptMaps)
        {
            var concept = cm.GetConcept(conceptId);
            if (concept != null)
                return concept;
        }
        return null;
    }

    /// <summary>
    /// Gets a concept by title from any ConceptMap in the collection.
    /// </summary>
    public Concept? GetConceptByTitle(string title)
    {
        foreach (var cm in ConceptMaps)
        {
            var concept = cm.GetConceptByTitle(title);
            if (concept != null)
                return concept;
        }
        return null;
    }

    /// <summary>
    /// Gets all concepts ordered by complexity across all ConceptMaps.
    /// </summary>
    public IEnumerable<Concept> GetConceptsByComplexity()
    {
        return ConceptMaps.SelectMany(cm => cm.GetConceptsByComplexity());
    }

    /// <summary>
    /// Prepares data for D3.js force layout visualization.
    /// </summary>
    public (object[] nodes, object[] links) GetForceLayoutData()
    {
        var nodes = GetAllConcepts().Select(c => new
        {
            id = c.Id,
            title = c.Title,
            group = c.Tags.FirstOrDefault() ?? "default"
        }).ToArray();

        var links = GetAllRelations().Select(r => new
        {
            source = r.SourceConceptId,
            target = r.TargetConceptId,
            type = r.RelationType.ToString()
        }).ToArray();

        return (nodes, links);
    }
}

/// <summary>
/// Represents a course-specific merged view of ConceptMaps.
/// 
/// This is stored SEPARATELY from the original resource ConceptMaps.
/// When you add resources to a course, their ConceptMaps are COPIED into this merged map.
/// Deduplication and merging only affects this course-specific copy, NOT the originals.
/// 
/// This allows:
/// - Each course to have its own deduplicated concept graph
/// - Original resource ConceptMaps to remain unchanged
/// - Resources to be shared across courses with different merge decisions
/// </summary>
public class MergedCourseConceptMap
{
    /// <summary>
    /// Unique identifier (usually same as Course.Id for easy lookup).
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// The Course ID this merged map belongs to.
    /// </summary>
    public string CourseId { get; set; } = "";

    /// <summary>
    /// The merged/deduplicated concepts from all resources in the course.
    /// </summary>
    public List<Concept> Concepts { get; set; } = [];

    /// <summary>
    /// The merged/deduplicated relationships.
    /// </summary>
    public List<ConceptRelationship> Relations { get; set; } = [];

    /// <summary>
    /// IDs of the source ConceptMaps that were merged into this.
    /// Used to detect when we need to rebuild (if a source changes).
    /// </summary>
    public List<string> SourceConceptMapIds { get; set; } = [];

    /// <summary>
    /// Tracking which concept pairs have been merged (for audit/undo).
    /// Maps removed concept ID to the canonical concept ID it was merged into.
    /// </summary>
    public Dictionary<string, string> MergedConceptMapping { get; set; } = [];

    /// <summary>
    /// When this merged map was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this merged map was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Status of the merged map.
    /// </summary>
    public MergedMapStatus Status { get; set; } = MergedMapStatus.NotBuilt;

    /// <summary>
    /// Total concept count in the merged map.
    /// </summary>
    public int TotalConcepts => Concepts.Count;

    /// <summary>
    /// Total relationship count in the merged map.
    /// </summary>
    public int TotalRelations => Relations.Count;

    /// <summary>
    /// Gets a concept by ID.
    /// </summary>
    public Concept? GetConcept(string conceptId)
    {
        return Concepts.FirstOrDefault(c => c.Id == conceptId);
    }

    /// <summary>
    /// Gets a concept by title (case-insensitive).
    /// </summary>
    public Concept? GetConceptByTitle(string title)
    {
        return Concepts.FirstOrDefault(c =>
            c.Title.Equals(title, StringComparison.OrdinalIgnoreCase) ||
            c.Aliases.Any(a => a.Equals(title, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Creates a deep copy of concepts and relations from source ConceptMaps.
    /// </summary>
    public void InitializeFromConceptMaps(IEnumerable<ConceptMap> sourceMaps)
    {
        Concepts.Clear();
        Relations.Clear();
        SourceConceptMapIds.Clear();
        MergedConceptMapping.Clear();

        foreach (var sourceMap in sourceMaps)
        {
            SourceConceptMapIds.Add(sourceMap.Id);

            // Deep copy concepts
            foreach (var concept in sourceMap.Concepts)
            {
                Concepts.Add(new Concept
                {
                    Id = concept.Id,
                    Title = concept.Title,
                    Summary = concept.Summary,
                    Content = concept.Content,
                    Aliases = [..concept.Aliases],
                    Tags = [..concept.Tags],
                    PrerequisiteIds = [..concept.PrerequisiteIds],
                    RelatedIds = [..concept.RelatedIds],
                    ConceptMapId = concept.ConceptMapId,
                    SourceResourceIds = [..concept.SourceResourceIds],
                    ConfidenceScore = concept.ConfidenceScore,
                    CreatedAt = concept.CreatedAt,
                    UpdatedAt = concept.UpdatedAt
                });
            }

            // Deep copy relations
            foreach (var rel in sourceMap.Relations)
            {
                Relations.Add(new ConceptRelationship
                {
                    Id = rel.Id,
                    SourceConceptId = rel.SourceConceptId,
                    TargetConceptId = rel.TargetConceptId,
                    RelationType = rel.RelationType,
                    ConfidenceScore = rel.ConfidenceScore,
                    SemanticSimilarity = rel.SemanticSimilarity,
                    SemanticDistance = rel.SemanticDistance,
                    LexicalDistance = rel.LexicalDistance,
                    CoOccurrenceCount = rel.CoOccurrenceCount,
                    EvidenceResourceIds = [..rel.EvidenceResourceIds],
                    EvidenceChunkIds = [..rel.EvidenceChunkIds]
                });
            }
        }

        Status = MergedMapStatus.Ready;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Prepares data for D3.js force layout visualization.
    /// </summary>
    public (object[] nodes, object[] links) GetForceLayoutData()
    {
        var nodes = Concepts.Select(c => new
        {
            id = c.Id,
            title = c.Title,
            group = c.Tags.FirstOrDefault() ?? "default"
        }).ToArray();

        var links = Relations.Select(r => new
        {
            source = r.SourceConceptId,
            target = r.TargetConceptId,
            type = r.RelationType.ToString()
        }).ToArray();

        return (nodes, links);
    }
}

/// <summary>
/// Status of a merged course concept map.
/// </summary>
public enum MergedMapStatus
{
    /// <summary>
    /// Not yet built.
    /// </summary>
    NotBuilt,

    /// <summary>
    /// Currently being built.
    /// </summary>
    Building,

    /// <summary>
    /// Ready for use.
    /// </summary>
    Ready,

    /// <summary>
    /// Needs rebuild (source maps changed).
    /// </summary>
    NeedsRebuild,

    /// <summary>
    /// Build failed.
    /// </summary>
    Failed
}
