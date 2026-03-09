using LogicTrack;
using LogicTrack.Identity;
using LogicTrack.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

builder.Services
    .AddControllers(options => options.Filters.Add<LogicTrack.Filters.ApiResponseFilter>())
    .AddJsonOptions(options => 
    { options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve; options.JsonSerializerOptions.WriteIndented = true; });

// Build full SQLite file path from configuration (key "DbPath" expected to be the relative filename)
var dbRelativePath = builder.Configuration["DbPath"] ?? "logictrack.db";
var dbFile = Path.Combine(AppContext.BaseDirectory, dbRelativePath);
var sqliteConnectionString = $"Data Source={dbFile}";

builder.Services.AddDbContextPool<LogicTrackContext>(options =>
    options.UseSqlite(sqliteConnectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<LogicTrackContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
	options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
	options.SignIn.RequireConfirmedEmail = true;
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Password.RequireDigit = false;
	options.Password.RequiredLength = 3;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
});

// Configure JWT authentication
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");
var jwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
var jwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseSwagger(); 
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<LogicTrackContext>();
	db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();
