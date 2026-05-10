using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

/// <summary>
/// Tests cover the prompt-to-questions parsing path that the import pipeline
/// relies on to bake quizzes into the .tutorcourse bundle. Uses a temp data
/// directory + FakeLlmService so no real LLM call is made.
/// </summary>
public class QuizGenerationServiceTests
{
    private string sandbox = string.Empty;
    private string? prevKnowledgeBasesPath;

    [SetUp]
    public void SetUp()
    {
        sandbox = Path.Combine(Path.GetTempPath(), "tutor-quizgen-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);
        prevKnowledgeBasesPath = DataStorageSettings.KnowledgeBasesPath;
        DataStorageSettings.KnowledgeBasesPath = sandbox;
    }

    [TearDown]
    public void TearDown()
    {
        DataStorageSettings.KnowledgeBasesPath = prevKnowledgeBasesPath;
        try { Directory.Delete(sandbox, recursive: true); } catch { }
    }

    [Test]
    public async Task GenerateAsync_ReturnsQuestions_ForValidLlmResponse()
    {
        var (service, llm, mapId, conceptIds) = await SetupAsync();

        llm.NextReply = """
            [
              {
                "questionText": "What is photosynthesis?",
                "correctAnswer": "The process plants use to convert sunlight into energy.",
                "explanation": "Photosynthesis converts CO2 and water into glucose using sunlight.",
                "difficulty": 2
              },
              {
                "questionText": "Where does it occur?",
                "correctAnswer": "Chloroplasts in plant cells.",
                "explanation": "Specifically in the thylakoid membranes.",
                "difficulty": 1
              }
            ]
            """;

        var sectionIds = new List<string> { "section-1" };
        var questions = await service.GenerateAsync(mapId, conceptIds, sectionIds, questionCount: 2);

        Assert.That(questions, Has.Count.EqualTo(2));
        Assert.That(questions[0].QuestionText, Is.EqualTo("What is photosynthesis?"));
        Assert.That(questions[0].Difficulty, Is.EqualTo(2));
        Assert.That(questions[0].Points, Is.EqualTo(2));
        Assert.That(questions[0].RelatedSectionIds, Is.EqualTo(sectionIds));
        Assert.That(questions[0].RelatedConceptIds, Is.EqualTo(conceptIds));
        Assert.That(llm.CallCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GenerateAsync_ReturnsEmpty_WhenLlmRepliesWithGarbage()
    {
        var (service, llm, mapId, conceptIds) = await SetupAsync();
        llm.NextReply = "I'm sorry, I can't help with that.";

        var questions = await service.GenerateAsync(mapId, conceptIds, new List<string>(), 5);

        Assert.That(questions, Is.Empty);
        Assert.That(llm.CallCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GenerateAsync_ReturnsEmpty_WhenNoConceptIds()
    {
        var (service, llm, mapId, _) = await SetupAsync();

        var questions = await service.GenerateAsync(mapId, new List<string>(), new List<string>(), 5);

        Assert.That(questions, Is.Empty);
        // Short-circuits before hitting the LLM.
        Assert.That(llm.CallCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GenerateAsync_ReturnsEmpty_WhenConceptMapMissing()
    {
        var llm = new FakeLlmService();
        var conceptMapStorage = new ConceptMapStorageService();
        var service = new QuizGenerationService(llm, conceptMapStorage);

        var questions = await service.GenerateAsync(
            "nonexistent-id", new List<string> { "c1" }, new List<string>(), 5);

        Assert.That(questions, Is.Empty);
        Assert.That(llm.CallCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GenerateAsync_ReturnsEmpty_WhenZeroQuestionsRequested()
    {
        var (service, llm, mapId, conceptIds) = await SetupAsync();

        var questions = await service.GenerateAsync(mapId, conceptIds, new List<string>(), 0);

        Assert.That(questions, Is.Empty);
        Assert.That(llm.CallCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GenerateAsync_ToleratesPreambleAroundJsonArray()
    {
        var (service, llm, mapId, conceptIds) = await SetupAsync();

        llm.NextReply = """
            Sure! Here are your questions:

            [
              {
                "questionText": "Define energy.",
                "correctAnswer": "The capacity to do work.",
                "difficulty": 1
              }
            ]

            Hope these help.
            """;

        var questions = await service.GenerateAsync(mapId, conceptIds, new List<string>(), 1);

        Assert.That(questions, Has.Count.EqualTo(1));
        Assert.That(questions[0].QuestionText, Is.EqualTo("Define energy."));
    }

    private async Task<(QuizGenerationService Service, FakeLlmService Llm, string MapId, List<string> ConceptIds)> SetupAsync()
    {
        var conceptMapStorage = new ConceptMapStorageService();
        var map = await conceptMapStorage.CreateAsync("Biology Basics");

        var concept1 = new Concept
        {
            Title = "Photosynthesis",
            Summary = "How plants make food from light.",
            Content = "Plants use chlorophyll to capture sunlight and turn CO2 + water into glucose.",
            ConceptMapId = map.Id
        };
        var concept2 = new Concept
        {
            Title = "Chloroplast",
            Summary = "The plant-cell organelle that hosts photosynthesis.",
            Content = "Chloroplasts contain stacks of thylakoids where light reactions happen.",
            ConceptMapId = map.Id
        };
        map.Concepts.Add(concept1);
        map.Concepts.Add(concept2);
        await conceptMapStorage.SaveAsync(map);

        var llm = new FakeLlmService();
        var service = new QuizGenerationService(llm, conceptMapStorage);
        return (service, llm, map.Id, new List<string> { concept1.Id, concept2.Id });
    }
}
