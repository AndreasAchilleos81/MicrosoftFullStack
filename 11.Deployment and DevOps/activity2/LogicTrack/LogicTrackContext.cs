using LogicTrack.Models;
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

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<InventoryItem> InventoryItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }

		// retrieve order summaries removing tracking this way we can retrieve order summaries without loading the entire order and its items into memory, improving performance when we only need summary information
		public async Task<IQueryable<OrderSummaryDto>> GetOrderSummaries(int page, int pageSize, string? customer, DateTime from, DateTime to)
		{
			// Base query (AsNoTracking for faster read-only queries)
			var query = Orders.AsNoTracking();

			// Apply filters
			if (!string.IsNullOrWhiteSpace(customer))
				query = query.Where(o => o.CustomerName.Contains(customer));

			query = query.Where(o => o.DatePlaced >= from && o.DatePlaced <= to);

			// Projection to DTO -- EF will translate Items.Count() to SQL subquery
			var projected = (await GetOrderSummaries())
				.Skip((page - 1) * pageSize)
				.Take(pageSize);

			return projected;
		}

		public async Task<IQueryable<OrderSummaryDto>> GetOrderSummaries()
		{
			var projected = Orders
				.AsNoTracking()
				.OrderByDescending(o => o.DatePlaced)
				.Select(o => new OrderSummaryDto
				{
					OrderId = o.OrderId,
					CustomerName = o.CustomerName,
					ItemCount = o.Items.Count(),
					DatePlaced = o.DatePlaced
				});
			return projected;
		}
	}
}
