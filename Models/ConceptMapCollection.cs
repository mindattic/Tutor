namespace Tutor.Models;

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

    /// <summary>
    /// Converts from legacy KnowledgeBaseCollection.
    /// </summary>
    public static ConceptMapCollection FromKnowledgeBaseCollection(KnowledgeBaseCollection kbc)
    {
        return new ConceptMapCollection
        {
            Id = kbc.Id,
            Name = kbc.Name,
            Description = kbc.Description,
            ConceptMapIds = [.. kbc.KnowledgeBaseIds],
            CreatedAt = kbc.CreatedAt,
            UpdatedAt = kbc.UpdatedAt
        };
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
