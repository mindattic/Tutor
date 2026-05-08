using Tutor.Core.Services.Abstractions;

namespace Tutor.Cli.Services;

/// <summary>
/// CLI implementation of <see cref="IAppDataPathProvider"/> that resolves to the same
/// %LocalAppData%\Tutor directory the Blazor host uses, so courses created from the
/// CLI are visible in the UI without translation.
/// </summary>
public sealed class CliAppDataPathProvider : IAppDataPathProvider
{
    /// <inheritdoc />
    public string AppDataDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor");
}
