using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ConceptNodeTests
{
    [Fact]
    public void NewNode_HasUniqueId()
    {
        var a = new ConceptNode();
        var b = new ConceptNode();
        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
    public void DefaultCollections_AreEmpty()
    {
        var node = new ConceptNode();
        Assert.Empty(node.Aliases);
        Assert.Empty(node.PrerequisiteIds);
        Assert.Empty(node.DependentIds);
        Assert.Empty(node.Embedding);
        Assert.Empty(node.SemanticSignature);
        Assert.Empty(node.SourceResourceIds);
        Assert.Empty(node.RelevantChunkIds);
        Assert.Empty(node.Tags);
    }

    [Fact]
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
        Assert.Equal("Velocity", node.Term);
        Assert.Equal(2, node.HierarchyDepth);
        Assert.Equal(2, node.PrerequisiteIds.Count);
        Assert.Single(node.DependentIds);
    }

    [Fact]
    public void IsFoundational_TrueWhenNoPrerequisites()
    {
        var node = new ConceptNode();
        Assert.True(node.IsFoundational);
    }

    [Fact]
    public void IsFoundational_FalseWhenHasPrerequisites()
    {
        var node = new ConceptNode { PrerequisiteIds = ["other"] };
        Assert.False(node.IsFoundational);
    }

    [Fact]
    public void IsLeaf_TrueWhenNoDependents()
    {
        var node = new ConceptNode();
        Assert.True(node.IsLeaf);
    }

    [Fact]
    public void IsLeaf_FalseWhenHasDependents()
    {
        var node = new ConceptNode { DependentIds = ["dep1"] };
        Assert.False(node.IsLeaf);
    }

    [Fact]
    public void GetEmbeddingText_IncludesTerm()
    {
        var node = new ConceptNode { Term = "Force" };
        Assert.Contains("Force", node.GetEmbeddingText());
    }

    [Fact]
    public void GetEmbeddingText_IncludesDescription()
    {
        var node = new ConceptNode { Term = "Force", Description = "Push or pull" };
        var text = node.GetEmbeddingText();
        Assert.Contains("Force", text);
        Assert.Contains("Push or pull", text);
    }

    [Fact]
    public void GetEmbeddingText_IncludesAliases()
    {
        var node = new ConceptNode { Term = "Force", Aliases = ["F", "net force"] };
        var text = node.GetEmbeddingText();
        Assert.Contains("Also known as", text);
        Assert.Contains("net force", text);
    }

    [Fact]
    public void DefaultConfidenceScore_IsOne()
    {
        var node = new ConceptNode();
        Assert.Equal(1.0f, node.ConfidenceScore);
    }

    [Fact]
    public void Embedding_CanBeAssigned()
    {
        var node = new ConceptNode();
        node.Embedding = [0.1f, 0.2f, 0.3f];
        Assert.Equal(3, node.Embedding.Length);
    }
}
