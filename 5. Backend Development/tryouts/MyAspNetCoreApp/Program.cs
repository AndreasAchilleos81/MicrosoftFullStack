using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddHttpLogging(options => {
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode | 
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestQuery |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestBody |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseHeaders |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseBody;
 });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(error =>
    {
        error.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var errorResponse = new { Message = "An unexpected error occurred." };
            await context.Response.WriteAsJsonAsync(errorResponse);
        });
    });
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine($"FIRST MW LINE 43 Request Path: {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"FIRST MW LINE 45 Response Status Code: {context.Response.StatusCode}");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Second Middleware Line 50");
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    await next.Invoke();
    Console.WriteLine($"Second MW Line 53 Request took {stopwatch.ElapsedMilliseconds} ms");
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/kokos", () => "{\"message\": \"Hello from kokoks!\"}");

app.Run();
