using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EfcoreTwo
{
	public class HrDbContextFactory : IDesignTimeDbContextFactory<HRDbContext>
	{
		public HrDbContextFactory()
		{
		}
		
		public HRDbContext CreateDbContext(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			string configString = config["ConnectionStrings:DefaultConnection"];

			var dbPath = Path.GetFullPath(Path.Combine(Path.GetFullPath(Environment.CurrentDirectory), configString));
			var connectionString = $"Data Source={dbPath}";

			var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
			optionsBuilder.UseSqlite(connectionString);

			return new HRDbContext(optionsBuilder.Options);
		}
	}
}
