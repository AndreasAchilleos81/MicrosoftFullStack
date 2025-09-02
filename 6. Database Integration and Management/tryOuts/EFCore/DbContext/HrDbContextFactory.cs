using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class HrDbContextFactory : IDesignTimeDbContextFactory<HrDbContext>
{
	public HrDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<HrDbContext>();
		optionsBuilder.UseSqlite("Data Source=Hr.db");

		return new HrDbContext(optionsBuilder.Options);
	}
}
