using BosonWare.TUI;
using Cocona;
using WebKit.Core;
using WebKit.HttpServer;
using System.Net.WebSockets;
using System.Collections.Concurrent;

namespace WebKit.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class ServeCommand
{
    private static readonly ConcurrentBag<WebSocket> Clients = [];

    [Command("serve", Description = "Serves a static Web application with optional hot-reload")]
    public static async Task Build(
        [Option('h')] string host = "localhost",
        [Option('p')] int port = 3000,
        [Option(Description = "Skip build process.")] bool noBuild = false,
        [Option(Description = "Enable hot-reload with live refresh.")] bool watch = true)
    {
        if (!noBuild)
        {
            var exitCode = await BuildCommand.Build(debug: watch);
            if (exitCode != 0)
                return;
        }

        var serverHost = new ServerHost(
            Path.Combine(Environment.CurrentDirectory, Paths.BuildFolder),
            host,
            port);

        using var cancellationSource = new CancellationTokenSource();

        Console.CancelKeyPress += (_, _) =>
        {
            cancellationSource.Cancel();
        };

        if (watch)
        {
            StartWatchThread(cancellationSource.Token);
            
            serverHost.AddWebSocketEndpoint("/livereload", HandleWebSocketClient);
        }

        await serverHost.RunAsync(cancellationSource.Token);
    }

    private static void StartWatchThread(CancellationToken stoppingToken)
    {
        new Thread(() =>
        {
            using var watcher = new FileSystemWatcher(Environment.CurrentDirectory);
            watcher.NotifyFilter = NotifyFilters.LastWrite
                                   | NotifyFilters.FileName
                                   | NotifyFilters.CreationTime;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            var debounceTimer = new System.Timers.Timer(500) { AutoReset = false };
            var rebuildRequested = false;
            var rebuildLock = new object();

            watcher.Changed += (_, args) =>
            {
                if (args.FullPath.Contains(Path.Combine(Environment.CurrentDirectory, Paths.BuildFolder)))
                    return;

                lock (rebuildLock)
                {
                    rebuildRequested = true;
                    debounceTimer.Stop();
                    debounceTimer.Start();
                }
            };

            debounceTimer.Elapsed += async (_, _) =>
            {
                lock (rebuildLock)
                {
                    if (!rebuildRequested) return;
                    rebuildRequested = false;
                }

                try
                {
                    SmartConsole.LogInfo("[Cyan][Hot Reload][/] Changes detected. Rebuilding...");
                    var exitCode = await BuildCommand.Build(debug: true);

                    if (exitCode == 0)
                    {
                        SmartConsole.LogInfo("[Green][Hot Reload][/] Build succeeded! Notifying clients...");
                        await NotifyClientsAsync("reload");
                    }
                    else
                        SmartConsole.LogError("[Red][Hot Reload][/] Build failed.");
                }
                catch (Exception ex)
                {
                    SmartConsole.LogError($"[Red][Hot Reload][/] Exception during rebuild: {ex}");
                }
            };

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(500);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                debounceTimer.Dispose();
            }
        })
        {
            IsBackground = true
        }.Start();
    }

    private static async Task HandleWebSocketClient(WebSocket socket, CancellationToken token)
    {
        Clients.Add(socket);
        SmartConsole.LogInfo("[Magenta][LiveReload][/] Client connected.");

        var buffer = new byte[1024];
        try
        {
            while (socket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                await socket.ReceiveAsync(buffer, token); // Just keep it alive
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            SmartConsole.LogError($"[Red][LiveReload][/] WebSocket error: {ex}");
        }
        finally
        {
            Clients.TryTake(out _);
            if (socket.State == WebSocketState.Open)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutting down", token);
        }
    }

    private static async Task NotifyClientsAsync(string message)
    {
        var deadSockets = new List<WebSocket>();
        var data = System.Text.Encoding.UTF8.GetBytes(message);

        foreach (var client in Clients)
        {
            try
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    deadSockets.Add(client);
                }
            }
            catch
            {
                deadSockets.Add(client);
            }
        }

        foreach (var dead in deadSockets)
            Clients.TryTake(out _);
    }
}