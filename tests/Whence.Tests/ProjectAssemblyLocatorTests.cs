using Whence.Instrumentation;

namespace Whence.Tests;

public class ProjectAssemblyLocatorTests
{
    [Fact]
    public void Find_ProjectAndPackage_ReturnsOnlyProjectDll()
    {
        var folder = Directory.CreateTempSubdirectory();
        var depsJsonPath = Path.Combine(folder.FullName, "MyApp.deps.json");

        var json = """
        {
          "runtimeTarget": { "name": ".NETCoreApp,Version=v10.0" },
          "targets": {
            ".NETCoreApp,Version=v10.0": {
              "MyApp/1.0.0": {
                "runtime": { "MyApp.dll": {} }
              },
              "Newtonsoft.Json/13.0.3": {
                "runtime": { "lib/net6.0/Newtonsoft.Json.dll": {} }
              }
            }
          },
          "libraries": {
            "MyApp/1.0.0":            { "type": "project" },
            "Newtonsoft.Json/13.0.3": { "type": "package" }
          }
        }
        """;
        File.WriteAllText(depsJsonPath, json);

        var result = ProjectAssemblyLocator.Find(depsJsonPath);

        Assert.Equal(new[] { "MyApp.dll" }, result);
    }

    [Fact]
    public void Find_RuntimeSpecificBuild_UsesTargetFromRuntimeTarget()
    {
        var folder = Directory.CreateTempSubdirectory();
        var depsJsonPath = Path.Combine(folder.FullName, "MyApp.deps.json");

        // A build for a specific OS (dotnet publish -r linux-x64) has two entries
        // in "targets". "runtimeTarget" names the one that applies at runtime.
        var json = """
        {
          "runtimeTarget": { "name": ".NETCoreApp,Version=v10.0/linux-x64" },
          "targets": {
            ".NETCoreApp,Version=v10.0": {
              "MyApp/1.0.0": {
                "runtime": { "Wrong.dll": {} }
              }
            },
            ".NETCoreApp,Version=v10.0/linux-x64": {
              "MyApp/1.0.0": {
                "runtime": { "MyApp.dll": {} }
              }
            }
          },
          "libraries": {
            "MyApp/1.0.0": { "type": "project" }
          }
        }
        """;
        File.WriteAllText(depsJsonPath, json);

        var result = ProjectAssemblyLocator.Find(depsJsonPath);

        Assert.Equal(new[] { "MyApp.dll" }, result);
    }
}
