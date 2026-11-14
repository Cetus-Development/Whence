using System.Text.Json;

namespace Whence.Instrumentation;

public static class ProjectAssemblyLocator
{
    public static List<string> Find(string depsJsonPath)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(depsJsonPath));
        JsonElement root = doc.RootElement;

        // Step 1: which entries are our own code? -> "libraries"
        JsonElement libraries = root.GetProperty("libraries");
        List<string> projectNames = new List<string>();

        foreach (JsonProperty library in libraries.EnumerateObject())
        {
            string name = library.Name;
            string? type = library.Value.GetProperty("type").GetString();

            if (type == "project")
                projectNames.Add(name);
        }

        // Step 2: which DLL files belong to these entries? -> "targets"
        JsonElement targets = root.GetProperty("targets");
        string frameworkName = root.GetProperty("runtimeTarget").GetProperty("name").GetString()!;
        JsonElement framework = targets.GetProperty(frameworkName);
        List<string> dllNames = new List<string>();

        foreach (string projectName in projectNames)
        {
            JsonElement runtime = framework.GetProperty(projectName).GetProperty("runtime");

            foreach (JsonProperty dll in runtime.EnumerateObject())
                dllNames.Add(dll.Name);
        }

        return dllNames;
    }
}
