using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Service for managing knowledge graphs - creation, persistence, and operations.
/// Orchestrates concept extraction, correlation, and graph building.
/// </summary>
public sealed class KnowledgeGraphService
{
    private const string GraphsDirectoryName = "KnowledgeGraphs";
    private readonly IAppDataPathProvider pathProvider;
    private readonly string graphsDirectory;

    private readonly ConceptExtractionService extractionService;
    private readonly ConceptCorrelationService correlationService;
    private readonly EmbeddingService embeddingService;
    private readonly LSHService lshService;
    private readonly SimHashService simHashService;

    private readonly Dictionary<string, KnowledgeGraph> loadedGraphs = [];

    public event Action<string>? OnGraphUpdated;

    public KnowledgeGraphService(
        ConceptExtractionService extractionService,
        ConceptCorrelationService correlationService,
        EmbeddingService embeddingService,
        LSHService lshService,
        SimHashService simHashService,
        IAppDataPathProvider pathProvider)
    {
        this.extractionService = extractionService;
        this.correlationService = correlationService;
        this.embeddingService = embeddingService;
        this.lshService = lshService;
        this.simHashService = simHashService;
        this.pathProvider = pathProvider;

        graphsDirectory = Path.Combine(this.pathProvider.AppDataDirectory, GraphsDirectoryName);
        Directory.CreateDirectory(graphsDirectory);
    }

    /// <summary>
    /// Creates a new knowledge graph from course resources.
    /// </summary>
    public async Task<KnowledgeGraph> CreateGraphFromResourcesAsync(
        string name,
        string? courseId,
        List<CourseResource> resources,
        IProgress<GraphBuildProgress>? progress = null,
        CancellationToken ct = default)
    {
        var graph = new KnowledgeGraph
        {
            Name = name,
            CourseId = courseId,
            Description = $"Knowledge graph for {name}"
        };

        // Total steps: resources + embeddings + relationships + finalization
        var totalSteps = resources.Count + 3;
        var currentStep = 0;

        void ReportProgress(string message, string? detail = null)
        {
            progress?.Report(new GraphBuildProgress
            {
                CurrentStep = currentStep,
                TotalSteps = totalSteps,
                Message = message,
                Detail = detail,
                PercentComplete = totalSteps > 0 ? (int)((double)currentStep / totalSteps * 100) : 0
            });
        }

        ReportProgress("Extracting concepts from resources...");

        // Extract concepts from all resources
        var allExtracted = new List<ExtractedConcept>();
        foreach (var resource in resources)
        {
            ct.ThrowIfCancellationRequested();
            currentStep++;
            ReportProgress($"Analyzing resource {currentStep} of {resources.Count}", resource.Title);

            var concepts = await extractionService.ExtractConceptsAsync(
                resource.Content, 
                resource.Id, 
                ct);
            
            allExtracted.AddRange(concepts);
        }

        if (allExtracted.Count == 0)
        {
            ReportProgress("No concepts found in resources.");
            return graph;
        }

        currentStep++;
        ReportProgress($"Generating embeddings for {allExtracted.Count} concepts...");

        // Create concept nodes with embeddings
        var nodes = await extractionService.CreateConceptNodesAsync(allExtracted, ct);
        
        // Add nodes to graph
        foreach (var node in nodes)
        {
            graph.Nodes[node.Id] = node;
        }

        currentStep++;
        ReportProgress("Finding relationships between concepts...");

        // Find related pairs
        var pairs = correlationService.FindRelatedPairs(nodes);
        
        ReportProgress($"Analyzing {pairs.Count} potential relationships...");

        // Infer prerequisites using LLM
        var relationships = await correlationService.InferPrerequisitesAsync(
            pairs, 
            graph.Nodes, 
            ct);

        // Add relationships to graph
        foreach (var rel in relationships)
        {
            graph.Relationships[rel.Id] = rel;
        }

        currentStep++;
        ReportProgress("Building graph structure...");

        // Build adjacency lists
        graph.RebuildAdjacencyLists();

        // Remove cycles
        ConceptCorrelationService.RemoveCycles(graph);

        // Calculate depths
        graph.RecalculateDepths();

        ReportProgress("Saving knowledge graph...");

        // Save the graph
        await SaveGraphAsync(graph, ct);

        loadedGraphs[graph.Id] = graph;
        OnGraphUpdated?.Invoke(graph.Id);

        var stats = graph.GetStats();
        ReportProgress($"Complete! {stats.TotalConcepts} concepts, {stats.TotalRelationships} relationships");

        return graph;
    }

