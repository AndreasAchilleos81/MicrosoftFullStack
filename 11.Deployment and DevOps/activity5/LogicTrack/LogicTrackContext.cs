using LogicTrack.Identity;
using LogicTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace LogicTrack
{
    public class LogicTrackContext : DbContext
    {
        public LogicTrackContext(DbContextOptions<LogicTrackContext> options)
            : base(options)
        {
            Batteries.Init();
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

		// add near the DbSet properties inside LogicTrackContext class
		private static readonly Func<LogicTrackContext,int,int,string,DateTime,DateTime,IEnumerable<OrderSummaryDto>>
			_compiledOrderSummaries = EF.CompileQuery((LogicTrackContext ctx, int page, int pageSize, string customer, DateTime from, DateTime to) =>
				ctx.Orders
				   .AsNoTracking()
				   .Where(o => (string.IsNullOrEmpty(customer) || EF.Functions.Like(o.CustomerName, "%" + customer + "%"))
							   && o.DatePlaced >= from && o.DatePlaced <= to)
				   .OrderByDescending(o => o.DatePlaced)
				   .Select(o => new OrderSummaryDto
				   {	
					   OrderId = o.OrderId,
					   CustomerName = o.CustomerName,
					   ItemCount = o.Items.Count(),
					   DatePlaced = o.DatePlaced
				   })
				   .Skip((page - 1) * pageSize)
				   .Take(pageSize)
			);

		private static readonly Func<LogicTrackContext, int, Order?> _compiledGetOrderByIdEntity =
			EF.CompileQuery((LogicTrackContext ctx, int id) =>
				ctx.Orders
				   .AsNoTracking()
				   .Include(o => o.Items)
					   .ThenInclude(oi => oi.InventoryItem)
				   .FirstOrDefault(o => o.OrderId == id)
			);

		// wrapper methods
		public IEnumerable<OrderSummaryDto> GetOrderSummariesCompiled(int page, int pageSize, string? customer, DateTime from, DateTime to)
			=> _compiledOrderSummaries(this, page, pageSize, customer ?? string.Empty, from, to);

		public Order? GetOrderByIdCompiled(int id) => _compiledGetOrderByIdEntity(this, id);

		// (optional) keep a non-compiled non-paged projection method if you used it elsewhere:
		public IQueryable<OrderSummaryDto> GetOrderSummariesProjection()
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

		private static readonly Func<LogicTrackContext, int, int, string, DateTime, DateTime, IAsyncEnumerable<OrderSummaryDto>> _compiledOrderSummariesAsync =
			EF.CompileAsyncQuery((LogicTrackContext ctx, int page, int pageSize, string customer, DateTime from, DateTime to) =>
				ctx.Orders
				   .AsNoTracking()
				   .Where(o => (string.IsNullOrEmpty(customer) || EF.Functions.Like(o.CustomerName, "%" + customer + "%"))
							   && o.DatePlaced >= from && o.DatePlaced <= to)
				   .OrderByDescending(o => o.DatePlaced)
				   .Select(o => new OrderSummaryDto
				   {
					   OrderId = o.OrderId,
					   CustomerName = o.CustomerName,
					   ItemCount = o.Items.Count(),
					   DatePlaced = o.DatePlaced
				   })
				   .Skip((page - 1) * pageSize)
				   .Take(pageSize)
			);

		// Consume with await foreach or ToListAsync wrapper
		public async Task<List<OrderSummaryDto>> GetOrderSummariesCompiledAsync(int page, int pageSize, string? customer, DateTime from, DateTime to)
			=> await _compiledOrderSummariesAsync(this, page, pageSize, customer ?? string.Empty, from, to).ToListAsync();
	}
}
