using Whence.Instrumentation;

namespace Whence.Tests;

public class AppFolderCopierTests
{
    [Fact]
    public void Copy_FolderWithSubfolder_CopiesAllFiles()
    {
        var source = Directory.CreateTempSubdirectory().FullName;
        File.WriteAllText(Path.Combine(source, "MyApp.dll"), "");

        Directory.CreateDirectory(Path.Combine(source, "de"));
        File.WriteAllText(Path.Combine(source, "de", "MyApp.resources.dll"), "");

        var target = Path.Combine(Directory.CreateTempSubdirectory().FullName, "copy");
        AppFolderCopier.Copy(source, target);

        Assert.True(File.Exists(Path.Combine(target, "MyApp.dll")));
        Assert.True(File.Exists(Path.Combine(target, "de", "MyApp.resources.dll")));
    }
}