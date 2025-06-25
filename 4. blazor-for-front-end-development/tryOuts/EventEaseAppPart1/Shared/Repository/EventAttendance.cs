using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repository
{
	public class EventAttendance
	{
		private readonly AttendanceRepository _attendanceRepo;
		private readonly EventCardRepository _eventCardRepo;

		public EventAttendance(IGenericRepository<Attendance> attendanceRepo, IGenericRepository<EventCard> eventCardRepo)
		{
			_attendanceRepo = attendanceRepo as AttendanceRepository;
			_eventCardRepo = eventCardRepo as EventCardRepository;	
		}

		public async Task<IEnumerable<EventCard>> GetAllUpcomingEvents(int daysTo = 1)
		{
			var allEvents = await _eventCardRepo.GetAll();
			var eventCards = allEvents.Where(s => s.Date.Date == DateTime.Now.Date.AddDays(daysTo));
			return eventCards;
		}

		public async Task<IEnumerable<string>> GetUsersIdsForUpcomingEvents(IEnumerable<string> eventCardIds)
		{
			return await _attendanceRepo.GetUsersForUpcomingEventsIds(eventCardIds);
		}

		public async Task<IEnumerable<EventCard>> GetUsersForUpcomingEvents(string UserId)
		{
			var upcomingEvents = (await GetAllUpcomingEvents()).ToList();
			var userInteresteedEvents = await _attendanceRepo.GetAttendances(UserId);
			// keep from upcoming events only those that the user is interested in or going to
			var userEvents = upcomingEvents.Where(e => userInteresteedEvents.Any(a => a.EventId == e.Id)).ToList();
			return userEvents;
		}
	}
}