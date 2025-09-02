using EfcoreTwo.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace HrDbContext.DatabaseContext
{
	public class HrDatabase : Microsoft.EntityFrameworkCore.DbContext
	{
		private readonly string connectionString;

		public DbSet<Department> Departments { get; set; }

		public DbSet<Employee> Employees { get; set; }

		public HrDatabase(IConfig configuration)
		{
			Batteries.Init();
			connectionString = configuration.ConnectionString;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Department>().Property(d => d.Name).IsRequired(true);
			modelBuilder.Entity<Department>().HasMany(d => d.Employees).WithOne(e => e.Department);

			modelBuilder.Entity<Employee>().Property(e => e.FirstName).IsRequired(true);
			modelBuilder.Entity<Employee>()
						.HasOne(e => e.Department)
						.WithMany(e => e.Employees)
						.HasForeignKey(e => e.DepartmentId);
		}
	}
}

