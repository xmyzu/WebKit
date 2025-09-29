using BosonWare;

namespace WebKit.Core.Resources;

[PublicAPI]
public abstract class Resource
{
    public required string FilePath { get; init; }

    public string Extension => Path.GetExtension(FilePath);

    public string FileName => Path.GetFileNameWithoutExtension(FilePath);
    
    public async Task<string> LoadTextAsync()
    {
        return await Cache<string>.GetAsync(
            $"Webkit.Resource.Static.{FilePath}", 
            async () => await File.ReadAllTextAsync(FilePath), 
            TimeSpan.FromSeconds(1));
    }
    
    public async Task<byte[]> LoadAsync()
    {
        return await File.ReadAllBytesAsync(FilePath);
    }
}