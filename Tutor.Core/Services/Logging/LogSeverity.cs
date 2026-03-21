namespace Tutor.Core.Services.Logging;

/// <summary>
/// Severity levels for log entries.
/// </summary>
public enum LogSeverity
{
    /// <summary>Detailed trace-level messages for debugging.</summary>
    Trace,
    
    /// <summary>Debug information helpful during development.</summary>
    Debug,
    
    /// <summary>Informational messages about normal operations.</summary>
    Information,
    
    /// <summary>Warnings about potential issues or unexpected behavior.</summary>
    Warning,
    
    /// <summary>Errors that indicate failures in operations.</summary>
    Error,
    
    /// <summary>Critical errors that may cause application instability.</summary>
    Critical
}
