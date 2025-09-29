namespace WebKit.Core.Resources;

[PublicAPI]
public static class PropertyUtility
{
    public static bool IsValid(ReadOnlySpan<char> span)
    {
        foreach (var c in span)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }

        return true;
    }
}