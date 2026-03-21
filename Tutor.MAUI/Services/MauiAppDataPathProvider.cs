using Tutor.Core.Services.Abstractions;

namespace Tutor.MAUI.Services;

public class MauiAppDataPathProvider : IAppDataPathProvider
{
    public string AppDataDirectory => FileSystem.AppDataDirectory;
}
