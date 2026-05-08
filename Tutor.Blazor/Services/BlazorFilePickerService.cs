using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

/// <summary>
/// Placeholder <see cref="IFilePickerService"/> for the Blazor Server host. Real
/// file picking happens through the <c>InputFile</c> component / JavaScript interop
/// inside CourseManagement; this service exists so DI registrations resolve, and
/// it always returns null.
/// </summary>
public class BlazorFilePickerService : IFilePickerService
{
    /// <inheritdoc />
    public Task<PickedFile?> PickFileAsync(string[] allowedExtensions)
    {
        return Task.FromResult<PickedFile?>(null);
    }
}
