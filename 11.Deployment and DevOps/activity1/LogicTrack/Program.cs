using LogicTrack;
using LogicTrack.Identity;
using LogicTrack.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using System.Security.Claims;
using System.Text;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

builder.Services
    .AddControllers(options => options.Filters.Add<LogicTrack.Filters.ApiResponseFilter>())
    .AddJsonOptions(options => 
    { options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve; options.JsonSerializerOptions.WriteIndented = true; });

// Response compression (Brotli + Gzip) - enable for HTTPS
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

builder.Services.AddDbContextPool<LogicTrackContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DbPath")));

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

// Enable response compression middleware
app.UseResponseCompression();

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


// Efficient, paged, filtered and cached order summaries endpoint
app.MapGet("/ordersummaries", 
	async (int page, int pageSize, string? customer, DateTime? from, DateTime? to, LogicTrackContext db, IMemoryCache cache) 
	=>
{
	// normalize paging
	page = Math.Max(1, page == 0 ? 1 : page);
	pageSize = Math.Clamp(pageSize == 0 ? 20 : pageSize, 1, 200);

	// build cache key from parameters
	string cacheKey = $"orders:page={page}:size={pageSize}:customer={customer ?? "null"}:from={(from?.ToString("O") ?? "null")}:to={(to?.ToString("O") ?? "null")}";

	if (cache.TryGetValue(cacheKey, out List<OrderSummaryDto>? cached))
	{
		return Results.Ok(cached);
	}

	var projected = await db.GetOrderSummaries(page, pageSize, customer, from, to);
	var list = await projected.ToListAsync();

	// Cache the page for a short duration (tune as needed)
	var cacheEntryOptions = new MemoryCacheEntryOptions
		().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

	cache.Set(cacheKey, list, cacheEntryOptions);

	return Results.Ok(list);
})
.WithName("GetOrderSummaries");

app.Run();
