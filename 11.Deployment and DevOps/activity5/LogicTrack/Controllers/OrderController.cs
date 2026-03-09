using System.Diagnostics;
using LogicTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogicTrack.Services;

namespace LogicTrack.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public partial class OrderController : ControllerBase
	{
		private readonly LogicTrackContext _db;
		private readonly ICacheService _cacheService;

		public OrderController(LogicTrackContext db, ICacheService cacheService)
		{
			_db = db;
			_cacheService = cacheService;
		}

		// GET /api/orders (summary)
		[HttpGet]
		public async Task<ActionResult> GetAll(DateTime from, DateTime to, int page = 0, int pageSize = 20, string? customer = null)
		{
			var sw = Stopwatch.StartNew();

			// normalize / validate parameters as you prefer
			page = Math.Max(1, page == 0 ? 1 : page);
			pageSize = Math.Clamp(pageSize, 1, 200);

			// use compiled query which returns IQueryable<OrderSummaryDto>
			var query = await _db.GetOrderSummariesCompiledAsync(page, pageSize, customer, from, to);
			var list = query.ToList();

			sw.Stop();

			return Ok(new {
				elapsedMs = sw.ElapsedMilliseconds,
				data = list
			});
		}

		// GET /api/orders/{id}
		[HttpGet("{id}", Name = "GetOrderById")]
		public async Task<ActionResult> GetById(int id)
		{
			var sw = Stopwatch.StartNew();

			// Use compiled entity query then map to DTO
			var orderEntity = _db.GetOrderByIdCompiled(id);
			if (orderEntity is null) return NotFound();

			var dto = new OrderDto
			{
				OrderId = orderEntity.OrderId,
				CustomerName = orderEntity.CustomerName,
				DatePlaced = orderEntity.DatePlaced,
				Items = orderEntity.Items.Select(oi => new OrderItemDto
				{
					InventoryItemId = oi.InventoryItemId,
					InventoryItemName = oi.InventoryItem?.Name ?? string.Empty,
					Quantity = oi.Quantity,
					UnitPrice = oi.UnitPrice
				}).ToList()
			};

			sw.Stop();

			return Ok(new { elapsedMs = sw.ElapsedMilliseconds, data = dto });
		}

		//POST /api/orders
	   [HttpPost]
		public async Task<ActionResult> Create([FromBody] CreateOrderDto dto)
		{
			// Validate incoming DTO minimally
			if (string.IsNullOrWhiteSpace(dto.CustomerName))
				return BadRequest("CustomerName is required.");

			var requestedIds = dto.InventoryItems.Select(x => x.ItemId).Distinct().ToList();
			if (!requestedIds.Any())
				return BadRequest("At least one inventory item is required.");

			var items = await _db.InventoryItems
				.Where(i => requestedIds.Contains(i.ItemId))
				.ToListAsync();

			var missing = requestedIds.Except(items.Select(i => i.ItemId)).ToList();
			if (missing.Any())
				return BadRequest($"The following InventoryItem ids were not found: {string.Join(',', missing)}");

			var order = new Order
			{
				CustomerName = dto.CustomerName,
				DatePlaced = dto.DatePlaced ?? DateTime.UtcNow
			};

			foreach (var line in dto.InventoryItems)
			{
				if (line.Quantity <= 0)
					return BadRequest($"Invalid quantity for item {line.ItemId}. Quantity must be >= 1.");

				order.Items.Add(new OrderItem
				{
					InventoryItemId = line.ItemId,
					Quantity = line.Quantity
				});
			}

			_db.Orders.Add(order);
			await _db.SaveChangesAsync();

			// invalidate orders caches
			_cacheService.IncrementVersion("orders_version");

			return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);

		}

		// DELETE /api/orders/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var order = await _db.Orders.FindAsync(id);
			if (order is null) return NotFound();

			_db.Orders.Remove(order);
			await _db.SaveChangesAsync();

			_cacheService.IncrementVersion("orders_version");

			return Ok(new { message = "Deleted" });
		}
	}
}
