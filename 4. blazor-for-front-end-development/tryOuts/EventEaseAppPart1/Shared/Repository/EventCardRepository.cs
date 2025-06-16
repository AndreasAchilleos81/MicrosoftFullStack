using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class EventCardRepository : GenericRepository<EventCard>
	{
		public EventCardRepository(IConfiguration configuration, ILogger<EventCard> logger)
			: base(configuration, logger) { }

		public async Task<IEnumerable<EventCard>> GetEvents( IEnumerable<string> eventIds)
		{
			eventIds = eventIds.Select(id => $"'{id}'").ToList(); // Ensure IDs are properly quoted for SQL
			IEnumerable<EventCard> result = null;
			try
			{
				string tableName = GetTableName();
				string keyColumn = GetKeyColumnName();
				string query = $"SELECT  * From {tableName} WHERE {keyColumn} IN ({ String.Join(',', eventIds) })";
				result = await _connection.QueryAsync<EventCard>(query);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return result;
		}
	}
}
