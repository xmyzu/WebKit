namespace WebKit.Core;

public static class Scripts
{
    public const string Debugger = """
                                   <script>
                                       (function() {
                                         const socket = new WebSocket("ws://localhost:3000/livereload");
                                         socket.onmessage = (event) => {
                                           if (event.data === "reload") {
                                             console.log("[LiveReload] 🔥 Reloading page...");
                                             location.reload();
                                           }
                                         };
                                       })();
                                   </script>
                                   """;
}