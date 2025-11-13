using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace QueryPerfMonitorConsole
{
	public class QueryPerformanceMonitor
	{
		private readonly string _connectionString;
		public QueryPerformanceMonitor(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<QueryPerformanceResult> MonitorQuery(string query, object parameters = null)
		{

			var result = new QueryPerformanceResult
			{
				Query = query,
				StartTime = DateTime.Now
			};

			void AddStatistics(object sender, SqlInfoMessageEventArgs e)
			{
				result.Statistics.Add(e.Message);
			}


			var stopwatch = Stopwatch.StartNew();

			using var connection = new SqlConnection(_connectionString);
			try
			{
				using var command = new SqlCommand(query, connection);
				if (parameters != null) command.Parameters.AddWithValue("@parameters", parameters);
				await connection.OpenAsync();

				using var statistcsCommand = new SqlCommand("SET STATISTICS TIME ON; SET STATISTICS IO ON;", connection);
				await statistcsCommand.ExecuteNonQueryAsync();

				connection.InfoMessage += AddStatistics;

				using var reader = await command.ExecuteReaderAsync();
				result.RowCount = 0;
				while (await reader.ReadAsync())
				{
					result.RowCount++;
				}

				stopwatch.Stop();
				result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
				result.Success = true;
				connection.InfoMessage -= AddStatistics;

			}
			catch (Exception ex)
			{
				result.Success = false;
				result.ErrorMessage = ex.Message;
			}
			finally
			{
				stopwatch.Stop();
				result.EndTime = DateTime.Now;
				result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
			}

			return result;
		}

		public async Task DisaplayPerformanceReport(QueryPerformanceResult result)
		{
			Console.WriteLine("Query Performance Report");
			Console.WriteLine("------------------------");
			Console.WriteLine($"Query: {result.Query}");
			Console.WriteLine($"Start Time: {result.StartTime}");
			Console.WriteLine($"End Time: {result.EndTime}");
			Console.WriteLine($"Execution Time (ms): {result.ExecutionTimeMs}");
			Console.WriteLine($"Row Count: {result.RowCount}");
			Console.WriteLine($"Success: {result.Success}");
			if (!result.Success)
			{
				Console.WriteLine($"Error Message: {result.ErrorMessage}");
			}
			Console.WriteLine("Statistics:");
			foreach (var stat in result.Statistics)
			{
				Console.WriteLine(stat);
			}
			Console.WriteLine("------------------------");
		}
	}
}
