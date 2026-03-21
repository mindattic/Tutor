using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

public class BlazorAppDataPathProvider : IAppDataPathProvider
{
    public string AppDataDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor");
}
