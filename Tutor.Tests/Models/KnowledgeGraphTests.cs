using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class KnowledgeGraphTests
{
    [Fact]
    public void NewGraph_HasUniqueId()
    {
        var a = new KnowledgeGraph();
        var b = new KnowledgeGraph();
        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
    public void DefaultCollections_AreEmpty()
    {
        var graph = new KnowledgeGraph();
        Assert.Empty(graph.Nodes);
        Assert.Empty(graph.Relationships);
        Assert.Empty(graph.ForwardEdges);
        Assert.Empty(graph.BackwardEdges);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var graph = new KnowledgeGraph
        {
            Name = "Physics",
            Description = "Classical mechanics",
            CourseId = "course-1"
        };
        Assert.Equal("Physics", graph.Name);
        Assert.Equal("Classical mechanics", graph.Description);
        Assert.Equal("course-1", graph.CourseId);
    }

    [Fact]
    public void Nodes_CanBeAdded()
    {
        var graph = new KnowledgeGraph();
        var node = new ConceptNode { Id = "n1", Term = "Force" };
        graph.Nodes["n1"] = node;
        Assert.Single(graph.Nodes);
        Assert.Equal("Force", graph.Nodes["n1"].Term);
    }

    [Fact]
    public void Relationships_CanBeAdded()
    {
        var graph = new KnowledgeGraph();
        var rel = new ConceptRelationship
        {
            Id = "r1",
            SourceConceptId = "n1",
            TargetConceptId = "n2"
        };
        graph.Relationships["r1"] = rel;
        Assert.Single(graph.Relationships);
    }
}
