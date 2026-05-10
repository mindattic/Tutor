using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class SectionTests
{
    [Fact]
    public void Defaults_AreSensible()
    {
        var section = new Section();
        Assert.True(Guid.TryParse(section.Id, out _));
        Assert.Empty(section.Children);
        Assert.Empty(section.ConceptIds);
        Assert.Empty(section.LearningObjectives);
        Assert.Empty(section.KeyTerms);
        Assert.False(section.HasQuiz);
        Assert.Equal(3, section.QuizQuestionCount);
        Assert.Equal(70, section.QuizPassingScore);
        Assert.Equal(2, section.EstimatedReadingMinutes);
    }

    [Fact]
    public void GetAllConceptIds_IncludesDescendants()
    {
        var section = new Section
        {
            ConceptIds = { "c1" },
            Children =
            {
                new Section { ConceptIds = { "c2", "c3" } },
                new Section
                {
                    ConceptIds = { "c4" },
                    Children = { new Section { ConceptIds = { "c5" } } }
                }
            }
        };

        var ids = section.GetAllConceptIds().ToList();

        Assert.Equal(new[] { "c1", "c2", "c3", "c4", "c5" }, ids);
    }

    [Fact]
    public void GetLeafSectionCount_OnlyCountsLeaves()
    {
        var section = new Section
        {
            Children =
            {
                new Section(),                                                        // leaf
                new Section { Children = { new Section(), new Section() } },          // 2 leaves
                new Section { Children = { new Section { Children = { new Section() } } } } // 1 leaf
            }
        };

        Assert.Equal(4, section.GetLeafSectionCount());
    }

    [Fact]
    public void FindSection_FindsSelfAndDescendants()
    {
        var deep = new Section();
        var middle = new Section { Children = { deep } };
        var root = new Section { Children = { middle } };

        Assert.Same(root, root.FindSection(root.Id));
        Assert.Same(middle, root.FindSection(middle.Id));
        Assert.Same(deep, root.FindSection(deep.Id));
        Assert.Null(root.FindSection("nope"));
    }

    [Fact]
    public void GetSectionsWithQuizzes_FlattensQuizSections()
    {
        var section = new Section
        {
            HasQuiz = true,
            Children =
            {
                new Section { HasQuiz = false, Children = { new Section { HasQuiz = true } } },
                new Section { HasQuiz = true }
            }
        };

        Assert.Equal(3, section.GetSectionsWithQuizzes().Count());
    }

    [Fact]
    public void CalculateTotalReadingMinutes_SumsThroughTree()
    {
        var section = new Section
        {
            EstimatedReadingMinutes = 5,
            Children =
            {
                new Section { EstimatedReadingMinutes = 3 },
                new Section
                {
                    EstimatedReadingMinutes = 7,
                    Children = { new Section { EstimatedReadingMinutes = 2 } }
                }
            }
        };

        Assert.Equal(17, section.CalculateTotalReadingMinutes());
    }
}
