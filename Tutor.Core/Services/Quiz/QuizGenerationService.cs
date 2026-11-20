using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Pure quiz-question generator. Wraps the LLM call so both the import pipeline
/// (pre-generating questions at bundle time) and <see cref="LocalQuizController"/>
/// (live fallback when no pre-generated set exists) share one implementation.
/// Takes <see cref="ILlmService"/> rather than <see cref="LlmServiceRouter"/>
/// directly so tests can substitute a fake.
/// </summary>
public sealed class QuizGenerationService
{
    // The prompt asks the LLM for camelCase keys (questionText, correctAnswer)
    // but the DTO is PascalCase. Without case-insensitive matching the parse
    // silently returned empty — a latent bug in the original LocalQuizController.
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILlmService llm;
    private readonly ConceptMapStorageService conceptMapStorage;

    public QuizGenerationService(
        ILlmService llm,
        ConceptMapStorageService conceptMapStorage)
    {
        this.llm = llm;
        this.conceptMapStorage = conceptMapStorage;
    }

    /// <summary>
    /// Generates <paramref name="questionCount"/> questions for the given concept IDs.
    /// Returns an empty list if the concepts can't be loaded or the LLM response is
    /// unparseable — callers should treat that as "no quiz available" rather than
    /// failing the surrounding pipeline.
    /// </summary>
    public async Task<List<QuizQuestion>> GenerateAsync(
        string conceptMapCollectionId,
        List<string> conceptIds,
        List<string> sectionIds,
        int questionCount,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(conceptMapCollectionId) || conceptIds.Count == 0 || questionCount <= 0)
            return new();

        // Despite the parameter name, this overload takes a single ConceptMap id (the build
        // pipelines pass the merged map). A course's ConceptMapCollectionId names a collection,
        // not a map — callers holding one resolve its concepts and use GenerateFromConceptsAsync.
        var conceptMap = await conceptMapStorage.LoadAsync(conceptMapCollectionId, ct);
        if (conceptMap == null) return new();

        return await GenerateFromConceptsAsync(conceptMap.Concepts, conceptIds, sectionIds, questionCount, ct);
    }

    /// <summary>
    /// Most concepts sent to the LLM in one quiz prompt. A whole-course quiz can match hundreds
    /// of concepts; a random sample keeps the prompt bounded and varies the quiz between runs.
    /// </summary>
    public const int MaxConceptsPerPrompt = 12;

    /// <summary>
    /// Generates questions from already-loaded concepts (e.g. every concept across a course's
    /// ConceptMapCollection). Only concepts whose id is in <paramref name="conceptIds"/> are used.
    /// Same empty-on-failure contract as <see cref="GenerateAsync"/>.
    /// </summary>
    public async Task<List<QuizQuestion>> GenerateFromConceptsAsync(
        IEnumerable<Concept> concepts,
        List<string> conceptIds,
        List<string> sectionIds,
        int questionCount,
        CancellationToken ct = default)
    {
        if (conceptIds.Count == 0 || questionCount <= 0)
            return new();

        var wanted = conceptIds.ToHashSet();
        var relevantConcepts = concepts
            .Where(c => wanted.Contains(c.Id))
            .GroupBy(c => c.Id)
            .Select(g => g.First())
            .ToList();
        if (relevantConcepts.Count == 0) return new();

        if (relevantConcepts.Count > MaxConceptsPerPrompt)
            relevantConcepts = relevantConcepts.OrderBy(_ => Random.Shared.Next()).Take(MaxConceptsPerPrompt).ToList();
        var usedConceptIds = relevantConcepts.Select(c => c.Id).ToList();

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
            var messages = new List<ChatMessage>
            {
                new ChatMessage("user", prompt, "")
            };
            var reply = await llm.GetReplyAsync(messages,
                "You are a quiz generator. Generate questions in valid JSON format only.", ct);
            var response = reply.Text;

            var jsonStart = response.IndexOf('[');
            var jsonEnd = response.LastIndexOf(']');
            if (jsonStart < 0 || jsonEnd <= jsonStart) return new();

            var json = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
            var parsed = JsonSerializer.Deserialize<List<QuizQuestionDto>>(json, JsonOpts);
            if (parsed == null) return new();

            return parsed.Select(q => new QuizQuestion
            {
                Id = Guid.NewGuid().ToString(),
                QuestionText = q.QuestionText ?? "",
                CorrectAnswer = q.CorrectAnswer ?? "",
                Explanation = q.Explanation,
                Difficulty = q.Difficulty,
                Points = q.Difficulty,
                RelatedSectionIds = sectionIds,
                RelatedConceptIds = usedConceptIds
            }).ToList();
        }
        catch
        {
            return new();
        }
    }

    private class QuizQuestionDto
    {
        public string? QuestionText { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public int Difficulty { get; set; } = 1;
    }
}
