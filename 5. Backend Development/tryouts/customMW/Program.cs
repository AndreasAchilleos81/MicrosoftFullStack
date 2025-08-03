var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1. Request duration middleware (should be first to measure total time)
app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    await next.Invoke();
    var duration = DateTime.UtcNow - startTime;
    Console.WriteLine($"Duration: {duration} for {context.Request.Path}");
});

// 2. Request/Response logging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});


app.Use(async (context, next) =>
{
    Console.WriteLine("Third Middleware Before next");
    await next.Invoke();
    Console.WriteLine("Third Middleware After next");
});


app.MapGet("/", () => "Hello World!");

app.Run();
