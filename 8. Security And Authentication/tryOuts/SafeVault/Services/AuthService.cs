using static SafeVault.Helpers.ValidationHelpers;
using Microsoft.Data.Sqlite;
namespace SafeVault.Services
{
	public class AuthService
	{
		private readonly string _connectionString;
		public AuthService(IConfiguration configuration)
		{
			_connectionString = "Data Source=" + Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuration["SafeValueDatabase"]!)); ;
		}
		internal AuthService(string connectionString)
		{
			_connectionString = connectionString;
		}

		public bool LoginUser(string username, string password)
		{
			string allowedSpecialCharactersForPassword = "!@#$%^&*";

			if (!IsValidInput(username))
			{
				return false;
			}

			if (!IsValidInput(password, allowedSpecialCharactersForPassword))
			{
				return false;
			}

			string query = "SELECT COUNT(1) FROM Users WHERE Username = @username AND Password = @password";

			using (var connection = new SqliteConnection(_connectionString))
			{
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.CommandText = query;
					command.Parameters.AddWithValue("@username", username);
					command.Parameters.AddWithValue("@password", password);
					var result = command.ExecuteScalar();
					int count = Convert.ToInt32(result);
					return count == 1;
				}
			}
		}
	}
}
