using LogicTrack;
using LogicTrack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<LogicTrackContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DbPath")));

builder.Services.AddMemoryCache();
var app = builder.Build();

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

// Chage Price to be decimla
// Change Date in orders table to be DateTime
// Seed the Database

