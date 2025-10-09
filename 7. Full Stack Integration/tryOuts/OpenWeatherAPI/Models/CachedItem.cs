namespace OpenWeatherAPI.Models
{
	public class CachedItem
	{
		public object Data { get; set; }

		public DateTime Expiration { get; set; }
	}
}
