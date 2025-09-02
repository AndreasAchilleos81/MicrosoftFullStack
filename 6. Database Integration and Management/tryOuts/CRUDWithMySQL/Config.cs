using Microsoft.Extensions.Configuration;

namespace CRUDWithMySQL
{
	public static class Config
	{
		public static string ConnectionString { get; set; }

		public static string ServerVersion { get; set; }

		static Config()
		{
			var config = new ConfigurationBuilder().AddJsonFile("./appsettings.json").Build();
			ConnectionString = config["ConnectionStrings:DefaultConnection"];
			ServerVersion = config["ConnectionStrings:ServerVersion"];
		}
	}
}
