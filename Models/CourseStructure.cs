namespace Tutor.Models;

/// <summary>
/// Status of the course structure generation process.
/// </summary>
public enum CourseStructureStatus
{
    /// <summary>
    /// Structure has not been generated yet.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Currently generating lessons from the knowledge base.
    /// </summary>
    GeneratingLessons,

    /// <summary>
    /// Currently organizing topics within lessons.
    /// </summary>
    OrganizingTopics,

    /// <summary>
    /// Currently ordering concepts within topics.
    /// </summary>
    OrderingConcepts,

    /// <summary>
    /// Currently generating hierarchical sections within lessons.
    /// </summary>
    GeneratingSections,

    /// <summary>
    /// Currently generating content for sections.
    /// </summary>
    GeneratingContent,

    /// <summary>
    /// Structure is ready for use.
    /// </summary>
    Ready,

    /// <summary>
    /// Structure generation failed.
    /// </summary>
    Failed
}

/// <summary>
/// Represents a curated learning path for a Course.
/// 
/// Key architectural points:
/// - CourseStructure defines the hierarchical learning path: Lessons -> Topics -> Concepts
/// - It REFERENCES Concepts from the KnowledgeBase by ID (no duplication of data)
/// - A Course has one CourseStructure and one assigned KnowledgeBase
/// - The CourseStructure is generated FROM the KnowledgeBase but is a separate entity
/// - Multiple Courses can share the same KnowledgeBase but have different structures
/// 
/// The goal is for the student to eventually understand all Concepts in the KnowledgeBase,
/// but in an ordered and guided way defined by the CourseStructure.
/// </summary>
public class CourseStructure
{
    /// <summary>
    /// Unique identifier for this course structure.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The name of this course structure.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// A description of this learning path.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// ID of the Course this structure belongs to.
    /// </summary>
    public string CourseId { get; set; } = "";

    /// <summary>
    /// ID of the KnowledgeBase this structure references.
    /// All ConceptIds in this structure point to Concepts in this KnowledgeBase.
    /// </summary>
    public string KnowledgeBaseId { get; set; } = "";

    /// <summary>
    /// Current status of the structure generation.
    /// </summary>
    public CourseStructureStatus Status { get; set; } = CourseStructureStatus.NotStarted;

    /// <summary>
    /// Error message if status is Failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Progress percentage (0-100) during generation.
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// The lessons in this course structure, in learning order.
    /// </summary>
    public List<Lesson> Lessons { get; set; } = [];

    /// <summary>
    /// Total estimated duration for the course in minutes.
    /// </summary>
    public int TotalEstimatedMinutes { get; set; }

    /// <summary>
    /// When this structure was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this structure was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Version number for tracking changes.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets the total number of lessons.
    /// </summary>
    public int TotalLessons => Lessons.Count;

    /// <summary>
    /// Gets the total number of topics across all lessons.
    /// </summary>
    public int TotalTopics => Lessons.Sum(l => l.Topics.Count);

    /// <summary>
    /// Gets the total number of concept references across all topics.
    /// Note: This counts references, not unique concepts.
    /// </summary>
    public int TotalConceptReferences => Lessons.Sum(l => l.TotalConceptCount);

    /// <summary>
    /// Gets a Lesson by ID.
    /// </summary>
    public Lesson? GetLesson(string lessonId)
    {
        return Lessons.FirstOrDefault(l => l.Id == lessonId);
    }

    /// <summary>
    /// Gets a LessonTopic by ID (searches all lessons).
    /// </summary>
    public LessonTopic? GetTopic(string topicId)
    {
        return Lessons
            .SelectMany(l => l.Topics)
            .FirstOrDefault(t => t.Id == topicId);
    }

    /// <summary>
    /// Gets all Lessons in order.
    /// </summary>
    public IEnumerable<Lesson> GetLessonsInOrder()
    {
        return Lessons.OrderBy(l => l.Order);
    }

    /// <summary>
    /// Gets all ConceptIds in the course structure in learning order.
    /// </summary>
    public IEnumerable<string> GetAllConceptIdsInOrder()
    {
        return GetLessonsInOrder()
            .SelectMany(l => l.GetAllConceptIds());
    }

    /// <summary>
    /// Finds the Lesson containing a specific ConceptId.
    /// </summary>
    public Lesson? FindLessonForConcept(string conceptId)
    {
        return Lessons.FirstOrDefault(l =>
            l.Topics.Any(t => t.ConceptIds.Contains(conceptId)));
    }

    /// <summary>
    /// Finds the LessonTopic containing a specific ConceptId.
    /// </summary>
    public LessonTopic? FindTopicForConcept(string conceptId)
    {
        return Lessons
            .SelectMany(l => l.Topics)
            .FirstOrDefault(t => t.ConceptIds.Contains(conceptId));
    }

    /// <summary>
    /// Gets the next ConceptId in the learning order after the given concept.
    /// </summary>
    public string? GetNextConceptId(string conceptId)
    {
        var allIds = GetAllConceptIdsInOrder().ToList();
        var currentIndex = allIds.IndexOf(conceptId);

        if (currentIndex < 0 || currentIndex >= allIds.Count - 1)
            return null;

        return allIds[currentIndex + 1];
    }

    /// <summary>
    /// Gets the previous ConceptId in the learning order before the given concept.
    /// </summary>
    public string? GetPreviousConceptId(string conceptId)
    {
        var allIds = GetAllConceptIdsInOrder().ToList();
        var currentIndex = allIds.IndexOf(conceptId);

        if (currentIndex <= 0)
            return null;

        return allIds[currentIndex - 1];
    }

    /// <summary>
    /// Gets the position of a concept in the overall learning order (1-based).
    /// </summary>
    public int GetConceptPosition(string conceptId)
    {
        var allIds = GetAllConceptIdsInOrder().ToList();
        var index = allIds.IndexOf(conceptId);
        return index >= 0 ? index + 1 : 0;
    }

    /// <summary>
    /// Gets the total number of sections across all lessons.
    /// </summary>
    public int TotalSections => Lessons.Sum(l => l.GetTotalSectionCount());

    /// <summary>
    /// Finds a section by ID across all lessons.
    /// </summary>
    public Section? FindSection(string sectionId)
    {
        foreach (var lesson in Lessons)
        {
            var section = lesson.FindSection(sectionId);
            if (section != null)
                return section;
        }
        return null;
    }

    /// <summary>
    /// Gets all sections flattened in learning order.
    /// </summary>
    public IEnumerable<Section> GetAllSectionsFlattened()
    {
        return GetLessonsInOrder().SelectMany(l => l.GetAllSectionsFlattened());
    }

    /// <summary>
    /// Finds the lesson containing a specific section.
    /// </summary>
    public Lesson? FindLessonForSection(string sectionId)
    {
        return Lessons.FirstOrDefault(l => l.FindSection(sectionId) != null);
    }
}
