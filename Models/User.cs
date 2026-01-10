namespace Tutor.Models;

public record User
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}
