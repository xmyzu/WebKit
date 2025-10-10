using WebKit.Core.Resources;

namespace WebKit.Core;

[PublicAPI]
public static class Paths
{
    public const string BuildFolder = "build";

    public const string ResourcesFolder = "Resources";

    public const string PagesFolder = "Pages";

    public const string SharedFolder = "Shared";

    public const string StaticFolder = "Static";

    public const string LayoutFile = "Layout.html";

    public const string WebKitXdslFile = "webkit.xdsl";

    public const string WebKitJsonFile = "webkit.json";

    public static string GetProperBuildFolder(EnvironmentMode env) => Path.Combine(
        BuildFolder,
        env == EnvironmentMode.Development
            ? "debug"
            : "release");
    
    public static string GetProperBuildFolder(bool isDebugBuild) 
        => GetProperBuildFolder(isDebugBuild ? EnvironmentMode.Development :  EnvironmentMode.Production);
}