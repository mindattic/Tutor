using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Service for generating structured learning paths and tables of contents
/// from knowledge graphs. Produces pedagogically ordered sequences that
/// respect prerequisite relationships and can start from any concept.
/// </summary>
public sealed class TableOfContentsService
{
    private readonly KnowledgeGraphService graphService;

    public TableOfContentsService(KnowledgeGraphService graphService)
    {
        this.graphService = graphService;
    }

    /// <summary>
    /// Generates a complete table of contents for a knowledge graph.
    /// Groups concepts by depth level and orders them for optimal learning.
    /// </summary>
    public async Task<TableOfContents> GenerateTableOfContentsAsync(
        string graphId,
        CancellationToken ct = default)
    {
        var graph = await graphService.LoadGraphAsync(graphId, ct);
        if (graph == null)
            return new TableOfContents { Title = "Unknown", Sections = [] };

        var toc = new TableOfContents
        {
            Title = graph.Name,
            Description = graph.Description,
            GraphId = graph.Id
        };

        // Get topologically sorted concepts
        var orderedConcepts = graph.GetFullLearningOrder();

        // Group by depth level
        var byDepth = orderedConcepts.GroupBy(c => c.HierarchyDepth).OrderBy(g => g.Key);

        foreach (var depthGroup in byDepth)
        {
            var section = new TocSection
            {
                Level = depthGroup.Key,
                Title = GetLevelTitle(depthGroup.Key, graph.MaxDepth),
                Items = depthGroup.Select(concept => new TocItem
                {
                    ConceptId = concept.Id,
                    Term = concept.Term,
                    Description = concept.Description,
                    PrerequisiteCount = concept.PrerequisiteIds.Count,
                    DependentCount = concept.DependentIds.Count,
                    IsFoundational = concept.IsFoundational,
                    IsLeaf = concept.IsLeaf
                }).ToList()
            };

            toc.Sections.Add(section);
        }

        toc.TotalConcepts = orderedConcepts.Count;
        toc.MaxDepth = graph.MaxDepth;

        return toc;
    }

    /// <summary>
    /// Generates a focused learning path to understand a specific target concept.
    /// Includes all prerequisites in the correct order.
    /// </summary>
    public async Task<LearningPath> GenerateLearningPathToAsync(
        string graphId,
        string targetTerm,
        CancellationToken ct = default)
    {
        var graph = await graphService.LoadGraphAsync(graphId, ct);
        if (graph == null)
            return new LearningPath { Title = "Unknown", Steps = [] };

        var target = graph.FindByTerm(targetTerm);
        if (target == null)
            return new LearningPath { Title = $"Path to '{targetTerm}' (not found)", Steps = [] };

        var orderedConcepts = graph.GetLearningPathTo(target.Id);

        var path = new LearningPath
        {
            Title = $"Learning Path to: {target.Term}",
            Description = $"Complete prerequisite chain for understanding {target.Term}",
            TargetConceptId = target.Id,
            TargetTerm = target.Term,
            Direction = LearningDirection.Prerequisites
        };

        int stepNumber = 1;
        foreach (var concept in orderedConcepts)
        {
            var step = new LearningStep
            {
                StepNumber = stepNumber++,
                ConceptId = concept.Id,
                Term = concept.Term,
                Description = concept.Description,
                Depth = concept.HierarchyDepth,
                IsTarget = concept.Id == target.Id,
                Prerequisites = graph.GetPrerequisites(concept.Id).Select(p => p.Term).ToList(),
                UnlocksNext = graph.GetDependents(concept.Id).Select(d => d.Term).ToList()
            };

            path.Steps.Add(step);
        }

        return path;
    }

    /// <summary>
    /// Generates a forward learning path showing what concepts build on a starting concept.
    /// </summary>
    public async Task<LearningPath> GenerateLearningPathFromAsync(
        string graphId,
        string startTerm,
        CancellationToken ct = default)
    {
        var graph = await graphService.LoadGraphAsync(graphId, ct);
        if (graph == null)
            return new LearningPath { Title = "Unknown", Steps = [] };

        var start = graph.FindByTerm(startTerm);
        if (start == null)
            return new LearningPath { Title = $"Path from '{startTerm}' (not found)", Steps = [] };

        var orderedConcepts = graph.GetLearningPathFrom(start.Id);

        var path = new LearningPath
        {
            Title = $"Learning Path from: {start.Term}",
            Description = $"Concepts that build upon {start.Term}",
            TargetConceptId = start.Id,
            TargetTerm = start.Term,
            Direction = LearningDirection.Dependents
        };

        int stepNumber = 1;
        foreach (var concept in orderedConcepts)
        {
            var step = new LearningStep
            {
                StepNumber = stepNumber++,
                ConceptId = concept.Id,
                Term = concept.Term,
                Description = concept.Description,
                Depth = concept.HierarchyDepth,
                IsTarget = concept.Id == start.Id,
                Prerequisites = graph.GetPrerequisites(concept.Id).Select(p => p.Term).ToList(),
                UnlocksNext = graph.GetDependents(concept.Id).Select(d => d.Term).ToList()
            };

            path.Steps.Add(step);
        }

        return path;
    }

