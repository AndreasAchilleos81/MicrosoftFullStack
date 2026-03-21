using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.DbContext;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var dbPrefix = builder.Configuration["DbSettings:DbPreFix"];
var dbRelativePath = builder.Configuration["DbSettings:ConnectionString"] ?? "skillSnap.db";
var dbFile = Path.Combine(AppContext.BaseDirectory, dbRelativePath);
var sqliteConnectionString = $"{dbPrefix}{dbFile}";

builder.Services.AddDbContextPool<SkillSnapContext>(options =>
	options.UseSqlite(sqliteConnectionString));

builder.Services
	.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<SkillSnapContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
	options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
	options.SignIn.RequireConfirmedEmail = false;
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Password.RequireDigit = false;
	options.Password.RequiredLength = 3;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(); // https://localhost:7238/swagger/index.html

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowClient", policy =>
	{
		policy.WithOrigins("https://localhost:41335")
			   .AllowAnyHeader()
			   .AllowAnyMethod();
	});
});


var app = builder.Build();

app.UseCors("AllowClient");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<SkillSnapContext>();
	db.Database.Migrate();
}


app.UseHttpsRedirection();


app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

