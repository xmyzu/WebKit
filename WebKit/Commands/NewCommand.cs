using System.IO.Compression;
using BosonWare.TUI;
using Cocona;
using WebKit.Core.Utils;

namespace WebKit.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class NewCommand
{
    [Command("new", Aliases = ["init"], Description = "Creates a static Web application")]
    public static async Task New(
        [Option('n', Description = "The name of the website")]
        string name = "MyWebKitProject",
        [Option('t', Description = "The template to use (zip file path or template name).")]
        string template = "default-v1",
        [Option("cdn", Description = "The base CDN URL for templates (default: https://cdn.bosonware.org)")]
        string cdn = "https://cdn.bosonware.org")
    {
        Stream? templateStream;

        // Case 1: Local template file
        if (File.Exists(template) && Path.GetExtension(template).Equals(".zip", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                templateStream = File.OpenRead(template);
                SmartConsole.LogInfo($"📦 Using local template: {template}");
            }
            catch (Exception ex)
            {
                SmartConsole.LogError($"❌ Failed to open local template '{template}': {ex.Message}");
                return;
            }
        }
        else
        {
            // Case 2: CDN download
            var cacheKey = $"{template.ToLower()}.zip";
            var url = $"{cdn.TrimEnd('/')}/webkit-{cacheKey}";

            try
            {
                var bytes = await Http.DownloadAsync(url, cacheKey);
                templateStream = new MemoryStream(bytes);
                SmartConsole.LogInfo($"🌐 Downloaded template from {url}");
            }
            catch (HttpRequestException e)
            {
                SmartConsole.LogError($"❌ Failed to download template '{template}' from CDN '{cdn}': {e.Message}");
                return;
            }
        }

        using var archive = new ZipArchive(templateStream, ZipArchiveMode.Read);
        var basePath = Path.Combine(Environment.CurrentDirectory, name);

        foreach (var entry in archive.Entries)
        {
            if (entry.FullName.EndsWith('/'))
                continue; // skip directories

            await using var sourceStream = entry.Open();

            // Strip root folder in zip (Should always exist)
            var relativePath = Path.GetRelativePath(entry.FullName.Split('/')[0], entry.FullName);
            var destinationPath = Path.Combine(basePath, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

            await using var destinationStream = File.Create(destinationPath);
            await sourceStream.CopyToAsync(destinationStream);
        }

        var configPath = Path.Combine(basePath, "webkit.json");
        try
        {
            var txt = await File.ReadAllTextAsync(configPath);
            txt = txt.Replace("@PROJECT_NAME", name);
            await File.WriteAllTextAsync(configPath, txt);
        }
        catch (Exception ex)
        {
            SmartConsole.LogError($"⚠️ Failed to update '@PROJECT_NAME' in 'webkit.json': {ex.Message}");
        }
    }
}