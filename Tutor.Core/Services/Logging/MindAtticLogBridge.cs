using MindAttic.Log.Migration;
using MindAttic.Log.Sinks;
using MindAttic.Vault.Paths;
using Serilog.Events;
using Serilog.Parsing;

namespace Tutor.Core.Services.Logging;

/// <summary>
/// Bridges Tutor's own <see cref="Log"/>/<see cref="LogStore"/> facade into the shared
/// MindAttic.Log pipeline, so every <c>Log.Info(...)</c>/<c>Log.Error(...)</c> call site —
/// unchanged — lands in a queryable standalone <c>MindAttic.Log.db</c> (Phase 2, MindAttic.Log
/// 3.0.0 — see its docs/BIBLE.md §1.1) that MindAttic.Log.Reader can open, instead of only the
/// single <c>app-logs.json</c> file <see cref="LogStorageService"/> rewrites in full on every save.
/// <para>
/// Additive, not a replacement: <see cref="LogStorageService"/>'s JSON persistence is untouched,
/// so the live in-app log viewer (which reads <see cref="Log.Store"/> directly, not this file)
/// keeps working exactly as before.
/// </para>
/// </summary>
public sealed class MindAtticLogBridge : IDisposable
{
    private readonly MindAtticSqliteSink sink;
    private readonly MessageTemplateParser templateParser = new();

    public MindAtticLogBridge(string? logsDirectory = null)
    {
        // Phase 2: carry forward any rows left in Phase 1's rolled MindAttic.Log.<yyyy-MM>.db
        // files (LogFileRoller is retired, so these are never written to again) into the new
        // standalone database, then remove the now-empty rolled files. Idempotent and a clean
        // no-op on every later startup once nothing is left to migrate.
        var directory = logsDirectory ?? DataStorageSettings.GetLogsDirectory();
        var dbPath = VaultPaths.LoggingDbPath("Tutor");
        if (Directory.Exists(directory))
        {
            foreach (var rolledFile in Directory.GetFiles(directory, "MindAttic.Log.*.db"))
            {
                MindAtticLogMigrator.MigrateFromSqliteAsync(rolledFile, dbPath, dropSource: true)
                    .GetAwaiter().GetResult();
                // Microsoft.Data.Sqlite pools native connections by default — a handle from the
                // migrator's own connection can still be open past its Dispose, so clear the pool
                // before deleting or this races the pool's own cleanup.
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { File.Delete(rolledFile); } catch (IOException) { } catch (UnauthorizedAccessException) { }
            }
        }

        sink = new MindAtticSqliteSink(dbPath, application: "Tutor");
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
