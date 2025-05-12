using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Shared.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.BrowserConsole(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}")
	.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Services.AddSingleton<IEventCardDataService>();

await builder.Build().RunAsync();