    /// <summary>
    /// Adds new concepts from a resource to an existing graph.
    /// </summary>
    public async Task<int> AddResourceToGraphAsync(
        string graphId,
        CourseResource resource,
        CancellationToken ct = default)
    {
        var graph = await LoadGraphAsync(graphId, ct);
        if (graph == null)
            throw new InvalidOperationException($"Graph {graphId} not found.");

        // Extract new concepts
        var extracted = await extractionService.ExtractConceptsAsync(
            resource.Content, 
            resource.Id, 
            ct);

        if (extracted.Count == 0)
            return 0;

        // Filter out concepts that already exist (by term)
        var newExtracted = extracted
            .Where(e => graph.FindByTerm(e.Term) == null)
            .ToList();

        if (newExtracted.Count == 0)
            return 0;

        // Create nodes with embeddings
        var newNodes = await extractionService.CreateConceptNodesAsync(newExtracted, ct);

        foreach (var node in newNodes)
        {
            graph.Nodes[node.Id] = node;
        }

        // Find relationships between new nodes and existing nodes
        var allNodes = graph.Nodes.Values.ToList();
        var pairs = new List<ConceptPairScore>();

        foreach (var newNode in newNodes)
        {
            foreach (var existingNode in allNodes)
            {
                if (newNode.Id == existingNode.Id)
                    continue;

                var score = correlationService.ComputePairScore(newNode, existingNode);
                if (score.CombinedScore >= 0.3f)
                {
                    pairs.Add(score);
                }
            }
        }

        // Infer new relationships
        var newRelationships = await correlationService.InferPrerequisitesAsync(
            pairs,
            graph.Nodes,
            ct);

        foreach (var rel in newRelationships)
        {
            graph.Relationships[rel.Id] = rel;
        }

        // Rebuild graph structure
        graph.RebuildAdjacencyLists();
        ConceptCorrelationService.RemoveCycles(graph);
        graph.RecalculateDepths();
        graph.UpdatedAt = DateTime.UtcNow;
        graph.Version++;

        await SaveGraphAsync(graph, ct);
        OnGraphUpdated?.Invoke(graph.Id);

        return newNodes.Count;
    }

    /// <summary>
    /// Adds a single concept to the graph and finds its relationships.
    /// </summary>
    public async Task<ConceptNode> AddConceptAsync(
        string graphId,
        string term,
        string description,
        CancellationToken ct = default)
    {
        var graph = await LoadGraphAsync(graphId, ct);
        if (graph == null)
            throw new InvalidOperationException($"Graph {graphId} not found.");

        // Check if concept already exists
        var existing = graph.FindByTerm(term);
        if (existing != null)
            return existing;

        // Create embedding
        var embeddingText = $"{term}. {description}";
        var embedding = await embeddingService.GetEmbeddingAsync(embeddingText, ct);

        var node = new ConceptNode
        {
            Term = term,
            Description = description,
            Embedding = embedding,
            SemanticSignature = embedding.Length > 0 ? lshService.GetSignature(embedding) : [],
            LexicalSignature = simHashService.GetSignature64(embeddingText),
            IsVerified = true // Manually added
        };

        graph.Nodes[node.Id] = node;

        // Find relationships to existing concepts
        var pairs = new List<ConceptPairScore>();
        foreach (var existingNode in graph.Nodes.Values.Where(n => n.Id != node.Id))
        {
            var score = correlationService.ComputePairScore(node, existingNode);
            if (score.CombinedScore >= 0.3f)
            {
                pairs.Add(score);
            }
        }

        // Get top candidates and infer relationships
        var topPairs = pairs.OrderByDescending(p => p.CombinedScore).Take(10).ToList();
        var relationships = await correlationService.InferPrerequisitesAsync(
            topPairs, 
            graph.Nodes, 
            ct);

        foreach (var rel in relationships)
        {
            graph.Relationships[rel.Id] = rel;
        }

        // Rebuild graph
        graph.RebuildAdjacencyLists();
        ConceptCorrelationService.RemoveCycles(graph);
        graph.RecalculateDepths();
        graph.UpdatedAt = DateTime.UtcNow;
        graph.Version++;

        await SaveGraphAsync(graph, ct);
        OnGraphUpdated?.Invoke(graph.Id);

        return node;
    }

