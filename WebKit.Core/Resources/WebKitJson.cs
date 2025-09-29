namespace WebKit.Core.Resources;

[PublicAPI]
public sealed class WebKitJson
{
    public Properties Properties { get; set; } = [];

    public Result Validate(Result result)
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
}