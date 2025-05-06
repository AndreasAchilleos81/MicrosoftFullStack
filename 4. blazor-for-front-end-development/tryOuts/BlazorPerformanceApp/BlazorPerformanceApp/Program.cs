using BlazorPerformanceApp.Components;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level}: {Message}{NewLine}")
	.WriteTo.Console() // Optional: Keep console logging
	.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Logs to a file with daily rolling
	.CreateLogger();

builder.Logging.AddSerilog(); // Replace default logging with Serilog

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
