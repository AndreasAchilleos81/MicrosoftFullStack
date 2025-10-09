using OpenWeatherAPI.Exceptions;
using System.Net.Http.Json;

namespace OpenWeatherAPI.Services
{
	public class WeatherApiService
	{
		private readonly HttpClient _httpClient;
		private readonly Caching _caching;
		private readonly RateLimiter _rateLimiter;

		public WeatherApiService(HttpClient httpClient, Caching caching, RateLimiter rateLimiter)
		{
			_httpClient = httpClient;
			_caching = caching;
			_rateLimiter = rateLimiter;
		}

		public async Task<T> MakeRequest<T>(string request)
		{
			// check if you have request cached in first - return if so
			var item = _caching.GetCachedItem<T>(request);

			if (item != null)
			{
				return item;
			}

			// if not cached Check if you canmake request based on rate limit
			// if you cannot make request - throw exception or return null
			if (_rateLimiter.IsRequestAllowed(1))
			{
				try
				{
					// Make the request
					var response = await _httpClient.GetFromJsonAsync<T>(request);
					// cache the result
					_caching.SetCachedItem(request, response!);
					return response;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error making request: {ex.Message}");
					throw ex;
				}
			}

			throw new RateLimitHasBeenReached();
		}
	}
}
