using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Local JSON-based authentication controller.
/// Stores credentials and profiles in Users.json.
/// 
/// Future implementation can swap this for an API-based controller
/// that authenticates via external services (Google, Facebook, etc.)
/// </summary>
public class LocalAuthController : IAuthController
{
    private const string UsersFileName = "Users.json";
    private readonly string usersFilePath;
    private UsersStore? cachedStore;
    private readonly SemaphoreSlim fileLock = new(1, 1);
    private bool isInitialized;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public LocalAuthController()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        var usersFolder = Path.Combine(appDataPath, "Users");
        Directory.CreateDirectory(usersFolder);
        usersFilePath = Path.Combine(usersFolder, UsersFileName);
    }

    private async Task EnsureInitializedAsync()
    {
        if (isInitialized)
            return;
            
        await fileLock.WaitAsync();
        try
        {
            if (isInitialized)
                return;
            
            var usersFolder = Path.GetDirectoryName(usersFilePath)!;
            
            if (!File.Exists(usersFilePath))
            {
                // Create default users: ryan (Admin + Student) and erin (Student)
                var store = new UsersStore();
                
                // Ryan - Admin
                var ryanId = Guid.NewGuid().ToString();
                var ryanHash = HashPassword("aaa");
                store.Credentials["ryan"] = new UserCredentials
                {
                    Username = "ryan",
                    PasswordHash = ryanHash,
                    CreatedAt = DateTime.UtcNow
                };
                store.Profiles["ryan"] = new UserProfile
                {
                    Id = ryanId,
                    Username = "ryan",
                    DisplayName = "Ryan",
                    Roles = [UserRole.Admin, UserRole.Student],
                    CreatedAt = DateTime.UtcNow
                };

                // Erin - Student
                var erinId = Guid.NewGuid().ToString();
                var erinHash = HashPassword("aaa");
                store.Credentials["erin"] = new UserCredentials
                {
                    Username = "erin",
                    PasswordHash = erinHash,
                    CreatedAt = DateTime.UtcNow
                };
                store.Profiles["erin"] = new UserProfile
                {
                    Id = erinId,
                    Username = "erin",
                    DisplayName = "Erin",
                    Roles = [UserRole.Student],
                    CreatedAt = DateTime.UtcNow
                };

                cachedStore = store;
                var json = JsonSerializer.Serialize(store, JsonOptions);
                await File.WriteAllTextAsync(usersFilePath, json);
                
                // Create individual user data files
                await CreateUserDataFileAsync(usersFolder, "ryan");
                await CreateUserDataFileAsync(usersFolder, "erin");
            }
            
            isInitialized = true;
        }
        finally
        {
            fileLock.Release();
        }
    }
    
    private async Task CreateUserDataFileAsync(string usersFolder, string username)
    {
        var userDataPath = Path.Combine(usersFolder, $"User-{username}.json");
        if (File.Exists(userDataPath))
            return;
            
        var userData = new
        {
            Username = username,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CourseProgress = new Dictionary<string, object>(),
            Settings = (object?)null,
            LastActiveCourseId = (string?)null,
            TotalLearningMinutes = 0
        };
        
        var json = JsonSerializer.Serialize(userData, JsonOptions);
        await File.WriteAllTextAsync(userDataPath, json);
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return AuthResult.Failed("Username and password are required.");

        await EnsureInitializedAsync();
        
        username = username.ToLowerInvariant().Trim();
        var store = await LoadStoreAsync();

        if (!store.Credentials.TryGetValue(username, out var credentials))
            return AuthResult.Failed("Invalid username or password.");

        var passwordHash = HashPassword(password);
        if (credentials.PasswordHash != passwordHash)
            return AuthResult.Failed("Invalid username or password.");

        if (!store.Profiles.TryGetValue(username, out var profile))
            return AuthResult.Failed("User profile not found.");

        // Update last login
        credentials.LastLoginAt = DateTime.UtcNow;
        profile.LastLoginAt = DateTime.UtcNow;
        store.LastLoggedInUsername = username;
        store.UpdatedAt = DateTime.UtcNow;
        await SaveStoreAsync(store);

        return AuthResult.Succeeded(profile);
    }

    public async Task<AuthResult> RegisterAsync(string username, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(username))
            return AuthResult.Failed("Username is required.");
        if (string.IsNullOrWhiteSpace(password))
            return AuthResult.Failed("Password is required.");
        if (password.Length < 3)
            return AuthResult.Failed("Password must be at least 3 characters.");

        await EnsureInitializedAsync();
        
        username = username.ToLowerInvariant().Trim();
        displayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName.Trim();

        var store = await LoadStoreAsync();

        if (store.Credentials.ContainsKey(username))
            return AuthResult.Failed("Username already exists.");

        // Create new user
        var passwordHash = HashPassword(password);
        store.Credentials[username] = new UserCredentials
        {
            Username = username,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        var profile = new UserProfile
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            DisplayName = displayName,
            Roles = [UserRole.Student], // All new users start as Student
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };
        store.Profiles[username] = profile;
        store.LastLoggedInUsername = username;
        store.UpdatedAt = DateTime.UtcNow;

        await SaveStoreAsync(store);

        return AuthResult.Succeeded(profile);
    }

    public async Task LogoutAsync()
    {
        // Nothing to clear in local storage - logout is handled by AuthenticationService
        await Task.CompletedTask;
    }

    public async Task<UserProfile?> GetUserAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        await EnsureInitializedAsync();
        username = username.ToLowerInvariant().Trim();
        var store = await LoadStoreAsync();
        return store.Profiles.GetValueOrDefault(username);
    }

    public async Task<List<UserProfile>> GetAllUsersAsync()
    {
        await EnsureInitializedAsync();
        var store = await LoadStoreAsync();
        return [.. store.Profiles.Values.OrderBy(p => p.Username)];
    }

    public async Task<bool> UpdateUserRolesAsync(string username, List<UserRole> roles)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        await EnsureInitializedAsync();
        username = username.ToLowerInvariant().Trim();
        var store = await LoadStoreAsync();

        if (!store.Profiles.TryGetValue(username, out var profile))
            return false;

        profile.Roles = roles;
        store.UpdatedAt = DateTime.UtcNow;
        await SaveStoreAsync(store);
        
        return true;
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        await EnsureInitializedAsync();
        username = username.ToLowerInvariant().Trim();
        var store = await LoadStoreAsync();

        if (!store.Credentials.ContainsKey(username))
            return false;

        store.Credentials.Remove(username);
        store.Profiles.Remove(username);
        store.UpdatedAt = DateTime.UtcNow;
        await SaveStoreAsync(store);
        
        return true;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        await EnsureInitializedAsync();
        username = username.ToLowerInvariant().Trim();
        var store = await LoadStoreAsync();
        return store.Credentials.ContainsKey(username);
    }

    public async Task<string?> GetLastLoggedInUsernameAsync()
    {
        await EnsureInitializedAsync();
        var store = await LoadStoreAsync();
        return store.LastLoggedInUsername;
    }

    private async Task<UsersStore> LoadStoreAsync()
    {
        if (cachedStore != null)
            return cachedStore;

        await fileLock.WaitAsync();
        try
        {
            if (cachedStore != null)
                return cachedStore;

            if (!File.Exists(usersFilePath))
            {
                cachedStore = new UsersStore();
                return cachedStore;
            }

            var json = await File.ReadAllTextAsync(usersFilePath);
            cachedStore = JsonSerializer.Deserialize<UsersStore>(json) ?? new UsersStore();
            return cachedStore;
        }
        finally
        {
            fileLock.Release();
        }
    }

    private async Task SaveStoreAsync(UsersStore store)
    {
        await fileLock.WaitAsync();
        try
        {
            cachedStore = store;
            var json = JsonSerializer.Serialize(store, JsonOptions);
            await File.WriteAllTextAsync(usersFilePath, json);
        }
        finally
        {
            fileLock.Release();
        }
    }

    private static string HashPassword(string password)
    {
        // Simple SHA256 hash - in production, use a proper password hashing library
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
