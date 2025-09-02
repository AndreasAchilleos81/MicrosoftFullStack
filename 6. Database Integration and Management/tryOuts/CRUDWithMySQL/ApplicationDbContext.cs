using CRUDWithMySQL.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUDWithMySQL
{
	public class ApplicationDbContext : DbContext
	{

		public DbSet<Product> Products { get; set; }	

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySql(Config.ConnectionString, new MySqlServerVersion(new Version(Config.ServerVersion)));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>().HasKey(p => p.Id);
			modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired(true);
			modelBuilder.Entity<Product>().Property(p => p.Price).IsRequired(true);
			modelBuilder.Entity<Product>().Property(p => p.Description).IsRequired(false);
			modelBuilder.Entity<Product>().Property(p => p.ArrivalDate).IsRequired(true);
		}
	}
}