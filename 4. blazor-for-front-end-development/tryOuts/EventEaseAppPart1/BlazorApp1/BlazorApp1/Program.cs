using BlazorApp1.Components;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}", restrictedToMinimumLevel: LogEventLevel.Warning)
	.WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}", restrictedToMinimumLevel: LogEventLevel.Warning)
	.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}")
	.CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}



app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp1.Client._Imports).Assembly);

app.Run();
