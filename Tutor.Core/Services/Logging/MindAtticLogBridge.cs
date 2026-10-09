using MindAttic.Log.Sinks;
using Serilog.Events;
using Serilog.Parsing;

namespace Tutor.Core.Services.Logging;

/// <summary>
/// Bridges Tutor's own <see cref="Log"/>/<see cref="LogStore"/> facade into the shared
/// MindAttic.Log pipeline, so every <c>Log.Info(...)</c>/<c>Log.Error(...)</c> call site —
/// unchanged — lands in a queryable <c>MindAttic_Log</c> table that MindAttic.Log.Reader can open,
/// instead of only the single <c>app-logs.json</c> file <see cref="LogStorageService"/> rewrites
/// in full on every save (see docs/MIGRATION.md in the MindAttic.Log repo for why that shape was
/// rejected for the no-database tier generally).
/// <para>
/// Additive, not a replacement: <see cref="LogStorageService"/>'s JSON persistence is untouched,
/// so the live in-app log viewer (which reads <see cref="Log.Store"/> directly, not this file)
/// keeps working exactly as before. Tutor has no general-purpose database of its own — TutorAuth
/// is MindAttic.Authentication's identity schema only (see <c>TutorAuthDbContext</c>) — so this
/// uses the rolled-SQLite no-database tier, not the SQL Server tier.
/// </para>
/// </summary>
public sealed class MindAtticLogBridge : IDisposable
{
    private readonly MindAtticSqliteSink sink;
    private readonly MessageTemplateParser templateParser = new();

    public MindAtticLogBridge(string? logsDirectory = null, int retainedFileCount = 12)
    {
        var directory = logsDirectory ?? DataStorageSettings.GetLogsDirectory();
        LogFileRoller.PruneOldFiles(directory, retainedFileCount);
        sink = new MindAtticSqliteSink(LogFileRoller.CurrentPath(directory, DateTime.UtcNow), application: "Tutor");
        Log.Store.EntryAdded += OnEntryAdded;
    }

    private void OnEntryAdded(object? sender, LogEntry entry)
    {
        try
        {
            var template = templateParser.Parse(entry.Message);
            var logEvent = new LogEvent(
                new DateTimeOffset(entry.TimestampUtc, TimeSpan.Zero),
                MapLevel(entry.Severity),
                entry.Exception,
                template,
                Properties(entry));
            sink.Emit(logEvent);
        }
        catch
        {
            // Same rule LogStore.Add itself follows: a logging observer must never throw back
            // into the app code that just tried to log something.
        }
    }

    private static IEnumerable<LogEventProperty> Properties(LogEntry entry)
    {
        if (!string.IsNullOrEmpty(entry.CallingMember) || !string.IsNullOrEmpty(entry.FilePath))
            yield return new LogEventProperty("SourceContext", new ScalarValue(SourceContext(entry)));
    }

    private static string SourceContext(LogEntry entry) => string.IsNullOrEmpty(entry.FilePath)
        ? entry.CallingMember ?? ""
        : $"{Path.GetFileName(entry.FilePath)}:{entry.LineNumber} ({entry.CallingMember})";

    private static LogEventLevel MapLevel(LogSeverity severity) => severity switch
    {
        LogSeverity.Trace => LogEventLevel.Verbose,
        LogSeverity.Debug => LogEventLevel.Debug,
        LogSeverity.Information => LogEventLevel.Information,
        LogSeverity.Warning => LogEventLevel.Warning,
        LogSeverity.Error => LogEventLevel.Error,
        LogSeverity.Critical => LogEventLevel.Fatal,
        _ => LogEventLevel.Information,
    };

    public void Dispose()
    {
        Log.Store.EntryAdded -= OnEntryAdded;
        sink.Dispose();
    }
}
