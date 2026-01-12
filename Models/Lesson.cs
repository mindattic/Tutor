namespace Tutor.Models;

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
