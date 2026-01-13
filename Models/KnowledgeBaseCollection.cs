namespace Tutor.Models;

/// <summary>
/// Represents a collection of KnowledgeBases that are combined for a Course.
/// 
/// Key architectural points:
/// - Each Resource generates its own KnowledgeBase independently
/// - A Course combines multiple Resources' KnowledgeBases into a KnowledgeBaseCollection
/// - This allows dynamic composition of courses from any subset of resources
/// - Reduces overhead since resources aren't merged into a single KnowledgeBase
/// 
/// Example:
/// - Course "Intro to Science" has Resources: Physics Textbook, Chemistry Guide
/// - Physics Textbook ? KnowledgeBase A (physics concepts)
/// - Chemistry Guide ? KnowledgeBase B (chemistry concepts)
/// - Course.KnowledgeBaseCollection contains [A, B]
/// - Queries aggregate results from both KnowledgeBases
/// </summary>
public class KnowledgeBaseCollection
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
    /// IDs of KnowledgeBases in this collection.
    /// Each KnowledgeBase corresponds to a single Resource.
    /// </summary>
    public List<string> KnowledgeBaseIds { get; set; } = [];

    /// <summary>
    /// When this collection was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this collection was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the total number of KnowledgeBases in this collection.
    /// </summary>
    public int Count => KnowledgeBaseIds.Count;

    /// <summary>
    /// Whether the collection has any KnowledgeBases.
    /// </summary>
    public bool HasKnowledgeBases => KnowledgeBaseIds.Count > 0;

    /// <summary>
    /// Adds a KnowledgeBase to the collection if not already present.
    /// </summary>
    public bool AddKnowledgeBase(string knowledgeBaseId)
    {
        if (string.IsNullOrEmpty(knowledgeBaseId) || KnowledgeBaseIds.Contains(knowledgeBaseId))
            return false;

        KnowledgeBaseIds.Add(knowledgeBaseId);
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Removes a KnowledgeBase from the collection.
    /// </summary>
    public bool RemoveKnowledgeBase(string knowledgeBaseId)
    {
        var removed = KnowledgeBaseIds.Remove(knowledgeBaseId);
        if (removed)
            UpdatedAt = DateTime.UtcNow;
        return removed;
    }

    /// <summary>
    /// Checks if a KnowledgeBase is in this collection.
    /// </summary>
    public bool ContainsKnowledgeBase(string knowledgeBaseId)
    {
        return KnowledgeBaseIds.Contains(knowledgeBaseId);
    }
}

/// <summary>
/// Represents an aggregated view of multiple KnowledgeBases loaded in memory.
/// Provides unified query methods across all KnowledgeBases in a collection.
/// </summary>
public class LoadedKnowledgeBaseCollection
{
    /// <summary>
    /// The collection metadata.
    /// </summary>
    public KnowledgeBaseCollection Collection { get; }

    /// <summary>
    /// The loaded KnowledgeBases.
    /// </summary>
    public List<KnowledgeBase> KnowledgeBases { get; }

    public LoadedKnowledgeBaseCollection(KnowledgeBaseCollection collection, List<KnowledgeBase> knowledgeBases)
    {
        Collection = collection;
        KnowledgeBases = knowledgeBases;
    }

    /// <summary>
    /// Gets all concepts across all KnowledgeBases in the collection.
    /// </summary>
    public IEnumerable<Concept> GetAllConcepts()
    {
        return KnowledgeBases.SelectMany(kb => kb.Concepts);
    }

    /// <summary>
    /// Gets all relationships across all KnowledgeBases in the collection.
    /// </summary>
    public IEnumerable<ConceptRelationship> GetAllRelations()
    {
        return KnowledgeBases.SelectMany(kb => kb.Relations);
    }

    /// <summary>
    /// Gets all complexity orderings across all KnowledgeBases in the collection.
    /// </summary>
    public IEnumerable<ConceptComplexity> GetAllComplexityOrder()
    {
        return KnowledgeBases.SelectMany(kb => kb.ComplexityOrder);
    }

    /// <summary>
    /// Gets the total number of concepts across all KnowledgeBases.
    /// </summary>
    public int TotalConcepts => KnowledgeBases.Sum(kb => kb.TotalConcepts);

