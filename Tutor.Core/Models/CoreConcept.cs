namespace Tutor.Core.Models;

/// <summary>
/// Represents a core concept that the student can quickly reference.
/// </summary>
public record CoreConcept
{
    /// <summary>
    /// The term or name of the concept.
    /// </summary>
    public required string Term { get; init; }

    /// <summary>
    /// A brief reminder/description of the concept.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// When the concept was added to the list.
    /// </summary>
    public DateTime AddedAt { get; init; } = DateTime.Now;
}
