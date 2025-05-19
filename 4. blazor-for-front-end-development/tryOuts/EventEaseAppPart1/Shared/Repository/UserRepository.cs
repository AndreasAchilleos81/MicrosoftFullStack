using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class UserRepository : GenericRepository<User>
	{
		public UserRepository(IConfiguration configuration, ILogger<User> logger) : base(configuration, logger)
		{
		}
	}
}
