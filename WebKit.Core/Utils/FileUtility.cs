namespace WebKit.Core.Utils;

[PublicAPI]
public static class FileUtility
{
    public static async Task Write(string path, string content)
    {
        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
        
        await File.WriteAllTextAsync(path, content);
    }
    
    public static void Copy(string source, string destination)
    {
        File.Copy(source, destination);
    }
}