    /// <summary>
    /// Gets the total number of relationships across all KnowledgeBases.
    /// </summary>
    public int TotalRelations => KnowledgeBases.Sum(kb => kb.TotalRelations);

    /// <summary>
    /// Finds a concept by ID across all KnowledgeBases.
    /// </summary>
    public Concept? GetConcept(string conceptId)
    {
        foreach (var kb in KnowledgeBases)
        {
            var concept = kb.GetConcept(conceptId);
            if (concept != null)
                return concept;
        }
        return null;
    }

    /// <summary>
    /// Finds a concept by title across all KnowledgeBases.
    /// </summary>
    public Concept? GetConceptByTitle(string title)
    {
        foreach (var kb in KnowledgeBases)
        {
            var concept = kb.GetConceptByTitle(title);
            if (concept != null)
                return concept;
        }
        return null;
    }

    /// <summary>
    /// Gets all concepts ordered by complexity (foundational first) across all KnowledgeBases.
    /// Concepts are interleaved by level to provide a unified learning path.
    /// </summary>
    public IEnumerable<Concept> GetConceptsByComplexity()
    {
        // Collect all complexity entries with their concepts
        var allComplexity = new List<(int Level, int PrereqCount, Concept Concept)>();

        foreach (var kb in KnowledgeBases)
        {
            foreach (var complexity in kb.ComplexityOrder.OrderBy(c => c.Level).ThenBy(c => c.PrerequisiteCount))
            {
                var concept = kb.GetConcept(complexity.ConceptId);
                if (concept != null)
                {
                    allComplexity.Add((complexity.Level, complexity.PrerequisiteCount, concept));
                }
            }
        }

        // Order by level, then by prerequisite count
        foreach (var item in allComplexity.OrderBy(x => x.Level).ThenBy(x => x.PrereqCount))
        {
            yield return item.Concept;
        }

        // Include any concepts not in complexity order
        var orderedIds = allComplexity.Select(x => x.Concept.Id).ToHashSet();
        foreach (var kb in KnowledgeBases)
        {
            foreach (var concept in kb.Concepts.Where(c => !orderedIds.Contains(c.Id)))
            {
                yield return concept;
            }
        }
    }

    /// <summary>
    /// Gets the maximum complexity level across all KnowledgeBases.
    /// </summary>
    public int MaxComplexityLevel => KnowledgeBases.Count > 0
        ? KnowledgeBases.Max(kb => kb.MaxComplexityLevel)
        : 0;

    /// <summary>
    /// Checks if all KnowledgeBases in the collection are ready.
    /// </summary>
    public bool AllReady => KnowledgeBases.All(kb => kb.Status == KnowledgeBaseStatus.Ready);

    /// <summary>
    /// Gets KnowledgeBases that are not yet ready.
    /// </summary>
    public IEnumerable<KnowledgeBase> GetPendingKnowledgeBases()
    {
        return KnowledgeBases.Where(kb => kb.Status != KnowledgeBaseStatus.Ready);
    }

    /// <summary>
    /// Gets prerequisites for a concept, searching across all KnowledgeBases.
    /// </summary>
    public IEnumerable<Concept> GetPrerequisites(string conceptId)
    {
        foreach (var kb in KnowledgeBases)
        {
            foreach (var prereq in kb.GetPrerequisites(conceptId))
            {
                yield return prereq;
            }
        }
    }

    /// <summary>
    /// Gets dependents of a concept, searching across all KnowledgeBases.
    /// </summary>
    public IEnumerable<Concept> GetDependents(string conceptId)
    {
        foreach (var kb in KnowledgeBases)
        {
            foreach (var dependent in kb.GetDependents(conceptId))
            {
                yield return dependent;
            }
        }
    }

    /// <summary>
    /// Gets related concepts, searching across all KnowledgeBases.
    /// </summary>
    public IEnumerable<Concept> GetRelatedConcepts(string conceptId)
    {
        foreach (var kb in KnowledgeBases)
        {
            foreach (var related in kb.GetRelatedConcepts(conceptId))
            {
                yield return related;
            }
        }
    }
}
