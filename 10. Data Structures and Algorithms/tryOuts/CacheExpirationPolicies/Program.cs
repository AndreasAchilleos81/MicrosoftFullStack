using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;

var services = new ServiceCollection();

services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379";
	options.InstanceName = "SampleInstance";
});


var serviceProvider = services.BuildServiceProvider();
var cache = serviceProvider.GetRequiredService<IDistributedCache>();


// Store a value in the cache Sliding Expiration
var cacheEntryOptions = new DistributedCacheEntryOptions
{
	AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1), // Expires in 10 minutes
	SlidingExpiration = TimeSpan.FromSeconds(15) // Resets if accessed within 2 minutes
};

await cache.SetStringAsync("taskKey", "Sample Task", cacheEntryOptions);
Console.WriteLine("Cache entry set with absolute and sliding expiration.");

// Retrieve and display cached value
var cachedValue = await cache.GetStringAsync("taskKey");
Console.WriteLine(cachedValue != null ? $"Cache hit: {cachedValue}" : "Cache miss: Value expired.");

// Trigger cache invalidation
Console.WriteLine("Press any key to invalidate cache...");
Console.ReadKey();
await InvalidateCache(cache, "taskKey");

// Simulate repeated cache access
for (int i = 0; i < 5; i++)
{
	await GetCachedData(cache, "taskKey");
	await Task.Delay(TimeSpan.FromSeconds(1)); // Simulate waiting time
}

async Task InvalidateCache(IDistributedCache cache, string key)
{
	await cache.RemoveAsync(key);
	Console.WriteLine($"Cache entry '{key}' has been invalidated.");
}

async Task<string> GetCachedData(IDistributedCache cache, string key)
{
	var cachedValue = await cache.GetStringAsync(key);

	if (cachedValue != null)
	{
		Console.WriteLine($"Cache hit: {cachedValue}");
		return cachedValue;
	}

	Console.WriteLine("Cache miss: Fetching new data...");
	var newValue = "Fetched Task Data";
	await cache.SetStringAsync(key, newValue, new DistributedCacheEntryOptions
	{
		AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
		SlidingExpiration = TimeSpan.FromMinutes(2)
	});
	Console.WriteLine("New data cached.");
	return newValue;
}
