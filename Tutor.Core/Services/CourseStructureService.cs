using System.Text;
using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

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
    private readonly LlmServiceRouter router;
    private readonly CourseStructureStorageService storageService;
    private readonly ConceptMapStorageService conceptMapStorageService;
    private readonly SectionContentService? sectionContentService;

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
        LlmServiceRouter router,
        CourseStructureStorageService storageService,
        ConceptMapStorageService conceptMapStorageService,
        SectionContentService? sectionContentService = null)
    {
        this.router = router;
        this.storageService = storageService;
        this.conceptMapStorageService = conceptMapStorageService;
        this.sectionContentService = sectionContentService;
    }

    /// <summary>
    /// Generates a CourseStructure from a ConceptMap.
    /// </summary>
    public async Task<CourseStructure> GenerateFromConceptMapAsync(
        string courseId,
        string conceptMapId,
        CancellationToken ct = default)
    {
        Log.Info($"CourseStructure: Generating from ConceptMap {conceptMapId} for course {courseId}");
        
        // Load the ConceptMap
        var conceptMap = await conceptMapStorageService.LoadAsync(conceptMapId, ct);
        if (conceptMap == null)
        {
            Log.Error($"CourseStructure: ConceptMap not found: {conceptMapId}");
            throw new InvalidOperationException($"ConceptMap not found: {conceptMapId}");
        }

        if (conceptMap.Status != ConceptMapStatus.Ready)
        {
            Log.Error($"CourseStructure: ConceptMap not ready. Status: {conceptMap.Status}");
            throw new InvalidOperationException($"ConceptMap is not ready. Status: {conceptMap.Status}");
        }
        
        Log.Debug($"CourseStructure: ConceptMap '{conceptMap.Name}' has {conceptMap.Concepts.Count} concepts");

        // Create new CourseStructure
        var structure = new CourseStructure
        {
            Name = $"Learning Path for {conceptMap.Name}",
            Description = $"Structured curriculum based on {conceptMap.Name}",
            CourseId = courseId,
            KnowledgeBaseId = conceptMapId,
            Status = CourseStructureStatus.NotStarted
        };

        try
        {
            // Step 1: Generate lessons using AI
            Log.Info("CourseStructure: Step 1 - Generating lesson structure...");
            ReportProgress(structure, CourseStructureStatus.GeneratingLessons, 20, 
                "Generating lesson structure...");
            structure.Status = CourseStructureStatus.GeneratingLessons;


            var lessonData = await GenerateLessonsAsync(conceptMap, ct);
            
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
            structure.Lessons = BuildLessonsFromResponse(lessonData, conceptMap, structure.Id);

            // Validate that all concepts are included
            var includedConceptIds = structure.GetAllConceptIdsInOrder().ToHashSet();
            var missingConcepts = conceptMap.Concepts.Where(c => !includedConceptIds.Contains(c.Id)).ToList();

            if (missingConcepts.Count > 0)
            {
                Log.Warn($"CourseStructure: {missingConcepts.Count} concepts not assigned to lessons, adding to 'Additional Topics'");
                // Add missing concepts to an "Additional Topics" lesson
                AddMissingConceptsLesson(structure, missingConcepts);
            }

            // Step 4: Generate hierarchical sections for each lesson
            if (sectionContentService != null)
            {
                Log.Info("CourseStructure: Step 4 - Generating hierarchical sections...");
                ReportProgress(structure, CourseStructureStatus.GeneratingSections, 75,
                    "Generating hierarchical sections...");
                structure.Status = CourseStructureStatus.GeneratingSections;

                foreach (var lesson in structure.Lessons)
                {
                    var sections = await sectionContentService.GenerateSectionsForLessonAsync(lesson, conceptMap, ct);
                    lesson.Sections = sections;
                    
                    ReportProgress(structure, CourseStructureStatus.GeneratingSections, 
                        75 + (structure.Lessons.IndexOf(lesson) * 10 / structure.Lessons.Count),
                        $"Generated sections for: {lesson.Title}");
                }

                // Step 5: Generate content for sections
                Log.Info("CourseStructure: Step 5 - Generating section content...");
                ReportProgress(structure, CourseStructureStatus.GeneratingContent, 85,
                    "Generating section content...");
                structure.Status = CourseStructureStatus.GeneratingContent;

                foreach (var lesson in structure.Lessons)
                {
                    await sectionContentService.GenerateAllSectionContentAsync(lesson, conceptMap, ct);
                    
                    ReportProgress(structure, CourseStructureStatus.GeneratingContent,
                        85 + (structure.Lessons.IndexOf(lesson) * 10 / structure.Lessons.Count),
                        $"Generated content for: {lesson.Title}");
                }
            }

            // Calculate estimated duration
            structure.TotalEstimatedMinutes = CalculateTotalEstimatedMinutes(structure);

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
        ConceptMap conceptMap,
        CancellationToken ct)
    {
        // Build concept list with complexity info
        var conceptList = new StringBuilder();
        foreach (var concept in conceptMap.GetConceptsByComplexity())
        {
            var complexity = conceptMap.GetComplexity(concept.Id);
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
        ConceptMap conceptMap,
        string structureId)
    {
        if (response?.Lessons == null || response.Lessons.Count == 0)
        {
            // Fallback: create a single lesson with all concepts
            return [CreateDefaultLesson(conceptMap, structureId)];
        }

        var lessons = new List<Lesson>();
        var conceptLookup = conceptMap.Concepts.ToDictionary(
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
    private Lesson CreateDefaultLesson(ConceptMap conceptMap, string structureId)
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
        foreach (var concept in conceptMap.GetConceptsByComplexity())
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
    /// Uses the ConceptMap's complexity ordering as a guide.
    /// </summary>
    public void ReorderByPrerequisites(CourseStructure structure, ConceptMap conceptMap)
    {
        foreach (var lesson in structure.Lessons)
        {
            foreach (var topic in lesson.Topics)
            {
                // Sort concepts by their complexity level
                topic.ConceptIds = topic.ConceptIds
                    .OrderBy(id => conceptMap.GetComplexity(id)?.Level ?? 0)
                    .ThenBy(id => conceptMap.GetComplexity(id)?.PrerequisiteCount ?? 0)
                    .ToList();
            }

            // Sort topics by the average complexity of their concepts
            lesson.Topics = lesson.Topics
                .OrderBy(t => t.ConceptIds.Count > 0
                    ? t.ConceptIds.Average(id => conceptMap.GetComplexity(id)?.Level ?? 0)
                    : 0)
                .Select((t, i) => { t.Order = i; return t; })
                .ToList();
        }

        // Sort lessons by the average complexity of their topics
        structure.Lessons = structure.Lessons
            .OrderBy(l => l.Topics.Count > 0
                ? l.Topics.Average(t => t.ConceptIds.Count > 0
                    ? t.ConceptIds.Average(id => conceptMap.GetComplexity(id)?.Level ?? 0)
                    : 0)
                : 0)
            .Select((l, i) => { l.Order = i; return l; })
            .ToList();
    }

    // Helper methods


    private async Task<T?> CallAiAsync<T>(string systemPrompt, string content, CancellationToken ct)
    {
        var input = string.IsNullOrWhiteSpace(content)
            ? "Please analyze and respond."
            : $"Content to analyze:\n\n{content}";

        var messages = new[] { new ChatMessage("user", input, input) };
        var reply = await router.GetReplyAsync(messages, systemPrompt, ct);

        var responseText = reply.Text;
        return ParseJsonResponse<T>(responseText);
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
            SectionsGenerated = structure.Lessons.Sum(l => l.GetTotalSectionCount()),
            ErrorMessage = errorMessage
        });
    }

    /// <summary>
    /// Calculates total estimated minutes based on section reading times.
    /// </summary>
    private static int CalculateTotalEstimatedMinutes(CourseStructure structure)
    {
        var total = 0;
        foreach (var lesson in structure.Lessons)
        {
            foreach (var section in lesson.GetAllSectionsFlattened())
            {
                total += section.EstimatedReadingMinutes;
                if (section.HasQuiz)
                    total += 5; // Add 5 minutes for quiz
            }
            
            // Fallback: if no sections, estimate from topics
            if (lesson.Sections.Count == 0)
            {
                total += lesson.TotalConceptCount * 3; // 3 min per concept
            }
        }
        return total > 0 ? total : structure.TotalConceptReferences * 5;
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
    public int SectionsGenerated { get; set; }
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
