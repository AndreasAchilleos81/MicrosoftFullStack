using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisWebApi.Models;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "redisLocal";
});

var app = builder.Build();
app.MapControllers();
app.UseHttpsRedirection();

var redis = app.Services.GetRequiredService<IConnectionMultiplexer>();
var subscriber = redis.GetSubscriber();
subscriber.Subscribe("__keyevent@0__:expired", (channel, message) =>
{
    Console.WriteLine($"Received message: {message} on channel: {channel}");
});


app.MapGet("/", () => "Hello World!");

app.MapPost(
    "PostData",
    async (IDistributedCache cache, [FromBody] EventData data) =>
    {
        var key = data.Id.ToString();
        var value = System.Text.Json.JsonSerializer.Serialize(data);
        var serializedValue = System.Text.Encoding.UTF8.GetBytes(value);

		var redis = app.Services.GetRequiredService<IConnectionMultiplexer>();

        var db = redis.GetDatabase(0);

        await db.StringSetAsync(key, serializedValue, TimeSpan.FromSeconds(10));

        //var options = new DistributedCacheEntryOptions
        //{
        //    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10),
           
        //};

        //await cache.SetAsync(key, serializedValue, options);
        return Results.Ok(new { Key = key, Name = data.Name });
    }
);

app.MapGet(
    "/GetData/{id:int}",
    async (int id, IDistributedCache cache) =>
    {
        var key = id.ToString();
        var value = await cache.GetAsync(key);
        if (value == null)
        {
            return Results.NotFound("Event Not found i Redis");
        }

        var valueString = System.Text.Encoding.UTF8.GetString(value);
		var data = System.Text.Json.JsonSerializer.Deserialize<EventData>(valueString);
        return Results.Ok(data);
    }
);

app.MapDelete(
    "/DeleteData/{id:int}",
    async (int id, IDistributedCache cache) =>
    {
        var key = id.ToString();
        await cache.RemoveAsync(key);
        return Results.Ok($"Event with id {id} deleted from Redis cache.");
    }
);

app.Run();
