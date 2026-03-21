using System.Runtime.Versioning;
using System.Text.Json;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Manages storage settings including the learning resources directory.
/// Settings are persisted to a local JSON file.
/// </summary>
public static class StorageSettings
{
    private static readonly string SettingsDirectory = StoragePaths.SettingsDirectory;
    private static readonly string SettingsFilePath = StoragePaths.StorageSettingsPath;

    private static readonly object lockObj = new();
    private static bool isLoaded = false;

    /// <summary>
    /// Directory where learning resources are stored.
    /// Defaults to the LearningResources folder in the project.
    /// </summary>
    public static string LearningResourcesDirectory { get; set; } = "";

    /// <summary>
    /// Whether to save formatted resources as separate .md files.
    /// </summary>
    public static bool SaveFormattedAsMarkdown { get; set; } = true;

    /// <summary>
    /// Whether to preserve original uploaded files.
    /// </summary>
    public static bool PreserveOriginalFiles { get; set; } = true;

    private sealed class Data
    {
        public string? LearningResourcesDirectory { get; set; }
        public bool SaveFormattedAsMarkdown { get; set; } = true;
        public bool PreserveOriginalFiles { get; set; } = true;
    }

    public static void Load()
    {
        lock (lockObj)
        {
            if (isLoaded) return;
            isLoaded = true; // Set early to prevent recursion
            
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var data = JsonSerializer.Deserialize<Data>(json);
                    if (data != null)
                    {
                        LearningResourcesDirectory = data.LearningResourcesDirectory ?? GetDefaultDirectory();
                        SaveFormattedAsMarkdown = data.SaveFormattedAsMarkdown;
                        PreserveOriginalFiles = data.PreserveOriginalFiles;
                    }
                    else
                    {
                        LearningResourcesDirectory = GetDefaultDirectory();
                    }
                }
                else
                {
                    // Use default and create settings file
                    LearningResourcesDirectory = GetDefaultDirectory();
                    Save();
                }

                // Ensure the directory exists
                EnsureDirectoryExists();
            }
            catch
            {
                if (string.IsNullOrEmpty(LearningResourcesDirectory))
                {
                    LearningResourcesDirectory = GetDefaultDirectory();
                }
            }
        }
    }

    public static void Save()
    {
        lock (lockObj)
        {
            try
            {
                Directory.CreateDirectory(SettingsDirectory);
                var json = JsonSerializer.Serialize(new Data
                {
                    LearningResourcesDirectory = LearningResourcesDirectory,
                    SaveFormattedAsMarkdown = SaveFormattedAsMarkdown,
                    PreserveOriginalFiles = PreserveOriginalFiles
                }, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, json);
            }
            catch
            {
                // Silently fail if we can't save settings
            }
        }
    }

    /// <summary>
    /// Gets the resolved learning resources directory path.
    /// </summary>
    public static string GetResolvedDirectory()
    {
        Load();
        return Path.GetFullPath(LearningResourcesDirectory);
    }

    /// <summary>
    /// Sets a custom learning resources directory.
    /// </summary>
    public static void SetDirectory(string path)
    {
        LearningResourcesDirectory = path;
        EnsureDirectoryExists();
        Save();
    }

    private static string GetDefaultDirectory()
    {
        // Try to find the project's LearningResources folder
        var baseDir = AppContext.BaseDirectory;
        
        // Navigate up from bin/Debug/net10.0-windows... to project root
        var projectRoot = baseDir;
        for (int i = 0; i < 5; i++)
        {
            var parent = Directory.GetParent(projectRoot);
            if (parent == null) break;
            projectRoot = parent.FullName;
            
            // Check if this looks like the project root (has .csproj file)
            if (Directory.GetFiles(projectRoot, "*.csproj").Length > 0)
            {
                return Path.Combine(projectRoot, "Resources");
            }
        }

        // Fallback to app data directory
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "Resources");
    }

    private static void EnsureDirectoryExists()
    {
        try
        {
            // Use the property directly to avoid recursion
            var dir = Path.GetFullPath(LearningResourcesDirectory);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Create subdirectories
            Directory.CreateDirectory(Path.Combine(dir, "Original"));
            Directory.CreateDirectory(Path.Combine(dir, "Formatted"));
            Directory.CreateDirectory(Path.Combine(dir, "Courses"));
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>
    /// Opens the resources directory in the file explorer.
    /// Only supported on Windows, macOS (via Catalyst), and Android.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("android")]
    public static void OpenInExplorer()
    {
        try
        {
            var dir = GetResolvedDirectory();
            if (Directory.Exists(dir))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = dir,
                    UseShellExecute = true
                });
                Log.Debug($"StorageSettings: Opened directory in explorer: {dir}");
            }
            else
            {
                Log.Warn($"StorageSettings: Directory does not exist: {dir}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"StorageSettings: Failed to open explorer - {ex.Message}");
        }
    }
}
