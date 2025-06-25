
using Shared.Communication;
using Shared.Repository;
using System.Collections.Concurrent;

namespace BlazorApp1.Services
{
	public class EventNotificationService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly EventAttendance _eventAttendance;
		private readonly ILogger<EventNotificationService> _logger;

		public EventNotificationService(
			EventAttendance eventAttendance,
			ILogger<EventNotificationService> logger,
			IServiceProvider serviceProvider)
		{
			_eventAttendance = eventAttendance;
			_logger = logger;
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			do
			{
				try
				{
					var upcomingEvents = await _eventAttendance.GetAllUpcomingEvents();
					using (var scope = _serviceProvider.CreateScope())
					{
						var signalRService = scope.ServiceProvider.GetRequiredService<SignalRService>();
						await signalRService.EnsureConnectionOpen();
						foreach (var eventId in upcomingEvents)
						{
							// Notify users about upcoming events
							// send userId and event name to the SignalR hub
							await signalRService.EventCardTimeApproaching(eventId.Id, $"Event '{eventId.Name}' is approaching on {eventId.Date}");
							_logger.LogInformation($"Event '{eventId.Name}' is approaching on {eventId.Date}");
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError("SignalR and its dependencies are not ready yet");
				}
				await Task.Delay(5000, stoppingToken); // Initial delay before starting the loop

			} while (!stoppingToken.IsCancellationRequested);
		}
	}
}