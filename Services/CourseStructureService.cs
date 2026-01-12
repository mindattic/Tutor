using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for generating a CourseStructure from a KnowledgeBase.
/// 
/// The CourseStructure is a curated learning path that:
/// - Organizes Concepts into Lessons and Topics
/// - References Concepts by ID from the KnowledgeBase (no duplication)
/// - Provides a linear, guided learning experience
/// 
/// Generation pipeline:
/// 1. Analyze the KnowledgeBase's Concepts and relationships
/// 2. Generate Lessons that group related topics
/// 3. Organize Topics within Lessons with ordered ConceptIds
/// 4. Ensure the order respects prerequisite relationships
/// </summary>
public sealed class CourseStructureService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    private readonly CourseStructureStorageService storageService;
    private readonly KnowledgeBaseStorageService kbStorageService;

    private const string LessonGenerationPrompt = """
        You are an expert curriculum designer. Given a list of concepts from an educational domain,
        organize them into a structured course with lessons and topics.
        
        A Lesson is a major section of the course (like a chapter).
        A Topic is a focused grouping of related concepts within a lesson.
        
        The course should:
        - Progress from foundational concepts to advanced ones
        - Group related concepts into coherent topics
        - Group related topics into coherent lessons
        - Ensure prerequisites are taught before dependent concepts
        
        Given these concepts (with their complexity levels):
        {CONCEPTS}
        
        Return your curriculum design as JSON:
        {
            "lessons": [
                {
                    "title": "Lesson Title",
                    "summary": "Brief description of what this lesson covers",
                    "order": 0,
                    "topics": [
                        {
                            "title": "Topic Title",
                            "summary": "Brief description of this topic",
                            "order": 0,
                            "conceptTerms": ["Concept 1", "Concept 2"]
                        }
                    ]
                }
            ]
        }
        
        Guidelines:
        - Create 3-8 lessons depending on the amount of content
        - Each lesson should have 2-5 topics
        - Each topic should have 2-8 concepts
        - Order concepts within topics from simple to complex
        - Every concept must appear in exactly one topic
        - First lesson should cover foundational concepts
        - Last lessons should cover advanced topics
        """;

    /// <summary>
    /// Event fired when generation progress changes.
    /// </summary>
    public event Action<CourseStructureProgress>? OnProgressChanged;

    public CourseStructureService(
        HttpClient http,
        OpenAIOptions opt,
        CourseStructureStorageService storageService,
        KnowledgeBaseStorageService kbStorageService)
    {
        this.http = http;
        this.opt = opt;
        this.storageService = storageService;
        this.kbStorageService = kbStorageService;
    }

    /// <summary>
    /// Generates a CourseStructure from a KnowledgeBase.
    /// </summary>
    public async Task<CourseStructure> GenerateFromKnowledgeBaseAsync(
        string courseId,
        string knowledgeBaseId,
        CancellationToken ct = default)
    {
        Log.Info($"CourseStructure: Generating from KB {knowledgeBaseId} for course {courseId}");
        
        // Load the KnowledgeBase
        var kb = await kbStorageService.LoadAsync(knowledgeBaseId, ct);
        if (kb == null)
        {
            Log.Error($"CourseStructure: KnowledgeBase not found: {knowledgeBaseId}");
            throw new InvalidOperationException($"KnowledgeBase not found: {knowledgeBaseId}");
        }

        if (kb.Status != KnowledgeBaseStatus.Ready)
        {
            Log.Error($"CourseStructure: KnowledgeBase not ready. Status: {kb.Status}");
            throw new InvalidOperationException($"KnowledgeBase is not ready. Status: {kb.Status}");
        }
        
        Log.Debug($"CourseStructure: KB '{kb.Name}' has {kb.Concepts.Count} concepts");

        // Create new CourseStructure
        var structure = new CourseStructure
        {
            Name = $"Learning Path for {kb.Name}",
            Description = $"Structured curriculum based on {kb.Name}",
            CourseId = courseId,
            KnowledgeBaseId = knowledgeBaseId,
            Status = CourseStructureStatus.NotStarted
        };

        try
        {
            // Step 1: Generate lessons using AI
            Log.Info("CourseStructure: Step 1 - Generating lesson structure...");
            ReportProgress(structure, CourseStructureStatus.GeneratingLessons, 20, 
                "Generating lesson structure...");
            structure.Status = CourseStructureStatus.GeneratingLessons;

            var lessonData = await GenerateLessonsAsync(kb, ct);
            
            // Step 2: Organize topics
            Log.Info("CourseStructure: Step 2 - Organizing topics...");
            ReportProgress(structure, CourseStructureStatus.OrganizingTopics, 50,
                "Organizing topics...");
            structure.Status = CourseStructureStatus.OrganizingTopics;

            // Step 3: Order concepts within topics
            Log.Info("CourseStructure: Step 3 - Ordering concepts...");
            ReportProgress(structure, CourseStructureStatus.OrderingConcepts, 70,
                "Ordering concepts...");
            structure.Status = CourseStructureStatus.OrderingConcepts;

            // Build the structure from AI response
            structure.Lessons = BuildLessonsFromResponse(lessonData, kb, structure.Id);

            // Validate that all concepts are included
            var includedConceptIds = structure.GetAllConceptIdsInOrder().ToHashSet();
            var missingConcepts = kb.Concepts.Where(c => !includedConceptIds.Contains(c.Id)).ToList();

            if (missingConcepts.Count > 0)
            {
                Log.Warn($"CourseStructure: {missingConcepts.Count} concepts not assigned to lessons, adding to 'Additional Topics'");
                // Add missing concepts to an "Additional Topics" lesson
                AddMissingConceptsLesson(structure, missingConcepts);
            }

            // Calculate estimated duration
            structure.TotalEstimatedMinutes = structure.TotalConceptReferences * 5; // 5 min per concept

            // Mark as ready
            structure.Status = CourseStructureStatus.Ready;
            structure.Progress = 100;
            structure.UpdatedAt = DateTime.UtcNow;
            structure.Version++;
            await storageService.SaveAsync(structure, ct);

            Log.Info($"CourseStructure: Generated {structure.TotalLessons} lessons, {structure.TotalTopics} topics");
            ReportProgress(structure, CourseStructureStatus.Ready, 100,
                $"Course structure ready with {structure.TotalLessons} lessons and {structure.TotalTopics} topics");

            return structure;
        }
        catch (Exception ex)
        {
            Log.Error($"CourseStructure: Generation failed - {ex.Message}", ex);
            structure.Status = CourseStructureStatus.Failed;
            structure.ErrorMessage = ex.Message;
            await storageService.SaveAsync(structure, ct);

            ReportProgress(structure, CourseStructureStatus.Failed, structure.Progress,
                "Generation failed", ex.Message);

            throw;
        }
    }

    /// <summary>
    /// Generates lesson organization using AI.
    /// </summary>
    private async Task<LessonGenerationResponse?> GenerateLessonsAsync(
        KnowledgeBase kb,
        CancellationToken ct)
    {
        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is missing.");

        // Build concept list with complexity info
        var conceptList = new StringBuilder();
        foreach (var concept in kb.GetConceptsByComplexity())
        {
            var complexity = kb.GetComplexity(concept.Id);
            var level = complexity?.Level ?? 0;
            conceptList.AppendLine($"- {concept.Title} (Level {level}): {concept.Summary}");
        }

        var prompt = LessonGenerationPrompt.Replace("{CONCEPTS}", conceptList.ToString());

        return await CallAiAsync<LessonGenerationResponse>(prompt, "", ct);
    }

    /// <summary>
    /// Builds Lesson objects from the AI response.
    /// </summary>
    private List<Lesson> BuildLessonsFromResponse(
        LessonGenerationResponse? response,
        KnowledgeBase kb,
        string structureId)
    {
        if (response?.Lessons == null || response.Lessons.Count == 0)
        {
            // Fallback: create a single lesson with all concepts
            return [CreateDefaultLesson(kb, structureId)];
        }

        var lessons = new List<Lesson>();
        var conceptLookup = kb.Concepts.ToDictionary(
            c => c.Title.ToLowerInvariant(),
            c => c.Id);

        foreach (var lessonDto in response.Lessons.OrderBy(l => l.Order))
        {
            var lesson = new Lesson
            {
                Title = lessonDto.Title ?? $"Lesson {lessonDto.Order + 1}",
                Summary = lessonDto.Summary ?? "",
                Order = lessonDto.Order,
                CourseStructureId = structureId
            };

            if (lessonDto.Topics != null)
            {
                foreach (var topicDto in lessonDto.Topics.OrderBy(t => t.Order))
                {
                    var topic = new LessonTopic
                    {
                        Title = topicDto.Title ?? $"Topic {topicDto.Order + 1}",
                        Summary = topicDto.Summary ?? "",
                        Order = topicDto.Order
                    };

                    // Map concept terms to IDs
                    if (topicDto.ConceptTerms != null)
                    {
                        foreach (var term in topicDto.ConceptTerms)
                        {
                            var key = term.ToLowerInvariant();
                            if (conceptLookup.TryGetValue(key, out var conceptId))
                            {
                                topic.ConceptIds.Add(conceptId);
                            }
                        }
                    }

                    if (topic.ConceptIds.Count > 0)
                        lesson.Topics.Add(topic);
                }
            }

            if (lesson.Topics.Count > 0)
                lessons.Add(lesson);
        }

        return lessons.OrderBy(l => l.Order).ToList();
    }

    /// <summary>
    /// Creates a default lesson containing all concepts (fallback).
    /// </summary>
    private Lesson CreateDefaultLesson(KnowledgeBase kb, string structureId)
    {
        var lesson = new Lesson
        {
            Title = "Course Content",
            Summary = "All course concepts",
            Order = 0,
            CourseStructureId = structureId
        };

        var topic = new LessonTopic
        {
            Title = "All Concepts",
            Summary = "Complete concept list",
            Order = 0
        };

        // Add concepts in complexity order
        foreach (var concept in kb.GetConceptsByComplexity())
        {
            topic.ConceptIds.Add(concept.Id);
        }

        lesson.Topics.Add(topic);
        return lesson;
    }

    /// <summary>
    /// Adds a lesson for concepts that were not included in the AI response.
    /// </summary>
    private void AddMissingConceptsLesson(CourseStructure structure, List<Concept> missingConcepts)
    {
        var lesson = new Lesson
        {
            Title = "Additional Topics",
            Summary = "Supplementary concepts",
            Order = structure.Lessons.Count,
            CourseStructureId = structure.Id
        };

        var topic = new LessonTopic
        {
            Title = "Other Concepts",
            Summary = "Additional concepts not covered in main lessons",
            Order = 0
        };

        foreach (var concept in missingConcepts)
        {
            topic.ConceptIds.Add(concept.Id);
        }

        lesson.Topics.Add(topic);
        structure.Lessons.Add(lesson);
    }

    /// <summary>
    /// Reorders concepts within the structure to respect prerequisites.
    /// Uses the KnowledgeBase's complexity ordering as a guide.
    /// </summary>
    public void ReorderByPrerequisites(CourseStructure structure, KnowledgeBase kb)
    {
        foreach (var lesson in structure.Lessons)
        {
            foreach (var topic in lesson.Topics)
            {
                // Sort concepts by their complexity level
                topic.ConceptIds = topic.ConceptIds
                    .OrderBy(id => kb.GetComplexity(id)?.Level ?? 0)
                    .ThenBy(id => kb.GetComplexity(id)?.PrerequisiteCount ?? 0)
                    .ToList();
            }

            // Sort topics by the average complexity of their concepts
            lesson.Topics = lesson.Topics
                .OrderBy(t => t.ConceptIds.Count > 0
                    ? t.ConceptIds.Average(id => kb.GetComplexity(id)?.Level ?? 0)
                    : 0)
                .Select((t, i) => { t.Order = i; return t; })
                .ToList();
        }

        // Sort lessons by the average complexity of their topics
        structure.Lessons = structure.Lessons
            .OrderBy(l => l.Topics.Count > 0
                ? l.Topics.Average(t => t.ConceptIds.Count > 0
                    ? t.ConceptIds.Average(id => kb.GetComplexity(id)?.Level ?? 0)
                    : 0)
                : 0)
            .Select((l, i) => { l.Order = i; return l; })
            .ToList();
    }

    // Helper methods

    private async Task<T?> CallAiAsync<T>(string systemPrompt, string content, CancellationToken ct)
    {
        var apiKey = await opt.GetApiKeyAsync();

        var input = string.IsNullOrWhiteSpace(content)
            ? "Please analyze and respond."
            : $"Content to analyze:\n\n{content}";

        var payload = new
        {
            model = opt.Model,
            input = input,
            instructions = systemPrompt
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var resp = await http.SendAsync(req, ct);
        var json = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI error {(int)resp.StatusCode}: {json}");
        }

        var responseText = ExtractResponseText(json);
        return ParseJsonResponse<T>(responseText);
    }

    private static string ExtractResponseText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("output_text", out var outText) &&
                outText.ValueKind == JsonValueKind.String)
            {
                return outText.GetString() ?? "";
            }

            if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0)
            {
                var first = choices[0];
                if (first.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var msgContent))
                {
                    return msgContent.GetString() ?? "";
                }
            }

            if (doc.RootElement.TryGetProperty("output", out var output) &&
                output.ValueKind == JsonValueKind.Array && output.GetArrayLength() > 0)
            {
                foreach (var item in output.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out var contentArr) &&
                        contentArr.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var c in contentArr.EnumerateArray())
                        {
                            if (c.TryGetProperty("text", out var txt))
                                return txt.GetString() ?? "";
                        }
                    }
                }
            }

            return json;
        }
        catch
        {
            return json;
        }
    }

    private static T? ParseJsonResponse<T>(string text)
    {
        var jsonStart = text.IndexOf('{');
        var jsonEnd = text.LastIndexOf('}');

        if (jsonStart >= 0 && jsonEnd > jsonStart)
        {
            var jsonText = text[jsonStart..(jsonEnd + 1)];
            try
            {
                return JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch { }
        }

        try
        {
            return JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return default;
        }
    }

    private void ReportProgress(
        CourseStructure structure,
        CourseStructureStatus status,
        int progress,
        string message,
        string? errorMessage = null)
    {
        structure.Progress = progress;

        OnProgressChanged?.Invoke(new CourseStructureProgress
        {
            Status = status,
            Progress = progress,
            Message = message,
            LessonsGenerated = structure.Lessons.Count,
            TopicsGenerated = structure.TotalTopics,
            ErrorMessage = errorMessage
        });
    }
}

/// <summary>
/// Progress information during course structure generation.
/// </summary>
public class CourseStructureProgress
{
    public CourseStructureStatus Status { get; set; }
    public int Progress { get; set; }
    public string Message { get; set; } = "";
    public int LessonsGenerated { get; set; }
    public int TopicsGenerated { get; set; }
    public string? ErrorMessage { get; set; }
}

// DTOs for AI responses

internal class LessonGenerationResponse
{
    public List<LessonDto>? Lessons { get; set; }
}

internal class LessonDto
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public int Order { get; set; }
    public List<TopicDto>? Topics { get; set; }
}

internal class TopicDto
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public int Order { get; set; }
    public List<string>? ConceptTerms { get; set; }
}
