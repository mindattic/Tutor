namespace Tutor.Core.Models;

/// <summary>
/// Represents a lesson in a Course's learning path.
/// A Lesson is a major section of the curriculum that groups related Topics.
/// 
/// Key distinction:
/// - Lessons belong to the Course structure (the curated learning path)
/// - Lessons reference Concepts from the KnowledgeBase by ID (no duplication)
/// - The hierarchy is: Course -> Lessons -> Topics -> Concepts
/// </summary>
public class Lesson
{
    /// <summary>
    /// Unique identifier for this lesson.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The title of this lesson.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// A brief summary of what this lesson covers.
    /// </summary>
    public string Summary { get; set; } = "";

    /// <summary>
    /// The order of this lesson in the course (0-based).
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// The ID of the CourseStructure this lesson belongs to.
    /// </summary>
    public string CourseStructureId { get; set; } = "";

    /// <summary>
    /// The topics contained in this lesson, in learning order.
    /// </summary>
    public List<LessonTopic> Topics { get; set; } = [];

    /// <summary>
    /// Optional icon for display in navigation (Bootstrap Icons class name).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Optional tags for categorization.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Estimated duration in minutes.
    /// </summary>
    public int EstimatedMinutes { get; set; }

    /// <summary>
    /// When this lesson was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this lesson was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the total number of topics in this lesson.
    /// </summary>
    public int TopicCount => Topics.Count;

    /// <summary>
    /// Gets the total number of concepts across all topics.
    /// </summary>
    public int TotalConceptCount => Topics.Sum(t => t.ConceptIds.Count);

    /// <summary>
    /// Gets all concept IDs in this lesson in order.
    /// </summary>
    public IEnumerable<string> GetAllConceptIds()
    {
        return Topics
            .OrderBy(t => t.Order)
            .SelectMany(t => t.ConceptIds);
    }

    /// <summary>
    /// Hierarchical sections within this lesson/chapter.
    /// Sections provide a multi-level TOC structure.
    /// </summary>
    public List<Section> Sections { get; set; } = [];

    /// <summary>
    /// Gets total count of all sections (including nested).
    /// </summary>
    public int GetTotalSectionCount()
    {
        return Sections.Sum(s => 1 + CountNestedSections(s));
    }

    private static int CountNestedSections(Section section)
    {
        return section.Children.Sum(c => 1 + CountNestedSections(c));
    }

    /// <summary>
    /// Finds a section by ID within this lesson.
    /// </summary>
    public Section? FindSection(string sectionId)
    {
        foreach (var section in Sections)
        {
            var found = section.FindSection(sectionId);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// Gets all sections flattened in order.
    /// </summary>
    public IEnumerable<Section> GetAllSectionsFlattened()
    {
        foreach (var section in Sections.OrderBy(s => s.Order))
        {
            yield return section;
            foreach (var nested in GetNestedSectionsFlattened(section))
            {
                yield return nested;
            }
        }
    }

    private static IEnumerable<Section> GetNestedSectionsFlattened(Section section)
    {
        foreach (var child in section.Children.OrderBy(c => c.Order))
        {
            yield return child;
            foreach (var nested in GetNestedSectionsFlattened(child))
            {
                yield return nested;
            }
        }
    }

    /// <summary>
    /// Generates section numbers for the TOC (1a, 1b, 1a-i, etc.)
    /// </summary>
    public void GenerateSectionNumbers(int lessonNumber)
    {
        var letters = "abcdefghijklmnopqrstuvwxyz";
        var romanNumerals = new[] { "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix", "x" };
        
        for (int i = 0; i < Sections.Count; i++)
        {
            var section = Sections[i];
            section.Number = $"{lessonNumber}{letters[i % 26]}";
            section.Depth = 0;
            GenerateChildNumbers(section, romanNumerals);
        }
    }

    private static void GenerateChildNumbers(Section parent, string[] romanNumerals)
    {
        var greekLetters = "αβγδεζηθικλμνξοπρστυφχψω";
        
        for (int i = 0; i < parent.Children.Count; i++)
        {
            var child = parent.Children[i];
            child.Depth = parent.Depth + 1;
            
            child.Number = child.Depth switch
            {
                1 => $"{parent.Number}-{romanNumerals[i % romanNumerals.Length]}",
                2 => $"{parent.Number}-{greekLetters[i % greekLetters.Length]}",
                _ => $"{parent.Number}.{i + 1}"
            };
            
            GenerateChildNumbers(child, romanNumerals);
        }
    }
}

/// <summary>
/// Represents a topic's placement within a lesson.
/// Topics group related concepts and reference them by ID from the KnowledgeBase.
/// </summary>
public class LessonTopic
{
    /// <summary>
    /// Unique identifier for this topic instance within the lesson.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The title of this topic.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// A brief summary of what this topic covers.
    /// </summary>
    public string Summary { get; set; } = "";

    /// <summary>
    /// The order of this topic within the lesson (0-based).
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// IDs of Concepts from the KnowledgeBase, in learning order.
    /// These reference existing Concepts - no duplication of data.
    /// </summary>
    public List<string> ConceptIds { get; set; } = [];

    /// <summary>
    /// Optional icon for display in navigation.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Optional tags for categorization.
    /// </summary>
    public List<string> Tags { get; set; } = [];
}
