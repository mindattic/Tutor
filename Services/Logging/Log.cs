using System.Runtime.CompilerServices;

namespace Tutor.Services.Logging;

/// <summary>
/// Global application logger. Provides static methods for easy logging throughout the app.
/// Supports filtering by severity level via LogSettings.
/// </summary>
public static class Log
{
    /// <summary>
    /// The shared log store instance.
    /// </summary>
    public static LogStore Store { get; } = new();

    /// <summary>
    /// Settings that control which log levels are enabled.
    /// </summary>
    public static LogSettings Settings { get; } = new();

    /// <summary>
    /// Logs a trace-level message if trace logging is enabled.
    /// </summary>
    public static void Trace(
        string message,
        [CallerMemberName] string? callingMember = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (Settings.LogTrace)
            Store.Add(LogSeverity.Trace, message, null, callingMember, filePath, lineNumber);
    }

    /// <summary>
    /// Logs a debug-level message if debug logging is enabled.
    /// </summary>
    public static void Debug(
        string message,
        [CallerMemberName] string? callingMember = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (Settings.LogDebug)
            Store.Add(LogSeverity.Debug, message, null, callingMember, filePath, lineNumber);
    }

    /// <summary>
    /// Logs an informational message if info logging is enabled.
    /// </summary>
    public static void Info(
        string message,
        [CallerMemberName] string? callingMember = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (Settings.LogInfo)
            Store.Add(LogSeverity.Information, message, null, callingMember, filePath, lineNumber);
    }

    /// <summary>
    /// Logs a warning message if warning logging is enabled.
    /// </summary>
    public static void Warn(
        string message,
        [CallerMemberName] string? callingMember = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (Settings.LogWarning)
            Store.Add(LogSeverity.Warning, message, null, callingMember, filePath, lineNumber);
    }

    /// <summary>
    /// Logs an error message with optional exception if error logging is enabled.
    /// </summary>
    public static void Error(
        string message,
        Exception? ex = null,
        [CallerMemberName] string? callingMember = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (Settings.LogError)
            Store.Add(LogSeverity.Error, message, ex, callingMember, filePath, lineNumber);
    }

    /// <summary>
    /// Logs a critical error message with optional exception if critical logging is enabled.
    /// </summary>
    public static void Critical(
        string message,
        Exception? ex = null,
        [CallerMemberName] string? callingMember = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (Settings.LogCritical)
            Store.Add(LogSeverity.Critical, message, ex, callingMember, filePath, lineNumber);
    }

    /// <summary>
    /// Checks if a specific severity level is enabled.
    /// </summary>
    public static bool IsEnabled(LogSeverity severity) => severity switch
    {
        LogSeverity.Trace => Settings.LogTrace,
        LogSeverity.Debug => Settings.LogDebug,
        LogSeverity.Information => Settings.LogInfo,
        LogSeverity.Warning => Settings.LogWarning,
        LogSeverity.Error => Settings.LogError,
        LogSeverity.Critical => Settings.LogCritical,
        _ => true
    };
}

/// <summary>
/// Settings that control which log levels are captured.
/// </summary>
public class LogSettings
{
    /// <summary>Enable/disable trace-level logging.</summary>
    public bool LogTrace { get; set; } = false;

    /// <summary>Enable/disable debug-level logging.</summary>
    public bool LogDebug { get; set; } = true;

    /// <summary>Enable/disable info-level logging.</summary>
    public bool LogInfo { get; set; } = true;

    /// <summary>Enable/disable warning-level logging.</summary>
    public bool LogWarning { get; set; } = true;

    /// <summary>Enable/disable error-level logging.</summary>
    public bool LogError { get; set; } = true;

    /// <summary>Enable/disable critical-level logging.</summary>
    public bool LogCritical { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum severity level to log.
    /// Messages below this level will be ignored.
    /// </summary>
    public LogSeverity MinimumLevel
    {
        get
        {
            if (LogTrace) return LogSeverity.Trace;
            if (LogDebug) return LogSeverity.Debug;
            if (LogInfo) return LogSeverity.Information;
            if (LogWarning) return LogSeverity.Warning;
            if (LogError) return LogSeverity.Error;
            if (LogCritical) return LogSeverity.Critical;
            return LogSeverity.Critical;
        }
        set
        {
            LogTrace = value <= LogSeverity.Trace;
            LogDebug = value <= LogSeverity.Debug;
            LogInfo = value <= LogSeverity.Information;
            LogWarning = value <= LogSeverity.Warning;
            LogError = value <= LogSeverity.Error;
            LogCritical = value <= LogSeverity.Critical;
        }
    }

    /// <summary>
    /// Enables all log levels.
    /// </summary>
    public void EnableAll()
    {
        LogTrace = true;
        LogDebug = true;
        LogInfo = true;
        LogWarning = true;
        LogError = true;
        LogCritical = true;
    }

    /// <summary>
    /// Disables all log levels except Critical.
    /// </summary>
    public void DisableAll()
    {
        LogTrace = false;
        LogDebug = false;
        LogInfo = false;
        LogWarning = false;
        LogError = false;
        LogCritical = true; // Always keep critical enabled
    }
}
