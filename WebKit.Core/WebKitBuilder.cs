using WebKit.Core.Rendering;
using WebKit.Core.Resources;
using WebKit.Core.Utils;

namespace WebKit.Core;

public sealed class WebKitBuilder : IWebKitBuilder
{
    public async Task<Result> BuildAsync(string path, bool isDebugBuild)
    {
        ArgumentNullException.ThrowIfNull(path);

        var outputPath = Path.Combine(path, Paths.GetProperBuildFolder(isDebugBuild: isDebugBuild));

        var resourceProvider = new ResourceProvider { BasePath = path };

        var result = await resourceProvider.ProbeAsync();

        if (result.HasErrors)
        {
            return result.WithErrors($"Build failed.");
        }

        if (isDebugBuild)
        {
            resourceProvider.WebKitConfig.Properties.Add("Debugger", Scripts.Debugger);
        }
        
        var renderedPages = new List<RenderedPage>();

        foreach (var page in resourceProvider.Pages)
        {
            var renderer = IRenderer.Create(page.Extension);
            
            renderedPages.Add(await renderer.Render(page, resourceProvider));
        }

        foreach (var renderedPage in renderedPages)
        {
            var destination = Path.Combine(outputPath, 
                renderedPage.Resource.IsIndex 
                    ? "index.html" 
                    : renderedPage.Resource.FileName + ".html");
            
            await FileUtility.Write(destination, renderedPage.Content);
        }

        foreach (var staticResource in resourceProvider.StaticResources)
        {
            var destination = Path.Combine(outputPath, staticResource.RelativePath);
            
            if (staticResource.SupportsExpressions)
            {
                var content = await staticResource.LoadTextAsync();

                content = await CommonRenderer.EvalExpressions(content, 
                    resourceProvider.WebKitConfig.Properties.Clone());
                
                await FileUtility.Write(destination, content);
                
                continue;
            }
            
            FileUtility.Copy(staticResource.FilePath, destination);
        }

        return result;
    }
}