using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Interface for authentication operations.
/// Designed for future API/OAuth integration.
/// </summary>
public interface IAuthController
{
    /// <summary>Authenticate user with username and password</summary>
    Task<AuthResult> LoginAsync(string username, string password);
    
    /// <summary>Register a new user</summary>
    Task<AuthResult> RegisterAsync(string username, string password, string displayName);
    
    /// <summary>Log out current user</summary>
    Task LogoutAsync();
    
    /// <summary>Get user by username</summary>
    Task<UserProfile?> GetUserAsync(string username);
    
    /// <summary>Get all users (admin only)</summary>
    Task<List<UserProfile>> GetAllUsersAsync();
    
    /// <summary>Update user roles (admin only)</summary>
    Task<bool> UpdateUserRolesAsync(string username, List<UserRole> roles);
    
    /// <summary>Delete user (admin only)</summary>
    Task<bool> DeleteUserAsync(string username);
    
    /// <summary>Check if username exists</summary>
    Task<bool> UsernameExistsAsync(string username);
    
    /// <summary>Get the last logged in username</summary>
    Task<string?> GetLastLoggedInUsernameAsync();
}
