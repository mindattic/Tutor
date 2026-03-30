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

    [Fact]
    public void GetConcept_ById_ReturnsConcept()
    {
        var map = CreateSampleMap();
        var c = map.GetConcept("c2");
        Assert.NotNull(c);
        Assert.Equal("Multiplication", c.Title);
    }

    [Fact]
    public void GetConcept_UnknownId_ReturnsNull()
    {
        var map = CreateSampleMap();
        Assert.Null(map.GetConcept("unknown"));
    }

    [Fact]
    public void GetConceptByTitle_CaseInsensitive()
    {
        var map = CreateSampleMap();
        var c = map.GetConceptByTitle("addition");
        Assert.NotNull(c);
        Assert.Equal("c1", c.Id);
    }

    [Fact]
    public void TotalConcepts_And_TotalRelations()
    {
        var map = CreateSampleMap();
        Assert.Equal(4, map.TotalConcepts);
        Assert.Equal(2, map.TotalRelations);
    }

    [Fact]
    public void GetPrerequisites_ReturnsCorrectConcepts()
    {
        var map = CreateSampleMap();
        var prereqs = map.GetPrerequisites("c2").ToList();
        Assert.Single(prereqs);
        Assert.Equal("c1", prereqs[0].Id);
    }

    [Fact]
    public void GetDependents_ReturnsCorrectConcepts()
    {
        var map = CreateSampleMap();
        var deps = map.GetDependents("c1").ToList();
        Assert.Single(deps);
        Assert.Equal("c2", deps[0].Id);
    }

    [Fact]
    public void GetRelatedConcepts_ExcludesPrerequisites()
    {
        var map = CreateSampleMap();
        var related = map.GetRelatedConcepts("c2").ToList();
        Assert.Contains(related, c => c.Id == "c3");
    }

    [Fact]
    public void GetConceptsByComplexity_OrdersFoundationalFirst()
    {
        var map = CreateSampleMap();
        var ordered = map.GetConceptsByComplexity().ToList();
        Assert.Equal("c1", ordered[0].Id);
        Assert.Equal("c2", ordered[1].Id);
        Assert.Equal("c3", ordered[2].Id);
    }

    [Fact]
    public void GetConceptsByComplexity_IncludesConceptsNotInOrder()
    {
        var map = CreateSampleMap();
        var ordered = map.GetConceptsByComplexity().ToList();
        Assert.Equal(4, ordered.Count);
        Assert.Contains(ordered, c => c.Id == "c4");
    }

    [Fact]
    public void GetConceptsAtLevel_FiltersCorrectly()
    {
        var map = CreateSampleMap();
        var level0 = map.GetConceptsAtLevel(0).ToList();
        Assert.Single(level0);
        Assert.Equal("c1", level0[0].Id);
    }

    [Fact]
    public void MaxComplexityLevel_ReturnsHighest()
    {
        var map = CreateSampleMap();
        Assert.Equal(2, map.MaxComplexityLevel);
    }

    [Fact]
    public void MaxComplexityLevel_EmptyOrder_ReturnsZero()
    {
        var map = new ConceptMap();
        Assert.Equal(0, map.MaxComplexityLevel);
    }

    [Fact]
    public void FindConnectedComponents_IdentifiesOrphan()
    {
        var map = CreateSampleMap();
        var components = map.FindConnectedComponents();
        Assert.Equal(2, components.Count);
        Assert.Equal(3, components[0].Size); // main cluster
        Assert.Equal(1, components[1].Size); // orphan c4
    }

    [Fact]
    public void GetMainComponent_ReturnsLargest()
    {
        var map = CreateSampleMap();
        var main = map.GetMainComponent();
        Assert.NotNull(main);
        Assert.Equal(3, main.Size);
    }

    [Fact]
    public void GetOrphanedComponents_ReturnsNonMain()
    {
        var map = CreateSampleMap();
        var orphans = map.GetOrphanedComponents();
        Assert.Single(orphans);
        Assert.Contains("c4", orphans[0].ConceptIds);
    }

    [Fact]
    public void HasOrphanedConcepts_TrueWhenMultipleComponents()
    {
        var map = CreateSampleMap();
        Assert.True(map.HasOrphanedConcepts);
    }

    [Fact]
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
        Assert.False(map.HasOrphanedConcepts);
    }

    [Fact]
    public void OrphanedConceptCount_CorrectCount()
    {
        var map = CreateSampleMap();
        Assert.Equal(1, map.OrphanedConceptCount);
    }

    [Fact]
    public void GetConnectivityInfo_PopulatesAllFields()
    {
        var map = CreateSampleMap();
        var info = map.GetConnectivityInfo();
        Assert.Equal(2, info.TotalComponents);
        Assert.Equal(3, info.MainComponentSize);
        Assert.Equal(1, info.OrphanedComponentCount);
        Assert.Equal(1, info.OrphanedConceptCount);
        Assert.False(info.IsFullyConnected);
    }

    [Fact]
    public void GetComplexity_ReturnsCorrectInfo()
    {
        var map = CreateSampleMap();
        var complexity = map.GetComplexity("c2");
        Assert.NotNull(complexity);
        Assert.Equal(1, complexity.Level);
        Assert.Equal(1, complexity.PrerequisiteCount);
    }

    [Fact]
    public void EmptyMap_FindConnectedComponents_ReturnsEmpty()
    {
        var map = new ConceptMap();
        var components = map.FindConnectedComponents();
        Assert.Empty(components);
    }

    [Fact]
    public void Status_DefaultsToNotStarted()
    {
        var map = new ConceptMap();
        Assert.Equal(ConceptMapStatus.NotStarted, map.Status);
    }

    [Fact]
    public void Version_DefaultsToOne()
    {
        var map = new ConceptMap();
        Assert.Equal(1, map.Version);
    }
}
