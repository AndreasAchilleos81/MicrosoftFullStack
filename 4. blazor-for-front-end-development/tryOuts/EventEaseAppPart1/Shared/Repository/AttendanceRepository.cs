using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Shared.Repository
{
	public class AttendanceRepository : GenericRepository<Attendance>
	{
		public AttendanceRepository(IConfiguration configuration, ILogger<Attendance> logger) 
			: base(configuration, logger) { }

		public async Task<Attendance> GetAttendance(string eventId, string userId)
		{
			IEnumerable<Attendance> result = null;
			try
			{
				string tableName = GetTableName();
				var keyColumns = GetKeycolumnNames().ToArray();
				string query = $"SELECT  * From {tableName} WHERE {keyColumns[0]} = '{eventId}' AND {keyColumns[1]} = '{userId}' ";
				result = await _connection.QueryAsync<Attendance>(query);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return result.FirstOrDefault();
		}

		public async Task<IEnumerable<Attendance>> GetAttendances(string userId)
		{
			string tableName = GetTableName();
			var keyColumns = GetKeycolumnNames().ToArray();

			string query = $"SELECT  * From {tableName} WHERE {keyColumns[1]} = '{userId}' ";
			var result = await _connection.QueryAsync<Attendance>(query);

			return result;	
		}
	}
}
