var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "OAuth Server is running.");

app.Run();
