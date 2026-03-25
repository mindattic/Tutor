using Tutor.Core.Services.Abstractions;

namespace Tutor.MAUI.Services;

/// <summary>
/// MAUI platform implementation of <see cref="IFilePickerService"/>.
/// Presents the native file picker dialog with platform-specific MIME type mapping,
/// allowing users to select learning resources (PDFs, documents, etc.) for import.
/// </summary>
public class MauiFilePickerService : IFilePickerService
{
    /// <summary>
    /// Opens the native file picker filtered to the specified extensions.
    /// Maps extensions to MIME types for Android compatibility.
    /// Returns <c>null</c> if the user cancels the picker.
    /// </summary>
    public async Task<PickedFile?> PickFileAsync(string[] allowedExtensions)
    {
        var customFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, allowedExtensions },
                { DevicePlatform.macOS, allowedExtensions },
                { DevicePlatform.MacCatalyst, allowedExtensions },
                { DevicePlatform.Android, allowedExtensions.Select(ext => "application/" + ext.TrimStart('.')).ToArray() },
                { DevicePlatform.iOS, allowedExtensions },
            });

        var options = new PickOptions
        {
            FileTypes = customFileType,
            PickerTitle = "Select a learning resource"
        };

        var result = await FilePicker.PickAsync(options);
        if (result == null)
            return null;

        var stream = await result.OpenReadAsync();
        return new PickedFile(result.FileName, stream);
    }
}
