namespace WebKit.Core.Resources;

public sealed class StaticResource : Resource
{
    public bool SupportsExpressions => Extension.ToLower() switch
    {
        ".js" or "js" or ".css" or "css" => true,
        _ => false
    };
    
    public required string RelativePath { get; init; }
}