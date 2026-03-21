namespace Tutor.Core.Models;

/// <summary>
/// Represents a complete knowledge graph for a course or subject domain.
/// Contains concept nodes and their relationships, supporting:
/// - Hierarchical traversal (prerequisites ? dependents)
/// - Topological sorting for optimal learning order
/// - Bidirectional navigation from any concept
/// </summary>
public class KnowledgeGraph
{
    /// <summary>
    /// Unique identifier for this knowledge graph.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name of the knowledge domain or course this graph represents.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Description of the subject matter covered.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// ID of the course this graph belongs to (if any).
    /// </summary>
    public string? CourseId { get; set; }

    /// <summary>
    /// All concept nodes in the graph, keyed by their ID for fast lookup.
    /// </summary>
    public Dictionary<string, ConceptNode> Nodes { get; set; } = [];

    /// <summary>
    /// All relationships in the graph, keyed by their ID.
    /// </summary>
    public Dictionary<string, ConceptRelationship> Relationships { get; set; } = [];

    /// <summary>
    /// Adjacency list for forward traversal (concept ID ? dependent concept IDs).
    /// Built from relationships for efficient graph navigation.
    /// </summary>
    public Dictionary<string, List<string>> ForwardEdges { get; set; } = [];

    /// <summary>
    /// Adjacency list for backward traversal (concept ID ? prerequisite concept IDs).
    /// </summary>
    public Dictionary<string, List<string>> BackwardEdges { get; set; } = [];

    /// <summary>
    /// When this graph was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this graph was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Version number for tracking changes.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets all foundational concepts (no prerequisites).
    /// These are the starting points for learning paths.
    /// </summary>
    public IEnumerable<ConceptNode> GetFoundationalConcepts()
    {
        return Nodes.Values.Where(n => n.IsFoundational).OrderBy(n => n.Term);
    }

    /// <summary>
    /// Gets all leaf concepts (no dependents).
    /// These are the endpoints of learning paths.
    /// </summary>
    public IEnumerable<ConceptNode> GetLeafConcepts()
    {
        return Nodes.Values.Where(n => n.IsLeaf).OrderByDescending(n => n.HierarchyDepth);
    }

    /// <summary>
    /// Gets concepts at a specific depth level.
    /// </summary>
    public IEnumerable<ConceptNode> GetConceptsAtDepth(int depth)
    {
        return Nodes.Values.Where(n => n.HierarchyDepth == depth).OrderBy(n => n.Term);
    }

    /// <summary>
    /// Gets the maximum depth of the hierarchy.
    /// </summary>
    public int MaxDepth => Nodes.Count > 0 ? Nodes.Values.Max(n => n.HierarchyDepth) : 0;

    /// <summary>
    /// Gets all direct prerequisites for a concept.
    /// </summary>
    public IEnumerable<ConceptNode> GetPrerequisites(string conceptId)
    {
        if (!Nodes.ContainsKey(conceptId))
            return [];

        var prereqIds = BackwardEdges.TryGetValue(conceptId, out var edges) ? edges : [];
        return prereqIds.Where(id => Nodes.ContainsKey(id)).Select(id => Nodes[id]);
    }

    /// <summary>
    /// Gets all direct dependents of a concept.
    /// </summary>
    public IEnumerable<ConceptNode> GetDependents(string conceptId)
    {
        if (!Nodes.ContainsKey(conceptId))
            return [];

        var depIds = ForwardEdges.TryGetValue(conceptId, out var edges) ? edges : [];
        return depIds.Where(id => Nodes.ContainsKey(id)).Select(id => Nodes[id]);
    }

    /// <summary>
    /// Gets ALL prerequisites transitively (the complete prerequisite chain).
    /// Returns concepts ordered from most foundational to immediate prerequisites.
    /// </summary>
    public List<ConceptNode> GetAllPrerequisites(string conceptId)
    {
        var result = new List<ConceptNode>();
        var visited = new HashSet<string>();
        CollectPrerequisitesRecursive(conceptId, visited, result);
        
        // Order by hierarchy depth (foundational first)
        return result.OrderBy(n => n.HierarchyDepth).ThenBy(n => n.Term).ToList();
    }

