using Microsoft.EntityFrameworkCore;
using SafeVault.Models;
using SQLitePCL;

namespace SafeVault.Database
{
	public class SafeVaultDbContext : DbContext
	{
		private readonly string _connectionString;
		public DbSet<User> Users { get; set; }
		public SafeVaultDbContext(IConfiguration configuration)
		{
			_connectionString = "Data Source=" + Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuration["SafeValueDatabase"]!));
			Batteries.Init();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(_connectionString);
		}
	}
}
