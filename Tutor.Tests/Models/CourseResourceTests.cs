using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class CourseResourceTests
{
    [Fact]
    public void Defaults_AreSensible()
    {
        var r = new CourseResource();

        Assert.True(Guid.TryParse(r.Id, out _));
        Assert.Equal(ResourceType.Text, r.Type);
        Assert.False(r.IsProcessed);
        Assert.False(r.HasConceptMap);
        Assert.Equal(ConceptMapStatus.NotStarted, r.ConceptMapStatus);
        Assert.Null(r.FormattedContent);
    }

    [Fact]
    public void GetEffectiveContent_PrefersFormatted()
    {
        var r = new CourseResource
        {
            Content = "raw",
            FormattedContent = "formatted"
        };

        Assert.Equal("formatted", r.GetEffectiveContent());
    }

    [Fact]
    public void GetEffectiveContent_FallsBackToRaw()
    {
        var r = new CourseResource { Content = "raw" };
        Assert.Equal("raw", r.GetEffectiveContent());
    }

    [Fact]
    public void HasConceptMap_RequiresIdAndReadyStatus()
    {
        var r = new CourseResource
        {
            ConceptMapId = "cm-1",
            ConceptMapStatus = ConceptMapStatus.ExtractingConcepts
        };
        Assert.False(r.HasConceptMap);

        r.ConceptMapStatus = ConceptMapStatus.Ready;
        Assert.True(r.HasConceptMap);

        r.ConceptMapId = null;
        Assert.False(r.HasConceptMap);
    }

    [Fact]
    public void GetEffectiveMaxIterations_OverrideTakesPrecedence()
    {
        var r = new CourseResource { OrphanLinkingMaxIterations = 5 };
        Assert.Equal(5, r.GetEffectiveMaxIterations(globalDefault: 10));
    }

    [Fact]
    public void GetEffectiveMaxIterations_FallsBackToGlobal()
    {
        var r = new CourseResource { OrphanLinkingMaxIterations = null };
        Assert.Equal(10, r.GetEffectiveMaxIterations(globalDefault: 10));
    }

    [Fact]
    public void GetEffectiveMinConfidence_OverrideTakesPrecedence()
    {
        var r = new CourseResource { OrphanLinkingMinConfidence = 0.42f };
        Assert.Equal(0.42f, r.GetEffectiveMinConfidence(globalDefault: 0.7f));
    }

    [Fact]
    public void GetEffectiveMinConfidence_FallsBackToGlobal()
    {
        var r = new CourseResource { OrphanLinkingMinConfidence = null };
        Assert.Equal(0.7f, r.GetEffectiveMinConfidence(globalDefault: 0.7f));
    }
}
