using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Singleton service managing the current authenticated user.
/// Coordinates with IAuthController for authentication operations.
/// The CurrentUser is stored in memory and persists for the app's lifetime.
/// Inject this service anywhere to access the authenticated user.
/// </summary>
public sealed class AuthenticationService
{
    private readonly IAuthController authController;
    private UserProfile? currentUser;
    private bool isInitialized;
    
    /// <summary>Static instance for rare cases where DI isn't available</summary>
    public static AuthenticationService? Instance { get; private set; }
    
    /// <summary>Event fired when authentication state changes</summary>
    public event Action? OnAuthStateChanged;
    
    /// <summary>Event fired when user logs in</summary>
    public event Action<UserProfile>? OnUserLoggedIn;
    
    /// <summary>Event fired when user logs out</summary>
    public event Action? OnUserLoggedOut;

    public AuthenticationService(IAuthController authController)
    {
        this.authController = authController;
        Instance = this; // Set static instance for rare non-DI access
    }

    /// <summary>Gets whether a user is currently authenticated</summary>
    public bool IsAuthenticated => currentUser != null;

    /// <summary>Gets the current authenticated user (in-memory singleton)</summary>
    public UserProfile? CurrentUser => currentUser;
    
    /// <summary>Gets the current user's ID, or null if not authenticated</summary>
    public string? CurrentUserId => currentUser?.Id;
    
    /// <summary>Gets the current user's username, or null if not authenticated</summary>
    public string? CurrentUsername => currentUser?.Username;

    /// <summary>Gets whether the current user is an admin</summary>
    public bool IsAdmin => currentUser?.IsAdmin ?? false;

    /// <summary>
    /// Initialize the service and check for persisted session.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (isInitialized)
            return;

        // Check for persisted user session
        try
        {
            var json = await SecureStorage.GetAsync("CURRENT_USER");
            if (!string.IsNullOrEmpty(json))
            {
                var user = JsonSerializer.Deserialize<UserProfile>(json);
                if (user != null)
                {
                    // Verify user still exists
                    var freshUser = await authController.GetUserAsync(user.Username);
                    if (freshUser != null)
                    {
                        currentUser = freshUser;
                        OnAuthStateChanged?.Invoke();
                    }
                }
            }
        }
        catch
        {
            // Ignore - user will need to log in
        }

        isInitialized = true;
    }

    /// <summary>
    /// Get the last logged in username for pre-filling login form.
    /// </summary>
    public async Task<string?> GetLastLoggedInUsernameAsync()
    {
        return await authController.GetLastLoggedInUsernameAsync();
    }

    /// <summary>
    /// Attempt to log in with username and password.
    /// </summary>
    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var result = await authController.LoginAsync(username, password);
        
        if (result.Success && result.User != null)
        {
            currentUser = result.User;
            
            // Persist session
            var json = JsonSerializer.Serialize(currentUser);
            await SecureStorage.SetAsync("CURRENT_USER", json);
            
            OnUserLoggedIn?.Invoke(currentUser);
            OnAuthStateChanged?.Invoke();
        }
        
        return result;
    }

    /// <summary>
    /// Register a new user and log them in.
    /// </summary>
    public async Task<AuthResult> RegisterAsync(string username, string password, string displayName)
    {
        var result = await authController.RegisterAsync(username, password, displayName);
        
        if (result.Success && result.User != null)
        {
            currentUser = result.User;
            
            // Persist session
            var json = JsonSerializer.Serialize(currentUser);
            await SecureStorage.SetAsync("CURRENT_USER", json);
            
            OnUserLoggedIn?.Invoke(currentUser);
            OnAuthStateChanged?.Invoke();
        }
        
        return result;
    }

    /// <summary>
    /// Log out the current user.
    /// </summary>
    public async Task LogoutAsync()
    {
        currentUser = null;
        
        try
        {
            SecureStorage.Remove("CURRENT_USER");
        }
        catch { }
        
        await authController.LogoutAsync();
        
        OnUserLoggedOut?.Invoke();
        OnAuthStateChanged?.Invoke();
    }

    /// <summary>
    /// Get all users (admin only).
    /// </summary>
    public async Task<List<UserProfile>> GetAllUsersAsync()
    {
        if (!IsAdmin)
            return [];
        
        return await authController.GetAllUsersAsync();
    }

    /// <summary>
    /// Update a user's roles (admin only).
    /// </summary>
    public async Task<bool> UpdateUserRolesAsync(string username, List<UserRole> roles)
    {
        if (!IsAdmin)
            return false;
        
        return await authController.UpdateUserRolesAsync(username, roles);
    }

    /// <summary>
    /// Delete a user (admin only).
    /// </summary>
    public async Task<bool> DeleteUserAsync(string username)
    {
        if (!IsAdmin)
            return false;
        
        // Can't delete yourself
        if (currentUser?.Username == username)
            return false;
        
        return await authController.DeleteUserAsync(username);
    }

    /// <summary>
    /// Check if a username exists.
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await authController.UsernameExistsAsync(username);
    }

    /// <summary>
    /// Check if current user has a specific role.
    /// </summary>
    public bool HasRole(UserRole role)
    {
        return currentUser?.HasRole(role) ?? false;
    }
    
    /// <summary>
    /// Gets the current user, throwing if not authenticated.
    /// Use this when you require a user to be logged in.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no user is authenticated</exception>
    public UserProfile RequireUser()
    {
        return currentUser ?? throw new InvalidOperationException(
            "No user is currently authenticated. Please log in first.");
    }
    
    /// <summary>
    /// Gets the current user's ID, throwing if not authenticated.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no user is authenticated</exception>
    public string RequireUserId()
    {
        return CurrentUserId ?? throw new InvalidOperationException(
            "No user is currently authenticated. Please log in first.");
    }
}
