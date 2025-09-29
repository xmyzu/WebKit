using System.Text.Json;

namespace WebKit.Core.Utils;

[PublicAPI]
public static class JsonUtility
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip
    };
    
    public static T FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, SerializerOptions) ??  throw new JsonException($"Could not deserialize json '{json}'");
    }
}