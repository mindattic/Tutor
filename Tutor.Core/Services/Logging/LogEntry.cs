namespace Tutor.Core.Services.Logging;

/// <summary>
/// Represents a single log entry with timestamp, severity, message, and optional exception details.
/// </summary>
public sealed record LogEntry(
    string Id,
    DateTime TimestampUtc,
    LogSeverity Severity,
    string Message,
    string? CallingMember,
    string? FilePath,
    int LineNumber,
    Exception? Exception)
{
    /// <summary>
    /// Gets a display-friendly timestamp in local time using the configured date format.
    /// </summary>
    public string TimestampDisplay => SettingsService.FormatDate(TimestampUtc.ToLocalTime());

    /// <summary>
    /// Gets a short display of the source location.
    /// </summary>
    public string SourceDisplay => string.IsNullOrEmpty(FilePath)
        ? CallingMember ?? ""
        : $"{System.IO.Path.GetFileName(FilePath)}:{LineNumber}";

    /// <summary>
    /// Gets a severity icon for display.
    /// </summary>
    public string SeverityIcon => Severity switch
    {
        LogSeverity.Trace => "bi-footprints",
        LogSeverity.Debug => "bi-bug",
        LogSeverity.Information => "bi-info-circle",
        LogSeverity.Warning => "bi-exclamation-triangle",
        LogSeverity.Error => "bi-x-circle",
        LogSeverity.Critical => "bi-fire",
        _ => "bi-dot"
    };

    /// <summary>
    /// Gets a CSS class for the severity level.
    /// </summary>
    public string SeverityCssClass => Severity switch
    {
        LogSeverity.Trace => "trace",
        LogSeverity.Debug => "debug",
        LogSeverity.Information => "info",
        LogSeverity.Warning => "warning",
        LogSeverity.Error => "error",
        LogSeverity.Critical => "critical",
        _ => ""
    };
}
