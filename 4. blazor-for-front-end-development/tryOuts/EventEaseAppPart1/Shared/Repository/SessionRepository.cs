using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class SessionRepository : GenericRepository<Session>
	{
		public SessionRepository(IConfiguration configuration, ILogger<Session> logger) 
			: base(configuration, logger) { }
	}
}
