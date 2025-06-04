using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class UserRepository : GenericRepository<User>
	{
		public UserRepository(IConfiguration configuration, ILogger<User> logger) 
			: base(configuration, logger) { }

		public async Task<User> GetUserByEmail(string email)
		{
			try
			{
				string query = "SELECT * FROM User WHERE Email = @Email";
				return await _connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving user by email");
				return null;
			}
		}
	}
}
