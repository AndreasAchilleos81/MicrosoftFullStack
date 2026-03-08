using LogicTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LogicTrack.Models.OrderController;

namespace LogicTrack.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public partial class OrderController : ControllerBase
	{
		private readonly LogicTrackContext _db;

		public OrderController(LogicTrackContext db)
		{
			_db = db;
		}

		// GET /api/orders
		[Authorize(Roles = "Manager")]
		[HttpGet]
		public async Task<ActionResult<List<OrderSummaryDto>>> GetAll()
		{

			var sum = await _db.GetOrderSummaries();
			return Ok(await sum.ToListAsync());
		}

		// GET /api/orders/{id}
		[HttpGet("{id}", Name = "GetOrderById")]
		public async Task<ActionResult<Order>> GetById(int id)
		{
			var order = await _db.Orders
				.AsNoTracking()
				.Include(o => o.Items) // include related inventory items
				.FirstOrDefaultAsync(o => o.OrderId == id);

			if (order is null) return NotFound();

			return Ok(order);
		}

		//POST /api/orders
		[Authorize(Roles = "Manager")]
		[HttpPost]
		public async Task<ActionResult<Order>> Create([FromBody] CreateOrderDto dto)
		{
			// Validate incoming DTO minimally
			if (string.IsNullOrWhiteSpace(dto.CustomerName))
				return BadRequest("CustomerName is required.");

			// Load inventory items referenced by the client
			var items = await _db.InventoryItems
				.Where(i => dto.InventoryItemIds.Contains(i.ItemId))
				.ToListAsync();

			// Create order and attach existing inventory items (many-to-many)
			var order = new Order
			{
				CustomerName = dto.CustomerName,
				DatePlaced = dto.DatePlaced ?? DateTime.UtcNow
			};

			// Attach found InventoryItem entities to the order
			foreach (var it in items)
			{
				order.Items.Add(it.ConvertTo());
			}

			_db.Orders.Add(order);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);

		}

		// DELETE /api/orders/{id}
		[Authorize(Roles = "Manager")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var order = await _db.Orders.FindAsync(id);
			if (order is null) return NotFound();

			_db.Orders.Remove(order);
			await _db.SaveChangesAsync();

			return NoContent();
		}
	}
}
