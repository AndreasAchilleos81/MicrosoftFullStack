using Microsoft.EntityFrameworkCore;
using QueryPerfWebApiEFCore;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSingleton<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	var connectionString = builder.Configuration["DefaultConnection"];
	options.UseSqlServer(connectionString)
		   .LogTo(Console.WriteLine, LogLevel.Information)
		   .EnableSensitiveDataLogging()
		   .EnableDetailedErrors();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

	await dbContext.ExecutePersonDetails();
	await dbContext.ExecuteWorkersPerPostCode();

}

app.Run();
