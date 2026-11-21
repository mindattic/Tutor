using System.Runtime.CompilerServices;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

/// <summary>
/// Gives Tutor.Ingestion the folders Tutor already uses (DataStorageSettings), so the vector store, the LSH
/// hyperplanes and the OCR data live exactly where they did before the package was extracted (TUT-T64).
/// </summary>
public sealed class TutorIngestionPaths : IIngestionPaths
{
    public string GetVectorStoreDirectory() => DataStorageSettings.GetVectorStoreDirectory();

    public string GetLshDirectory() => DataStorageSettings.GetLSHDirectory();

    public IReadOnlyList<string> GetTessdataDirectories() =>
    [
        Path.Combine(AppContext.BaseDirectory, "tessdata"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor", "tessdata"),
    ];

    /// <summary>Routes the package's diagnostics into Tutor's log as soon as Tutor.Core loads (any host, tests included).</summary>
    [ModuleInitializer]
    internal static void RouteIngestionLog()
    {
        IngestionLog.Sink = static (level, message, ex) =>
        {
            switch (level)
            {
                case IngestionLogLevel.Debug: Log.Debug(message); break;
                case IngestionLogLevel.Info: Log.Info(message); break;
                case IngestionLogLevel.Warn: Log.Warn(message); break;
                default: Log.Error(message, ex); break;
            }
        };
    }
}
