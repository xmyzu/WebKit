using Cocona;

namespace WebKit.Commands;

[UsedImplicitly]
[HasSubCommands(typeof(CacheCommands), "cache", Description = "Cache management commands")]
internal sealed class CacheSection;