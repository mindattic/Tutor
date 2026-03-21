namespace Tutor.Core.Services.Abstractions;

public record PickedFile(string FileName, Stream Stream);

public interface IFilePickerService
{
    Task<PickedFile?> PickFileAsync(string[] allowedExtensions);
}
