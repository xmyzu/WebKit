namespace WebKit.Core.Resources;

public enum EnvironmentMode
{
    Production = 1,
    Development = 2
}

public static class WebEnv
{
    public static EnvironmentMode Mode { get; private set; } = EnvironmentMode.Production;

    public static void SetMode(EnvironmentMode mode)
    {
        Mode = mode;
    }
    
    public static void SetMode(bool debug)
    {
        SetMode(debug ? EnvironmentMode.Development : EnvironmentMode.Production);
    }
}