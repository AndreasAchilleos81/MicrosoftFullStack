namespace OpenWeatherAPI.Services
{
	using OpenWeatherAPI.Models;
	using System.Collections.Concurrent;


	public class Caching
	{
		private readonly ConcurrentDictionary<string, CachedItem> _cache;
		private readonly TimeSpan _defaultDuration = TimeSpan.FromMinutes(1);
		public Caching()
		{
			_cache = new ConcurrentDictionary<string, CachedItem>();
		}


		public T GetCachedItem<T>(string key)
		{
			if (_cache.TryGetValue(key, out var cachedItem))
			{
				if (cachedItem.Expiration > DateTime.UtcNow)
				{
					return (T)cachedItem.Data;
				}
				else
				{
					_cache.TryRemove(key, out _);
				}
			}
			return default;
		}

		public void SetCachedItem(string key, object data, TimeSpan? duration = null)
		{
			var cachedItem = new CachedItem
			{
				Data = data,
				Expiration = DateTime.UtcNow.Add(duration ?? _defaultDuration)
			};

			_cache[key] = cachedItem;
		}
	}
}
