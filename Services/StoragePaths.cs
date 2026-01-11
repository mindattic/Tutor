namespace Tutor.Services;

/// <summary>
/// Manages file paths for the application's persistent storage.
/// Learning resources are stored in the project directory for GitHub sync.
/// </summary>
public static class StoragePaths
{
    private const string AppName = "Tutor";
    
    /// <summary>
    /// Default learning resources directory within the project.
    /// This is checked into GitHub.
    /// </summary>
    public static string DefaultLearningResourcesDirectory => 
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "LearningResources");

    /// <summary>
    /// App-local settings directory (not checked into GitHub).
    /// </summary>
    public static string SettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName, "Settings");

    /// <summary>
    /// Path to the storage settings file.
    /// </summary>
    public static string StorageSettingsPath => Path.Combine(SettingsDirectory, "storage.settings.json");

    /// <summary>
    /// Path to the vector store (embeddings) file.
    /// </summary>
    public static string VectorStorePath => Path.Combine(SettingsDirectory, "content_chunks.json");
}
