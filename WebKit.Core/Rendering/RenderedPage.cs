using WebKit.Core.Resources;

namespace WebKit.Core.Rendering;

public sealed class RenderedPage
{
    public required Page Resource { get; init; }
    
    public required string Content { get; init; }
}