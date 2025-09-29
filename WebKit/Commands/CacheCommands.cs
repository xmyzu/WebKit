using BosonWare;
using BosonWare.TUI;
using Cocona;

namespace WebKit.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class CacheCommands
{
    [Command("clear", Description = "Clears the cache")]
    public static void Clear()
    {
        var cachePath = Application.GetDirectory("cache");

        if (Directory.Exists(cachePath))
        {
            Directory.Delete(cachePath, true);
        }
    }
    
    [Command("view", Description = "View the cache")]
    public static void View()
    {
        var cachePath = Application.GetDirectory("cache");

        if (!Directory.Exists(cachePath))
        {
            TUIConsole.WriteLine("Cache directory not found");
            
            return;
        }

        foreach (var file in Directory.GetFiles(cachePath))
        {
            TUIConsole.WriteLine($"[[Red]+[/]] {Path.GetFileName(file)}");
        }
    }
}