    /// <summary>
    /// Generates a bidirectional path showing both prerequisites and dependents for a concept.
    /// </summary>
    public async Task<LearningPath> GenerateBidirectionalPathAsync(
        string graphId,
        string conceptTerm,
        CancellationToken ct = default)
    {
        var graph = await graphService.LoadGraphAsync(graphId, ct);
        if (graph == null)
            return new LearningPath { Title = "Unknown", Steps = [] };

        var concept = graph.FindByTerm(conceptTerm);
        if (concept == null)
            return new LearningPath { Title = $"Context for '{conceptTerm}' (not found)", Steps = [] };

        // Get prerequisites and dependents
        var prerequisites = graph.GetAllPrerequisites(concept.Id);
        var dependents = graph.GetAllDependents(concept.Id);

        var path = new LearningPath
        {
            Title = $"Complete Context: {concept.Term}",
            Description = $"Prerequisites, the concept, and what it enables",
            TargetConceptId = concept.Id,
            TargetTerm = concept.Term,
            Direction = LearningDirection.Bidirectional
        };

        int stepNumber = 1;

        // Add prerequisites
        foreach (var prereq in prerequisites)
        {
            path.Steps.Add(new LearningStep
            {
                StepNumber = stepNumber++,
                ConceptId = prereq.Id,
                Term = prereq.Term,
                Description = prereq.Description,
                Depth = prereq.HierarchyDepth,
                IsTarget = false,
                IsPrerequisiteOfTarget = true,
                Prerequisites = graph.GetPrerequisites(prereq.Id).Select(p => p.Term).ToList(),
                UnlocksNext = graph.GetDependents(prereq.Id).Select(d => d.Term).ToList()
            });
        }

        // Add the target concept
        path.Steps.Add(new LearningStep
        {
            StepNumber = stepNumber++,
            ConceptId = concept.Id,
            Term = concept.Term,
            Description = concept.Description,
            Depth = concept.HierarchyDepth,
            IsTarget = true,
            Prerequisites = graph.GetPrerequisites(concept.Id).Select(p => p.Term).ToList(),
            UnlocksNext = graph.GetDependents(concept.Id).Select(d => d.Term).ToList()
        });

        // Add dependents
        foreach (var dep in dependents)
        {
            path.Steps.Add(new LearningStep
            {
                StepNumber = stepNumber++,
                ConceptId = dep.Id,
                Term = dep.Term,
                Description = dep.Description,
                Depth = dep.HierarchyDepth,
                IsTarget = false,
                IsDependentOfTarget = true,
                Prerequisites = graph.GetPrerequisites(dep.Id).Select(p => p.Term).ToList(),
                UnlocksNext = graph.GetDependents(dep.Id).Select(d => d.Term).ToList()
            });
        }

        return path;
    }

