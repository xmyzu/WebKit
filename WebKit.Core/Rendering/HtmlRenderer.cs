using WebKit.Core.Resources;

namespace WebKit.Core.Rendering;

[PublicAPI]
public sealed class HtmlRenderer : IRenderer
{
    public async Task<RenderedPage> Render(Page page, ResourceProvider provider)
    {
        var content = await File.ReadAllTextAsync(page.FilePath);

        // Clone the properties
        var properties = provider.WebKitJson.Properties.Clone();

        var processedContent = await CommonRenderer.EvalExpressions(content, properties);
        
        properties.Add("Content", processedContent);
        
        var finalPage = await CommonRenderer.EvalExpressions(
            await provider.Layout.LoadTextAsync(), properties);

        return new RenderedPage
        {
            Resource = page,
            Content = finalPage
        };
    }
}