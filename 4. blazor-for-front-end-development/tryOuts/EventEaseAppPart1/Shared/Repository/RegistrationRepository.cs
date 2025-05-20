using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class RegistrationRepository : GenericRepository<Registration>
	{
		public RegistrationRepository(IConfiguration configuration, ILogger<Registration> logger) 
			: base(configuration, logger) { }
	}
}
