using Tutor.Core.Services.Abstractions;

namespace Tutor.Cli.Services;

/// <summary>
/// No-op file picker used by the CLI. Tutor.Core requires <see cref="IFilePickerService"/>
/// to be registered, but the CLI takes file paths as arguments instead of opening a dialog,
/// so this implementation always returns null.
/// </summary>
public sealed class CliFilePickerService : IFilePickerService
{
    /// <inheritdoc />
    public Task<PickedFile?> PickFileAsync(string[] allowedExtensions)
        => Task.FromResult<PickedFile?>(null);
}
