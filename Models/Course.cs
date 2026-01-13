namespace Tutor.Models;

/// <summary>
/// Represents a learning course that uses a KnowledgeBaseCollection to teach students.
/// 
/// Key architectural points:
/// - Each Resource generates its own KnowledgeBase independently.
/// - A Course combines Resources' KnowledgeBases into a KnowledgeBaseCollection.
/// - The KnowledgeBaseCollection provides unified access to all concepts.
/// - A Course has one CourseStructure (the curated learning path).
/// - The Course itself stores metadata and references.
/// 
/// Relationship flow:
/// 1. Resources are uploaded to a Course.
/// 2. Each Resource generates its own KnowledgeBase.
/// 3. A KnowledgeBaseCollection is built from Resources' KnowledgeBases.
/// 4. A CourseStructure is generated from the KnowledgeBaseCollection.
/// 5. Students navigate the CourseStructure (Lessons/Topics) which references
///    Concepts from any KnowledgeBase in the collection.
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
    /// These are the raw source materials used to generate individual KnowledgeBases.
    /// </summary>
    public List<string> ResourceIds { get; set; } = [];

    /// <summary>
    /// ID of the KnowledgeBaseCollection for this course.
    /// The collection aggregates KnowledgeBases from each Resource.
    /// Null if no collection has been built yet.
    /// </summary>
    public string? KnowledgeBaseCollectionId { get; set; }

    /// <summary>
    /// ID of the single KnowledgeBase assigned to this course.
    /// Maintained for backward compatibility during migration.
    /// New code should use KnowledgeBaseCollectionId instead.
    /// </summary>
    [Obsolete("Use KnowledgeBaseCollectionId instead. This property is maintained for backward compatibility.")]
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
    /// Whether the course has a KnowledgeBaseCollection (or legacy KnowledgeBase).
    /// </summary>
    public bool HasKnowledgeBase => !string.IsNullOrEmpty(KnowledgeBaseCollectionId) 
        || !string.IsNullOrEmpty(KnowledgeBaseId);

    /// <summary>
    /// Whether the course has a CourseStructure generated.
    /// </summary>
    public bool HasCourseStructure => !string.IsNullOrEmpty(CourseStructureId);

    /// <summary>
    /// Whether the course is fully ready for learning (has both KB and structure).
    /// </summary>
    public bool IsReadyForLearning => HasKnowledgeBase && HasCourseStructure;
}
