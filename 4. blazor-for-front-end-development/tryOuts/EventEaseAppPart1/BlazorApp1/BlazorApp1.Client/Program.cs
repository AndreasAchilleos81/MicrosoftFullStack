using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Shared.Communication;
using Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.BrowserConsole(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}")
	.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddSingleton<SignalRService>();
builder.Services.AddScoped<ApplicationStorage>();
builder.Services.AddScoped<Shared.Services.LoginState>();

await builder.Build().RunAsync();
