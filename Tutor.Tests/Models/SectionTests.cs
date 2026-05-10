using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class SectionTests
{
    [Test]
    public void Defaults_AreSensible()
    {
        var section = new Section();
        Assert.That(Guid.TryParse(section.Id, out _), Is.True);
        Assert.That(section.Children, Is.Empty);
        Assert.That(section.ConceptIds, Is.Empty);
        Assert.That(section.LearningObjectives, Is.Empty);
        Assert.That(section.KeyTerms, Is.Empty);
        Assert.That(section.HasQuiz, Is.False);
        Assert.That(section.QuizQuestionCount, Is.EqualTo(3));
        Assert.That(section.QuizPassingScore, Is.EqualTo(70));
        Assert.That(section.EstimatedReadingMinutes, Is.EqualTo(2));
        Assert.That(section.PreGeneratedQuestions, Is.Empty);
    }

    [Test]
    public void PreGeneratedQuestions_RoundTripsThroughJson()
    {
        var section = new Section
        {
            HasQuiz = true,
            PreGeneratedQuestions =
            {
                new QuizQuestion { QuestionText = "Q1", CorrectAnswer = "A1", Difficulty = 2 },
                new QuizQuestion { QuestionText = "Q2", CorrectAnswer = "A2", Difficulty = 1 }
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(section);
        var restored = System.Text.Json.JsonSerializer.Deserialize<Section>(json)!;

        Assert.That(restored.PreGeneratedQuestions, Has.Count.EqualTo(2));
        Assert.That(restored.PreGeneratedQuestions[0].QuestionText, Is.EqualTo("Q1"));
        Assert.That(restored.PreGeneratedQuestions[0].CorrectAnswer, Is.EqualTo("A1"));
        Assert.That(restored.PreGeneratedQuestions[0].Difficulty, Is.EqualTo(2));
    }

    [Test]
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

        Assert.That(ids, Is.EqualTo(new[] { "c1", "c2", "c3", "c4", "c5" }));
    }

    [Test]
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

        Assert.That(section.GetLeafSectionCount(), Is.EqualTo(4));
    }

    [Test]
    public void FindSection_FindsSelfAndDescendants()
    {
        var deep = new Section();
        var middle = new Section { Children = { deep } };
        var root = new Section { Children = { middle } };

        Assert.That(root.FindSection(root.Id), Is.SameAs(root));
        Assert.That(root.FindSection(middle.Id), Is.SameAs(middle));
        Assert.That(root.FindSection(deep.Id), Is.SameAs(deep));
        Assert.That(root.FindSection("nope"), Is.Null);
    }

    [Test]
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

        Assert.That(section.GetSectionsWithQuizzes().Count(), Is.EqualTo(3));
    }

    [Test]
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

        Assert.That(section.CalculateTotalReadingMinutes(), Is.EqualTo(17));
    }
}
