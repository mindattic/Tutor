using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Models;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Service for generating hierarchical section content from concepts.
/// Creates encyclopedia-style content for course sections with explanations,
/// examples, and learning objectives.
/// </summary>
public sealed class SectionContentService
{
    private readonly HttpClient http;
    private readonly OpenAIOptions opt;
    private readonly ConceptMapStorageService conceptMapStorageService;

    private const string SectionContentPrompt = """
        You are an expert educational content writer. Given a list of concepts for a section,
        create comprehensive, encyclopedia-style educational content.
        
        Section: {SECTION_TITLE}
        Section Summary: {SECTION_SUMMARY}
        Parent Context: {PARENT_CONTEXT}
        
        Concepts to cover:
        {CONCEPTS}
        
        Create educational content that:
        1. Introduces the topic and its importance
        2. Explains each concept clearly with examples
        3. Shows relationships between concepts
        4. Uses analogies for complex ideas
        5. Includes practical applications where relevant
        
        Return as JSON:
        {
            "content": "The full markdown-formatted educational content (2-4 paragraphs)",
            "learningObjectives": ["objective 1", "objective 2"],
            "keyTerms": ["term1", "term2"],
            "estimatedReadingMinutes": 3
        }
        
        Guidelines:
        - Write in clear, accessible language
        - Use markdown formatting (headers, bold, lists)
        - Aim for 200-400 words for the content
        - Include 2-4 learning objectives
        - List all key terms introduced
        """;

    private const string HierarchicalSectionPrompt = """
        You are an expert curriculum designer. Given a chapter topic and its concepts,
        create a hierarchical section structure like an encyclopedia.
        
        Chapter: {CHAPTER_TITLE}
        Chapter Summary: {CHAPTER_SUMMARY}
        
        Concepts to organize:
        {CONCEPTS}
        
        Create a hierarchical structure with:
        - Sections (major topics within the chapter)
        - Subsections (detailed breakdowns within sections)
        - Sub-subsections where needed for complex topics (max depth 3)
        
        Each section should group related concepts and build logically.
        
        Return as JSON:
        {
            "sections": [
                {
                    "title": "Section Title",
                    "summary": "Brief description",
                    "conceptTerms": ["Concept 1", "Concept 2"],
                    "hasQuiz": true,
                    "children": [
                        {
                            "title": "Subsection Title",
                            "summary": "Brief description",
                            "conceptTerms": ["Concept 3"],
                            "hasQuiz": false,
                            "children": []
                        }
                    ]
                }
            ]
        }
        
        Guidelines:
        - Create 2-5 top-level sections per chapter
        - Each section can have 0-4 subsections
        - Subsections can have 0-3 sub-subsections
        - Place quizzes at meaningful milestones (end of major sections)
        - Every concept must appear in exactly one section/subsection
        - Order from foundational to advanced within each level
        - Keep titles concise but descriptive
        """;

    /// <summary>
    /// Event fired when content generation progress changes.
    /// </summary>
    public event Action<SectionContentProgress>? OnProgressChanged;

    public SectionContentService(
        HttpClient http,
        OpenAIOptions opt,
        ConceptMapStorageService conceptMapStorageService)
    {
        this.http = http;
        this.opt = opt;
        this.conceptMapStorageService = conceptMapStorageService;
    }

    /// <summary>
    /// Generates hierarchical sections for a lesson (chapter) from its topics and concepts.
    /// </summary>
    public async Task<List<Section>> GenerateSectionsForLessonAsync(
        Lesson lesson,
        ConceptMap conceptMap,
        CancellationToken ct = default)
    {
        Log.Info($"SectionContent: Generating sections for lesson '{lesson.Title}'");

        // Gather all concepts for this lesson
        var conceptList = new StringBuilder();
        var conceptLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var topic in lesson.Topics)
        {
            foreach (var conceptId in topic.ConceptIds)
            {
                var concept = conceptMap.GetConcept(conceptId);
                if (concept != null)
                {
                    conceptList.AppendLine($"- {concept.Title}: {concept.Summary}");
                    conceptLookup[concept.Title.ToLowerInvariant()] = conceptId;
                }
            }
        }

        if (conceptList.Length == 0)
        {
            Log.Warn($"SectionContent: No concepts found for lesson '{lesson.Title}'");
            return [];
        }

        var prompt = HierarchicalSectionPrompt
            .Replace("{CHAPTER_TITLE}", lesson.Title)
            .Replace("{CHAPTER_SUMMARY}", lesson.Summary)
            .Replace("{CONCEPTS}", conceptList.ToString());

