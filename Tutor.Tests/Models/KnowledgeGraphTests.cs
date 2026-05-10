using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class KnowledgeGraphTests
{
    [Test]
    public void NewGraph_HasUniqueId()
    {
        var a = new KnowledgeGraph();
        var b = new KnowledgeGraph();
        Assert.That(b.Id, Is.Not.EqualTo(a.Id));
    }

    [Test]
    public void DefaultCollections_AreEmpty()
    {
        var graph = new KnowledgeGraph();
        Assert.That(graph.Nodes, Is.Empty);
        Assert.That(graph.Relationships, Is.Empty);
        Assert.That(graph.ForwardEdges, Is.Empty);
        Assert.That(graph.BackwardEdges, Is.Empty);
    }

    [Test]
    public void Properties_CanBeSet()
    {
        var graph = new KnowledgeGraph
        {
            Name = "Physics",
            Description = "Classical mechanics",
            CourseId = "course-1"
        };
        Assert.That(graph.Name, Is.EqualTo("Physics"));
        Assert.That(graph.Description, Is.EqualTo("Classical mechanics"));
        Assert.That(graph.CourseId, Is.EqualTo("course-1"));
    }

    [Test]
    public void Nodes_CanBeAdded()
    {
        var graph = new KnowledgeGraph();
        var node = new ConceptNode { Id = "n1", Term = "Force" };
        graph.Nodes["n1"] = node;
        Assert.That(graph.Nodes, Has.Count.EqualTo(1));
        Assert.That(graph.Nodes["n1"].Term, Is.EqualTo("Force"));
    }

    [Test]
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
        Assert.That(graph.Relationships, Has.Count.EqualTo(1));
    }
}
