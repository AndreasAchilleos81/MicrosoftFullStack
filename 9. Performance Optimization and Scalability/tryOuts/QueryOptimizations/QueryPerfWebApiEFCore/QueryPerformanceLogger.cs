
namespace QueryPerfWebApiEFCore
{
	public class QueryPerformanceLogger : ILogger
	{
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			if (!IsEnabled(logLevel)) return;

			var message = formatter(state, exception);

			// Check for slow queries (EF Core command executed events)
			if (eventId.Name == "CommandExecuted")
			{
				// Parse execution time from message
				var match = System.Text.RegularExpressions.Regex.Match(
					message, @"in (\d+)ms");

				if (match.Success && int.TryParse(match.Groups[1].Value, out int ms))
				{
					if (ms > 1000)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine($"⚠️ SLOW QUERY DETECTED: {ms}ms");
						Console.ResetColor();
					}
				}
			}

			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{logLevel}] {message}");
		}
	}
}