    private void CollectPrerequisitesRecursive(string conceptId, HashSet<string> visited, List<ConceptNode> result)
    {
        if (!BackwardEdges.TryGetValue(conceptId, out var prereqIds))
            return;

        foreach (var prereqId in prereqIds)
        {
            if (visited.Contains(prereqId) || !Nodes.ContainsKey(prereqId))
                continue;

            visited.Add(prereqId);
            CollectPrerequisitesRecursive(prereqId, visited, result);
            result.Add(Nodes[prereqId]);
        }
    }

    /// <summary>
    /// Gets ALL dependents transitively (the complete dependent chain).
    /// Returns concepts ordered from immediate dependents to most advanced.
    /// </summary>
    public List<ConceptNode> GetAllDependents(string conceptId)
    {
        var result = new List<ConceptNode>();
        var visited = new HashSet<string>();
        CollectDependentsRecursive(conceptId, visited, result);
        
        // Order by hierarchy depth (closer dependents first)
        return result.OrderBy(n => n.HierarchyDepth).ThenBy(n => n.Term).ToList();
    }

    private void CollectDependentsRecursive(string conceptId, HashSet<string> visited, List<ConceptNode> result)
    {
        if (!ForwardEdges.TryGetValue(conceptId, out var depIds))
            return;

        foreach (var depId in depIds)
        {
            if (visited.Contains(depId) || !Nodes.ContainsKey(depId))
                continue;

            visited.Add(depId);
            result.Add(Nodes[depId]);
            CollectDependentsRecursive(depId, visited, result);
        }
    }

    /// <summary>
    /// Gets a complete learning path from foundational concepts to a target concept.
    /// Returns an ordered list representing the optimal learning sequence.
    /// </summary>
    public List<ConceptNode> GetLearningPathTo(string targetConceptId)
    {
        if (!Nodes.ContainsKey(targetConceptId))
            return [];

        var prerequisites = GetAllPrerequisites(targetConceptId);
        prerequisites.Add(Nodes[targetConceptId]);
        
        return TopologicalSort(prerequisites);
    }

    /// <summary>
    /// Gets a complete learning path from a concept to all its dependents.
    /// Returns an ordered list representing the forward learning sequence.
    /// </summary>
    public List<ConceptNode> GetLearningPathFrom(string startConceptId)
    {
        if (!Nodes.ContainsKey(startConceptId))
            return [];

        var path = new List<ConceptNode> { Nodes[startConceptId] };
        path.AddRange(GetAllDependents(startConceptId));
        
        return TopologicalSort(path);
    }

    /// <summary>
    /// Performs a topological sort on a subset of nodes.
    /// Ensures prerequisites come before their dependents.
    /// </summary>
    public List<ConceptNode> TopologicalSort(IEnumerable<ConceptNode> nodesToSort)
    {
        var nodeSet = nodesToSort.ToDictionary(n => n.Id);
        var result = new List<ConceptNode>();
        var visited = new HashSet<string>();
        var temp = new HashSet<string>();

        void Visit(string nodeId)
        {
            if (!nodeSet.ContainsKey(nodeId) || visited.Contains(nodeId))
                return;
            
            if (temp.Contains(nodeId))
                return; // Cycle detected, skip

            temp.Add(nodeId);

            // Visit prerequisites first
            if (BackwardEdges.TryGetValue(nodeId, out var prereqs))
            {
                foreach (var prereqId in prereqs)
                {
                    Visit(prereqId);
                }
            }

            temp.Remove(nodeId);
            visited.Add(nodeId);
            result.Add(nodeSet[nodeId]);
        }

        // Start from nodes with no prerequisites within the subset
        foreach (var node in nodeSet.Values.OrderBy(n => n.HierarchyDepth).ThenBy(n => n.Term))
        {
            Visit(node.Id);
        }

        return result;
    }

    /// <summary>
    /// Gets the complete topologically sorted list of all concepts.
    /// This represents the optimal learning order for the entire subject.
    /// </summary>
    public List<ConceptNode> GetFullLearningOrder()
    {
        return TopologicalSort(Nodes.Values);
    }

