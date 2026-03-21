using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

public class BlazorFilePickerService : IFilePickerService
{
    public Task<PickedFile?> PickFileAsync(string[] allowedExtensions)
    {
        // File picking in Blazor Server is handled via JavaScript interop
        // and InputFile component. This is a placeholder that returns null.
        // The CourseManagement component should use InputFile for Blazor Server.
        return Task.FromResult<PickedFile?>(null);
    }
}
