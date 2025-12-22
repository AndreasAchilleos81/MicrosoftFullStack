var builder = WebApplication.CreateBuilder(args);

var port = builder.Configuration.GetValue<int>("Port", 5001);
var serverName = builder.Configuration.GetValue<string>("ServerName", "Backend1");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(port);
});

var app = builder.Build();

app.MapGet("/", () => $"Hello World FROM: {serverName} on Port: {port}");
app.MapGet("/health", () => $"Healthy {serverName}");

app.Run();
