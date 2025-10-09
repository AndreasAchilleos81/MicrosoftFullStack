using System.Collections.Concurrent;

namespace OpenWeatherAPI.Services
{
	// Rate limiter per request type - ie for other requests create another ratelimitter
	public class RateLimiter
	{
		private  TimeSpan _timeWindow = TimeSpan.FromMinutes(1);
		private readonly ConcurrentQueue<DateTime> _requestTimestamps = new();
		private SemaphoreSlim _semaphore;

		public RateLimiter()
		{
			_semaphore = new SemaphoreSlim(MaxRequests, MaxRequests);
		}

		public int TimeWindowInMinutes { get; set; } = 1;
		public int MaxRequests { get; set; } = 1;


		public void Setup(int maxRequests, int timeWindowInMinutes)
		{
			MaxRequests = maxRequests;
			TimeWindowInMinutes = timeWindowInMinutes;
			_timeWindow = TimeSpan.FromMinutes(timeWindowInMinutes);
			_semaphore = new SemaphoreSlim(MaxRequests, MaxRequests);
		}

		public bool IsRequestAllowed(int requestCount)
		{
			AddRequest();
			var timeWindowFromNow = DateTime.UtcNow.AddMinutes(-TimeWindowInMinutes);

			// we want to remove requests that older than _timeWindow
			// we remove them if they have expired as in they are older than timeWindowFromNow
			while (_requestTimestamps.Count > 0 && _requestTimestamps.TryPeek(out var latest) && latest < timeWindowFromNow)
			{
				_requestTimestamps.TryDequeue(out _);
			}

			
			// do I have request 
			if (_requestTimestamps.Count >= MaxRequests)
			{
				// we need to check here that the latest  _requestTimestamps when added the _timewindow and we subtract from current time that it returns a negative number
				// the number being negative means we are within the time window cause DateTim.Now is bigger (ahead of time) than the enqueuend time of request
				_requestTimestamps.TryPeek(out var oldest);
				var requestTimeOfWait = oldest - DateTime.UtcNow;

				if (requestTimeOfWait > TimeSpan.Zero)
				{
					return false;
				}
			}

			return true;

		}

	

		private void AddRequest()
		{
			var now = DateTime.UtcNow;
			_requestTimestamps.Enqueue(now);
		}

	}
}