        var response = await CallAiAsync<HierarchicalSectionResponse>(prompt, "", ct);
        
        if (response?.Sections == null || response.Sections.Count == 0)
        {
            Log.Warn($"SectionContent: AI returned no sections for lesson '{lesson.Title}'");
            return CreateDefaultSections(lesson, conceptMap, conceptLookup);
        }

        return BuildSectionsFromResponse(response.Sections, lesson.Id, conceptLookup, 0, null);
    }

    /// <summary>
    /// Generates content for a single section based on its concepts.
    /// </summary>
    public async Task<SectionContentResult> GenerateSectionContentAsync(
        Section section,
        ConceptMap conceptMap,
        string? parentContext = null,
        CancellationToken ct = default)
    {
        Log.Debug($"SectionContent: Generating content for section '{section.Title}'");

        var conceptList = new StringBuilder();
        foreach (var conceptId in section.ConceptIds)
        {
            var concept = conceptMap.GetConcept(conceptId);
            if (concept != null)
            {
                conceptList.AppendLine($"- {concept.Title}: {concept.Summary}");
                if (!string.IsNullOrEmpty(concept.Content))
                {
                    conceptList.AppendLine($"  Details: {concept.Content}");
                }
            }
        }

        var prompt = SectionContentPrompt
            .Replace("{SECTION_TITLE}", section.Title)
            .Replace("{SECTION_SUMMARY}", section.Summary ?? "")
            .Replace("{PARENT_CONTEXT}", parentContext ?? "Top-level section")
            .Replace("{CONCEPTS}", conceptList.ToString());

        var response = await CallAiAsync<SectionContentResponse>(prompt, "", ct);

        return new SectionContentResult
        {
            Content = response?.Content ?? GenerateFallbackContent(section, conceptMap),
            LearningObjectives = response?.LearningObjectives ?? [],
            KeyTerms = response?.KeyTerms ?? [],
            EstimatedReadingMinutes = response?.EstimatedReadingMinutes ?? 2
        };
    }

    /// <summary>
    /// Generates content for all sections in a lesson hierarchy.
    /// </summary>
    public async Task GenerateAllSectionContentAsync(
        Lesson lesson,
        ConceptMap conceptMap,
        CancellationToken ct = default)
    {
        Log.Info($"SectionContent: Generating content for all sections in '{lesson.Title}'");
        
        var allSections = lesson.GetAllSectionsFlattened().ToList();
        var total = allSections.Count;
        var current = 0;

        foreach (var section in allSections)
        {
            ct.ThrowIfCancellationRequested();
            
            current++;
            ReportProgress(current, total, $"Generating content for: {section.Title}");
            
            // Skip if content already exists
            if (!string.IsNullOrEmpty(section.Content))
                continue;

            // Get parent context for better content generation
            var parentContext = GetParentContext(lesson, section);
            
            var result = await GenerateSectionContentAsync(section, conceptMap, parentContext, ct);
            
            section.Content = result.Content;
            section.LearningObjectives = result.LearningObjectives;
            section.KeyTerms = result.KeyTerms;
            section.EstimatedReadingMinutes = result.EstimatedReadingMinutes;
            
            // Small delay to avoid rate limiting
            await Task.Delay(500, ct);
        }

        ReportProgress(total, total, "Content generation complete");
    }

    private string GetParentContext(Lesson lesson, Section section)
    {
        if (string.IsNullOrEmpty(section.ParentSectionId))
            return $"Chapter: {lesson.Title}";

        var parent = lesson.FindSection(section.ParentSectionId);
        return parent != null
            ? $"Parent section: {parent.Title} - {parent.Summary}"
            : $"Chapter: {lesson.Title}";
    }

    private List<Section> BuildSectionsFromResponse(
        List<SectionDto> dtos,
        string lessonId,
        Dictionary<string, string> conceptLookup,
        int depth,
        string? parentSectionId)
    {
        var sections = new List<Section>();
        var order = 0;

        foreach (var dto in dtos)
        {
            var section = new Section
            {
                Title = dto.Title ?? $"Section {order + 1}",
                Summary = dto.Summary,
                Order = order,
                Depth = depth,
                LessonId = lessonId,
                ParentSectionId = parentSectionId,
                HasQuiz = dto.HasQuiz,
                Number = GenerateSectionNumber(depth, order)
            };

            // Map concept terms to IDs
            if (dto.ConceptTerms != null)
            {
                foreach (var term in dto.ConceptTerms)
                {
                    var key = term.ToLowerInvariant();
                    if (conceptLookup.TryGetValue(key, out var conceptId))
                    {
                        section.ConceptIds.Add(conceptId);
                    }
                }
            }

            // Recursively build children
            if (dto.Children != null && dto.Children.Count > 0 && depth < 3)
            {
                section.Children = BuildSectionsFromResponse(
                    dto.Children, lessonId, conceptLookup, depth + 1, section.Id);
            }

            sections.Add(section);
            order++;
        }

        return sections;
    }

    private static string GenerateSectionNumber(int depth, int order)
    {
        return depth switch
        {
            0 => ((char)('A' + order)).ToString(),
            1 => $"{order + 1}",
            2 => ToRomanNumeral(order + 1).ToLower(),
            _ => $"({(char)('a' + order)})"
        };
    }

    private static string ToRomanNumeral(int number)
    {
        if (number < 1) return "";
        if (number >= 10) return "x" + ToRomanNumeral(number - 10);
        if (number >= 9) return "ix" + ToRomanNumeral(number - 9);
        if (number >= 5) return "v" + ToRomanNumeral(number - 5);
        if (number >= 4) return "iv" + ToRomanNumeral(number - 4);
        return "i" + ToRomanNumeral(number - 1);
    }

    private List<Section> CreateDefaultSections(
        Lesson lesson,
        ConceptMap conceptMap,
        Dictionary<string, string> conceptLookup)
    {
        // Fallback: create one section per topic
        var sections = new List<Section>();
        var order = 0;

        foreach (var topic in lesson.Topics)
        {
            var section = new Section
            {
                Title = topic.Title,
                Summary = topic.Summary,
                Order = order,
                Depth = 0,
                LessonId = lesson.Id,
                HasQuiz = order == lesson.Topics.Count - 1, // Quiz at end of chapter
                Number = ((char)('A' + order)).ToString()
            };

            foreach (var conceptId in topic.ConceptIds)
            {
                section.ConceptIds.Add(conceptId);
            }

            sections.Add(section);
            order++;
        }

        return sections;
    }

    private string GenerateFallbackContent(Section section, ConceptMap conceptMap)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"## {section.Title}");
        sb.AppendLine();
        
        if (!string.IsNullOrEmpty(section.Summary))
        {
            sb.AppendLine(section.Summary);
            sb.AppendLine();
        }

        foreach (var conceptId in section.ConceptIds)
        {
            var concept = conceptMap.GetConcept(conceptId);
            if (concept != null)
            {
                sb.AppendLine($"### {concept.Title}");
                sb.AppendLine(concept.Summary);
                if (!string.IsNullOrEmpty(concept.Content))
                {
                    sb.AppendLine();
                    sb.AppendLine(concept.Content);
                }
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private async Task<T?> CallAiAsync<T>(string systemPrompt, string content, CancellationToken ct)
    {
        var apiKey = await opt.GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is missing.");

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
            Log.Error($"SectionContent: OpenAI error {(int)resp.StatusCode}: {json}");
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

    private void ReportProgress(int current, int total, string message)
    {
        var pct = total > 0 ? (int)((double)current / total * 100) : 0;
        OnProgressChanged?.Invoke(new SectionContentProgress
        {
            Current = current,
            Total = total,
            Percentage = pct,
            Message = message
        });
    }
}

/// <summary>
/// Progress information during section content generation.
/// </summary>
public class SectionContentProgress
{
    public int Current { get; set; }
    public int Total { get; set; }
    public int Percentage { get; set; }
    public string Message { get; set; } = "";
}

/// <summary>
/// Result of content generation for a single section.
/// </summary>
public class SectionContentResult
{
    public string Content { get; set; } = "";
    public List<string> LearningObjectives { get; set; } = [];
    public List<string> KeyTerms { get; set; } = [];
    public int EstimatedReadingMinutes { get; set; }
}

// DTOs for AI responses

internal class HierarchicalSectionResponse
{
    public List<SectionDto>? Sections { get; set; }
}

internal class SectionDto
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public List<string>? ConceptTerms { get; set; }
    public bool HasQuiz { get; set; }
    public List<SectionDto>? Children { get; set; }
}

internal class SectionContentResponse
{
    public string? Content { get; set; }
    public List<string>? LearningObjectives { get; set; }
    public List<string>? KeyTerms { get; set; }
    public int EstimatedReadingMinutes { get; set; }
}
