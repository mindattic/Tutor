using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

/// <summary>
/// Tests cover the prompt-to-questions parsing path that the import pipeline
/// relies on to bake quizzes into the .tutorcourse bundle. Uses a temp data
/// directory + FakeLlmService so no real LLM call is made.
/// </summary>
public class QuizGenerationServiceTests : IDisposable
{
    private readonly string sandbox;
    private readonly string? prevKnowledgeBasesPath;

    public QuizGenerationServiceTests()
    {
        sandbox = Path.Combine(Path.GetTempPath(), "tutor-quizgen-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);
        prevKnowledgeBasesPath = DataStorageSettings.KnowledgeBasesPath;
        DataStorageSettings.KnowledgeBasesPath = sandbox;
    }

    public void Dispose()
    {
        DataStorageSettings.KnowledgeBasesPath = prevKnowledgeBasesPath;
        try { Directory.Delete(sandbox, recursive: true); } catch { }
    }

    [Fact]
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

        Assert.Equal(2, questions.Count);
        Assert.Equal("What is photosynthesis?", questions[0].QuestionText);
        Assert.Equal(2, questions[0].Difficulty);
        Assert.Equal(2, questions[0].Points);
        Assert.Equal(sectionIds, questions[0].RelatedSectionIds);
        Assert.Equal(conceptIds, questions[0].RelatedConceptIds);
        Assert.Equal(1, llm.CallCount);
    }

    [Fact]
    public async Task GenerateAsync_ReturnsEmpty_WhenLlmRepliesWithGarbage()
    {
        var (service, llm, mapId, conceptIds) = await SetupAsync();
        llm.NextReply = "I'm sorry, I can't help with that.";

        var questions = await service.GenerateAsync(mapId, conceptIds, new List<string>(), 5);

        Assert.Empty(questions);
        Assert.Equal(1, llm.CallCount);
    }

    [Fact]
    public async Task GenerateAsync_ReturnsEmpty_WhenNoConceptIds()
    {
        var (service, llm, mapId, _) = await SetupAsync();

        var questions = await service.GenerateAsync(mapId, new List<string>(), new List<string>(), 5);

        Assert.Empty(questions);
        // Short-circuits before hitting the LLM.
        Assert.Equal(0, llm.CallCount);
    }

    [Fact]
    public async Task GenerateAsync_ReturnsEmpty_WhenConceptMapMissing()
    {
        var llm = new FakeLlmService();
        var conceptMapStorage = new ConceptMapStorageService();
        var service = new QuizGenerationService(llm, conceptMapStorage);

        var questions = await service.GenerateAsync(
            "nonexistent-id", new List<string> { "c1" }, new List<string>(), 5);

        Assert.Empty(questions);
        Assert.Equal(0, llm.CallCount);
    }

    [Fact]
    public async Task GenerateAsync_ReturnsEmpty_WhenZeroQuestionsRequested()
    {
        var (service, llm, mapId, conceptIds) = await SetupAsync();

        var questions = await service.GenerateAsync(mapId, conceptIds, new List<string>(), 0);

        Assert.Empty(questions);
        Assert.Equal(0, llm.CallCount);
    }

    [Fact]
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

        Assert.Single(questions);
        Assert.Equal("Define energy.", questions[0].QuestionText);
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
