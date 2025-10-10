using System.Diagnostics.CodeAnalysis;
using BosonWare;
using BosonWare.TUI;
using Markdig;
using WebKit.Core.Rendering;

namespace WebKit.Core.Resources;

[PublicAPI]
public sealed class Properties : Dictionary<string, string>
{
    public ResourceProvider? ResourceProvider { get; set; }

    public Properties() : base(StringComparer.OrdinalIgnoreCase) { }

    public Properties(IDictionary<string, string> dictionary) 
        : base(dictionary, StringComparer.OrdinalIgnoreCase) { }

    public async Task<string?> TryGetPropertyAsync(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (ResourceProvider is null)
            throw new InvalidOperationException("ResourceProvider must not be null when resolving properties.");

        if (!GetValue(key, out var value))
            return null;

        if (!value.StartsWith(Syntax.SharedProperty, StringComparison.OrdinalIgnoreCase))
            return value;

        var relative = value[Syntax.SharedProperty.Length..].Trim();
        var path = Path.Combine(ResourceProvider.ResourcesPath, Paths.SharedFolder, relative);

        TUIConsole.WriteLine(
            $"[[Red]+[/]] Loading Shared Resource at [Dim]{Path.GetRelativePath(ResourceProvider.BasePath, path)}[/]");

        return await GetSharedFileCachedAsync(path) ?? LogMissing(path, value);
    }

    private bool GetValue(string key, [NotNullWhen(true)] out string? value)
    {
        if (!TryGetValue(key, out value))
            return false;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim();
        return true;
    }

    public Properties Clone() => new(this) { ResourceProvider = ResourceProvider };

    private async Task<string?> GetSharedFileCachedAsync(string path) =>
        await Cache<string?>.GetAsync(
            path, 
            async () => await TryGetSharedFile(path), 
            TimeSpan.FromSeconds(1));

    private async Task<string?> TryGetSharedFile(string path)
    {
        var fileToRead = Path.GetExtension(path) switch
        {
            { Length: > 0 } when File.Exists(path) => path,
            _ when File.Exists(path + ".html") => path + ".html",
            _ when File.Exists(path + ".md") => path + ".md",
            _ => null
        };

        if (fileToRead is null) return null;

        var content = await File.ReadAllTextAsync(fileToRead);
        
        return await CommonRenderer.EvalExpressions(fileToRead.EndsWith(".md", StringComparison.OrdinalIgnoreCase)
            ? Markdown.ToHtml(content)
            : content, this);
    }

    private static string LogMissing(string path, string fallback)
    {
        SmartConsole.LogWarning($"Shared resource file not found: {path}");
        
        return fallback;
    }
}