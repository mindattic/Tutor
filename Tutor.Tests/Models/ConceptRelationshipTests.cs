using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ConceptRelationshipTests
{
    [Test]
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
        Assert.That(score, Is.InRange(0f, 1f));
        Assert.That(score > 0.5f, Is.True, "High similarity should yield high score");
    }

    [Test]
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
        Assert.That(score, Is.InRange(0f, 0.15f));
    }

    [Test]
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
        Assert.That(scoreSemanticHeavy > 0.8f, Is.True);
        Assert.That(scoreDistanceHeavy > 0.8f, Is.True);
    }

    [Test]
    public void DefaultRelationType_IsPrerequisite()
    {
        var rel = new ConceptRelationship();
        Assert.That(rel.RelationType, Is.EqualTo(ConceptRelationType.Prerequisite));
    }

    [TestCase(ConceptRelationType.Prerequisite)]
    [TestCase(ConceptRelationType.Related)]
    [TestCase(ConceptRelationType.Instance)]
    [TestCase(ConceptRelationType.PartOf)]
    [TestCase(ConceptRelationType.Specialization)]
    [TestCase(ConceptRelationType.CoOccurs)]
    [TestCase(ConceptRelationType.Contains)]
    [TestCase(ConceptRelationType.InstanceOf)]
    [TestCase(ConceptRelationType.SimilarTo)]
    [TestCase(ConceptRelationType.ContrastsWith)]
    public void AllRelationTypes_AreValid(ConceptRelationType type)
    {
        var rel = new ConceptRelationship { RelationType = type };
        Assert.That(rel.RelationType, Is.EqualTo(type));
    }

    [Test]
    public void AllRelationTypes_Count_Is10()
    {
        var values = Enum.GetValues<ConceptRelationType>();
        Assert.That(values.Length, Is.EqualTo(10));
    }

    [Test]
    public void EvidenceCollections_DefaultEmpty()
    {
        var rel = new ConceptRelationship();
        Assert.That(rel.EvidenceResourceIds, Is.Empty);
        Assert.That(rel.EvidenceChunkIds, Is.Empty);
    }
}
