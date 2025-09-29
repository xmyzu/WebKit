using System.Text;
using WebKit.Core.Expressions;
using WebKit.Core.Resources;

namespace WebKit.Core.Rendering;

[PublicAPI]
public static class CommonRenderer
{
    /// <summary>
    /// Evaluates all expressions inside the given content using the provided properties.
    /// </summary>
    public static async Task<string> EvalExpressions(string content, Properties properties)
    {
        var expressions = ParseExpressions(content, out var indexedContent);

        foreach (var expression in expressions)
        {
            var result = await expression.Evaluate(properties);
            
            indexedContent = indexedContent.Replace(
                expression.Index,
                expression is GetterExpression ? result : string.Empty
            );
        }

        return indexedContent;
    }

    /// <summary>
    /// Parses expressions within the content and replaces them with indexed placeholders.
    /// </summary>
    public static List<Expression> ParseExpressions(
        string content,
        out string indexedContent)
    {
        if (string.IsNullOrEmpty(content))
        {
            indexedContent = string.Empty;
            return [];
        }

        var chars = content.AsSpan();
        var tempExpression = new StringBuilder();
        var indexContentBuilder = new StringBuilder(content.Length);

        var expressions = new List<Expression>();
        var inExpression = false;

        for (var i = 0; i < chars.Length; i++)
        {
            var ch = chars[i];

            switch (ch)
            {
                case '\\': // Escape sequence
                    if (++i < chars.Length)
                        indexContentBuilder.Append(chars[i]);
                    continue;

                case '{' when i + 1 < chars.Length && chars[i + 1] == '{' && !inExpression:
                    inExpression = true;
                    i++; // skip second '{'
                    continue;

                case '}' when i + 1 < chars.Length && chars[i + 1] == '}' && inExpression:
                {
                    // Finalize and parse expression
                    var expression = ExpressionParser.Parse(tempExpression.ToString());
                    expressions.Add(expression);

                    indexContentBuilder.Append(expression.Index);

                    tempExpression.Clear();
                    inExpression = false;
                    i++; // skip second '}'
                    continue;
                }
            }

            if (inExpression)
                tempExpression.Append(ch);
            else
                indexContentBuilder.Append(ch);
        }

        indexedContent = indexContentBuilder.ToString();
        return expressions;
    }
}