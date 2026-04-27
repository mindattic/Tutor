using Tutor.Core.Services.Abstractions;

namespace Tutor.Cli.Services;

public sealed class CliAppDataPathProvider : IAppDataPathProvider
{
    public string AppDataDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor");
}
