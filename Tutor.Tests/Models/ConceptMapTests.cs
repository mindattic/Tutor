using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ConceptMapTests
{
    private static ConceptMap CreateSampleMap()
    {
        var c1 = new Concept { Id = "c1", Title = "Addition" };
        var c2 = new Concept { Id = "c2", Title = "Multiplication" };
        var c3 = new Concept { Id = "c3", Title = "Division" };
        var c4 = new Concept { Id = "c4", Title = "Algebra" }; // orphan

        var r1 = new ConceptRelationship
        {
            SourceConceptId = "c1",
            TargetConceptId = "c2",
            RelationType = ConceptRelationType.Prerequisite
        };
        var r2 = new ConceptRelationship
        {
            SourceConceptId = "c2",
            TargetConceptId = "c3",
            RelationType = ConceptRelationType.Related
        };

        return new ConceptMap
        {
            Id = "map1",
            Name = "Math Basics",
            Concepts = [c1, c2, c3, c4],
            Relations = [r1, r2],
            ComplexityOrder =
            [
                new ConceptComplexity { ConceptId = "c1", Level = 0, PrerequisiteCount = 0 },
                new ConceptComplexity { ConceptId = "c2", Level = 1, PrerequisiteCount = 1 },
                new ConceptComplexity { ConceptId = "c3", Level = 2, PrerequisiteCount = 0 }
            ]
        };
    }

    [Test]
    public void GetConcept_ById_ReturnsConcept()
    {
        var map = CreateSampleMap();
        var c = map.GetConcept("c2");
        Assert.That(c, Is.Not.Null);
        Assert.That(c!.Title, Is.EqualTo("Multiplication"));
    }

    [Test]
    public void GetConcept_UnknownId_ReturnsNull()
    {
        var map = CreateSampleMap();
        Assert.That(map.GetConcept("unknown"), Is.Null);
    }

    [Test]
    public void GetConceptByTitle_CaseInsensitive()
    {
        var map = CreateSampleMap();
        var c = map.GetConceptByTitle("addition");
        Assert.That(c, Is.Not.Null);
        Assert.That(c!.Id, Is.EqualTo("c1"));
    }

    [Test]
    public void TotalConcepts_And_TotalRelations()
    {
        var map = CreateSampleMap();
        Assert.That(map.TotalConcepts, Is.EqualTo(4));
        Assert.That(map.TotalRelations, Is.EqualTo(2));
    }

    [Test]
    public void GetPrerequisites_ReturnsCorrectConcepts()
    {
        var map = CreateSampleMap();
        var prereqs = map.GetPrerequisites("c2").ToList();
        Assert.That(prereqs, Has.Count.EqualTo(1));
        Assert.That(prereqs[0].Id, Is.EqualTo("c1"));
    }

    [Test]
    public void GetDependents_ReturnsCorrectConcepts()
    {
        var map = CreateSampleMap();
        var deps = map.GetDependents("c1").ToList();
        Assert.That(deps, Has.Count.EqualTo(1));
        Assert.That(deps[0].Id, Is.EqualTo("c2"));
    }

    [Test]
    public void GetRelatedConcepts_ExcludesPrerequisites()
    {
        var map = CreateSampleMap();
        var related = map.GetRelatedConcepts("c2").ToList();
        Assert.That(related, Has.Some.Matches<Concept>(c => c.Id == "c3"));
    }

    [Test]
    public void GetConceptsByComplexity_OrdersFoundationalFirst()
    {
        var map = CreateSampleMap();
        var ordered = map.GetConceptsByComplexity().ToList();
        Assert.That(ordered[0].Id, Is.EqualTo("c1"));
        Assert.That(ordered[1].Id, Is.EqualTo("c2"));
        Assert.That(ordered[2].Id, Is.EqualTo("c3"));
    }

    [Test]
    public void GetConceptsByComplexity_IncludesConceptsNotInOrder()
    {
        var map = CreateSampleMap();
        var ordered = map.GetConceptsByComplexity().ToList();
        Assert.That(ordered, Has.Count.EqualTo(4));
        Assert.That(ordered, Has.Some.Matches<Concept>(c => c.Id == "c4"));
    }

    [Test]
    public void GetConceptsAtLevel_FiltersCorrectly()
    {
        var map = CreateSampleMap();
        var level0 = map.GetConceptsAtLevel(0).ToList();
        Assert.That(level0, Has.Count.EqualTo(1));
        Assert.That(level0[0].Id, Is.EqualTo("c1"));
    }

    [Test]
    public void MaxComplexityLevel_ReturnsHighest()
    {
        var map = CreateSampleMap();
        Assert.That(map.MaxComplexityLevel, Is.EqualTo(2));
    }

    [Test]
    public void MaxComplexityLevel_EmptyOrder_ReturnsZero()
    {
        var map = new ConceptMap();
        Assert.That(map.MaxComplexityLevel, Is.EqualTo(0));
    }

    [Test]
    public void FindConnectedComponents_IdentifiesOrphan()
    {
        var map = CreateSampleMap();
        var components = map.FindConnectedComponents();
        Assert.That(components, Has.Count.EqualTo(2));
        Assert.That(components[0].Size, Is.EqualTo(3)); // main cluster
        Assert.That(components[1].Size, Is.EqualTo(1)); // orphan c4
    }

    [Test]
    public void GetMainComponent_ReturnsLargest()
    {
        var map = CreateSampleMap();
        var main = map.GetMainComponent();
        Assert.That(main, Is.Not.Null);
        Assert.That(main!.Size, Is.EqualTo(3));
    }

    [Test]
    public void GetOrphanedComponents_ReturnsNonMain()
    {
        var map = CreateSampleMap();
        var orphans = map.GetOrphanedComponents();
        Assert.That(orphans, Has.Count.EqualTo(1));
        Assert.That(orphans[0].ConceptIds, Does.Contain("c4"));
    }

    [Test]
    public void HasOrphanedConcepts_TrueWhenMultipleComponents()
    {
        var map = CreateSampleMap();
        Assert.That(map.HasOrphanedConcepts, Is.True);
    }

    [Test]
    public void HasOrphanedConcepts_FalseWhenFullyConnected()
    {
        var map = CreateSampleMap();
        // Connect c4 to the main cluster
        map.Relations.Add(new ConceptRelationship
        {
            SourceConceptId = "c3",
            TargetConceptId = "c4",
            RelationType = ConceptRelationType.Related
        });
        Assert.That(map.HasOrphanedConcepts, Is.False);
    }

    [Test]
    public void OrphanedConceptCount_CorrectCount()
    {
        var map = CreateSampleMap();
        Assert.That(map.OrphanedConceptCount, Is.EqualTo(1));
    }

    [Test]
    public void GetConnectivityInfo_PopulatesAllFields()
    {
        var map = CreateSampleMap();
        var info = map.GetConnectivityInfo();
        Assert.That(info.TotalComponents, Is.EqualTo(2));
        Assert.That(info.MainComponentSize, Is.EqualTo(3));
        Assert.That(info.OrphanedComponentCount, Is.EqualTo(1));
        Assert.That(info.OrphanedConceptCount, Is.EqualTo(1));
        Assert.That(info.IsFullyConnected, Is.False);
    }

    [Test]
    public void GetComplexity_ReturnsCorrectInfo()
    {
        var map = CreateSampleMap();
        var complexity = map.GetComplexity("c2");
        Assert.That(complexity, Is.Not.Null);
        Assert.That(complexity!.Level, Is.EqualTo(1));
        Assert.That(complexity.PrerequisiteCount, Is.EqualTo(1));
    }

    [Test]
    public void EmptyMap_FindConnectedComponents_ReturnsEmpty()
    {
        var map = new ConceptMap();
        var components = map.FindConnectedComponents();
        Assert.That(components, Is.Empty);
    }

    [Test]
    public void Status_DefaultsToNotStarted()
    {
        var map = new ConceptMap();
        Assert.That(map.Status, Is.EqualTo(ConceptMapStatus.NotStarted));
    }

    [Test]
    public void Version_DefaultsToOne()
    {
        var map = new ConceptMap();
        Assert.That(map.Version, Is.EqualTo(1));
    }
}
