using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Local quiz controller that uses OpenAI to generate questions
/// and evaluates answers.
/// </summary>
public class LocalQuizController : IQuizController
{
    private readonly IAppDataPathProvider _pathProvider;
    private readonly LlmServiceRouter openAI;
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorage;
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
        IAppDataPathProvider pathProvider)
    {
        this.openAI = openAI;
        this.courseService = courseService;
        this.conceptMapStorage = conceptMapStorage;
        _pathProvider = pathProvider;

        var appDataPath = _pathProvider.AppDataDirectory;
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
        var questions = new List<QuizQuestion>();
        
        // Get concept details for context
        var course = await courseService.GetCourseAsync(courseId);
        if (course == null || string.IsNullOrEmpty(course.ConceptMapCollectionId))
            return questions;

        var conceptMap = await conceptMapStorage.LoadAsync(course.ConceptMapCollectionId);
        if (conceptMap == null)
            return questions;

        var relevantConcepts = conceptMap.Concepts
            .Where(c => conceptIds.Contains(c.Id))
            .ToList();

        if (relevantConcepts.Count == 0)
            return questions;

        // Build context for question generation
        var conceptContext = string.Join("\n\n", relevantConcepts.Select(c => 
            $"**{c.Title}**\n{c.Summary}\n{c.Content}"));

        var prompt = $@"Generate {questionCount} quiz questions based on the following learning material.

LEARNING MATERIAL:
{conceptContext}


INSTRUCTIONS:
- Create short-answer questions that test understanding of key concepts
- Questions should require 1-3 sentence answers
- Include a mix of recall, understanding, and application questions
- Make questions specific and unambiguous
- Provide the correct answer for each question

OUTPUT FORMAT (JSON array):
[
  {{
    ""questionText"": ""What is..."",
    ""correctAnswer"": ""The answer is..."",
    ""explanation"": ""This is correct because..."",
    ""difficulty"": 1
  }}
]

Generate exactly {questionCount} questions:";

        try
        {
            var response = await GetAiResponseAsync(prompt, 
                "You are a quiz generator. Generate questions in valid JSON format only.");

            // Parse the response
            var jsonStart = response.IndexOf('[');
            var jsonEnd = response.LastIndexOf(']');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var json = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var parsedQuestions = JsonSerializer.Deserialize<List<QuizQuestionDto>>(json);
                
                if (parsedQuestions != null)
                {
                    questions = parsedQuestions.Select((q, i) => new QuizQuestion
                    {
                        Id = Guid.NewGuid().ToString(),
                        QuestionText = q.QuestionText ?? "",
                        CorrectAnswer = q.CorrectAnswer ?? "",
                        Explanation = q.Explanation,
                        Difficulty = q.Difficulty,
                        Points = q.Difficulty,
                        RelatedSectionIds = sectionIds,
                        RelatedConceptIds = conceptIds
                    }).ToList();
                }
            }
        }
        catch
        {
            // Return empty list on error
        }

        return questions;
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

    private class QuizQuestionDto
    {
        public string? QuestionText { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public int Difficulty { get; set; } = 1;
    }

    private class EvaluationResult
    {
        public bool IsCorrect { get; set; }
        public string? Feedback { get; set; }
    }
}
