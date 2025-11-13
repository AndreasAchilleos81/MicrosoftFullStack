using Microsoft.Extensions.Caching.Memory;

internal class Program
{
    private static void Main(string[] args)
	{
		var cache = new MemoryCache(
			new MemoryCacheOptions
			{
				SizeLimit = 1024, // Maximum capacity in arbitrary units
				ExpirationScanFrequency = TimeSpan.FromSeconds(5), // Frequency to check for expired items
				CompactionPercentage = 0.2, // Percentage of items to remove during compaction
				TrackLinkedCacheEntries = true, // Enable tracking of linked cache entries
				TrackStatistics = true, // Enable tracking of cache statistics
			}
		);

		SetProducts(cache);

		if (cache.TryGetValue("ProductList", out string[]? products))
		{
			Console.WriteLine("Cached Products: " + string.Join(", ", products));
		}
		else
		{
			Console.WriteLine("Product list not found. Cache miss, fetching products");
			SetProducts(cache);
			Console.WriteLine("Product list added to Cache");
		}


		cache.Remove("ProductList");
		if (cache.TryGetValue("ProductList", out string[]? _))
		{
			Console.WriteLine("Cache entry 'ProductList' still exists.");
		}
		else
		{
			Console.WriteLine("Product list not found. Cache miss, NOT fetching products");
		}

		static void SetProducts(MemoryCache cache)
		{
			cache.Set(
				"ProductList",
				new[] { "Apple", "Banana", "Orange" },
				new MemoryCacheEntryOptions
				{
					Size = 1, // Size of this entry in arbitrary units
					SlidingExpiration = TimeSpan.FromSeconds(1), // Expire if not accessed for 5 minutes
					AbsoluteExpiration = DateTimeOffset.Now.AddHours(1), // Expire after 1 hour
					
				}
			);
		}
	}
}
