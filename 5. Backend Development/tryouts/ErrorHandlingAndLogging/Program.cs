using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
// Configure Console logging default ASP.NET Core
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging")); // Read log levels from config

builder.Host.UseSerilog();// Use Serilog for logging
builder.Services.AddControllers();  // to be able to use controllers 
var app = builder.Build();

app.MapControllers();
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error($"GLOBAL ERROR MESSAGE: {Environment.NewLine}{ex.Message}");
        context.Response.StatusCode = 500; // Internal Server Error
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");	}
});

app.MapGet("/", () => "Hello World!");


app.Run();
Log.CloseAndFlush(); // Ensure all logs are flushed before the application exits
