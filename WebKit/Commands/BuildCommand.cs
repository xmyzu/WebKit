using BosonWare.TUI;
using Cocona;
using WebKit.Core;

namespace WebKit.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class BuildCommand
{
    [Command("build", Description = "Builds a static Web application")]
    public static async Task<int> Build(bool debug = false)
    {
        var builder = new WebKitBuilder();

        var startTime = DateTime.UtcNow;
        var result = await builder.BuildAsync(Environment.CurrentDirectory, debug);
        var elapsed = DateTime.UtcNow - startTime;

        if (result.HasErrors)
        {
            result.LogErrors();
            
            return -1;
        }
        
        TUIConsole.WriteLine($"Build complete in [Violet]{elapsed.TotalMilliseconds}[/]ms");

        return 0;
    }
    
    [Command("rebuild", Description = "Cleans and Rebuilds the static website.")]
    public static async Task<int> ReBuild(bool debug = false)
    {
        CleanCommand.Clean();

        return await Build(debug);
    }
}