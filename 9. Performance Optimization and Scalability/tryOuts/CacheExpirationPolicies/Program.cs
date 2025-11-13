
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var redis = ConnectionMultiplexer.Connect("localhost:6379");
		var db = redis.GetDatabase();

		await AbsoluteExpirationTest(db, "mySet", "value1");

		await SlidingExpirationTest(db, "mySet", "value1");

		await DependentExpirationTest(db, "parentKey", "childKey");
	}

	private static async Task AbsoluteExpirationTest(IDatabase db, string key, string value)
	{
		await db.StringSetAsync(key, value);
		await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));

		// Retrieve and display the cached item
		Console.WriteLine($"Cached Value: {await db.StringGetAsync(key)}");

		// Delay for 11 seconds to check expiration
		await Task.Delay(11000); // Wait 11 seconds

		// Check if the item is still in the cache
		Console.WriteLine($"Cached Value after expiration: {await db.StringGetAsync(key)}"); // Should be null


		//await db.SetAddAsync(key, value);

		// this is the Microsofts abstraction for IDatabse of Redis the abstraction is IDistributedCache
		//var options = new DistributedCacheEntryOptions
		//{
		//	AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10),

		//};

		//await db.SetAsync(key, value, options);
	}

	public static async Task SlidingExpirationTest(IDatabase db, string key, string value)
	{
		// Store the cache item with an initial expiration of 20 seconds
		await db.StringSetAsync(key, value, TimeSpan.FromSeconds(20));

		for (int i = 0; i < 2; i++)
		{
			// Get the cached item as RedisValue and check for null
			RedisValue cachedValue = await db.StringGetAsync(key);

			if (!cachedValue.IsNullOrEmpty)
			{
				Console.WriteLine($"Access {i + 1}: Cached Value: {cachedValue}");

				// Reset the expiration timer to 20 seconds
				db.KeyExpire(key, TimeSpan.FromSeconds(20));
			}
			else
			{
				Console.WriteLine($"Access {i + 1}: Key '{key}' does not exist.");
				break; // Exit loop if the key no longer exists
			}

			// Wait 10 seconds before the next access
			await Task.Delay(10000);
		}

		// Final delay to allow the item to expire
		await Task.Delay(11000); // Wait 11 seconds to exceed expiration window

		// Verify the item has expired
		RedisValue finalValue = await db.StringGetAsync(key);
		Console.WriteLine($"Cached Value after expiration: {await db.StringGetAsync(key)}");
	}

	public static async Task DependentExpirationTest(IDatabase db, string parentKey, string childKey)
	{
		// Set the parent and child keys
		await db.StringSetAsync(parentKey, "Product data");
		await db.StringSetAsync(childKey, "Inventory data");

		Console.WriteLine("\nInitial Cache State:");
		Console.WriteLine($"Parent Key: {await db.StringGetAsync(parentKey)}");
		Console.WriteLine($"Child Key: {await db.StringGetAsync(childKey)}");

		// Simulate parent update
		Console.WriteLine("\nUpdating parent entry...");
		await db.StringSetAsync(parentKey, "Updated product data");

		// Invalidate the child entry if the parent is updated
		if (await db.StringGetAsync(parentKey) == "Updated product data")
		{
			Console.WriteLine("Parent updated. Expiring dependent entry...");
			await db.KeyDeleteAsync(childKey);
		}

		// Display the final state of the cache
		Console.WriteLine("\nFinal Cache State:");
		Console.WriteLine($"Parent Key: {await db.StringGetAsync(parentKey)}");
		Console.WriteLine($"Child Key: {await db.StringGetAsync(childKey)}"); // Should be null
	}
}