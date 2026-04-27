using Tutor.Core.Services.Abstractions;

namespace Tutor.Cli.Services;

// CLI never invokes the file picker — local files are passed as arguments.
public sealed class CliFilePickerService : IFilePickerService
{
    public Task<PickedFile?> PickFileAsync(string[] allowedExtensions)
        => Task.FromResult<PickedFile?>(null);
}
