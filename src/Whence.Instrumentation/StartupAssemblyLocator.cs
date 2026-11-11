namespace Whence.Instrumentation;

public static class StartupAssemblyLocator
{
    public static string Find(string appFolder)
    {
        string[] matches = Directory.GetFiles(appFolder, "*.runtimeconfig.json");

        if (matches.Length > 1)
            throw new InvalidOperationException("Multiple .runtimeconfig.json found");

        if (matches.Length < 1)
            throw new FileNotFoundException("No .runtimeconfig.json found");

        string path = matches[0];
        string fileName = Path.GetFileName(path);
        string appName = fileName.Replace(".runtimeconfig.json", "");
        return Path.Combine(appFolder, appName + ".dll");
    }
}