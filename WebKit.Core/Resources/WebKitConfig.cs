using BosonWare.TUI;
using Realtin.Xdsl;
using Realtin.Xdsl.Linq;
using WebKit.Core.Utils;

namespace WebKit.Core.Resources;

[PublicAPI]
public sealed class WebKitConfig
{
    public Properties Properties { get; set; } = [];

    private Result Validate(Result result)
    {
        foreach (var (name, _) in Properties)
        {
            if (!PropertyUtility.IsValid(name))
            {
                result.WithErrors($"The property '{name}' is invalid. It can only contain letters and numbers.");
            }
        }
        
        return result;
    }

    public static async Task<WebKitConfig?> TryResolveAsync(
        string basePath, 
        ResourceProvider provider, 
        Result result)
    {
        var config = await ParseConfigurationFromPath(basePath, result);

        if (config is null) return config;
        
        config.Properties.ResourceProvider = provider;
        config.Validate(result);

        return config;
    }

    private static async Task<WebKitConfig?> ParseConfigurationFromPath(string basePath, Result result)
    {
        var configPaths = GetConfigFiles(basePath).ToArray();

        if (configPaths.Length == 0)
        {
            result.WithErrors($"No WebKit files were found at '{basePath}', such as webkit.xdsl or webkit.json, both of which are missing.");

            return null;
        }

        var path = configPaths[0];
        if (configPaths.Length > 1)
        {
            SmartConsole.LogInfo($"{configPaths.Length} WebKit configuration files were found at {basePath}, selecting {path}.");
        }

        return await ParseFromFullPath(path, result);
    }

    private static async Task<WebKitConfig?> ParseFromFullPath(string path, Result result)
    {
        switch (Path.GetExtension(path).ToLower())
        {
            case ".xdsl" or "xdsl":
                var document = await XdslDocument.CreateFromFileAsync(path);

                if (document.IsEmpty || document.Root!.IsEmpty)
                {
                    return new WebKitConfig();
                }
            
                var properties = document.Root.GetChild("Properties")!;

                var config = new WebKitConfig();
                
                foreach (var property in properties.Where(x => x is { NodeType: XdslNodeType.Element, IsSpecialName: false }))
                {
                    if (property.TryGetAttribute("Condition", out var condition)) {
                        if (Enum.TryParse<EnvironmentMode>(condition.Value, out var mode)) {
                            if (mode != WebEnv.Mode)
                                continue;
                        }
                        else {
                            SmartConsole.LogWarning($"Invalid Condition {condition.Value}. Expected `Development` or `Production`.");
                            
                            continue;
                        }
                    }
                    
                    config.Properties.Add(property.Name, property.Text ?? string.Empty);
                }

                return config;
            case ".json" or "json":
                return (await File.ReadAllTextAsync(path)).FromJson<WebKitConfig>();
            default:
                result.WithErrors($"Unknown WebKit configuration file '{path}'.");
                return null;
        }
    }

    private static IEnumerable<string> GetConfigFiles(string basePath)
    {
        var path = Path.Combine(basePath, Paths.WebKitXdslFile);
        
        if (File.Exists(path)) yield return path;
        
        path = Path.Combine(basePath, Paths.WebKitJsonFile);
        
        if (File.Exists(path)) yield return path;
    }
}