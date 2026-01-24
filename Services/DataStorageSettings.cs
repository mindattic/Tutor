using System.Text.Json;
using Tutor.Services.Logging;

namespace Tutor.Services;

/// <summary>
/// Centralized settings for all data storage paths.
/// All paths are relative to AppDataDirectory by default.
/// </summary>
public static class DataStorageSettings
{
    private const string SettingsFileName = "data-storage.settings.json";
    
    // Default directory names (relative to AppDataDirectory)
    private const string DefaultKnowledgeBasesDir = "ConceptMaps";
    private const string DefaultKnowledgeGraphsDir = "KnowledgeGraphs";
    private const string DefaultVectorStoreDir = "VectorStore";
    private const string DefaultLSHDir = "LSH";
    private const string DefaultLogsDir = "Logs";
    private const string DefaultCourseStructuresDir = "CourseStructures";

    // Configurable paths (null = use default)
    public static string? KnowledgeBasesPath { get; set; }
    public static string? KnowledgeGraphsPath { get; set; }
    public static string? VectorStorePath { get; set; }
    public static string? LSHPath { get; set; }
    public static string? LogsPath { get; set; }
    public static string? CourseStructuresPath { get; set; }

    /// <summary>
    /// Gets the base app data directory.
    /// </summary>
    public static string AppDataDirectory => 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor");

    /// <summary>
    /// Gets the resolved KnowledgeBases directory path.
    /// </summary>
    public static string GetKnowledgeBasesDirectory()
    {
        var path = string.IsNullOrWhiteSpace(KnowledgeBasesPath)
            ? Path.Combine(AppDataDirectory, DefaultKnowledgeBasesDir)
            : KnowledgeBasesPath;
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Gets the resolved KnowledgeGraphs directory path.
    /// </summary>
    public static string GetKnowledgeGraphsDirectory()
    {
        var path = string.IsNullOrWhiteSpace(KnowledgeGraphsPath)
            ? Path.Combine(AppDataDirectory, DefaultKnowledgeGraphsDir)
            : KnowledgeGraphsPath;
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Gets the resolved VectorStore directory path.
    /// </summary>
    public static string GetVectorStoreDirectory()
    {
        var path = string.IsNullOrWhiteSpace(VectorStorePath)
            ? Path.Combine(AppDataDirectory, DefaultVectorStoreDir)
            : VectorStorePath;
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Gets the resolved LSH directory path.
    /// </summary>
    public static string GetLSHDirectory()
    {
        var path = string.IsNullOrWhiteSpace(LSHPath)
            ? Path.Combine(AppDataDirectory, DefaultLSHDir)
            : LSHPath;
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Gets the resolved Logs directory path.
    /// </summary>
    public static string GetLogsDirectory()
    {
        var path = string.IsNullOrWhiteSpace(LogsPath)
            ? Path.Combine(AppDataDirectory, DefaultLogsDir)
            : LogsPath;
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Gets the resolved CourseStructures directory path.
    /// </summary>
    public static string GetCourseStructuresDirectory()
    {
        var path = string.IsNullOrWhiteSpace(CourseStructuresPath)
            ? Path.Combine(AppDataDirectory, DefaultCourseStructuresDir)
            : CourseStructuresPath;
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Load settings from disk.
    /// </summary>
    public static void Load()
    {
        try
        {
            var settingsDir = Path.Combine(AppDataDirectory, "Settings");
            Directory.CreateDirectory(settingsDir);
            var settingsPath = Path.Combine(settingsDir, SettingsFileName);

            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                var settings = JsonSerializer.Deserialize<DataStorageSettingsDto>(json);
                
                if (settings != null)
                {
                    KnowledgeBasesPath = settings.KnowledgeBasesPath;
                    KnowledgeGraphsPath = settings.KnowledgeGraphsPath;
                    VectorStorePath = settings.VectorStorePath;
                    LSHPath = settings.LSHPath;
                    LogsPath = settings.LogsPath;
                    CourseStructuresPath = settings.CourseStructuresPath;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"DataStorageSettings: Failed to load settings - {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Save settings to disk.
    /// </summary>
    public static void Save()
    {
        try
        {
            var settingsDir = Path.Combine(AppDataDirectory, "Settings");
            Directory.CreateDirectory(settingsDir);
            var settingsPath = Path.Combine(settingsDir, SettingsFileName);

            var settings = new DataStorageSettingsDto
            {
                KnowledgeBasesPath = KnowledgeBasesPath,
                KnowledgeGraphsPath = KnowledgeGraphsPath,
                VectorStorePath = VectorStorePath,
                LSHPath = LSHPath,
                LogsPath = LogsPath,
                CourseStructuresPath = CourseStructuresPath
            };

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsPath, json);
        }
        catch (Exception ex)
        {
            Log.Error($"DataStorageSettings: Failed to save settings - {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Reset all paths to defaults.
    /// </summary>
    public static void ResetToDefaults()
    {
        KnowledgeBasesPath = null;
        KnowledgeGraphsPath = null;
        VectorStorePath = null;
        LSHPath = null;
        LogsPath = null;
        CourseStructuresPath = null;
        Save();
    }

    private class DataStorageSettingsDto
    {
        public string? KnowledgeBasesPath { get; set; }
        public string? KnowledgeGraphsPath { get; set; }
        public string? VectorStorePath { get; set; }
        public string? LSHPath { get; set; }
        public string? LogsPath { get; set; }
        public string? CourseStructuresPath { get; set; }
    }
}
