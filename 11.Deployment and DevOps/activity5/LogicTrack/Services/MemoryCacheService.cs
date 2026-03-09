using Microsoft.Extensions.Caching.Memory;

namespace LogicTrack.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly object _lock = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, MemoryCacheEntryOptions? options = null)
        {
            if (_cache.TryGetValue(key, out T? cached))
            {
                return cached!;
            }

            var value = await factory();
            _cache.Set(key, value, options ?? new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));
            return value;
        }

        public int GetVersion(string key)
        {
            if (_cache.TryGetValue(key, out int v)) return v;
            return 0;
        }

        public int IncrementVersion(string key)
        {
            lock (_lock)
            {
                var v = GetVersion(key) + 1;
                _cache.Set(key, v);
                return v;
            }
        }

        public string MakeVersionedKey(string baseKey, params (string, string)[] parts)
        {
            int version = GetVersion(baseKey + "_version");
            var sb = new System.Text.StringBuilder(baseKey);
            foreach (var p in parts)
            {
                sb.Append($":{p.Item1}={p.Item2}");
            }
            sb.Append($":v={version}");
            return sb.ToString();
        }
    }
}
