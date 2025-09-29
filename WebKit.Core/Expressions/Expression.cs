using WebKit.Core.Resources;

namespace WebKit.Core.Expressions;

// Abstract base class
[PublicAPI]
public abstract class Expression
{
    public string Index { get; } = Guid.NewGuid().ToString("P");
    
    public abstract Task<string> Evaluate(Properties properties);
}

// Expression parts
[PublicAPI]
public interface IExpressionPart
{
    Task<string> Evaluate(Properties properties);
}

[PublicAPI]
public sealed class LiteralPart(string value) : IExpressionPart
{
    public string Value { get; } = value;

    public Task<string> Evaluate(Properties properties)
    {
        return Task.FromResult(Value);
    }
}

[PublicAPI]
public sealed class PropertyPart(string propertyName) : IExpressionPart
{
    public string PropertyName { get; } = propertyName;

    public async Task<string> Evaluate(Properties properties)
    {
        return await properties.TryGetPropertyAsync(PropertyName) ?? "";
    }
}