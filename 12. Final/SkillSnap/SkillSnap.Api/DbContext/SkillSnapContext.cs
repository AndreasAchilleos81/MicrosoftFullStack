using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using SQLitePCL;

namespace SkillSnap.Api.DbContext;

public class SkillSnapContext : IdentityDbContext<ApplicationUser>
{
	public SkillSnapContext(DbContextOptions<SkillSnapContext> options) : base(options)
	{
		Batteries.Init();
	}
	public DbSet<PortfolioUser> PortfolioUsers { get; set; }
	public DbSet<Skill> Skills { get; set; }
	public DbSet<Project> Projects { get; set; }

	public DbSet<ApplicationUser> ApplicationUsers { get; set; }

	public DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }

	public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }

	public DbSet<IdentityRole> IdentityRoles { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<PortfolioUser>()
			.HasMany(p => p.Projects);

		modelBuilder.Entity<PortfolioUser>()
			.HasMany(p => p.Skills);

		//modelBuilder.Entity<Skill>()
		//	.HasMany<PortfolioUser>()
		//	.WithMany(p => p.Skills);

		//modelBuilder.Entity<Project>()
		//	.HasOne<PortfolioUser>()
		//	.WithMany(p => p.Projects)
		//	.HasForeignKey(p => p.PortfolioUserId);
	}
}

