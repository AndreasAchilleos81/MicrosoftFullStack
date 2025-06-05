using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class SessionRepository : GenericRepository<Session>
	{
		public SessionRepository(IConfiguration configuration, ILogger<Session> logger) 
			: base(configuration, logger) { }


		public async Task<bool> IsSessionActive(string userId)
		{
			if (userId == null) return false;
			var session = await GetLastSession(userId); 
			var isActive  = session != null && session.EndTime == null || session.EndTime > DateTime.Now;
			return isActive;
		}

		public async Task<Session> GetLastSession(string userId)
		{
			string tableName = GetTableName();
			string keyColumn = GetKeyColumnName();
			string query = $"SELECT  * From {tableName} WHERE {keyColumn} = '{userId}' ORDER BY start_at DESC LIMIT 1";
			var result = await _connection.QueryAsync<RawSession>(query);
			return fromRaw(result.FirstOrDefault());
		}

		private Session fromRaw(RawSession rawSession)
		{
			if (rawSession == null)
			{
				return null;
			}	
			return new Session
			{
				UserId = rawSession.UserId,
				StartAt = DateTime.Parse(rawSession.start_at),
				EndTime = string.IsNullOrEmpty(rawSession.end_time) ? null : DateTime.Parse(rawSession.end_time)
			};
		}
	}
}
