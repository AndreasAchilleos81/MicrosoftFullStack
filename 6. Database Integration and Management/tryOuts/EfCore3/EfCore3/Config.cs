using Microsoft.Extensions.Configuration;
using HrDbContext;

namespace EfCore3
{
	public class Config : IConfig
	{
		public Config()
		{
			IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("./appsettings.json", false).Build();
			var relativeConnection = configuration["ConnectionStrings:DefaultConnection"];
			var currentDir = Directory.GetCurrentDirectory();
			// Getting parent to keep going one up in directory structure
			var desiredDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDir).ToString()).ToString()).ToString()).ToString();
			var pathToDb = Path.GetFullPath(Path.Combine(Path.GetFullPath(desiredDirectory), relativeConnection));
			var connectionString = $"Data Source={pathToDb}";
			ConnectionString = connectionString;
		}

		public string ConnectionString { get; set; }
	}
}
