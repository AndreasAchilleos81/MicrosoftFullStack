using LogicTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using static LogicTrack.Models.OrderController;

namespace LogicTrack.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public partial class OrderController : ControllerBase
	{
		private readonly LogicTrackContext _db;
		private readonly IMemoryCache _cache;

		public OrderController(LogicTrackContext db, IMemoryCache cache)
		{
			_db = db;
			_cache = cache;
		}

		// GET /api/orders
		[Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var sw = Stopwatch.StartNew();

            // caching versioning to allow easy invalidation on mutations
            int version = 0;
            _cache.TryGetValue("Orders_Version", out version);

            string cacheKey = $"orders:all:v={version}";

            if (!_cache.TryGetValue(cacheKey, out List<OrderSummaryDto>? list))
            {
                var query = _db.GetOrderSummaries();
                list = await query.ToListAsync();
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                _cache.Set(cacheKey, list, cacheOptions);
            }

            sw.Stop();
            return Ok(new { data = list, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
        }

		// GET /api/orders/{id}
		[HttpGet("{id}", Name = "GetOrderById")]
        public async Task<ActionResult> GetById(int id)
        {
            var sw = Stopwatch.StartNew();

            int version = 0;
            _cache.TryGetValue("Orders_Version", out version);
            string cacheKey = $"orders:id={id}:v={version}";

            if (!_cache.TryGetValue(cacheKey, out Order? order))
            {
                order = await _db.Orders
                    .AsNoTracking()
                    .Include(o => o.Items)
                        .ThenInclude(oi => oi.InventoryItem)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order != null)
                {
                    _cache.Set(cacheKey, order, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));
                }
            }

            if (order is null)
            {
                sw.Stop();
                return NotFound(new { error = "Order not found", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
            }

            sw.Stop();
            return Ok(new { data = order, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
        }

		//POST /api/orders
		[Authorize(Roles = "Manager")]
		[HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var sw = Stopwatch.StartNew();

            // Validate incoming DTO minimally
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                sw.Stop();
                return BadRequest(new { error = "CustomerName is required.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
            }

            // Validate and load inventory items referenced by the client (respect quantities)
            var requestedIds = dto.InventoryItems.Select(x => x.ItemId).Distinct().ToList();
            if (!requestedIds.Any())
            {
                sw.Stop();
                return BadRequest(new { error = "At least one inventory item is required.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
            }

            var items = await _db.InventoryItems
                .Where(i => requestedIds.Contains(i.ItemId))
                .ToListAsync();

            // Check for missing items
            var missing = requestedIds.Except(items.Select(i => i.ItemId)).ToList();
            if (missing.Any())
            {
                sw.Stop();
                return BadRequest(new { error = $"The following InventoryItem ids were not found: {string.Join(',', missing)}", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
            }

            // Create order and attach OrderItem entries with quantities provided by the client
            var order = new Order
            {
                CustomerName = dto.CustomerName,
                DatePlaced = dto.DatePlaced ?? DateTime.UtcNow
            };

            foreach (var line in dto.InventoryItems)
            {
                if (line.Quantity <= 0)
                {
                    sw.Stop();
                    return BadRequest(new { error = $"Invalid quantity for item {line.ItemId}. Quantity must be >= 1.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
                }

                order.Items.Add(new OrderItem
                {
                    InventoryItemId = line.ItemId,
                    Quantity = line.Quantity
                });
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // bump version to invalidate cached lists/items
            int version = 0;
            _cache.TryGetValue("Orders_Version", out version);
            _cache.Set("Orders_Version", version + 1);

            sw.Stop();
            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, new { data = order, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
        }

		// DELETE /api/orders/{id}
		[Authorize(Roles = "Manager")]
		[HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var sw = Stopwatch.StartNew();

            var order = await _db.Orders.FindAsync(id);
            if (order is null)
            {
                sw.Stop();
                return NotFound(new { error = "Order not found", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
            }

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            int version = 0;
            _cache.TryGetValue("Orders_Version", out version);
            _cache.Set("Orders_Version", version + 1);

            sw.Stop();
            return Ok(new { message = "Deleted", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
        }
	}
}
