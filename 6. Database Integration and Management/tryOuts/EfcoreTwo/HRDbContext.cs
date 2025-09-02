using EfcoreTwo.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

namespace EfcoreTwo
{
	public class HRDbContext : DbContext
	{
		public HRDbContext()
		{
			Batteries.Init();
		}

		public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
		{
			Batteries.Init();
		}

		public DbSet<Employee> Employees { get; set; }
		public DbSet<Department> Departments { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

				string configString = config["ConnectionStrings:DefaultConnection"];

				var dbPath = Path.GetFullPath(Path.Combine(Path.GetFullPath(Environment.CurrentDirectory), configString));
				var connectionString = $"Data Source={dbPath}";

				optionsBuilder.UseSqlite(connectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			// setting up the same as above from the perspective of the Department
			modelBuilder.Entity<Department>()
				.HasMany(d => d.Employees)
				.WithOne(e => e.Department)
				.HasForeignKey(e => e.DepartmentId);

			modelBuilder.Entity<Department>(entity =>
			{
				entity.HasKey(d => d.Id);
				entity.Property(d => d.Name).IsRequired();
				entity.Property(d => d.Description).IsRequired(false);
				entity.Property(d => d.DepartmentHead).IsRequired(false);
			});


			// setup relationship of Employee to Department
			modelBuilder.Entity<Employee>()
		.HasOne(e => e.Department)
		.WithMany(d => d.Employees)
		.HasForeignKey(e => e.DepartmentId);

			modelBuilder.Entity<Employee>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.FirstName).IsRequired();
				entity.Property(e => e.LastName).IsRequired();
				entity.Property(e => e.Email).IsRequired();
				entity.Property(e => e.Salary).HasDefaultValue(1000m);
				entity.Property(e => e.Phone).IsRequired(true);
			});



			modelBuilder.Entity<Department>().HasData(
				new Department { Id = 1, Name = "HR", Description = "Human Resources Department", DepartmentHead = "Head" },
				new Department { Id = 2, Name = "IT", Description = "Information Technology Department", DepartmentHead = "Head" },
				new Department { Id = 3, Name = "Payments", Description = "Payments Department", DepartmentHead = "Head" },
				new Department { Id = 4, Name = "Innovation", Description = "Innovation Department", DepartmentHead = "Head" }
			);
			modelBuilder.Entity<Employee>().HasData(
				new Employee { Id = 1, DepartmentId = 1, Email = "email1@email.com", FirstName = "FirstName1", LastName = "LastName1", Phone = "384684646" },
				new Employee { Id = 2, DepartmentId = 2, Email = "email2@email.com", FirstName = "FirstName2", LastName = "LastName2", Phone = "384684646" },
				new Employee { Id = 3, DepartmentId = 3, Email = "email3@email.com", FirstName = "FirstName3", LastName = "LastName3", Phone = "384684646" },
				new Employee { Id = 4, DepartmentId = 4, Email = "email4@email.com", FirstName = "FirstName4", LastName = "LastName4", Phone = "384684646" }
			);
		}
	}
}