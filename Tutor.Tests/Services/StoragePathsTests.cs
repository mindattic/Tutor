using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class StoragePathsTests
{
    [Fact]
    public void SettingsDirectory_IsRootedUnderLocalAppData()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        Assert.StartsWith(localAppData, StoragePaths.SettingsDirectory);
        Assert.Contains("Tutor", StoragePaths.SettingsDirectory);
        Assert.EndsWith("Settings", StoragePaths.SettingsDirectory);
    }

    [Fact]
    public void StorageSettingsPath_LivesInSettingsDirectory()
    {
        Assert.Equal(StoragePaths.SettingsDirectory,
            Path.GetDirectoryName(StoragePaths.StorageSettingsPath));
        Assert.Equal("storage.settings.json",
            Path.GetFileName(StoragePaths.StorageSettingsPath));
    }

    [Fact]
    public void VectorStorePath_LivesInSettingsDirectory()
    {
        Assert.Equal(StoragePaths.SettingsDirectory,
            Path.GetDirectoryName(StoragePaths.VectorStorePath));
        Assert.Equal("content_chunks.json",
            Path.GetFileName(StoragePaths.VectorStorePath));
    }

    [Fact]
    public void DefaultLearningResourcesDirectory_EndsWithLearningResources()
    {
        var full = Path.GetFullPath(StoragePaths.DefaultLearningResourcesDirectory);
        Assert.EndsWith("LearningResources", full);
    }
}
