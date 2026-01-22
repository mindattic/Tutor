namespace Tutor.Models;

/// <summary>
/// Represents a learning course that uses a ConceptMapCollection to teach students.
/// 
/// Key architectural points:
/// - Each Resource generates its own ConceptMap independently.
/// - A Course combines Resources' ConceptMaps into a ConceptMapCollection.
/// - The ConceptMapCollection provides unified access to all concepts.
/// - A Course has one CourseStructure (the curated learning path).
/// - The Course itself stores metadata and references.
/// 
/// Relationship flow:
/// 1. Resources are uploaded to a Course.
/// 2. Each Resource generates its own ConceptMap.
/// 3. A ConceptMapCollection is built from Resources' ConceptMaps.
/// 4. A CourseStructure is generated from the ConceptMapCollection.
/// 5. Students navigate the CourseStructure (Lessons/Topics) which references
///    Concepts from any ConceptMap in the collection.
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
    /// These are the raw source materials used to generate individual ConceptMaps.
    /// </summary>
    public List<string> ResourceIds { get; set; } = [];

    /// <summary>
    /// ID of the ConceptMapCollection for this course.
    /// The collection aggregates ConceptMaps from each Resource.
    /// Null if no collection has been built yet.
    /// </summary>
    public string? ConceptMapCollectionId { get; set; }

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
    /// Whether the course has a ConceptMapCollection built.
    /// </summary>
    public bool HasConceptMapCollection => !string.IsNullOrEmpty(ConceptMapCollectionId);

    /// <summary>
    /// Whether the course has a CourseStructure generated.
    /// </summary>
    public bool HasCourseStructure => !string.IsNullOrEmpty(CourseStructureId);

    /// <summary>
    /// Whether the course is fully ready for learning (has both collection and structure).
    /// </summary>
    public bool IsReadyForLearning => HasConceptMapCollection && HasCourseStructure;

    // Legacy properties for backward compatibility

    /// <summary>
    /// ID of the KnowledgeBaseCollection for this course.
    /// </summary>
    [Obsolete("Use ConceptMapCollectionId instead. This property is maintained for backward compatibility.")]
    public string? KnowledgeBaseCollectionId 
    { 
        get => ConceptMapCollectionId; 
        set => ConceptMapCollectionId = value; 
    }

    /// <summary>
    /// ID of the single KnowledgeBase assigned to this course.
    /// </summary>
    [Obsolete("Use ConceptMapCollectionId instead. This property is maintained for backward compatibility.")]
    public string? KnowledgeBaseId { get; set; }

    /// <summary>
    /// Whether the course has a KnowledgeBaseCollection.
    /// </summary>
    [Obsolete("Use HasConceptMapCollection instead. This property is maintained for backward compatibility.")]
    public bool HasKnowledgeBase => HasConceptMapCollection || !string.IsNullOrEmpty(KnowledgeBaseId);
}
