namespace BlazorServerApp.Services;

using BlazorServerApp.Models.Delegates;
using Microsoft.Extensions.Caching.Memory;

public class CacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, FuncAsync<T> createItem, TimeSpan? expiration)
    {
        if (!_memoryCache.TryGetValue(key, out T cacheEntry))
        {
            cacheEntry = await createItem();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
                SlidingExpiration = TimeSpan.FromMinutes(2),
            };

            _memoryCache.Set(key, cacheEntry, cacheEntryOptions);
        }
        return cacheEntry!;
    }
}
