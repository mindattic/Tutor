using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class CourseTests
{
    [Fact]
    public void NewCourse_HasUniqueId()
    {
        var a = new Course();
        var b = new Course();
        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
    public void HasConceptMapCollection_FalseWhenNull()
    {
        var course = new Course();
        Assert.False(course.HasConceptMapCollection);
    }

    [Fact]
    public void HasConceptMapCollection_TrueWhenSet()
    {
        var course = new Course { ConceptMapCollectionId = "col-1" };
        Assert.True(course.HasConceptMapCollection);
    }

    [Fact]
    public void HasCourseStructure_FalseWhenNull()
    {
        var course = new Course();
        Assert.False(course.HasCourseStructure);
    }

    [Fact]
    public void HasCourseStructure_TrueWhenSet()
    {
        var course = new Course { CourseStructureId = "str-1" };
        Assert.True(course.HasCourseStructure);
    }

    [Fact]
    public void IsReadyForLearning_RequiresBoth()
    {
        var course = new Course();
        Assert.False(course.IsReadyForLearning);

        course.ConceptMapCollectionId = "col-1";
        Assert.False(course.IsReadyForLearning);

        course.CourseStructureId = "str-1";
        Assert.True(course.IsReadyForLearning);
    }

    [Fact]
    public void DefaultCollections_AreEmpty()
    {
        var course = new Course();
        Assert.Empty(course.ResourceIds);
        Assert.Empty(course.Tags);
    }

    [Fact]
    public void Timestamps_AreReasonable()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var course = new Course();
        var after = DateTime.UtcNow.AddSeconds(1);

        Assert.InRange(course.CreatedAt, before, after);
        Assert.InRange(course.UpdatedAt, before, after);
    }
}
