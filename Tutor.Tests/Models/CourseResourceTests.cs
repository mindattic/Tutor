using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class CourseResourceTests
{
    [Test]
    public void Defaults_AreSensible()
    {
        var r = new CourseResource();

        Assert.That(Guid.TryParse(r.Id, out _), Is.True);
        Assert.That(r.Type, Is.EqualTo(ResourceType.Text));
        Assert.That(r.IsProcessed, Is.False);
        Assert.That(r.HasConceptMap, Is.False);
        Assert.That(r.ConceptMapStatus, Is.EqualTo(ConceptMapStatus.NotStarted));
        Assert.That(r.FormattedContent, Is.Null);
    }

    [Test]
    public void GetEffectiveContent_PrefersFormatted()
    {
        var r = new CourseResource
        {
            Content = "raw",
            FormattedContent = "formatted"
        };

        Assert.That(r.GetEffectiveContent(), Is.EqualTo("formatted"));
    }

    [Test]
    public void GetEffectiveContent_FallsBackToRaw()
    {
        var r = new CourseResource { Content = "raw" };
        Assert.That(r.GetEffectiveContent(), Is.EqualTo("raw"));
    }

    [Test]
    public void HasConceptMap_RequiresIdAndReadyStatus()
    {
        var r = new CourseResource
        {
            ConceptMapId = "cm-1",
            ConceptMapStatus = ConceptMapStatus.ExtractingConcepts
        };
        Assert.That(r.HasConceptMap, Is.False);

        r.ConceptMapStatus = ConceptMapStatus.Ready;
        Assert.That(r.HasConceptMap, Is.True);

        r.ConceptMapId = null;
        Assert.That(r.HasConceptMap, Is.False);
    }

    [Test]
    public void GetEffectiveMaxIterations_OverrideTakesPrecedence()
    {
        var r = new CourseResource { OrphanLinkingMaxIterations = 5 };
        Assert.That(r.GetEffectiveMaxIterations(globalDefault: 10), Is.EqualTo(5));
    }

    [Test]
    public void GetEffectiveMaxIterations_FallsBackToGlobal()
    {
        var r = new CourseResource { OrphanLinkingMaxIterations = null };
        Assert.That(r.GetEffectiveMaxIterations(globalDefault: 10), Is.EqualTo(10));
    }

    [Test]
    public void GetEffectiveMinConfidence_OverrideTakesPrecedence()
    {
        var r = new CourseResource { OrphanLinkingMinConfidence = 0.42f };
        Assert.That(r.GetEffectiveMinConfidence(globalDefault: 0.7f), Is.EqualTo(0.42f));
    }

    [Test]
    public void GetEffectiveMinConfidence_FallsBackToGlobal()
    {
        var r = new CourseResource { OrphanLinkingMinConfidence = null };
        Assert.That(r.GetEffectiveMinConfidence(globalDefault: 0.7f), Is.EqualTo(0.7f));
    }
}
