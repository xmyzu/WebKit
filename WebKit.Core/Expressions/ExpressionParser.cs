using System.Text;

namespace WebKit.Core.Expressions;

[PublicAPI]
public static class ExpressionParser
{
    public static Expression Parse(string input)
    {
        // Trim {{ and }}
        var content = input.Trim();
        
        if (content.StartsWith("{{")) content = content[2..];
        if (content.EndsWith("}}")) content = content[..^2];
        
        content = content.Trim();

        // Detect setter expression (has '=')
        if (!content.Contains('='))
        {
            // Getter expression must be a single property
            
            return new GetterExpression(StripDot(content));
        }
        
        var parts = content.Split('=', 2);
        var left = parts[0].Trim(); // e.g. .Title
        var right = parts[1]; // keep spaces as-is

        var exprParts = TokenizeParts(right);
        
        return new SetterExpression(StripDot(left), exprParts);
    }

    private static List<IExpressionPart> TokenizeParts(string input)
    {
        var parts = new List<IExpressionPart>();
        var buffer = new StringBuilder();
        var i = 0;

        while (i < input.Length)
        {
            if (input[i] == '.')
            {
                // Flush any literal buffer first
                if (buffer.Length > 0)
                {
                    parts.Add(new LiteralPart(buffer.ToString()));
                    buffer.Clear();
                }

                i++;
                var propBuffer = new StringBuilder();
                while (i < input.Length && char.IsLetterOrDigit(input[i]))
                {
                    propBuffer.Append(input[i]);
                    i++;
                }

                if (propBuffer.Length > 0)
                    parts.Add(new PropertyPart(propBuffer.ToString()));
                else
                    buffer.Append('.'); // lone '.' counts as literal
            }
            else
            {
                buffer.Append(input[i]);
                i++;
            }
        }

        if (buffer.Length > 0)
            parts.Add(new LiteralPart(buffer.ToString()));

        return parts;
    }

    private static string StripDot(string input)
    {
        return input.StartsWith('.') ? input[1..] : input;
    }
}