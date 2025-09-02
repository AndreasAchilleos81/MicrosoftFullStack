using Microsoft.EntityFrameworkCore.Design;

namespace HrDbContext.DatabaseContext
{
	public class DesignTimeDbContext : IDesignTimeDbContextFactory<HrDatabase>
	{
		public HrDatabase CreateDbContext(string[] args)
		{
			var config = new ConfigureMe();
			return new HrDatabase(config);
		}
	}

	public class ConfigureMe : IConfig
	{
		public string ConnectionString { get => "Data Source=C:\\Users\\a.achilleos\\code\\MicrosoftFullStack\\6. Database Integration and Management\\tryOuts\\EfCore3\\Hr.db;"; set => throw new NotImplementedException(); }
	}
}
