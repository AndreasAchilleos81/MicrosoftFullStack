using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Shared.Repository
{
	public class EventCardRepository<EventCard> : GenericRepository<EventCard>
	{
		public EventCardRepository(IConfiguration configuration, ILogger<EventCard> logger)
			: base(configuration, logger) { }


	}
}
