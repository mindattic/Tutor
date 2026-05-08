using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

/// <summary>
/// Blazor host implementation of <see cref="IAppDataPathProvider"/>. Resolves to
/// %LocalAppData%\Tutor — the canonical on-disk root shared with the CLI.
/// </summary>
public class BlazorAppDataPathProvider : IAppDataPathProvider
{
    /// <inheritdoc />
    public string AppDataDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor");
}
