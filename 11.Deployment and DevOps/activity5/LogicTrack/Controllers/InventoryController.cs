using LogicTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using LogicTrack.Services;
using Microsoft.Extensions.Caching.Memory;

namespace LogicTrack.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InventoryController : ControllerBase
	{
        private readonly LogicTrackContext _db;
        private readonly ICacheService _cacheService;

        public InventoryController(LogicTrackContext db, ICacheService cacheService)
        {
            _db = db;
            _cacheService = cacheService;
        }

		// GET /api/inventory
		[HttpGet]
		public async Task<ActionResult> GetAll([FromQuery] int page = 0, [FromQuery] int pageSize = 50, [FromQuery] string? q = null)
		{
			page = Math.Max(0, page);
			pageSize = Math.Clamp(pageSize, 1, 500);

			string baseKey = "inventory";
			string cacheKey = _cacheService.MakeVersionedKey(baseKey, ("page", page.ToString()), ("size", pageSize.ToString()), ("q", q ?? "null"));

			var list = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
			{
				var query = _db.InventoryItems.AsNoTracking().AsQueryable();

				if (!string.IsNullOrWhiteSpace(q))
					query = query.Where(i => EF.Functions.Like(i.Name, $"%{q}%"));

				return await query
					.OrderBy(i => i.Name)
					.Skip(page * pageSize)
					.Take(pageSize)
					.Select(i => new InventoryItemDto
					{
						ItemId = i.ItemId,
						Name = i.Name,
						Quantity = i.Quantity,
						Location = i.Location
					}).ToListAsync();
			}, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) });

			return Ok(list); // ApiResponseFilter will wrap with executionTimeMs
		}

		// GET /api/inventory/{id} (used by POST CreatedAtAction)
		[HttpGet("{id}", Name = "GetInventoryById")]
        public async Task<ActionResult> GetById(int id)
        {
            var entity = await _db.InventoryItems.AsNoTracking().FirstOrDefaultAsync(i => i.ItemId == id);
            if (entity is null) return NotFound(new { error = $"Inventory Item with Id {id} was not found" });

            var dto = new InventoryItemDto
            {
                ItemId = entity.ItemId,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Location = entity.Location
            };

            return Ok(dto);
        }

		// POST /api/inventory
		[Authorize(Roles = "Manager")]
		[HttpPost]
        public async Task<ActionResult> Create([FromBody] InventoryItem item)
        {
            _db.InventoryItems.Add(item);
            await _db.SaveChangesAsync();

            // bump inventory version to invalidate caches
            _cacheService.IncrementVersion("inventory_version");

            return CreatedAtRoute("GetInventoryById", new { id = item.ItemId }, item);
        }

		// DELETE /api/inventory/{id}
		[Authorize(Roles = "Manager")]
		[HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _db.InventoryItems.FindAsync(id);
            if (item is null)
            {
                return NotFound(new { error = $"Inventory Item with Id {id} was not found" });
            }

            _db.InventoryItems.Remove(item);
            await _db.SaveChangesAsync();

            // bump inventory version to invalidate caches
            _cacheService.IncrementVersion("inventory_version");

            return Ok(new { message = "Deleted" });
        }
	}
}
