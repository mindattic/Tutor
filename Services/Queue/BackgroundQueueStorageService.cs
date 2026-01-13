using System.Text.Json;
using Tutor.Services.Logging;

namespace Tutor.Services.Queue;

/// <summary>
/// Service for persisting background queue state to disk.
/// Enables resume capability after app restarts.
/// </summary>
public sealed class BackgroundQueueStorageService
{
    private const string QueueStateFileName = "background-queue-state.json";
    private readonly object fileLock = new();

    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Gets the path to the queue state file.
    /// </summary>
    private string QueueStateFilePath => 
        Path.Combine(DataStorageSettings.AppDataDirectory, QueueStateFileName);

    /// <summary>
    /// Saves the queue state to disk.
    /// </summary>
    public async Task SaveAsync(BackgroundQueueState state, CancellationToken ct = default)
    {
        if (state == null) return;

        try
        {
            // Prune old history before saving
            state.PruneHistory();

            var json = JsonSerializer.Serialize(state, jsonOptions);

            lock (fileLock)
            {
                // Ensure directory exists
                var dir = Path.GetDirectoryName(QueueStateFilePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Write to temp file first, then rename (atomic)
                var tempPath = QueueStateFilePath + ".tmp";
                File.WriteAllText(tempPath, json);
                
                if (File.Exists(QueueStateFilePath))
                {
                    File.Delete(QueueStateFilePath);
                }
                
                File.Move(tempPath, QueueStateFilePath);
            }

            Log.Trace($"BackgroundQueueStorage: Saved {state.Items.Count} items to disk");
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueStorage: Failed to save state - {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Loads the queue state from disk.
    /// </summary>
    public async Task<BackgroundQueueState?> LoadAsync(CancellationToken ct = default)
    {
        try
        {
            if (!File.Exists(QueueStateFilePath))
            {
                Log.Debug("BackgroundQueueStorage: No persisted state found");
                return null;
            }

            string json;
            lock (fileLock)
            {
                json = File.ReadAllText(QueueStateFilePath);
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var state = JsonSerializer.Deserialize<BackgroundQueueState>(json, jsonOptions);
            
            if (state != null)
            {
                Log.Info($"BackgroundQueueStorage: Loaded {state.Items.Count} items from disk");
                
                // Migrate if needed
                state = MigrateIfNeeded(state);
            }

            return state;
        }
        catch (JsonException ex)
        {
            Log.Error($"BackgroundQueueStorage: Failed to parse state - {ex.Message}", ex);
            
            // Backup corrupted file
            try
            {
                var backupPath = QueueStateFilePath + $".corrupted.{DateTime.UtcNow:yyyyMMddHHmmss}";
                File.Move(QueueStateFilePath, backupPath);
                Log.Warn($"BackgroundQueueStorage: Moved corrupted file to {backupPath}");
            }
            catch { }
            
            return null;
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueStorage: Failed to load state - {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// Deletes the persisted queue state.
    /// </summary>
    public void Delete()
    {
        try
        {
            lock (fileLock)
            {
                if (File.Exists(QueueStateFilePath))
                {
                    File.Delete(QueueStateFilePath);
                    Log.Info("BackgroundQueueStorage: Deleted persisted state");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueStorage: Failed to delete state - {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Migrates old state formats to current version.
    /// </summary>
    private BackgroundQueueState MigrateIfNeeded(BackgroundQueueState state)
    {
        // Currently at version 1, no migrations needed yet
        // Future migrations would go here
        
        if (state.Version < 1)
        {
            state.Version = 1;
            Log.Info("BackgroundQueueStorage: Migrated state to version 1");
        }

        return state;
    }

    /// <summary>
    /// Creates a backup of the current state.
    /// </summary>
    public async Task<string?> BackupAsync(CancellationToken ct = default)
    {
        try
        {
            if (!File.Exists(QueueStateFilePath))
            {
                return null;
            }

            var backupPath = QueueStateFilePath + $".backup.{DateTime.UtcNow:yyyyMMddHHmmss}";
            
            lock (fileLock)
            {
                File.Copy(QueueStateFilePath, backupPath);
            }

            Log.Info($"BackgroundQueueStorage: Created backup at {backupPath}");
            return backupPath;
        }
        catch (Exception ex)
        {
            Log.Error($"BackgroundQueueStorage: Failed to create backup - {ex.Message}", ex);
            return null;
        }
    }
}
