using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class LessonTests
{
    [Fact]
    public void Defaults_AreSensible()
    {
        var lesson = new Lesson();
        Assert.True(Guid.TryParse(lesson.Id, out _));
        Assert.Empty(lesson.Topics);
        Assert.Empty(lesson.Sections);
        Assert.Empty(lesson.Tags);
        Assert.Equal(0, lesson.Order);
        Assert.Equal(0, lesson.EstimatedMinutes);
    }

    [Fact]
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

        Assert.Equal(2, lesson.TopicCount);
        Assert.Equal(3, lesson.TotalConceptCount);
    }

    [Fact]
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

        Assert.Equal(new[] { "a1", "b1", "b2" }, lesson.GetAllConceptIds().ToArray());
    }

    [Fact]
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

        Assert.Equal(4, lesson.GetTotalSectionCount());
    }

    [Fact]
    public void FindSection_ReturnsNestedSectionById()
    {
        var nested = new Section { Title = "deep" };
        var parent = new Section { Title = "shallow", Children = { nested } };
        var lesson = new Lesson { Sections = { parent } };

        Assert.Same(nested, lesson.FindSection(nested.Id));
        Assert.Same(parent, lesson.FindSection(parent.Id));
        Assert.Null(lesson.FindSection("does-not-exist"));
    }

    [Fact]
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
        Assert.Equal(new[] { "first", "first.a", "first.b", "second" }, titles);
    }

    [Fact]
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

        Assert.Equal("3a", lesson.Sections[0].Number);
        Assert.Equal("3b", lesson.Sections[1].Number);
        Assert.Equal("3a-i", lesson.Sections[0].Children[0].Number);
        Assert.Equal("3a-ii", lesson.Sections[0].Children[1].Number);
        Assert.Equal(0, lesson.Sections[0].Depth);
        Assert.Equal(1, lesson.Sections[0].Children[0].Depth);
    }
}
