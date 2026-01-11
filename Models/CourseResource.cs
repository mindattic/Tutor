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
    public string Content { get; set; } = "";
    public ResourceType Type { get; set; } = ResourceType.Text;
    public string FileName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ResourceType
{
    Text,
    Txt,
    Docx,
    Pdf
}
