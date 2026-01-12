namespace Tutor.Models;

/// <summary>
/// Represents a learning course that uses a KnowledgeBase to teach students.
/// 
/// Key architectural points:
/// - A Course has one assigned KnowledgeBase (the totality of knowledge to teach).
/// - A Course has one CourseStructure (the curated learning path).
/// - The Course itself stores metadata and references; the actual content is in the KnowledgeBase.
/// - Resources are associated with the Course but used to build the KnowledgeBase.
/// 
/// Relationship flow:
/// 1. Resources are uploaded to a Course.
/// 2. Resources are combined to generate a KnowledgeBase.
/// 3. A CourseStructure is generated from the KnowledgeBase.
/// 4. Students navigate the CourseStructure (Lessons/Topics) which references
///    Concepts from the KnowledgeBase.
/// </summary>
public class Course
{
    /// <summary>
    /// Unique identifier for this course.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The name of the course.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// A description of what this course covers.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// IDs of Resources that belong to this course.
    /// These are the raw source materials used to generate the KnowledgeBase.
    /// </summary>
    public List<string> ResourceIds { get; set; } = [];

    /// <summary>
    /// ID of the KnowledgeBase assigned to this course.
    /// The KnowledgeBase contains all Concepts and their relationships.
    /// Null if no KnowledgeBase has been built/assigned yet.
    /// </summary>
    public string? KnowledgeBaseId { get; set; }

    /// <summary>
    /// ID of the CourseStructure for this course.
    /// The CourseStructure contains the learning path (Lessons/Topics).
    /// Null if the structure has not been generated yet.
    /// </summary>
    public string? CourseStructureId { get; set; }

    /// <summary>
    /// Optional icon for display (Bootstrap Icons class name).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Optional tags for categorization.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Estimated total duration of the course in minutes.
    /// </summary>
    public int EstimatedMinutes { get; set; }

    /// <summary>
    /// When this course was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this course was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the course has a KnowledgeBase assigned.
    /// </summary>
    public bool HasKnowledgeBase => !string.IsNullOrEmpty(KnowledgeBaseId);

    /// <summary>
    /// Whether the course has a CourseStructure generated.
    /// </summary>
    public bool HasCourseStructure => !string.IsNullOrEmpty(CourseStructureId);

    /// <summary>
    /// Whether the course is fully ready for learning (has both KB and structure).
    /// </summary>
    public bool IsReadyForLearning => HasKnowledgeBase && HasCourseStructure;
}
