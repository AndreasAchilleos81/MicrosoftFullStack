using Microsoft.Extensions.Configuration;
using Shared.Interfaces;
using Shared.Models;
using Microsoft.Data.Sqlite;
using Dapper;
using SQLitePCL;

namespace Shared.Services
{
	public class EventCardDataService : IEventCardDataService, IDisposable
	{
		private readonly IConfiguration _configuration;
		private readonly SqliteConnection _connection;
		private bool disposed = false;

		public EventCardDataService(IConfiguration configuration)
		{
			Batteries.Init();
			_configuration = configuration;
			var relativeLocation = _configuration["Logging:ConnectionStrings:DefaultConnection"].TrimEnd(';');
			var absolutePath =  Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeLocation));
			var connectionString = $"Data Source={absolutePath};";
			_connection = new SqliteConnection(connectionString);
			_connection.Open();
		}

		// Create
		public async Task AddEventCardAsync(EventCard eventCard)
		{
			var query = "INSERT INTO EventCard (Id, Name, Description, Location, IsPublic, MaxAttendees, CurrentAttendees) values (@Id, @Name, @Description, @Location, @IsPublic, @MaxAttendees, @CurrentAttendees)";
			var result = await _connection.ExecuteAsync(query, new
			{
				Id = eventCard.Id,
				Name = eventCard.Name,
				Description = eventCard.Description,
				Location = eventCard.Location,
				IsPublic = eventCard.IsPublic,
				MaxAttendees = eventCard.MaxAttendees,
				CurrentAttendees = eventCard.CurrentAttendees
			});
		}

		// Read
		public async Task<EventCard?> GetEventCardByIdAsync(string id)
		{
			var query = "SELECT * FROM EventCard WHERE Id = @Id LIMIT 1";
			var result = await _connection.QueryFirstOrDefaultAsync<EventCard>(query, new { Id = id });
			return result;
		}

		public async IAsyncEnumerable<EventCard> GetAllEventCardsAsync()
		{
			var query = "SELECT * FROM EventCard";
			using var command  = new SqliteCommand(query, _connection);
			using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				yield return new EventCard
				{
					Id = reader.GetString(0),
					Name = reader.GetString(1),
					Description = reader.GetString(2),
					Location = reader.GetString(3),
					IsPublic = reader.GetBoolean(4),
					CurrentAttendees = reader.GetInt32(6)
				};
			}
		}

		// Update
		public async Task<bool> UpdateEventCardAsync(EventCard updatedEventCard)
		{
			return await Task.FromResult(false);
		}

		// Delete
		public async Task<bool> DeleteEventCardAsync(Guid id)
		{
			return await Task.FromResult(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_connection.Close();
				}
				disposed = true;
			}
		}
	}
}
