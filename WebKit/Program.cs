using BosonWare;
using Cocona;
using WebKit.Commands;

Application.Initialize<Program>();

var builder = CoconaLiteApp.CreateBuilder();

var app = builder.Build();

app.AddCommands<NewCommand>();
app.AddCommands<BuildCommand>();
app.AddCommands<CleanCommand>();
app.AddCommands<ServeCommand>();
app.AddCommands<CacheSection>();

await app.RunAsync();