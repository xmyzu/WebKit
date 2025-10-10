namespace WebKit.Core;

[PublicAPI]
public interface IWebKitBuilder
{
    public Task<Result> BuildAsync(string path, bool isDebugBuild);
}