using Microsoft.AspNetCore.Http.HttpResults;


static bool isvalid()
{
    return true;
}


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.WebHost.UseKestrel(options => // Configure HTTP 
{
    // Set the HTTP port to 5000
    options.ListenLocalhost(5294);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next.Invoke();
    if (context.Response.StatusCode >= 400)
    {
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: {context.Response.StatusCode}");
    }
});


app.Use(async (context, next) =>
{
    if (
        context.Request.Query["secure"] != "true" )
    {
        // If the query parameter "Secure" is not set to "true", return a 403 Forbidden response
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Simulated HTTPS required Access denied. Secure connection required.");
        return;
    }

    // Call the next middleware in the pipeline
    await next.Invoke();
});


app.Use(async (context, next) =>
{
    var maliciousPatterns = new[]
       {
            "<script>", "javascript:", "DROP TABLE", "SELECT * FROM"
        };

    foreach (var queryEntry in context.Request.Query) {
        if (maliciousPatterns.Any(pattern => queryEntry.Value.ToString().Contains(pattern, StringComparison.OrdinalIgnoreCase)))
        {
            context.Response.StatusCode = 400; // Bad Request
            await context.Response.WriteAsync("Malicious content detected in request body.");
            return;
        }
    }

    // Call the next middleware in the pipeline
    await next.Invoke();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path.Value!.Contains("/unauthorized"))
    {
        // If the request path starts with "/unauthorized", return a 401 Unauthorized response
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized access.");
        return;
    }

    // Call the next middleware in the pipeline
    await next.Invoke();
});


app.Use(async (context, next) =>
{
    await context.Response.WriteAsync("You are being served thank for pinging us");
    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


