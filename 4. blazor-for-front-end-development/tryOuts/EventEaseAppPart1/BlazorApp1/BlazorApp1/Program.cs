using BlazorApp1.Components;
using Serilog;
using Serilog.Events;

using Shared.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddDataRepositories(builder.Configuration);
builder.Services.AddScoped<Shared.Communication.SignalRService>();

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}", restrictedToMinimumLevel: LogEventLevel.Information)
	.WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}", restrictedToMinimumLevel: LogEventLevel.Information)
	.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}")
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

app.UseStatusCodePagesWithRedirects("/PageNotExists");

app.MapHub<DataHub>("/datahub");

app.Run();