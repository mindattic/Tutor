namespace Tutor.Models;

/// <summary>
/// Represents a learning resource (document, text, etc.) that can be added to a course.
/// </summary>
public class CourseResource
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Year { get; set; } = "";
    public string Description { get; set; } = "";
    
    /// <summary>
    /// Original content as uploaded/pasted.
    /// </summary>
    public string Content { get; set; } = "";
    
    /// <summary>
    /// AI-formatted version of the content (if processed).
    /// </summary>
    public string? FormattedContent { get; set; }
    
    public ResourceType Type { get; set; } = ResourceType.Text;
    public string FileName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether this resource has been processed (formatted, concepts extracted).
    /// </summary>
    public bool IsProcessed { get; set; }

    /// <summary>
    /// ID of the ConceptMap generated from this resource.
    /// Each resource generates its own independent ConceptMap.
    /// Null if no ConceptMap has been built yet.
    /// </summary>
    public string? ConceptMapId { get; set; }

    /// <summary>
    /// Current status of the ConceptMap generation for this resource.
    /// </summary>
    public ConceptMapStatus ConceptMapStatus { get; set; } = ConceptMapStatus.NotStarted;

    /// <summary>
    /// Whether this resource has a ConceptMap built.
    /// </summary>
    public bool HasConceptMap => !string.IsNullOrEmpty(ConceptMapId) 
        && ConceptMapStatus == ConceptMapStatus.Ready;

    /// <summary>
    /// Gets the best available content (formatted if available, otherwise original).
    /// </summary>
    public string GetEffectiveContent() => FormattedContent ?? Content;
}

public enum ResourceType
{
    Text,
    Txt,
    Docx,
    Pdf
}
