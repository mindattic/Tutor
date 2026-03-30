using Tutor.Core.Services.Abstractions;

namespace Tutor.Tests.Fakes;

public class FakeFilePickerService : IFilePickerService
{
    public PickedFile? NextResult { get; set; }

    public Task<PickedFile?> PickFileAsync(string[] allowedExtensions)
    {
        return Task.FromResult(NextResult);
    }
}
