using Microsoft.EntityFrameworkCore;
using SQLitePCL;

public class HrDbContext : DbContext
{
	public HrDbContext(DbContextOptions<HrDbContext> options) : base(options)
	{
		Batteries.Init();
	}

	public DbSet<Employee> Employees { get; set; }
	public DbSet<Department> Departments { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlite("Data Source=Hr.db");
		}
	}


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Department>(entity =>
		{
			entity.HasKey(d => d.DepartmentId);
			entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
			entity.Property(d => d.Description).HasMaxLength(250);
		});

		modelBuilder.Entity<Employee>()
			.HasOne(e => e.Department)
			.WithMany(d => d.Employees)
			.HasForeignKey(e => e.DepartmentId);

		modelBuilder.Entity<Employee>(entity =>
		{
			entity.HasKey(e => e.EmployeeId);
			entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
			entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
			entity.Property(e => e.HireDate).IsRequired();
		});

		modelBuilder.Entity<Department>().HasMany(d => d.Employees)
			.WithOne(e => e.Department)
			.HasForeignKey(e => e.DepartmentId);

	}
}