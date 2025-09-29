using WebKit.Core.Resources;

namespace WebKit.Core.Rendering;

[PublicAPI]
public interface IRenderer
{
    public Task<RenderedPage> Render(Page page, ResourceProvider provider);

    public static IRenderer Create(string extension)
    {
        return extension.ToLower() switch
        {
            ".html" or "html" => new HtmlRenderer(),
            ".md" or "md" => new MarkdownRenderer(),
            _ => throw new NotSupportedException($"Extension {extension} not supported")
        };
    }
}