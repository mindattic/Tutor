using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ConceptNodeTests
{
    [Test]
    public void NewNode_HasUniqueId()
    {
        var a = new ConceptNode();
        var b = new ConceptNode();
        Assert.That(b.Id, Is.Not.EqualTo(a.Id));
    }

    [Test]
    public void DefaultCollections_AreEmpty()
    {
        var node = new ConceptNode();
        Assert.That(node.Aliases, Is.Empty);
        Assert.That(node.PrerequisiteIds, Is.Empty);
        Assert.That(node.DependentIds, Is.Empty);
        Assert.That(node.Embedding, Is.Empty);
        Assert.That(node.SemanticSignature, Is.Empty);
        Assert.That(node.SourceResourceIds, Is.Empty);
        Assert.That(node.RelevantChunkIds, Is.Empty);
        Assert.That(node.Tags, Is.Empty);
    }

    [Test]
    public void Properties_CanBeSet()
    {
        var node = new ConceptNode
        {
            Term = "Velocity",
            Description = "Rate of change of position",
            HierarchyDepth = 2,
            Aliases = ["speed vector"],
            PrerequisiteIds = ["n-pos", "n-time"],
            DependentIds = ["n-accel"]
        };
        Assert.That(node.Term, Is.EqualTo("Velocity"));
        Assert.That(node.HierarchyDepth, Is.EqualTo(2));
        Assert.That(node.PrerequisiteIds, Has.Count.EqualTo(2));
        Assert.That(node.DependentIds, Has.Count.EqualTo(1));
    }

    [Test]
    public void IsFoundational_TrueWhenNoPrerequisites()
    {
        var node = new ConceptNode();
        Assert.That(node.IsFoundational, Is.True);
    }

    [Test]
    public void IsFoundational_FalseWhenHasPrerequisites()
    {
        var node = new ConceptNode { PrerequisiteIds = ["other"] };
        Assert.That(node.IsFoundational, Is.False);
    }

    [Test]
    public void IsLeaf_TrueWhenNoDependents()
    {
        var node = new ConceptNode();
        Assert.That(node.IsLeaf, Is.True);
    }

    [Test]
    public void IsLeaf_FalseWhenHasDependents()
    {
        var node = new ConceptNode { DependentIds = ["dep1"] };
        Assert.That(node.IsLeaf, Is.False);
    }

    [Test]
    public void GetEmbeddingText_IncludesTerm()
    {
        var node = new ConceptNode { Term = "Force" };
        Assert.That(node.GetEmbeddingText(), Does.Contain("Force"));
    }

    [Test]
    public void GetEmbeddingText_IncludesDescription()
    {
        var node = new ConceptNode { Term = "Force", Description = "Push or pull" };
        var text = node.GetEmbeddingText();
        Assert.That(text, Does.Contain("Force"));
        Assert.That(text, Does.Contain("Push or pull"));
    }

    [Test]
    public void GetEmbeddingText_IncludesAliases()
    {
        var node = new ConceptNode { Term = "Force", Aliases = ["F", "net force"] };
        var text = node.GetEmbeddingText();
        Assert.That(text, Does.Contain("Also known as"));
        Assert.That(text, Does.Contain("net force"));
    }

    [Test]
    public void DefaultConfidenceScore_IsOne()
    {
        var node = new ConceptNode();
        Assert.That(node.ConfidenceScore, Is.EqualTo(1.0f));
    }

    [Test]
    public void Embedding_CanBeAssigned()
    {
        var node = new ConceptNode();
        node.Embedding = [0.1f, 0.2f, 0.3f];
        Assert.That(node.Embedding.Length, Is.EqualTo(3));
    }
}
