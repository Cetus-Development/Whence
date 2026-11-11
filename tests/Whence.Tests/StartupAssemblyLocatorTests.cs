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
}