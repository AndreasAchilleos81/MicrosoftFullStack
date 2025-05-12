namespace BlazorServerApp.Models
{
	public class ServerConfiguration
	{
		private readonly IConfiguration _configuration;
		public ServerConfiguration(IConfiguration configuration)
		{
				_configuration = configuration;
		}

		public string ConnectionString => _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
	}
}
