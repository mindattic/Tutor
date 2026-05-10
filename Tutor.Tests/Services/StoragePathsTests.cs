using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class StoragePathsTests
{
    [Test]
    public void SettingsDirectory_IsRootedUnderLocalAppData()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        Assert.That(StoragePaths.SettingsDirectory, Does.StartWith(localAppData));
        Assert.That(StoragePaths.SettingsDirectory, Does.Contain("Tutor"));
        Assert.That(StoragePaths.SettingsDirectory, Does.EndWith("Settings"));
    }

    [Test]
    public void StorageSettingsPath_LivesInSettingsDirectory()
    {
        Assert.That(Path.GetDirectoryName(StoragePaths.StorageSettingsPath),
            Is.EqualTo(StoragePaths.SettingsDirectory));
        Assert.That(Path.GetFileName(StoragePaths.StorageSettingsPath),
            Is.EqualTo("storage.settings.json"));
    }

    [Test]
    public void VectorStorePath_LivesInSettingsDirectory()
    {
        Assert.That(Path.GetDirectoryName(StoragePaths.VectorStorePath),
            Is.EqualTo(StoragePaths.SettingsDirectory));
        Assert.That(Path.GetFileName(StoragePaths.VectorStorePath),
            Is.EqualTo("content_chunks.json"));
    }

    [Test]
    public void DefaultLearningResourcesDirectory_EndsWithLearningResources()
    {
        var full = Path.GetFullPath(StoragePaths.DefaultLearningResourcesDirectory);
        Assert.That(full, Does.EndWith("LearningResources"));
    }
}
