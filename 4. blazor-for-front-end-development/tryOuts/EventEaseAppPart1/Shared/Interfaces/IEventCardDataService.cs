using Shared.Models;

namespace Shared.Interfaces
{
	public interface IEventCardDataService
	{
		Task AddEventCardAsync(EventCard eventCard);
		Task<EventCard?> GetEventCardByIdAsync(string id);
		Task<IEnumerable<EventCard>> GetAllEventCardsAsync();
		Task<bool> UpdateEventCardAsync(EventCard updatedEventCard);
		Task<bool> DeleteEventCardAsync(Guid id);
	}
}
