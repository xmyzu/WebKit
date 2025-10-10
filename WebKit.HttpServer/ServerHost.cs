using System.Net;
using System.Net.WebSockets;
using System.Text;
using BosonWare.TUI;
using JetBrains.Annotations;

namespace WebKit.HttpServer;

[PublicAPI]
public sealed class ServerHost(string rootPath, string host, int port)
{
    private readonly Dictionary<string, Func<WebSocket, CancellationToken, Task>> _webSocketHandlers = new();

    public string RootPath { get; } = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
    public string Host { get; } = host ?? throw new ArgumentNullException(nameof(host));
    public int Port { get; } = port;

    public void AddWebSocketEndpoint(string path, Func<WebSocket, CancellationToken, Task> handler)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or whitespace.", nameof(path));

        if (!path.StartsWith('/'))
            path = "/" + path;

        _webSocketHandlers[path] = handler;
        SmartConsole.LogInfo($"[Magenta][ServerHost][/] WebSocket endpoint registered at {path}");
    }

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        var url = $"http://{Host}:{Port}/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();

        TUIConsole.WriteLine($"[[Green]+[/]] Server listening on {url}");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    var contextTask = listener.GetContextAsync();
                    context = await contextTask.WaitAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    SmartConsole.LogError($"[Red][ServerHost][/] Listener error: {ex}");
                    continue;
                }

                _ = HandleRequestAsync(context, stoppingToken);
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        // Handle WebSocket upgrade
        if (context.Request.IsWebSocketRequest &&
            _webSocketHandlers.TryGetValue(context.Request.Url!.AbsolutePath, out var wsHandler))
        {
            try
            {
                var wsContext = await context.AcceptWebSocketAsync(null);
                var socket = wsContext.WebSocket;
                await wsHandler(socket, cancellationToken);
            }
            catch (Exception ex)
            {
                SmartConsole.LogError($"[Red][ServerHost][/] WebSocket error: {ex}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Close();
            }

            return;
        }

        // Otherwise serve static file
        var relativePath = context.Request.Url?.PathAndQuery.TrimStart('/');

        if (string.IsNullOrEmpty(relativePath))
        {
            relativePath = "index.html";
        }

        if (Path.GetExtension(relativePath) is "" or null)
        {
            relativePath += ".html";
        }

        var fullPath = Path.Combine(RootPath, relativePath);

        if (!File.Exists(fullPath))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            var notFoundPage = Path.Combine(RootPath, "404.html");

            if (File.Exists(notFoundPage))
            {
                await SendFileAsync(context, notFoundPage, cancellationToken);
            }
            else
            {
                SmartConsole.LogWarning($"[Yellow][ServerHost][/] Missing file: {relativePath}");
                await SendPlainTextAsync(
                    context,
                    $"<h1>404 - Not Found</h1><p>{relativePath} was not found on this server.</p>",
                    cancellationToken
                );
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await SendFileAsync(context, fullPath, cancellationToken);
        }

        context.Response.OutputStream.Close();
    }

    private static async Task SendFileAsync(HttpListenerContext context, string path, CancellationToken cancellationToken)
    {
        var data = await File.ReadAllBytesAsync(path, cancellationToken);
        context.Response.ContentType = MediaTypeUtility.GetMediaType(Path.GetExtension(path));
        context.Response.ContentLength64 = data.Length;
        await context.Response.OutputStream.WriteAsync(data, cancellationToken);
    }

    private static async Task SendPlainTextAsync(HttpListenerContext context, string message, CancellationToken cancellationToken)
    {
        var data = Encoding.UTF8.GetBytes(message);
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.ContentLength64 = data.Length;
        await context.Response.OutputStream.WriteAsync(data, cancellationToken);
    }
}