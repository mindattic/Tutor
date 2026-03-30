using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ConceptTests
{
    [Fact]
    public void NewConcept_HasUniqueId()
    {
        var a = new Concept();
        var b = new Concept();
        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
    public void DefaultConfidenceScore_IsOne()
    {
        var c = new Concept();
        Assert.Equal(1.0f, c.ConfidenceScore);
    }

    [Fact]
    public void DefaultCollections_AreEmpty()
    {
        var c = new Concept();
        Assert.Empty(c.Aliases);
        Assert.Empty(c.Tags);
        Assert.Empty(c.PrerequisiteIds);
        Assert.Empty(c.RelatedIds);
        Assert.Empty(c.SourceResourceIds);
    }

    [Fact]
    public void IsDynamicallyExpanded_DefaultFalse()
    {
        var c = new Concept();
        Assert.False(c.IsDynamicallyExpanded);
        Assert.Null(c.ExpansionQuery);
    }

    [Fact]
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

        Assert.Equal("Photosynthesis", c.Title);
        Assert.Equal("Plants convert light to energy", c.Summary);
        Assert.Equal(0.95f, c.ConfidenceScore);
        Assert.True(c.IsDynamicallyExpanded);
        Assert.NotNull(c.SourceExcerpt);
    }
}
