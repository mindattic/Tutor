using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ConceptTests
{
    [Test]
    public void NewConcept_HasUniqueId()
    {
        var a = new Concept();
        var b = new Concept();
        Assert.That(b.Id, Is.Not.EqualTo(a.Id));
    }

    [Test]
    public void DefaultConfidenceScore_IsOne()
    {
        var c = new Concept();
        Assert.That(c.ConfidenceScore, Is.EqualTo(1.0f));
    }

    [Test]
    public void DefaultCollections_AreEmpty()
    {
        var c = new Concept();
        Assert.That(c.Aliases, Is.Empty);
        Assert.That(c.Tags, Is.Empty);
        Assert.That(c.PrerequisiteIds, Is.Empty);
        Assert.That(c.RelatedIds, Is.Empty);
        Assert.That(c.SourceResourceIds, Is.Empty);
    }

    [Test]
    public void IsDynamicallyExpanded_DefaultFalse()
    {
        var c = new Concept();
        Assert.That(c.IsDynamicallyExpanded, Is.False);
        Assert.That(c.ExpansionQuery, Is.Null);
    }

    [Test]
    public void Properties_CanBeSet()
    {
        var c = new Concept
        {
            Title = "Photosynthesis",
            Summary = "Plants convert light to energy",
            Content = "Detailed explanation...",
            Aliases = ["light synthesis"],
            Tags = ["biology"],
            ConceptMapId = "map-1",
            ConfidenceScore = 0.95f,
            IsDynamicallyExpanded = true,
            ExpansionQuery = "How do plants make food?",
            SourceExcerpt = "Plants use sunlight..."
        };

        Assert.That(c.Title, Is.EqualTo("Photosynthesis"));
        Assert.That(c.Summary, Is.EqualTo("Plants convert light to energy"));
        Assert.That(c.ConfidenceScore, Is.EqualTo(0.95f));
        Assert.That(c.IsDynamicallyExpanded, Is.True);
        Assert.That(c.SourceExcerpt, Is.Not.Null);
    }
}
