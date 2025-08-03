
using DPInjection.interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IMyService, DPInjection.Services.MyService>();
var app = builder.Build();


app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    await myService.LogCreation("First middleware called");
    await next();
});


app.MapGet("/", (IMyService myService) => 
{
	myService.LogCreation("Accessing the service endpoint.");
	return "Service accessed successfully!";
});


app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    await myService.LogCreation("Second middleware called");
    await next();
});


app.Run();

