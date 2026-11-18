namespace Whence.Instrumentation
{
    public static class AppFolderCopier
    {
        public static void Copy(string sourceFolder, string targetFolder)
        {
            Directory.CreateDirectory(targetFolder);

            string[] files = Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string relativePath = Path.GetRelativePath(sourceFolder, file);
                string targetFile = Path.Combine(targetFolder, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
                File.Copy(file, targetFile, overwrite: true);
            }
        }
    }
}