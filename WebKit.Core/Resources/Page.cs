namespace WebKit.Core.Resources;

public sealed class Page : Resource
{
    public bool IsIndex
        => FileName.Equals("Index", StringComparison.OrdinalIgnoreCase)
           || FileName.Equals("Home", StringComparison.OrdinalIgnoreCase);
}