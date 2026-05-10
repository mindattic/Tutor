using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Local quiz controller. Picks pre-generated questions off the course structure
/// when available (the import pipeline bakes them in), and falls back to live
/// LLM generation via <see cref="QuizGenerationService"/> otherwise. Evaluates
/// student answers via the LLM router.
/// </summary>
public class LocalQuizController : IQuizController
{
    private readonly IAppDataPathProvider pathProvider;
    private readonly LlmServiceRouter openAI;
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly CourseStructureStorageService structureStorage;
    private readonly QuizGenerationService quizGenerator;
    private readonly string quizResultsPath;
    private readonly SemaphoreSlim fileLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public LocalQuizController(
        LlmServiceRouter openAI,
        CourseService courseService,
        ConceptMapStorageService conceptMapStorage,
        CourseStructureStorageService structureStorage,
        QuizGenerationService quizGenerator,
        IAppDataPathProvider pathProvider)
    {
        this.openAI = openAI;
        this.courseService = courseService;
        this.conceptMapStorage = conceptMapStorage;
        this.structureStorage = structureStorage;
        this.quizGenerator = quizGenerator;
        this.pathProvider = pathProvider;

        var appDataPath = pathProvider.AppDataDirectory;
        var quizFolder = Path.Combine(appDataPath, "Quizzes");
        Directory.CreateDirectory(quizFolder);
        quizResultsPath = quizFolder;
    }

    /// <summary>
    /// Helper method to call OpenAI for quiz generation.
    /// </summary>
    private async Task<string> GetAiResponseAsync(string prompt, string systemPrompt)
    {
        var messages = new List<ChatMessage>
        {
            new ChatMessage("user", prompt, "")
        };
        
        var reply = await openAI.GetReplyAsync(messages, systemPrompt);
        return reply.Text;
    }

    public async Task<List<QuizQuestion>> GenerateQuestionsAsync(
        string courseId,
        List<string> sectionIds,
        List<string> conceptIds,
        int questionCount = 5)
    {
        var course = await courseService.GetCourseAsync(courseId);
        if (course == null || string.IsNullOrEmpty(course.ConceptMapCollectionId))
            return new();

        // Pre-generated questions live on the section. The import pipeline bakes
        // them in for every section with HasQuiz=true, and they ride along in the
        // .tutorcourse bundle, so re-imports never have to spend LLM tokens.
        var preGenerated = await TryLoadPreGeneratedAsync(course, sectionIds, questionCount);
        if (preGenerated.Count > 0)
            return preGenerated;

        return await quizGenerator.GenerateAsync(
            course.ConceptMapCollectionId, conceptIds, sectionIds, questionCount);
    }

    private async Task<List<QuizQuestion>> TryLoadPreGeneratedAsync(
        Course course, List<string> sectionIds, int questionCount)
    {
        if (sectionIds.Count == 0 || string.IsNullOrEmpty(course.CourseStructureId))
            return new();

        var structure = await structureStorage.LoadByCourseIdAsync(course.Id);
        if (structure == null) return new();

        var hits = new List<QuizQuestion>();
        foreach (var sid in sectionIds)
        {
            var section = structure.FindSection(sid);
            if (section == null || section.PreGeneratedQuestions.Count == 0) continue;
            hits.AddRange(section.PreGeneratedQuestions);
        }

        // Deterministic order, then trim to the requested count. Fewer is fine —
        // the quiz UI handles short sets — but never return more than asked.
        return hits.Take(questionCount).ToList();
    }

    public async Task<(bool isCorrect, string? feedback)> EvaluateAnswerAsync(
        QuizQuestion question,
        string userAnswer)
    {
        if (string.IsNullOrWhiteSpace(userAnswer))
            return (false, "Please provide an answer.");

        // Use AI to evaluate the answer
        var prompt = $@"Evaluate if the student's answer is correct.

QUESTION: {question.QuestionText}

CORRECT ANSWER: {question.CorrectAnswer}

STUDENT'S ANSWER: {userAnswer}

Respond with a JSON object:
{{
  ""isCorrect"": true/false,
  ""feedback"": ""Brief explanation of why the answer is correct or what's missing""
}}";

        try
        {
            var response = await GetAiResponseAsync(prompt,
                "You are a quiz evaluator. Be fair but accurate. Accept answers that demonstrate understanding even if wording differs. Respond only with valid JSON.");

            var jsonStart = response.IndexOf('{');
            var jsonEnd = response.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var json = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var result = JsonSerializer.Deserialize<EvaluationResult>(json);
                
                if (result != null)
                    return (result.IsCorrect, result.Feedback);
            }
        }
        catch
        {
            // Fall back to simple string comparison
            var correct = userAnswer.Trim().Equals(question.CorrectAnswer.Trim(), 
                StringComparison.OrdinalIgnoreCase);
            return (correct, correct ? "Correct!" : $"The correct answer was: {question.CorrectAnswer}");
        }

        return (false, "Unable to evaluate answer.");
    }

    public async Task SaveQuizResultAsync(QuizResult result)
    {
        await fileLock.WaitAsync();
        try
        {
            var filePath = GetUserQuizResultsPath(result.UserId);
            var results = await LoadQuizResultsAsync(result.UserId);
            results.Add(result);
            
            var json = JsonSerializer.Serialize(results, JsonOptions);
            await File.WriteAllTextAsync(filePath, json);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public async Task<List<QuizResult>> GetQuizResultsAsync(string userId, string? courseId = null)
    {
        var results = await LoadQuizResultsAsync(userId);
        
        if (!string.IsNullOrEmpty(courseId))
            results = results.Where(r => r.CourseId == courseId).ToList();
        
        return results.OrderByDescending(r => r.CompletedAt).ToList();
    }

    private async Task<List<QuizResult>> LoadQuizResultsAsync(string userId)
    {
        var filePath = GetUserQuizResultsPath(userId);
        
        if (!File.Exists(filePath))
            return [];

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<QuizResult>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private string GetUserQuizResultsPath(string userId)
    {
        return Path.Combine(quizResultsPath, $"QuizResults-{userId}.json");
    }

    private class EvaluationResult
    {
        public bool IsCorrect { get; set; }
        public string? Feedback { get; set; }
    }
}
