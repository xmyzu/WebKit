using System.Text;
using WebKit.Core.Resources;

namespace WebKit.Core.Expressions;

// Setter expression: {{ .Title = .Name .LastName Hello World }}
[PublicAPI]
public sealed class SetterExpression(string target, IEnumerable<IExpressionPart> parts) : Expression
{
    public string TargetProperty { get; } = target;
    public List<IExpressionPart> Parts { get; } = parts.ToList();

    public override async Task<string> Evaluate(Properties properties)
    {
        var builder = new StringBuilder();
        
        foreach (var part in Parts)
        {
            builder.Append(await part.Evaluate(properties));
        }
        
        var result = builder.ToString();
        
        properties[TargetProperty] = result;
        
        return result;
    }
}