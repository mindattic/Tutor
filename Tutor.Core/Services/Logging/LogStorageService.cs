using System.Text.Json;

namespace Tutor.Core.Services.Logging;

/// <summary>
/// Service for persisting logs to file storage.
/// </summary>
public sealed class LogStorageService
{
    private const string LogFileName = "app-logs.json";
    private readonly object _saveLock = new();
    private bool _isInitialized;

    /// <summary>
    /// Gets the current log file path (uses DataStorageSettings).
    /// </summary>
    private string LogFilePath => Path.Combine(DataStorageSettings.GetLogsDirectory(), LogFileName);

    public LogStorageService()
    {
    }

    /// <summary>
    /// Initializes the logging system by loading persisted logs and subscribing to new entries.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        // Load persisted logs
        LoadLogs();

        // Subscribe to new entries for auto-save
        Log.Store.EntryAdded += OnEntryAdded;
        Log.Store.EntriesCleared += OnEntriesCleared;

        Log.Info("Logging system initialized");
    }

    private void OnEntryAdded(object? sender, LogEntry entry)
    {
        // Debounced save - only save every 10 entries or on important ones
        if (entry.Severity >= LogSeverity.Warning || Log.Store.Count % 10 == 0)
        {
            SaveLogsAsync().ConfigureAwait(false);
        }
    }

    private void OnEntriesCleared(object? sender, EventArgs e)
    {
        SaveLogsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Loads persisted logs from storage.
    /// </summary>
    public void LoadLogs()
    {
        try
        {
            if (!File.Exists(LogFilePath)) return;

            var json = File.ReadAllText(LogFilePath);
            var dtos = JsonSerializer.Deserialize<List<LogEntryDto>>(json, JsonOptions);

            if (dtos != null && dtos.Count > 0)
            {
                var entries = dtos.Select(dto => new LogEntry(
                    Id: dto.Id ?? Guid.NewGuid().ToString(),
                    TimestampUtc: dto.TimestampUtc,
                    Severity: dto.Severity,
                    Message: dto.Message ?? "",
                    CallingMember: dto.CallingMember,
                    FilePath: dto.FilePath,
                    LineNumber: dto.LineNumber,
                    Exception: null // Exceptions are not persisted
                )).ToList();

                Log.Store.LoadEntries(entries);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LogStorageService] Failed to load logs: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves all logs to persistent storage.
    /// </summary>
    public async Task SaveLogsAsync()
    {
        try
        {
            var entries = Log.Store.Entries;
            var dtos = entries.Select(e => new LogEntryDto
            {
                Id = e.Id,
                TimestampUtc = e.TimestampUtc,
                Severity = e.Severity,
                Message = e.Message,
                CallingMember = e.CallingMember,
                FilePath = e.FilePath,
                LineNumber = e.LineNumber,
                ExceptionMessage = e.Exception?.Message,
                ExceptionType = e.Exception?.GetType().Name
            }).ToList();

            var json = JsonSerializer.Serialize(dtos, JsonOptions);

            await Task.Run(() =>
            {
                lock (_saveLock)
                {
                    File.WriteAllText(LogFilePath, json);
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LogStorageService] Failed to save logs: {ex.Message}");
        }
    }

    /// <summary>
    /// Forces an immediate save of all logs.
    /// </summary>
    public void ForceSave()
    {
        lock (_saveLock)
        {
            try
            {
                var entries = Log.Store.Entries;
                var dtos = entries.Select(e => new LogEntryDto
                {
                    Id = e.Id,
                    TimestampUtc = e.TimestampUtc,
                    Severity = e.Severity,
                    Message = e.Message,
                    CallingMember = e.CallingMember,
                    FilePath = e.FilePath,
                    LineNumber = e.LineNumber,
                    ExceptionMessage = e.Exception?.Message,
                    ExceptionType = e.Exception?.GetType().Name
                }).ToList();

                var json = JsonSerializer.Serialize(dtos, JsonOptions);
                File.WriteAllText(LogFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LogStorageService] Failed to force save logs: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Clears all persisted logs.
    /// </summary>
    public void ClearPersistedLogs()
    {
        Log.Store.Clear();
        lock (_saveLock)
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    File.Delete(LogFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LogStorageService] Failed to delete log file: {ex.Message}");
            }
        }
        Log.Info("Logs cleared");
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private class LogEntryDto
    {
        public string? Id { get; set; }
        public DateTime TimestampUtc { get; set; }
        public LogSeverity Severity { get; set; }
        public string? Message { get; set; }
        public string? CallingMember { get; set; }
        public string? FilePath { get; set; }
        public int LineNumber { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? ExceptionType { get; set; }
    }
}
