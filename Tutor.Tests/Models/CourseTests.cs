using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class CourseTests
{
    [Test]
    public void NewCourse_HasUniqueId()
    {
        var a = new Course();
        var b = new Course();
        Assert.That(b.Id, Is.Not.EqualTo(a.Id));
    }

    [Test]
    public void HasConceptMapCollection_FalseWhenNull()
    {
        var course = new Course();
        Assert.That(course.HasConceptMapCollection, Is.False);
    }

    [Test]
    public void HasConceptMapCollection_TrueWhenSet()
    {
        var course = new Course { ConceptMapCollectionId = "col-1" };
        Assert.That(course.HasConceptMapCollection, Is.True);
    }

    [Test]
    public void HasCourseStructure_FalseWhenNull()
    {
        var course = new Course();
        Assert.That(course.HasCourseStructure, Is.False);
    }

    [Test]
    public void HasCourseStructure_TrueWhenSet()
    {
        var course = new Course { CourseStructureId = "str-1" };
        Assert.That(course.HasCourseStructure, Is.True);
    }

    [Test]
    public void IsReadyForLearning_RequiresBoth()
    {
        var course = new Course();
        Assert.That(course.IsReadyForLearning, Is.False);

        course.ConceptMapCollectionId = "col-1";
        Assert.That(course.IsReadyForLearning, Is.False);

        course.CourseStructureId = "str-1";
        Assert.That(course.IsReadyForLearning, Is.True);
    }

    [Test]
    public void DefaultCollections_AreEmpty()
    {
        var course = new Course();
        Assert.That(course.ResourceIds, Is.Empty);
        Assert.That(course.Tags, Is.Empty);
    }

    [Test]
    public void Timestamps_AreReasonable()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var course = new Course();
        var after = DateTime.UtcNow.AddSeconds(1);

        Assert.That(course.CreatedAt, Is.InRange(before, after));
        Assert.That(course.UpdatedAt, Is.InRange(before, after));
    }
}
