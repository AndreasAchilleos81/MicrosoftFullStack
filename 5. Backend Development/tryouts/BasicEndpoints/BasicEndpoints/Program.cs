var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello kokos!");
app.MapGet("/downloads", () => "Downloads page");
app.MapPut("/", () => "Put request received");
app.MapPost("/", () => "Post request received");
app.MapDelete("/", () => "Delete request received");

app.Run();


