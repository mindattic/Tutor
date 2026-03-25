using Tutor.Core.Services.Abstractions;

namespace Tutor.MAUI.Services;

/// <summary>
/// MAUI platform implementation of <see cref="IAppDataPathProvider"/>.
/// Resolves to the platform-specific app data directory (e.g., <c>%LOCALAPPDATA%</c> on Windows,
/// <c>~/Library/Application Support</c> on macOS) for persistent file storage.
/// </summary>
public class MauiAppDataPathProvider : IAppDataPathProvider
{
    /// <inheritdoc/>
    public string AppDataDirectory => FileSystem.AppDataDirectory;
}
