using LogicTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicTrack.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InventoryController : ControllerBase
	{
		private readonly LogicTrackContext _db;

		public InventoryController(LogicTrackContext db)
		{
			_db = db;
		}

		// GET /api/inventory
		[HttpGet]
		public async Task<ActionResult<List<InventoryItem>>> GetAll()
		{
			var items = await _db.InventoryItems
				.AsNoTracking()
				.ToListAsync();

			return Ok(items);
		}

		// GET /api/inventory/{id} (used by POST CreatedAtAction)
		[HttpGet("{id}", Name = "GetInventoryById")]
		public async Task<ActionResult<InventoryItem>> GetById(int id)
		{
			var item = await _db.InventoryItems.FindAsync(id);
			if (item is null) return NotFound();
			return Ok(item);
		}

		// POST /api/inventory
		[HttpPost]
		public async Task<ActionResult<InventoryItem>> Create([FromBody] InventoryItem item)
		{
			_db.InventoryItems.Add(item);
			await _db.SaveChangesAsync();

			// Returns 201 with Location header pointing to GET /api/inventory/{id}
			return CreatedAtRoute("GetInventoryById", new { id = item.ItemId }, item);
		}

		// DELETE /api/inventory/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var item = await _db.InventoryItems.FindAsync(id);
			if (item is null) return NotFound();

			_db.InventoryItems.Remove(item);
			await _db.SaveChangesAsync();

			return NoContent();
		}
	}
}
