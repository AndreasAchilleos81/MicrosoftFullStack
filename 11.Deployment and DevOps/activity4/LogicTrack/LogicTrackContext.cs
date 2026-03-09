using LogicTrack.Identity;
using LogicTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace LogicTrack
{
	public class LogicTrackContext : DbContext
	{
		private readonly string _connectionString;
		public LogicTrackContext(IConfiguration configuration)
		{
			_connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, configuration["DbPath"]!)}";
			Batteries.Init();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(_connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Order -> OrderItems (one-to-many)
			modelBuilder.Entity<Order>()
				.HasMany(o => o.Items)
				.WithOne(oi => oi.Order)
				.HasForeignKey(oi => oi.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			// InventoryItem -> OrderItems (one-to-many)
			modelBuilder.Entity<InventoryItem>()
				.HasMany(ii => ii.OrderItems)
				.WithOne(oi => oi.InventoryItem)
				.HasForeignKey(oi => oi.InventoryItemId)
				.OnDelete(DeleteBehavior.Restrict); // prevent deleting product while referenced by order lines

			modelBuilder.Entity<IdentityUserRole<string>>(entity =>
			{
				entity.HasKey(r => new { r.UserId, r.RoleId });
			});

			// Indexes to improve query performance for commonly filtered/sorted columns
			// Add migrations after this change: `dotnet ef migrations add AddOrderIndexes` and `dotnet ef database update`
			modelBuilder.Entity<Order>()
				.HasIndex(o => o.DatePlaced)
				.HasDatabaseName("IX_Order_DatePlaced");

			// Consider indexing a normalized customer name column for case-insensitive searches.
			// For now we index the raw CustomerName; for large datasets add a NormalizedCustomerName column and index that instead.
			modelBuilder.Entity<Order>()
				.HasIndex(o => o.CustomerName)
				.HasDatabaseName("IX_Order_CustomerName");

			// Consider indexing InventoryItem.Name for search performance
			modelBuilder.Entity<InventoryItem>()
				.HasIndex(i => i.Name)
				.HasDatabaseName("IX_InventoryItem_Name");

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		public DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }

		public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }

		public DbSet<IdentityRole> IdentityRoles { get; set; }

		public DbSet<InventoryItem> InventoryItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }

        // retrieve order summaries without tracking; return IQueryable so caller can compose with paging asynchronously
        public IQueryable<OrderSummaryDto> GetOrderSummaries(int page, int pageSize, string? customer, DateTime from, DateTime to)
        {
            // Base query (AsNoTracking for faster read-only queries)
            var query = Orders.AsNoTracking();

            // Apply filters - use EF.Functions.Like for SQL LIKE translation when a customer filter is provided
            if (!string.IsNullOrWhiteSpace(customer))
                query = query.Where(o => EF.Functions.Like(o.CustomerName, $"%{customer}%"));

            query = query.Where(o => o.DatePlaced >= from && o.DatePlaced <= to);

            // Project and apply ordering and paging on the server
            var projected = query
                .OrderByDescending(o => o.DatePlaced)
                .Select(o => new OrderSummaryDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    ItemCount = o.Items.Count(),
                    DatePlaced = o.DatePlaced
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return projected;
        }

        // Non-paged variant
        public IQueryable<OrderSummaryDto> GetOrderSummaries()
        {
            return Orders
                .AsNoTracking()
                .OrderByDescending(o => o.DatePlaced)
                .Select(o => new OrderSummaryDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    ItemCount = o.Items.Count(),
                    DatePlaced = o.DatePlaced
                });
        }
	}
}
