namespace Tutor.Models;

/// <summary>
/// Basic user record for backward compatibility.
/// For full user management, use UserProfile from Authentication.cs.
/// </summary>
public record User
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Email { get; init; }
    public List<UserRole> Roles { get; init; } = [UserRole.Student];
    
    /// <summary>
    /// Creates a User from a UserProfile.
    /// </summary>
    public static User FromProfile(UserProfile profile) => new()
    {
        Id = profile.Id,
        Name = profile.DisplayName,
        Email = profile.Email,
        Roles = profile.Roles
    };
}
