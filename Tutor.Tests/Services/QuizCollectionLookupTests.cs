using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

/// <summary>
/// TUT-T62: the Quiz tab's own Start (a learned-concepts quiz) must yield questions for an
/// installed course, whose ConceptMapCollectionId names a collection rather than a ConceptMap.
/// </summary>
public class QuizCollectionLookupTests
{
    [Test]
    public void PickPreGeneratedForConcepts_ReturnsBakedQuestions_FromSectionsCoveringTheConcepts()
    {
        var q1 = new QuizQuestion { QuestionText = "Who is Alice?", RelatedConceptIds = ["c-alice"] };
        var q2 = new QuizQuestion { QuestionText = "What is the rabbit late for?", RelatedConceptIds = ["c-rabbit"] };
        var structure = new CourseStructure
        {
            Lessons =
            [
                new Lesson
                {
                    Sections =
                    [
                        new Section { ConceptIds = ["c-alice"], PreGeneratedQuestions = [q1] },
                        new Section { ConceptIds = ["c-rabbit"], PreGeneratedQuestions = [q2] },
                    ]
                }
            ]
        };

        var picked = LocalQuizController.PickPreGeneratedForConcepts(structure, ["c-alice"], 5);

        Assert.That(picked, Has.Count.EqualTo(1));
        Assert.That(picked[0].QuestionText, Is.EqualTo("Who is Alice?"));
    }

    [Test]
    public void PickPreGeneratedForConcepts_NeverReturnsMoreThanAsked()
    {
        var section = new Section { ConceptIds = ["c1"] };
        for (var i = 0; i < 10; i++)
            section.PreGeneratedQuestions.Add(new QuizQuestion { QuestionText = $"Q{i}" });
        var structure = new CourseStructure { Lessons = [new Lesson { Sections = [section] }] };

        Assert.That(LocalQuizController.PickPreGeneratedForConcepts(structure, ["c1"], 3), Has.Count.EqualTo(3));
    }

    [Test]
    public async Task GenerateFromConceptsAsync_UsesConceptsGatheredAcrossACollection()
    {
        var llm = new FakeLlmService
        {
            NextReply = """[{ "questionText": "Why does Alice follow the rabbit?", "correctAnswer": "Curiosity.", "difficulty": 1 }]"""
        };
        var service = new QuizGenerationService(llm, new ConceptMapStorageService());

        // Concepts from two different ConceptMaps of one collection.
        var alice = new Concept { Title = "Alice", Summary = "The protagonist." };
        var rabbit = new Concept { Title = "White Rabbit", Summary = "Always late." };

        var questions = await service.GenerateFromConceptsAsync(
            [alice, rabbit], [alice.Id, rabbit.Id], [], 1);

        Assert.That(questions, Has.Count.EqualTo(1));
        Assert.That(questions[0].QuestionText, Is.EqualTo("Why does Alice follow the rabbit?"));
    }

    [Test]
    public async Task GenerateFromConceptsAsync_ReturnsEmpty_WhenNoConceptMatches()
    {
        var service = new QuizGenerationService(new FakeLlmService(), new ConceptMapStorageService());
        var questions = await service.GenerateFromConceptsAsync(
            [new Concept { Title = "Alice" }], ["All Concepts"], [], 3);
        Assert.That(questions, Is.Empty);
    }
}