    /// <summary>
    /// Finds a concept by term (case-insensitive).
    /// </summary>
    public ConceptNode? FindByTerm(string term)
    {
        return Nodes.Values.FirstOrDefault(n => 
            n.Term.Equals(term, StringComparison.OrdinalIgnoreCase) ||
            n.Aliases.Any(a => a.Equals(term, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Rebuilds the adjacency lists from the relationships dictionary.
    /// Call this after deserialization or after bulk relationship changes.
    /// </summary>
    public void RebuildAdjacencyLists()
    {
        ForwardEdges.Clear();
        BackwardEdges.Clear();

        foreach (var rel in Relationships.Values.Where(r => r.RelationType == ConceptRelationType.Prerequisite))
        {
            // Forward: source (prerequisite) ? target (dependent)
            if (!ForwardEdges.ContainsKey(rel.SourceConceptId))
                ForwardEdges[rel.SourceConceptId] = [];
            
            if (!ForwardEdges[rel.SourceConceptId].Contains(rel.TargetConceptId))
                ForwardEdges[rel.SourceConceptId].Add(rel.TargetConceptId);

            // Backward: target (dependent) ? source (prerequisite)
            if (!BackwardEdges.ContainsKey(rel.TargetConceptId))
                BackwardEdges[rel.TargetConceptId] = [];
            
            if (!BackwardEdges[rel.TargetConceptId].Contains(rel.SourceConceptId))
                BackwardEdges[rel.TargetConceptId].Add(rel.SourceConceptId);
        }

        // Also update the node-level prerequisite/dependent lists
        foreach (var node in Nodes.Values)
        {
            node.PrerequisiteIds = BackwardEdges.TryGetValue(node.Id, out var prereqs) ? prereqs.ToList() : [];
            node.DependentIds = ForwardEdges.TryGetValue(node.Id, out var deps) ? deps.ToList() : [];
        }
    }

    /// <summary>
    /// Recalculates hierarchy depths for all nodes based on the graph structure.
    /// Foundational concepts (no prerequisites) get depth 0.
    /// </summary>
    public void RecalculateDepths()
    {
        // Reset all depths
        foreach (var node in Nodes.Values)
        {
            node.HierarchyDepth = 0;
        }

        // Calculate depth for each node
        var visited = new Dictionary<string, int>();
        
        int CalculateDepth(string nodeId)
        {
            if (visited.TryGetValue(nodeId, out var cached))
                return cached;

            if (!BackwardEdges.TryGetValue(nodeId, out var prereqs) || prereqs.Count == 0)
            {
                visited[nodeId] = 0;
                return 0;
            }

            // Depth is 1 + max depth of prerequisites
            var maxPrereqDepth = prereqs
                .Where(id => Nodes.ContainsKey(id))
                .Select(CalculateDepth)
                .DefaultIfEmpty(-1)
                .Max();

            var depth = maxPrereqDepth + 1;
            visited[nodeId] = depth;
            return depth;
        }

        foreach (var nodeId in Nodes.Keys)
        {
            Nodes[nodeId].HierarchyDepth = CalculateDepth(nodeId);
        }
    }

    /// <summary>
    /// Gets statistics about the knowledge graph.
    /// </summary>
    public KnowledgeGraphStats GetStats()
    {
        return new KnowledgeGraphStats
        {
            TotalConcepts = Nodes.Count,
            TotalRelationships = Relationships.Count,
            FoundationalConcepts = Nodes.Values.Count(n => n.IsFoundational),
            LeafConcepts = Nodes.Values.Count(n => n.IsLeaf),
            MaxDepth = MaxDepth,
            AverageDepth = Nodes.Count > 0 ? Nodes.Values.Average(n => n.HierarchyDepth) : 0,
            VerifiedConcepts = Nodes.Values.Count(n => n.IsVerified),
            VerifiedRelationships = Relationships.Values.Count(r => r.IsVerified)
        };
    }
}

/// <summary>
/// Statistics about a knowledge graph.
/// </summary>
public class KnowledgeGraphStats
{
    public int TotalConcepts { get; set; }
    public int TotalRelationships { get; set; }
    public int FoundationalConcepts { get; set; }
    public int LeafConcepts { get; set; }
    public int MaxDepth { get; set; }
    public double AverageDepth { get; set; }
    public int VerifiedConcepts { get; set; }
    public int VerifiedRelationships { get; set; }
}
