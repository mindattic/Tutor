using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Service for storing and loading user data to/from JSON files.
/// 
/// Storage structure:
/// - Users/Users.json - Master list of all users and credentials
/// - Users/User-{username}.json - Individual user progress and settings
/// </summary>
public class UserStorageService
{
    private readonly IAppDataPathProvider _pathProvider;
    private readonly string usersFolder;
    private readonly SemaphoreSlim fileLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public UserStorageService(IAppDataPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
        var appDataPath = _pathProvider.AppDataDirectory;
        usersFolder = Path.Combine(appDataPath, "Users");
        Directory.CreateDirectory(usersFolder);
    }

    /// <summary>
    /// Gets the path for a user's progress file.
    /// </summary>
    public string GetUserProgressPath(string username)
    {
        var sanitized = SanitizeUsername(username);
        return Path.Combine(usersFolder, $"User-{sanitized}.json");
    }

    /// <summary>
    /// Loads a user's complete data (progress across all courses).
    /// </summary>
    public async Task<UserData?> LoadUserDataAsync(string username)
    {
        var path = GetUserProgressPath(username);
        
        if (!File.Exists(path))
            return null;

        await fileLock.WaitAsync();
        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<UserData>(json);
        }
        catch
        {
            return null;
        }
        finally
        {
            fileLock.Release();
        }
    }

    /// <summary>
    /// Saves a user's complete data.
    /// </summary>
    public async Task SaveUserDataAsync(string username, UserData data)
    {
        var path = GetUserProgressPath(username);
        
        await fileLock.WaitAsync();
        try
        {
            data.UpdatedAt = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(data, JsonOptions);
            await File.WriteAllTextAsync(path, json);
        }
        finally
        {
            fileLock.Release();
        }
    }

    /// <summary>
    /// Loads user progress for a specific course.
    /// </summary>
    public async Task<UserProgress?> LoadUserProgressAsync(string username, string courseId)
    {
        var userData = await LoadUserDataAsync(username);
        if (userData == null)
            return null;

        return userData.CourseProgress.GetValueOrDefault(courseId);
    }

    /// <summary>
    /// Saves user progress for a specific course.
    /// </summary>
    public async Task SaveUserProgressAsync(string username, string courseId, UserProgress progress)
    {
        var userData = await LoadUserDataAsync(username) ?? new UserData { Username = username };
        userData.CourseProgress[courseId] = progress;
        await SaveUserDataAsync(username, userData);
    }

    /// <summary>
    /// Deletes all data for a user.
    /// </summary>
    public async Task DeleteUserDataAsync(string username)
    {
        var path = GetUserProgressPath(username);
        
        await fileLock.WaitAsync();
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        finally
        {
            fileLock.Release();
        }
    }

    /// <summary>
    /// Gets all usernames that have stored data.
    /// </summary>
    public IEnumerable<string> GetAllUsernames()
    {
        var files = Directory.GetFiles(usersFolder, "User-*.json");
        return files
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Select(n => n.Replace("User-", ""))
            .Where(n => !string.IsNullOrEmpty(n));
    }

    private static string SanitizeUsername(string username)
    {
        // Remove invalid filename characters
        var invalid = Path.GetInvalidFileNameChars();
        return new string(username.ToLowerInvariant()
            .Where(c => !invalid.Contains(c))
            .ToArray());
    }
}

/// <summary>
/// Complete user data stored in User-{username}.json
/// </summary>
public class UserData
{
    public string Username { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Progress for each course (courseId -> progress)
    /// </summary>
    public Dictionary<string, UserProgress> CourseProgress { get; set; } = [];
    
    /// <summary>
    /// User preferences and settings
    /// </summary>
    public Dictionary<string, object>? Settings { get; set; }
    
    /// <summary>
    /// Last active course ID
    /// </summary>
    public string? LastActiveCourseId { get; set; }
    
    /// <summary>
    /// Total time spent learning across all courses (minutes)
    /// </summary>
    public int TotalLearningMinutes { get; set; }
}
