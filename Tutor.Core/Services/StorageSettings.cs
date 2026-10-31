using System.Runtime.Versioning;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// The storage options in effect (LAMP, TUT-T31): read from <see cref="TutorSettings"/>
/// (<c>learningResourcesDirectory</c>, <c>saveFormattedAsMarkdown</c>, <c>preserveOriginalFiles</c>),
/// edited only in MindAttic.Vault.Desktop. The pre-LAMP <c>storage.settings.json</c> is imported once
/// by <see cref="TutorSettings.Load"/> and never written again.
/// </summary>
public static class StorageSettings
{
    private static TutorSettingsSource? source;

    /// <summary>
    /// The settings source every static read goes through. Hosts set it at startup to the one they
    /// opened; when unset, the Vault file for this machine (or the test roots) is read.
    /// </summary>
    public static TutorSettingsSource Source
    {
        get => source ??= new TutorSettingsSource(MindAttic.Vault.Settings.JsonSettingsStore<TutorSettings>.ForSettingsApp());
        set => source = value;
    }

    /// <summary>
    /// Directory where learning resources are stored: the setting, or the project's Resources
    /// folder (else <c>%LOCALAPPDATA%\Tutor\Resources</c>) when it is empty.
    /// </summary>
    public static string LearningResourcesDirectory =>
        Source.Current.LearningResourcesDirectory is { Length: > 0 } dir ? dir : GetDefaultDirectory();

    /// <summary>Whether to save formatted resources as separate .md files.</summary>
    public static bool SaveFormattedAsMarkdown => Source.Current.SaveFormattedAsMarkdown;

    /// <summary>Whether to preserve original uploaded files.</summary>
    public static bool PreserveOriginalFiles => Source.Current.PreserveOriginalFiles;

    /// <summary>
    /// Gets the resolved learning resources directory path, creating it (and its Original,
    /// Formatted and Courses subfolders) when missing.
    /// </summary>
    public static string GetResolvedDirectory()
    {
        var dir = Path.GetFullPath(LearningResourcesDirectory);
        EnsureDirectoryExists(dir);
        return dir;
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

    private static void EnsureDirectoryExists(string dir)
    {
        try
        {
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
