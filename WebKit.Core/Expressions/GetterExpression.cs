using WebKit.Core.Resources;

namespace WebKit.Core.Expressions;

// Getter expression: {{ .Author }}
[PublicAPI]
public sealed class GetterExpression(string propertyName) : Expression
{
    public string PropertyName { get; } = propertyName;
    
    public override async Task<string> Evaluate(Properties properties)
    {
        return await properties.TryGetPropertyAsync(PropertyName) ?? "";
    }
}