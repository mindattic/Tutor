using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Tutor.Services.Logging;

/// <summary>
/// Thread-safe store for log entries with observable collection for UI binding.
/// Supports maximum entry limit and event notifications.
/// </summary>
public sealed class LogStore
{
    private readonly object _gate = new();
    private readonly List<LogEntry> _entries = [];

    /// <summary>
    /// Gets the log entries as a read-only list. Use EntryAdded event for updates.
    /// </summary>
    public IReadOnlyList<LogEntry> Entries
    {
        get
        {
            lock (_gate)
            {
                return _entries.ToList();
            }
        }
    }

    /// <summary>
    /// Maximum number of entries to keep in memory.
    /// </summary>
    public int MaxEntries { get; set; } = 5000;

    /// <summary>
    /// Event fired when a new entry is added.
    /// </summary>
    public event EventHandler<LogEntry>? EntryAdded;

    /// <summary>
    /// Event fired when entries are cleared.
    /// </summary>
    public event EventHandler? EntriesCleared;

    /// <summary>
    /// Adds a new log entry to the store.
    /// </summary>
    public void Add(
        LogSeverity severity,
        string message,
        Exception? exception = null,
        [CallerMemberName] string? callingMember = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        var entry = new LogEntry(
            Id: Guid.NewGuid().ToString(),
            TimestampUtc: DateTime.UtcNow,
            Severity: severity,
            Message: message,
            CallingMember: callingMember,
            FilePath: filePath,
            LineNumber: lineNumber,
            Exception: exception);

        lock (_gate)
        {
            _entries.Add(entry);
            while (_entries.Count > MaxEntries)
                _entries.RemoveAt(0);
        }

        try
        {
            EntryAdded?.Invoke(this, entry);
        }
        catch
        {
            // Never allow logger observers to throw back into app code.
        }
    }

    /// <summary>
    /// Clears all entries from the store.
    /// </summary>
    public void Clear()
    {
        lock (_gate)
        {
            _entries.Clear();
        }

        try
        {
            EntriesCleared?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            // Never allow logger observers to throw back into app code.
        }
    }

    /// <summary>
    /// Gets entries filtered by severity levels, ordered by timestamp descending.
    /// </summary>
    public IReadOnlyList<LogEntry> GetFiltered(HashSet<LogSeverity> includedSeverities)
    {
        lock (_gate)
        {
            return _entries
                .Where(e => includedSeverities.Contains(e.Severity))
                .OrderByDescending(e => e.TimestampUtc)
                .ToList();
        }
    }

    /// <summary>
    /// Gets all entries ordered by timestamp descending (newest first).
    /// </summary>
    public IReadOnlyList<LogEntry> GetAllDescending()
    {
        lock (_gate)
        {
            return _entries.OrderByDescending(e => e.TimestampUtc).ToList();
        }
    }

    /// <summary>
    /// Gets the count of entries.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }

    /// <summary>
    /// Loads entries from a list (used for persistence restoration).
    /// </summary>
    public void LoadEntries(IEnumerable<LogEntry> entries)
    {
        lock (_gate)
        {
            _entries.Clear();
            _entries.AddRange(entries);
            
            // Enforce max entries
            while (_entries.Count > MaxEntries)
                _entries.RemoveAt(0);
        }
    }
}
