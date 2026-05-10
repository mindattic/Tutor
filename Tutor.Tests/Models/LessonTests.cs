using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class LessonTests
{
    [Test]
    public void Defaults_AreSensible()
    {
        var lesson = new Lesson();
        Assert.That(Guid.TryParse(lesson.Id, out _), Is.True);
        Assert.That(lesson.Topics, Is.Empty);
        Assert.That(lesson.Sections, Is.Empty);
        Assert.That(lesson.Tags, Is.Empty);
        Assert.That(lesson.Order, Is.EqualTo(0));
        Assert.That(lesson.EstimatedMinutes, Is.EqualTo(0));
    }

    [Test]
    public void TopicCount_TotalConceptCount_AggregateAcrossTopics()
    {
        var lesson = new Lesson
        {
            Topics =
            {
                new LessonTopic { ConceptIds = ["c1", "c2"], Order = 0 },
                new LessonTopic { ConceptIds = ["c3"], Order = 1 }
            }
        };

        Assert.That(lesson.TopicCount, Is.EqualTo(2));
        Assert.That(lesson.TotalConceptCount, Is.EqualTo(3));
    }

    [Test]
    public void GetAllConceptIds_PreservesTopicOrder()
    {
        var lesson = new Lesson
        {
            Topics =
            {
                new LessonTopic { Order = 1, ConceptIds = ["b1", "b2"] },
                new LessonTopic { Order = 0, ConceptIds = ["a1"] }
            }
        };

        Assert.That(lesson.GetAllConceptIds().ToArray(), Is.EqualTo(new[] { "a1", "b1", "b2" }));
    }

    [Test]
    public void GetTotalSectionCount_CountsNestedSections()
    {
        var lesson = new Lesson
        {
            Sections =
            {
                new Section
                {
                    Title = "1",
                    Children = { new Section { Title = "1a" }, new Section { Title = "1b" } }
                },
                new Section { Title = "2" }
            }
        };

        Assert.That(lesson.GetTotalSectionCount(), Is.EqualTo(4));
    }

    [Test]
    public void FindSection_ReturnsNestedSectionById()
    {
        var nested = new Section { Title = "deep" };
        var parent = new Section { Title = "shallow", Children = { nested } };
        var lesson = new Lesson { Sections = { parent } };

        Assert.That(lesson.FindSection(nested.Id), Is.SameAs(nested));
        Assert.That(lesson.FindSection(parent.Id), Is.SameAs(parent));
        Assert.That(lesson.FindSection("does-not-exist"), Is.Null);
    }

    [Test]
    public void GetAllSectionsFlattened_VisitsParentBeforeChildren_AndOrders()
    {
        var lesson = new Lesson
        {
            Sections =
            {
                new Section { Title = "second", Order = 1 },
                new Section
                {
                    Title = "first",
                    Order = 0,
                    Children =
                    {
                        new Section { Title = "first.b", Order = 1 },
                        new Section { Title = "first.a", Order = 0 }
                    }
                }
            }
        };

        var titles = lesson.GetAllSectionsFlattened().Select(s => s.Title).ToArray();
        Assert.That(titles, Is.EqualTo(new[] { "first", "first.a", "first.b", "second" }));
    }

    [Test]
    public void GenerateSectionNumbers_AssignsHierarchicalIds()
    {
        var lesson = new Lesson
        {
            Sections =
            {
                new Section
                {
                    Order = 0,
                    Children =
                    {
                        new Section { Order = 0 },
                        new Section { Order = 1 }
                    }
                },
                new Section { Order = 1 }
            }
        };

        lesson.GenerateSectionNumbers(lessonNumber: 3);

        Assert.That(lesson.Sections[0].Number, Is.EqualTo("3a"));
        Assert.That(lesson.Sections[1].Number, Is.EqualTo("3b"));
        Assert.That(lesson.Sections[0].Children[0].Number, Is.EqualTo("3a-i"));
        Assert.That(lesson.Sections[0].Children[1].Number, Is.EqualTo("3a-ii"));
        Assert.That(lesson.Sections[0].Depth, Is.EqualTo(0));
        Assert.That(lesson.Sections[0].Children[0].Depth, Is.EqualTo(1));
    }
}
