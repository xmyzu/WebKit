using BosonWare;
using System.Net.Http.Headers;
using BosonWare.TUI;

namespace WebKit.Core.Utils;

[PublicAPI]
public static class Http
{
    private static readonly HttpClient Client = new();

    static Http()
    {
        // Set a nice default UA for CDN analytics / debugging
        Client.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("WebKitCLI", "1.0")
        );
    }

    public static async Task<byte[]> DownloadAsync(string url, string? cacheKey = null, bool forceDownload = false)
    {
        if (string.IsNullOrEmpty(cacheKey))
        {
            return await Client.GetByteArrayAsync(url);
        }

        // Sanitize cache key (avoid weird slashes/paths)
        var safeKey = cacheKey.Replace("/", "_").Replace("\\", "_");
        var cachePath = Application.GetPath("cache", safeKey);

        if (!forceDownload && File.Exists(cachePath))
        {
            SmartConsole.LogInfo($"📦 Cache hit: {safeKey}");
            return await File.ReadAllBytesAsync(cachePath);
        }

        try
        {
            SmartConsole.LogInfo($"🌐 Downloading {url}...");
            var bytes = await Client.GetByteArrayAsync(url);

            Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
            await File.WriteAllBytesAsync(cachePath, bytes);

            SmartConsole.LogInfo($"✅ Cached: {safeKey}");
            return bytes;
        }
        catch (Exception ex)
        {
            SmartConsole.LogError($"❌ Failed to download {url}: {ex.Message}");

            // fallback: try to load stale cache if available
            if (!File.Exists(cachePath)) throw;
            
            SmartConsole.LogWarning($"⚠️ Using stale cached copy: {safeKey}");
            
            return await File.ReadAllBytesAsync(cachePath);

        }
    }
}