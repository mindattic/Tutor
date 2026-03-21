namespace Tutor.Core.Models;

/// <summary>
/// Represents a section within a lesson chapter.
/// Sections are hierarchical and can contain sub-sections up to 4 levels deep.
/// 
/// Example structure:
/// 1. Introduction to the Wizarding World (Chapter/Lesson)
///    1a. History of Magic
///        1a-i. Ancient Times
///        1a-ii. Medieval Period
///    1b. Magical Creatures
///    1c. Spellcasting Basics
/// </summary>
public class Section
{
    /// <summary>Unique identifier for this section</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>Display number/letter (e.g., "1a", "1a-i")</summary>
    public string Number { get; set; } = "";
    
    /// <summary>Section title</summary>
    public string Title { get; set; } = "";
    
    /// <summary>Brief summary of what this section covers</summary>
    public string? Summary { get; set; }
    
    /// <summary>Order within parent (0-based)</summary>
    public int Order { get; set; }
    
    /// <summary>Nesting depth (0 = top level within chapter)</summary>
    public int Depth { get; set; }
    
    /// <summary>Parent section ID (null if top-level section in lesson)</summary>
    public string? ParentSectionId { get; set; }
    
    /// <summary>Lesson ID this section belongs to</summary>
    public string LessonId { get; set; } = "";
    
    /// <summary>Child sections</summary>
    public List<Section> Children { get; set; } = [];
    
    /// <summary>Concept IDs covered in this section</summary>
    public List<string> ConceptIds { get; set; } = [];
    
    /// <summary>Estimated reading time in minutes</summary>
    public int EstimatedReadingMinutes { get; set; } = 2;
    
    /// <summary>Content text for this section</summary>
    public string? Content { get; set; }
    
    /// <summary>Whether this section has a quiz</summary>
    public bool HasQuiz { get; set; }
    
    /// <summary>Number of quiz questions for this section (if HasQuiz)</summary>
    public int QuizQuestionCount { get; set; } = 3;
    
    /// <summary>Minimum passing score percentage for the section quiz</summary>
    public int QuizPassingScore { get; set; } = 70;
    
    /// <summary>Optional icon for navigation display (Bootstrap Icons class name)</summary>
    public string? Icon { get; set; }
    
    /// <summary>Learning objectives for this section</summary>
    public List<string> LearningObjectives { get; set; } = [];
    
    /// <summary>Key terms introduced in this section</summary>
    public List<string> KeyTerms { get; set; } = [];
    
    /// <summary>
    /// Gets all concept IDs from this section and all descendants.
    /// </summary>
    public IEnumerable<string> GetAllConceptIds()
    {
        var concepts = new List<string>(ConceptIds);
        foreach (var child in Children)
        {
            concepts.AddRange(child.GetAllConceptIds());
        }
        return concepts;
    }
    
    /// <summary>
    /// Gets total count of leaf sections (sections with no children).
    /// </summary>
    public int GetLeafSectionCount()
    {
        if (Children.Count == 0)
            return 1;
        return Children.Sum(c => c.GetLeafSectionCount());
    }
    
    /// <summary>
    /// Finds a section by ID recursively.
    /// </summary>
    public Section? FindSection(string sectionId)
    {
        if (Id == sectionId) return this;
        foreach (var child in Children)
        {
            var found = child.FindSection(sectionId);
            if (found != null) return found;
        }
        return null;
    }
    
    /// <summary>
    /// Gets all sections that have quizzes (including this one and descendants).
    /// </summary>
    public IEnumerable<Section> GetSectionsWithQuizzes()
    {
        if (HasQuiz)
            yield return this;
        foreach (var child in Children)
        {
            foreach (var quizSection in child.GetSectionsWithQuizzes())
            {
                yield return quizSection;
            }
        }
    }
    
    /// <summary>
    /// Gets the path from root to this section (breadcrumb).
    /// </summary>
    public List<string> GetBreadcrumb()
    {
        var path = new List<string> { Title };
        // Note: Parent path is built from the hierarchy when traversing from root
        return path;
    }
    
    /// <summary>
    /// Calculates estimated reading time based on content and child sections.
    /// </summary>
    public int CalculateTotalReadingMinutes()
    {
        var total = EstimatedReadingMinutes;
        foreach (var child in Children)
        {
            total += child.CalculateTotalReadingMinutes();
        }
        return total;
    }
}

/// <summary>
/// Progress tracking for a specific section.
/// </summary>
public class SectionProgress
{
    public string SectionId { get; set; } = "";
    public SectionStatus Status { get; set; } = SectionStatus.NotStarted;
    public DateTime? FirstVisitedAt { get; set; }
    public DateTime? MarkedReadAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentSeconds { get; set; }
    public string? LastQuizResultId { get; set; }
}

/// <summary>
/// Progress tracking for a chapter/lesson.
/// </summary>
public class ChapterProgress
{
    public string LessonId { get; set; } = "";
    public SectionStatus Status { get; set; } = SectionStatus.NotStarted;
    public int CompletedSections { get; set; }
    public int TotalSections { get; set; }
    public double ProgressPercentage => TotalSections > 0 ? (double)CompletedSections / TotalSections * 100 : 0;
    public DateTime? CompletedAt { get; set; }
}