    /// <summary>
    /// Removes a concept and its relationships from the graph.
    /// </summary>
    public async Task RemoveConceptAsync(string graphId, string conceptId, CancellationToken ct = default)
    {
        var graph = await LoadGraphAsync(graphId, ct);
        if (graph == null)
            return;

        if (!graph.Nodes.ContainsKey(conceptId))
            return;

        graph.Nodes.Remove(conceptId);

        // Remove relationships involving this concept
        var toRemove = graph.Relationships
            .Where(kvp => kvp.Value.SourceConceptId == conceptId || kvp.Value.TargetConceptId == conceptId)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var relId in toRemove)
        {
            graph.Relationships.Remove(relId);
        }

        graph.RebuildAdjacencyLists();
        graph.RecalculateDepths();
        graph.UpdatedAt = DateTime.UtcNow;
        graph.Version++;

        await SaveGraphAsync(graph, ct);
        OnGraphUpdated?.Invoke(graph.Id);
    }

    /// <summary>
    /// Adds a prerequisite relationship between two concepts.
    /// </summary>
    public async Task AddPrerequisiteAsync(
        string graphId,
        string prerequisiteConceptId,
        string dependentConceptId,
        CancellationToken ct = default)
    {
        var graph = await LoadGraphAsync(graphId, ct);
        if (graph == null)
            return;

        if (!graph.Nodes.ContainsKey(prerequisiteConceptId) ||
            !graph.Nodes.ContainsKey(dependentConceptId))
            return;

        // Check if relationship already exists
        var exists = graph.Relationships.Values.Any(r =>
            r.SourceConceptId == prerequisiteConceptId &&
            r.TargetConceptId == dependentConceptId &&
            r.RelationType == ConceptRelationType.Prerequisite);

        if (exists)
            return;

        var prereq = graph.Nodes[prerequisiteConceptId];
        var dependent = graph.Nodes[dependentConceptId];

        var rel = new ConceptRelationship
        {
            SourceConceptId = prerequisiteConceptId,
            TargetConceptId = dependentConceptId,
            RelationType = ConceptRelationType.Prerequisite,
            ConfidenceScore = 1.0f,
            SemanticSimilarity = prereq.Embedding.Length > 0 && dependent.Embedding.Length > 0
                ? EmbeddingService.CosineSimilarity(prereq.Embedding, dependent.Embedding)
                : 0f,
            IsVerified = true
        };

        graph.Relationships[rel.Id] = rel;
        graph.RebuildAdjacencyLists();
        ConceptCorrelationService.RemoveCycles(graph);
        graph.RecalculateDepths();
        graph.UpdatedAt = DateTime.UtcNow;
        graph.Version++;

        await SaveGraphAsync(graph, ct);
        OnGraphUpdated?.Invoke(graph.Id);
    }

    /// <summary>
    /// Finds similar concepts using embedding similarity.
    /// </summary>
    public async Task<List<ConceptNode>> FindSimilarConceptsAsync(
        string graphId,
        string query,
        int topK = 5,
        CancellationToken ct = default)
    {
        var graph = await LoadGraphAsync(graphId, ct);
        if (graph == null || graph.Nodes.Count == 0)
            return [];

        // Get query embedding
        var queryEmbedding = await embeddingService.GetEmbeddingAsync(query, ct);
        if (queryEmbedding.Length == 0)
            return [];

        // Use LSH for candidate generation
        var querySig = lshService.GetSignature(queryEmbedding);
        const int lshThreshold = 80;

        var candidates = graph.Nodes.Values
            .Where(n => n.SemanticSignature.Length > 0)
            .Select(n => new
            {
                Node = n,
                LshDist = LSHService.HammingDistance(querySig, n.SemanticSignature)
            })
            .Where(x => x.LshDist <= lshThreshold)
            .ToList();

        // Score candidates with full cosine similarity
        var scored = candidates
            .Select(c => new
            {
                c.Node,
                Similarity = EmbeddingService.CosineSimilarity(queryEmbedding, c.Node.Embedding)
            })
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .Select(x => x.Node)
            .ToList();

        return scored;
    }

    /// <summary>
    /// Gets the complete learning path to understand a specific concept.
    /// </summary>
    public async Task<List<ConceptNode>> GetLearningPathToAsync(
        string graphId,
        string targetTerm,
        CancellationToken ct = default)
    {
        var graph = await LoadGraphAsync(graphId, ct);
        if (graph == null)
            return [];

        var target = graph.FindByTerm(targetTerm);
        if (target == null)
            return [];

        return graph.GetLearningPathTo(target.Id);
    }

    /// <summary>
    /// Gets all concepts that build upon a starting concept.
    /// </summary>
    public async Task<List<ConceptNode>> GetLearningPathFromAsync(
        string graphId,
        string startTerm,
        CancellationToken ct = default)
    {
        var graph = await LoadGraphAsync(graphId, ct);
        if (graph == null)
            return [];

        var start = graph.FindByTerm(startTerm);
        if (start == null)
            return [];

        return graph.GetLearningPathFrom(start.Id);
    }

    /// <summary>
    /// Loads a graph from disk.
    /// </summary>
    public async Task<KnowledgeGraph?> LoadGraphAsync(string graphId, CancellationToken ct = default)
    {
        if (loadedGraphs.TryGetValue(graphId, out var cached))
            return cached;

        var filePath = GetGraphFilePath(graphId);
        if (!File.Exists(filePath))
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(filePath, ct);
            var graph = JsonSerializer.Deserialize<KnowledgeGraph>(json);
            
            if (graph != null)
            {
                graph.RebuildAdjacencyLists();
                loadedGraphs[graph.Id] = graph;
            }
            
            return graph;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Saves a graph to disk.
    /// </summary>
    public async Task SaveGraphAsync(KnowledgeGraph graph, CancellationToken ct = default)
    {
        var filePath = GetGraphFilePath(graph.Id);
        var json = JsonSerializer.Serialize(graph, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, ct);
        loadedGraphs[graph.Id] = graph;
    }

    /// <summary>
    /// Gets all available graphs.
    /// </summary>
    public async Task<List<KnowledgeGraphInfo>> GetAllGraphsAsync(CancellationToken ct = default)
    {
        var infos = new List<KnowledgeGraphInfo>();

        if (!Directory.Exists(graphsDirectory))
            return infos;

        foreach (var file in Directory.GetFiles(graphsDirectory, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file, ct);
                var graph = JsonSerializer.Deserialize<KnowledgeGraph>(json);
                
                if (graph != null)
                {
                    infos.Add(new KnowledgeGraphInfo
                    {
                        Id = graph.Id,
                        Name = graph.Name,
                        Description = graph.Description,
                        CourseId = graph.CourseId,
                        ConceptCount = graph.Nodes.Count,
                        RelationshipCount = graph.Relationships.Count,
                        CreatedAt = graph.CreatedAt,
                        UpdatedAt = graph.UpdatedAt
                    });
                }
            }
            catch
            {
                // Skip invalid files
            }
        }

        return infos.OrderByDescending(i => i.UpdatedAt).ToList();
    }

    /// <summary>
    /// Deletes a graph.
    /// </summary>
    public Task DeleteGraphAsync(string graphId, CancellationToken ct = default)
    {
        loadedGraphs.Remove(graphId);
        
        var filePath = GetGraphFilePath(graphId);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a graph by course ID.
    /// </summary>
    public async Task<KnowledgeGraph?> GetGraphForCourseAsync(string courseId, CancellationToken ct = default)
    {
        var graphs = await GetAllGraphsAsync(ct);
        var info = graphs.FirstOrDefault(g => g.CourseId == courseId);
        
        if (info == null)
            return null;

        return await LoadGraphAsync(info.Id, ct);
    }

    private string GetGraphFilePath(string graphId)
    {
        return Path.Combine(graphsDirectory, $"{graphId}.json");
    }
}

/// <summary>
/// Summary information about a knowledge graph.
/// </summary>
public class KnowledgeGraphInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string? CourseId { get; set; }
    public int ConceptCount { get; set; }
    public int RelationshipCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Progress information for knowledge graph building.
/// </summary>
public class GraphBuildProgress
{
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public int PercentComplete { get; set; }
    public string Message { get; set; } = "";
    public string? Detail { get; set; }
}
