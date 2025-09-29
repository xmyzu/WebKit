using System.Diagnostics.CodeAnalysis;
using WebKit.Core.Utils;

namespace WebKit.Core.Resources;

[PublicAPI]
public sealed class ResourceProvider
{
    public required string BasePath { get; init; }

    [field: AllowNull, MaybeNull]
    public string ResourcesPath => field ??= Path.Combine(BasePath, Paths.ResourcesFolder);

    public List<Resource> Resources { get; } = [];

    public IEnumerable<Page> Pages => Resources.OfType<Page>();

    public IEnumerable<StaticResource> StaticResources => Resources.OfType<StaticResource>();

    public LayoutResource Layout { get; private set; } = null!;

    public WebKitJson WebKitJson { get; private set; } = null!;

    public async Task<Result> ProbeAsync()
    {
        var result = new Result();

        if (!Directory.Exists(ResourcesPath))
            return result.WithErrors("Resources directory does not exist");

        // Handle Pages
        foreach (var filePath in GetFiles(result, ResourcesPath, Paths.PagesFolder, "md;html"))
            Resources.Add(new Page { FilePath = filePath });

        // Handle Static Resources
        foreach (var filePath in GetFiles(result, ResourcesPath, Paths.StaticFolder))
        {
            var relativePath = Path.GetRelativePath(
                Path.Combine(ResourcesPath, Paths.StaticFolder), filePath);

            Resources.Add(new StaticResource
            {
                FilePath = filePath,
                RelativePath = relativePath
            });
        }

        // Handle Layout.html
        var layoutPath = Path.Combine(ResourcesPath, Paths.LayoutFile);
        if (!File.Exists(layoutPath))
            return result.WithErrors("Layout file does not exist");

        Layout = new LayoutResource { FilePath = layoutPath };
        Resources.Add(Layout);

        // Handle webkit.json
        var webkitPath = Path.Combine(BasePath, Paths.WebKitFile);
        if (!File.Exists(webkitPath))
            return result.WithErrors("webkit.json file does not exist");

        WebKitJson = (await File.ReadAllTextAsync(webkitPath)).FromJson<WebKitJson>();
        WebKitJson.Properties.ResourceProvider = this;

        return WebKitJson.Validate(result);
    }

    private static IEnumerable<string> GetFiles(
        Result result,
        string basePath,
        string folder,
        string searchPattern = "")
    {
        var fullPath = Path.Combine(basePath, folder);

        if (!Directory.Exists(fullPath))
        {
            result.WithErrors($"{folder} directory does not exist");
            yield break;
        }

        if (searchPattern.Contains(';'))
        {
            var allowedExtensions = searchPattern.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var file in Directory.EnumerateFiles(fullPath))
            {
                var extension = Path.GetExtension(file);
                if (allowedExtensions.Any(x =>
                        extension.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    yield return file;
                }
            }
            yield break;
        }

        foreach (var file in Directory.EnumerateFiles(fullPath))
            yield return file;
    }
}