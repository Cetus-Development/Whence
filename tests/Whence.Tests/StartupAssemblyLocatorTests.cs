using Whence.Instrumentation;

namespace Whence.Tests;

public class StartupAssemblyLocatorTests
{
    [Fact]
    public void Find_FolderWithOneRuntimeConfig_ReturnsMatchingDll()
    {
        var folder = Directory.CreateTempSubdirectory();
        var path = folder.FullName;

        File.WriteAllText(Path.Combine(path, "MyApp.dll"), "");
        File.WriteAllText(Path.Combine(path, "MyApp.runtimeconfig.json"), "");
        File.WriteAllText(Path.Combine(path, "Library.dll"), "");

        var result = StartupAssemblyLocator.Find(path);

        Assert.Equal(Path.Combine(path, "MyApp.dll"), result);
    }

    [Fact]
    public void Find_FolderWithoutRuntimeConfig_ThrowsFileNotFoundException()
    {
        var folder = Directory.CreateTempSubdirectory();
        var path = folder.FullName;

        Assert.Throws<FileNotFoundException>(() => StartupAssemblyLocator.Find(path));
    }

    [Fact]
    public void Find_FolderWithMultipleRuntimeConfig_ThrowsInvalidOperationException()
    {
        var folder = Directory.CreateTempSubdirectory();
        var path = folder.FullName;

        File.WriteAllText(Path.Combine(path, "AppOne.runtimeconfig.json"), "");
        File.WriteAllText(Path.Combine(path, "AppTwo.runtimeconfig.json"), "");

        Assert.Throws<InvalidOperationException>(() => StartupAssemblyLocator.Find(path));
    }
}