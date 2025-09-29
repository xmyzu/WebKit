using BosonWare.TUI;
using Cocona;
using WebKit.Core;

namespace WebKit.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class CleanCommand
{
    [Command("clean", Description = "Cleans a static Web application")]
    public static void Clean()
    {
        var path = Path.Combine(Environment.CurrentDirectory, Paths.BuildFolder);

        if (!Directory.Exists(path)) return;
        
        Directory.Delete(path, true);
            
        TUIConsole.WriteLine($"Cleaned up [Violet]{path}[/]");
    }
}