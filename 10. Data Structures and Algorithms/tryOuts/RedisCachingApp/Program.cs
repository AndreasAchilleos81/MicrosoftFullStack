using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;

var services = new ServiceCollection();
services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379";
	options.InstanceName = "SampleInstance";
});

var provider = services.BuildServiceProvider();	
var cache = provider.GetRequiredService<IDistributedCache>();

Console.WriteLine("Setting cache value...");

string cacheKey = "ProductList";
string cacheValue = await cache.GetStringAsync(cacheKey);

string cacheKeyCounter = "SharedCounter";
string cachedValueCounter = await cache.GetStringAsync(cacheKeyCounter);
int counter = cachedValueCounter != null ? int.Parse(cachedValueCounter) : 0;
counter++;
await cache.SetStringAsync(cacheKeyCounter, counter.ToString());
Console.WriteLine($"Updated Counter: {counter}");

if (cacheValue != null)
{
	Console.WriteLine($"Cache hit! Value: {cacheValue}");
}
else
{
	Console.WriteLine("Cache miss! Setting value...");
	cacheValue = "Product1, Product2, Product3";
	await cache.SetStringAsync(cacheKey, cacheValue, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(10)));
	Console.WriteLine("Value set in cache.");
}



