using BosonWare.TUI;

namespace WebKit.Core;

[PublicAPI]
public sealed class Result
{
    public List<string>? Errors { get; private set; }
    
    public bool HasErrors => Errors is not null && Errors.Count != 0;

    public Result WithErrors(params ReadOnlySpan<string> errors)
    {
        Errors ??= [];
        
        Errors.AddRange(errors);

        return this;
    }

    public void LogErrors()
    {
        if (!HasErrors)
        {
            throw new Exception("No errors found");
        }

        foreach (var error in Errors!)
        {
            SmartConsole.LogError(error);
        }
    }
}