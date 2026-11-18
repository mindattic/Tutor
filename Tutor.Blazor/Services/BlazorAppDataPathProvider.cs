using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

/// <summary>
/// Blazor host implementation of <see cref="IAppDataPathProvider"/>. Resolves to
/// %LocalAppData%\Tutor (or TUTOR_DATA_DIR on a deployed host) — the canonical on-disk root shared with the CLI.
/// </summary>
public class BlazorAppDataPathProvider : IAppDataPathProvider
{
    /// <inheritdoc />
    public string AppDataDirectory =>
        Tutor.Core.Services.TutorDataRoot.Path;
}
