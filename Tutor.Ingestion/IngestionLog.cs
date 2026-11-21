namespace Tutor.Ingestion;

/// <summary>Severity of a line the ingestion pipeline reports through <see cref="IngestionLog.Sink"/>.</summary>
public enum IngestionLogLevel { Debug, Info, Warn, Error }

/// <summary>
/// The package's only logging hook. The host sets <see cref="Sink"/> once at startup to route the pipeline's
/// diagnostics into its own logger; with no sink nothing is written anywhere.
/// </summary>
public static class IngestionLog
{
    /// <summary>Receives every diagnostic line (level, message, exception when there is one).</summary>
    public static Action<IngestionLogLevel, string, Exception?>? Sink { get; set; }

    internal static void Debug(string message) => Sink?.Invoke(IngestionLogLevel.Debug, message, null);
    internal static void Info(string message) => Sink?.Invoke(IngestionLogLevel.Info, message, null);
    internal static void Warn(string message) => Sink?.Invoke(IngestionLogLevel.Warn, message, null);
    internal static void Error(string message, Exception? ex = null) => Sink?.Invoke(IngestionLogLevel.Error, message, ex);
}
