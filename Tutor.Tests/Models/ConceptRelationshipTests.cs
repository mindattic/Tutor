using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ConceptRelationshipTests
{
    [Fact]
    public void ComputeCombinedScore_DefaultWeights()
    {
        var rel = new ConceptRelationship
        {
            SemanticSimilarity = 0.9f,
            SemanticDistance = 10,
            LexicalDistance = 5,
            CoOccurrenceCount = 25
        };

        var score = rel.ComputeCombinedScore();
        Assert.InRange(score, 0f, 1f);
        Assert.True(score > 0.5f, "High similarity should yield high score");
    }

    [Fact]
    public void ComputeCombinedScore_ZeroInputs_ReturnsZeroish()
    {
        var rel = new ConceptRelationship
        {
            SemanticSimilarity = 0f,
            SemanticDistance = 256,
            LexicalDistance = 64,
            CoOccurrenceCount = 0
        };

        var score = rel.ComputeCombinedScore();
        Assert.InRange(score, 0f, 0.15f);
    }

    [Fact]
    public void ComputeCombinedScore_CustomWeights()
    {
        var rel = new ConceptRelationship
        {
            SemanticSimilarity = 1.0f,
            SemanticDistance = 0,
            LexicalDistance = 0,
            CoOccurrenceCount = 100
        };

        var scoreSemanticHeavy = rel.ComputeCombinedScore(semanticWeight: 0.8f, distanceWeight: 0.1f, coOccurrenceWeight: 0.1f);
        var scoreDistanceHeavy = rel.ComputeCombinedScore(semanticWeight: 0.1f, distanceWeight: 0.8f, coOccurrenceWeight: 0.1f);

        // Both should be high since all inputs are maximal
        Assert.True(scoreSemanticHeavy > 0.8f);
        Assert.True(scoreDistanceHeavy > 0.8f);
    }

    [Fact]
    public void DefaultRelationType_IsPrerequisite()
    {
        var rel = new ConceptRelationship();
        Assert.Equal(ConceptRelationType.Prerequisite, rel.RelationType);
    }

    [Theory]
    [InlineData(ConceptRelationType.Prerequisite)]
    [InlineData(ConceptRelationType.Related)]
    [InlineData(ConceptRelationType.Instance)]
    [InlineData(ConceptRelationType.PartOf)]
    [InlineData(ConceptRelationType.Specialization)]
    [InlineData(ConceptRelationType.CoOccurs)]
    [InlineData(ConceptRelationType.Contains)]
    [InlineData(ConceptRelationType.InstanceOf)]
    [InlineData(ConceptRelationType.SimilarTo)]
    [InlineData(ConceptRelationType.ContrastsWith)]
    public void AllRelationTypes_AreValid(ConceptRelationType type)
    {
        var rel = new ConceptRelationship { RelationType = type };
        Assert.Equal(type, rel.RelationType);
    }

    [Fact]
    public void AllRelationTypes_Count_Is10()
    {
        var values = Enum.GetValues<ConceptRelationType>();
        Assert.Equal(10, values.Length);
    }

    [Fact]
    public void EvidenceCollections_DefaultEmpty()
    {
        var rel = new ConceptRelationship();
        Assert.Empty(rel.EvidenceResourceIds);
        Assert.Empty(rel.EvidenceChunkIds);
    }
}
