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

    private static readonly JsonSerializerOptions EvaluationJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
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
        var preGenerated = await TryLoadPreGeneratedAsync(course, sectionIds, conceptIds, questionCount);
        if (preGenerated.Count > 0)
            return preGenerated;

        // Live fallback. ConceptMapCollectionId names a ConceptMapCollection (a list of
        // ConceptMap ids), not a ConceptMap, so resolve the concepts across the collection
        // rather than loading the id as a single map (which never exists — TUT-T62).
        var concepts = await LoadCourseConceptsAsync(course);
        return await quizGenerator.GenerateFromConceptsAsync(
            concepts, conceptIds, sectionIds, questionCount);
    }

    /// <summary>
    /// Every concept the course owns: its ConceptMapCollection's maps first, then (for courses
    /// whose collection is missing) a map stored under the collection id itself (legacy) and
    /// the per-resource maps.
    /// </summary>
    private async Task<List<Concept>> LoadCourseConceptsAsync(Course course)
    {
        var loaded = await courseService.GetCourseConceptMapCollectionAsync(course.Id);
        var concepts = loaded?.GetAllConcepts().ToList() ?? new List<Concept>();
        if (concepts.Count > 0)
            return concepts;

        if (!string.IsNullOrEmpty(course.ConceptMapCollectionId))
        {
            var legacy = await conceptMapStorage.LoadAsync(course.ConceptMapCollectionId);
            if (legacy != null) concepts.AddRange(legacy.Concepts);
        }

        foreach (var cmId in await courseService.GetResourceConceptMapIdsAsync(course.Id))
        {
            var cm = await conceptMapStorage.LoadAsync(cmId);
            if (cm != null) concepts.AddRange(cm.Concepts);
        }
        return concepts;
    }

    private async Task<List<QuizQuestion>> TryLoadPreGeneratedAsync(
        Course course, List<string> sectionIds, List<string> conceptIds, int questionCount)
    {
        if (string.IsNullOrEmpty(course.CourseStructureId))
            return new();
        if (sectionIds.Count == 0 && conceptIds.Count == 0)
            return new();

        var structure = await structureStorage.LoadByCourseIdAsync(course.Id);
        if (structure == null) return new();

        return sectionIds.Count > 0
            ? PickPreGeneratedForSections(structure, sectionIds, questionCount)
            : PickPreGeneratedForConcepts(structure, conceptIds, questionCount);
    }

    /// <summary>
    /// A section quiz: the sections' own baked questions, in order, trimmed to the count.
    /// Fewer is fine — the quiz UI handles short sets — but never more than asked.
    /// </summary>
    public static List<QuizQuestion> PickPreGeneratedForSections(
        CourseStructure structure, List<string> sectionIds, int questionCount)
    {
        var perSection = new List<List<QuizQuestion>>();
        foreach (var sid in sectionIds)
        {
            var section = structure.FindSection(sid);
            if (section == null || section.PreGeneratedQuestions.Count == 0) continue;
            perSection.Add(section.PreGeneratedQuestions);
        }

        // Take round-robin across the sections: a whole-course pick (the final exam passes every
        // section) must not fill its count from the first lesson alone. One section keeps its order.
        var hits = new List<QuizQuestion>();
        for (var round = 0; hits.Count < questionCount; round++)
        {
            var added = false;
            foreach (var questions in perSection)
            {
                if (round >= questions.Count) continue;
                hits.Add(questions[round]);
                added = true;
                if (hits.Count >= questionCount) break;
            }
            if (!added) break;
        }
        return hits;
    }

    /// <summary>
    /// A learned-concepts quiz (the Quiz tab's own Start): baked questions from every section
    /// that covers one of the concepts, shuffled and trimmed to the count. Costs no LLM tokens.
    /// </summary>
    public static List<QuizQuestion> PickPreGeneratedForConcepts(
        CourseStructure structure, List<string> conceptIds, int questionCount)
    {
        if (conceptIds.Count == 0 || questionCount <= 0) return new();
        var wanted = conceptIds.ToHashSet();
        return structure.GetAllSectionsFlattened()
            .Where(s => s.PreGeneratedQuestions.Count > 0
                && (s.ConceptIds.Any(wanted.Contains)
                    || s.PreGeneratedQuestions.Any(q => q.RelatedConceptIds.Any(wanted.Contains))))
            .SelectMany(s => s.PreGeneratedQuestions)
            .GroupBy(q => q.Id)
            .Select(g => g.First())
            .OrderBy(_ => Random.Shared.Next())
            .Take(questionCount)
            .ToList();
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
                // The prompt asks for camelCase ("isCorrect"); STJ is case-sensitive by default, which left
                // IsCorrect false (every answer marked wrong) and Feedback null.
                var result = JsonSerializer.Deserialize<EvaluationResult>(json, EvaluationJsonOptions);
                
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
            var tempPath = filePath + ".tmp";
            await File.WriteAllTextAsync(tempPath, json);

            if (File.Exists(filePath))
                File.Replace(tempPath, filePath, filePath + ".bak");
            else
                File.Move(tempPath, filePath);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public async Task<List<QuizResult>> GetQuizResultsAsync(string userId, string? courseId = null)
    {
        await fileLock.WaitAsync();
        List<QuizResult> results;
        try
        {
            results = await LoadQuizResultsAsync(userId);
        }
        finally
        {
            fileLock.Release();
        }

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
            // Keep the unreadable file aside so the next save does not destroy it.
            try
            {
                File.Copy(filePath, $"{filePath}.corrupt-{DateTime.UtcNow:yyyyMMddHHmmss}", overwrite: true);
            }
            catch { }
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
