using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class EventCardRepository : GenericRepository<EventCard>
	{
		public EventCardRepository(IConfiguration configuration, ILogger<EventCard> logger)
			: base(configuration, logger) { }


	}
}
