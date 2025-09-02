using Microsoft.EntityFrameworkCore.Design;

namespace CRUDWithMySQL
{
	public class DesignModeDbContext : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			return new ApplicationDbContext();
		}
	}
}
