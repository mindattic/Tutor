namespace Tutor.Core.Models;

/// <summary>
/// Roles available in the system.
/// </summary>
public enum UserRole
{
    Student,
    Admin
}

/// <summary>
/// Status of a section's progress.
/// </summary>
public enum SectionStatus
{
    /// <summary>Not yet viewed</summary>
    NotStarted,
    /// <summary>User has navigated to this section</summary>
    Visited,
    /// <summary>User has spent approximate reading time on this section</summary>
    Read,
    /// <summary>User has completed the quiz for this section</summary>
    Complete
}

/// <summary>
/// User credentials for authentication storage.
/// </summary>
public class UserCredentials
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Full user profile with roles and settings.
/// </summary>
public class UserProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Email { get; set; }
    public List<UserRole> Roles { get; set; } = [UserRole.Student];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public Dictionary<string, object>? Settings { get; set; }

    /// <summary>
    /// Checks if user has a specific role.
    /// </summary>
    public bool HasRole(UserRole role) => Roles.Contains(role);

    /// <summary>
    /// Checks if user is an admin.
    /// </summary>
    public bool IsAdmin => Roles.Contains(UserRole.Admin);
}

/// <summary>
/// Result of an authentication attempt.
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public UserProfile? User { get; set; }

    public static AuthResult Succeeded(UserProfile user) => new()
    {
        Success = true,
        User = user
    };

    public static AuthResult Failed(string message) => new()
    {
        Success = false,
        ErrorMessage = message
    };
}

/// <summary>
/// Master users storage file structure.
/// </summary>
public class UsersStore
{
    public Dictionary<string, UserCredentials> Credentials { get; set; } = [];
    public Dictionary<string, UserProfile> Profiles { get; set; } = [];
    public string? LastLoggedInUsername { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
