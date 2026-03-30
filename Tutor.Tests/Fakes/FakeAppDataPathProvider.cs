using Tutor.Core.Services.Abstractions;

namespace Tutor.Tests.Fakes;

/// <summary>
/// Provides a temp directory for tests, cleaned up on dispose.
/// </summary>
public class FakeAppDataPathProvider : IAppDataPathProvider, IDisposable
{
    public string AppDataDirectory { get; }

    public FakeAppDataPathProvider()
    {
        AppDataDirectory = Path.Combine(Path.GetTempPath(), "TutorTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(AppDataDirectory);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(AppDataDirectory))
                Directory.Delete(AppDataDirectory, recursive: true);
        }
        catch { /* best effort cleanup */ }
    }
}
