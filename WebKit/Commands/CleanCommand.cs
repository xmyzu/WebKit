using BosonWare.TUI;
using Cocona;
using WebKit.Core;
using WebKit.Core.Resources;

namespace WebKit.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class CleanCommand
{
    [Command("clean", Description = "Cleans the build output")]
    public static void Clean([Option] EnvironmentMode? env = null)
    {
        var path = Path.Combine(
            Environment.CurrentDirectory, 
            env is not null 
                ? Paths.GetProperBuildFolder(env: env.Value) 
                : Paths.BuildFolder);

        if (!Directory.Exists(path)) return;

        Directory.Delete(path, true);

        TUIConsole.WriteLine($"Cleaned up [Violet]{path}[/]");
    }
}