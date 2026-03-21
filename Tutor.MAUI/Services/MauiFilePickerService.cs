using Tutor.Core.Services.Abstractions;

namespace Tutor.MAUI.Services;

public class MauiFilePickerService : IFilePickerService
{
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
