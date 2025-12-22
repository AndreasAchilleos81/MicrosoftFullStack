using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.SqlServer;
using StackExchange.Redis;
using System.Data.SqlClient;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "DemoCache_";
});

// Alternative: SQL Server distributed cache (comment out Redis and uncomment this)
// builder.Services.AddDistributedSqlServerCache(options =>
// {
//     options.ConnectionString = "Server=localhost;Database=CacheDb;Trusted_Connection=True;";
//     options.SchemaName = "dbo";
//     options.TableName = "AppCache";
// });

builder.Services.AddMemoryCache();

var app = builder.Build();

// Demo endpoints
app.MapGet("/cache/set/{key}/{value}", async (string key, string value, IDistributedCache cache) =>
{
    await cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    });
    return Results.Ok($"Cached: {key} = {value}");
});

app.MapGet("/cache/get/{key}", async (string key, IDistributedCache cache) =>
{
    var value = await cache.GetStringAsync(key);
    return value != null ? Results.Ok(value) : Results.NotFound("Key not found");
});

app.MapGet("/cache/remove/{key}", async (string key, IDistributedCache cache) =>
{
    await cache.RemoveAsync(key);
    return Results.Ok($"Removed: {key}");
});

// Complex object caching example
app.MapGet("/user/{id}", async (int id, IDistributedCache cache) =>
{
    var cacheKey = $"user_{id}";
    var cachedUser = await cache.GetStringAsync(cacheKey);
    
    if (cachedUser != null)
    {
        return Results.Ok(new { source = "cache", data = JsonSerializer.Deserialize<User>(cachedUser) });
    }
    
    // Simulate DB fetch
    var user = new User { Id = id, Name = $"User{id}", Email = $"user{id}@example.com" };
    await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user), 
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
    
    return Results.Ok(new { source = "database", data = user });
});

app.Run();

record User(int Id, string Name, string Email);

/* 
Setup Instructions:

For Redis:
1. Install: dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
2. Run Redis: docker run -d -p 6379:6379 redis

For SQL Server:
1. Install: dotnet add package Microsoft.Extensions.Caching.SqlServer
2. Create cache table: dotnet sql-cache create "YourConnectionString" dbo AppCache

Test endpoints:
- SET: http://localhost:5000/cache/set/mykey/myvalue
- GET: http://localhost:5000/cache/get/mykey
- REMOVE: http://localhost:5000/cache/remove/mykey
- USER: http://localhost:5000/user/123
*/