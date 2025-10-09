namespace OpenWeatherAPI.Exceptions
{
	public class RateLimitHasBeenReached :Exception
	{
		public RateLimitHasBeenReached() : base("Rate limit has been reached. Please try again later.") { }
	}
}
