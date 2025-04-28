using AdvancedBlazorComponenentTwo.DataTypes;

namespace AdvancedBlazorComponenentTwo.Services
{
	public class Dataservice : IDataService

	{
		public async Task<List<Person>> GetData()
		{
			await Task.Delay(2000); // Simulate a delay
			return new List<Person>
							{
								new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
								new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" },
								new() { Id = 3, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@example.com" },
								new() { Id = 4, FirstName = "Bob", LastName = "Brown", Email = "bob.brown@example.com" },
								new() { Id = 5, FirstName = "Charlie", LastName = "Davis", Email = "charlie.davis@example.com" }
							};
		}
	}
}