    /// <summary>
    /// Gets a filtered view of the TOC by tag/category.
    /// </summary>
    public async Task<TableOfContents> GenerateFilteredTocAsync(
        string graphId,
        string tag,
        CancellationToken ct = default)
    {
        var graph = await graphService.LoadGraphAsync(graphId, ct);
        if (graph == null)
            return new TableOfContents { Title = "Unknown", Sections = [] };

        // Filter concepts by tag
        var taggedConcepts = graph.Nodes.Values
            .Where(c => c.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // Build a subgraph with these concepts
        var conceptIds = new HashSet<string>(taggedConcepts.Select(c => c.Id));
        var subgraphConcepts = graph.TopologicalSort(taggedConcepts);

        var toc = new TableOfContents
        {
            Title = $"{graph.Name} - {tag}",
            Description = $"Concepts tagged with '{tag}'",
            GraphId = graph.Id,
            FilterTag = tag
        };

        var byDepth = subgraphConcepts.GroupBy(c => c.HierarchyDepth).OrderBy(g => g.Key);

        foreach (var depthGroup in byDepth)
        {
            var section = new TocSection
            {
                Level = depthGroup.Key,
                Title = GetLevelTitle(depthGroup.Key, graph.MaxDepth),
                Items = depthGroup.Select(concept => new TocItem
                {
                    ConceptId = concept.Id,
                    Term = concept.Term,
                    Description = concept.Description,
                    PrerequisiteCount = concept.PrerequisiteIds.Count(id => conceptIds.Contains(id)),
                    DependentCount = concept.DependentIds.Count(id => conceptIds.Contains(id)),
                    IsFoundational = concept.IsFoundational,
                    IsLeaf = concept.IsLeaf
                }).ToList()
            };

            toc.Sections.Add(section);
        }

        toc.TotalConcepts = subgraphConcepts.Count;
        toc.MaxDepth = subgraphConcepts.Count > 0 ? subgraphConcepts.Max(c => c.HierarchyDepth) : 0;

        return toc;
    }

    /// <summary>
    /// Generates suggested next concepts to learn based on current understanding.
    /// </summary>
    public async Task<List<ConceptSuggestion>> GetNextConceptsToLearnAsync(
        string graphId,
        IEnumerable<string> knownConceptTerms,
        int maxSuggestions = 5,
        CancellationToken ct = default)
    {
        var graph = await graphService.LoadGraphAsync(graphId, ct);
        if (graph == null)
            return [];

        // Find concepts the user already knows
        var knownIds = new HashSet<string>(
            knownConceptTerms
                .Select(t => graph.FindByTerm(t)?.Id)
                .Where(id => id != null)!);

        var suggestions = new List<ConceptSuggestion>();

        // Find concepts where all prerequisites are known
        foreach (var concept in graph.Nodes.Values)
        {
            if (knownIds.Contains(concept.Id))
                continue; // Already known

            var prereqsMet = concept.PrerequisiteIds.All(id => knownIds.Contains(id));
            if (!prereqsMet)
                continue;

            // Calculate priority based on how many new concepts this unlocks
            var unlocksCount = CountNewUnlocks(concept.Id, knownIds, graph);

            suggestions.Add(new ConceptSuggestion
            {
                ConceptId = concept.Id,
                Term = concept.Term,
                Description = concept.Description,
                Depth = concept.HierarchyDepth,
                PrerequisitesMetCount = concept.PrerequisiteIds.Count,
                UnlocksCount = unlocksCount,
                Priority = unlocksCount + (concept.IsLeaf ? 0 : 1)
            });
        }

        return suggestions
            .OrderByDescending(s => s.Priority)
            .ThenBy(s => s.Depth)
            .Take(maxSuggestions)
            .ToList();
    }

    private static int CountNewUnlocks(string conceptId, HashSet<string> knownIds, KnowledgeGraph graph)
    {
        var wouldKnow = new HashSet<string>(knownIds) { conceptId };
        var unlocks = 0;

        foreach (var depId in graph.Nodes[conceptId].DependentIds)
        {
            if (wouldKnow.Contains(depId))
                continue;

            var dep = graph.Nodes[depId];
            if (dep.PrerequisiteIds.All(id => wouldKnow.Contains(id)))
            {
                unlocks++;
            }
        }

        return unlocks;
    }

    private static string GetLevelTitle(int depth, int maxDepth)
    {
        if (depth == 0)
            return "Foundations";
        if (depth == maxDepth)
            return "Advanced Topics";
        if (depth == 1)
            return "Core Concepts";
        if (depth <= maxDepth / 3)
            return "Building Blocks";
        if (depth <= 2 * maxDepth / 3)
            return "Intermediate Topics";
        return "Advanced Concepts";
    }
}

/// <summary>
/// A structured table of contents for a knowledge graph.
/// </summary>
public class TableOfContents
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string GraphId { get; set; } = "";
    public string? FilterTag { get; set; }
    public List<TocSection> Sections { get; set; } = [];
    public int TotalConcepts { get; set; }
    public int MaxDepth { get; set; }
}

/// <summary>
/// A section in the table of contents representing a depth level.
/// </summary>
public class TocSection
{
    public int Level { get; set; }
    public string Title { get; set; } = "";
    public List<TocItem> Items { get; set; } = [];
}

/// <summary>
/// An item in the table of contents.
/// </summary>
public class TocItem
{
    public string ConceptId { get; set; } = "";
    public string Term { get; set; } = "";
    public string Description { get; set; } = "";
    public int PrerequisiteCount { get; set; }
    public int DependentCount { get; set; }
    public bool IsFoundational { get; set; }
    public bool IsLeaf { get; set; }
}

/// <summary>
/// A learning path through the knowledge graph.
/// </summary>
public class LearningPath
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string TargetConceptId { get; set; } = "";
    public string TargetTerm { get; set; } = "";
    public LearningDirection Direction { get; set; }
    public List<LearningStep> Steps { get; set; } = [];
}

/// <summary>
/// Direction of traversal for a learning path.
/// </summary>
public enum LearningDirection
{
    Prerequisites,
    Dependents,
    Bidirectional
}

/// <summary>
/// A step in a learning path.
/// </summary>
public class LearningStep
{
    public int StepNumber { get; set; }
    public string ConceptId { get; set; } = "";
    public string Term { get; set; } = "";
    public string Description { get; set; } = "";
    public int Depth { get; set; }
    public bool IsTarget { get; set; }
    public bool IsPrerequisiteOfTarget { get; set; }
    public bool IsDependentOfTarget { get; set; }
    public List<string> Prerequisites { get; set; } = [];
    public List<string> UnlocksNext { get; set; } = [];
}

/// <summary>
/// A suggested next concept to learn.
/// </summary>
public class ConceptSuggestion
{
    public string ConceptId { get; set; } = "";
    public string Term { get; set; } = "";
    public string Description { get; set; } = "";
    public int Depth { get; set; }
    public int PrerequisitesMetCount { get; set; }
    public int UnlocksCount { get; set; }
    public int Priority { get; set; }
}
