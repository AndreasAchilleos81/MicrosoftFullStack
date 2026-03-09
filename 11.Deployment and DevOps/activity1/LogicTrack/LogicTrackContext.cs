using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogicTrack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.DatePlaced)
                .HasDatabaseName("IX_Order_DatePlaced");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerName)
                .HasDatabaseName("IX_Order_CustomerName");

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

        // compiled query - EF.CompileQuery returns a delegate that yields IEnumerable<T> for sequence projections
        private static readonly Func<LogicTrackContext, int, int, string, DateTime, DateTime, IEnumerable<OrderSummaryDto>> _compiledOrderSummaries =
            EF.CompileQuery((LogicTrackContext ctx, int page, int pageSize, string customer, DateTime from, DateTime to) =>
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
        // Return a materialized list asynchronously to avoid exposing IQueryable/DbContext lifetime to callers
        public Task<List<OrderSummaryDto>> GetOrderSummariesCompiledAsync(int page, int pageSize, string? customer, DateTime from, DateTime to)
        {
            var result = _compiledOrderSummaries(this, page, pageSize, customer ?? string.Empty, from, to);
            return Task.FromResult(result.ToList());
        }

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
    }
}
