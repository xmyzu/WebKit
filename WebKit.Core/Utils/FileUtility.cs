namespace WebKit.Core.Utils;

[PublicAPI]
public static class FileUtility
{
    public static async Task Write(string path, string content)
    {
        EnsureDirectoryExists(path);

        await File.WriteAllTextAsync(path, content);
    }

    public static void Copy(string source, string destination, bool overwrite = true)
    {
        EnsureDirectoryExists(destination);
        
        File.Copy(source, destination, overwrite: overwrite);
    }

    private static void EnsureDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            // Ensure the destination directory exists.
            Directory.CreateDirectory(directory!);
        }
    }
}