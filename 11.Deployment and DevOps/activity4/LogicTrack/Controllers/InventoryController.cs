using LogicTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace LogicTrack.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InventoryController : ControllerBase
	{
		private readonly LogicTrackContext _db;
		private readonly IMemoryCache _cache;

		public InventoryController(LogicTrackContext db, IMemoryCache cache)
		{
			_db = db;
			_cache = cache;
		}

		// GET /api/inventory
		[Authorize(Roles = "Manager")]
		[HttpGet]
		public async Task<ActionResult> GetAll([FromQuery] int page = 0, [FromQuery] int pageSize = 50, [FromQuery] string? q = null)
		{
			var sw = Stopwatch.StartNew();

			page = Math.Max(0, page);
			pageSize = Math.Clamp(pageSize, 1, 500);

			// caching versioning to allow easy invalidation on mutations
			int version = 0;
			_cache.TryGetValue("Inventory_Version", out version);

			string cacheKey = $"inventory:page={page}:size={pageSize}:q={q ?? "null"}:v={version}";

			if (!_cache.TryGetValue(cacheKey, out List<InventoryItem>? list))
			{
				// Build base query with AsNoTracking for read-only performance
				var query = _db.InventoryItems.AsNoTracking().AsQueryable();

				if (!string.IsNullOrWhiteSpace(q))
				{
					// Use EF.Functions.Like to translate to SQL LIKE for indexed searches
					query = query.Where(i => EF.Functions.Like(i.Name, $"%{q}%"));
				}

				list = await query
					.OrderBy(i => i.Name)
					.Skip(page * pageSize)
					.Take(pageSize)
					.ToListAsync();

				_cache.Set(cacheKey, list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));
			}

			sw.Stop();
			return Ok(new { data = list, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
		}

		// GET /api/inventory/{id} (used by POST CreatedAtAction)
		[HttpGet("{id}", Name = "GetInventoryById")]
        public async Task<ActionResult> GetById(int id)
        {
            var sw = Stopwatch.StartNew();

            // Use AsNoTracking for read-only get to avoid unnecessary change tracking
            var item = await _db.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ItemId == id);
            if (item is null)
            {
                sw.Stop();
                return NotFound(new { error = $"Inventory Item with Id {id} was not found", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
            }

            sw.Stop();
            return Ok(new { data = item, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
        }

		// POST /api/inventory
		[Authorize(Roles = "Manager")]
		[HttpPost]
        public async Task<ActionResult> Create([FromBody] InventoryItem item)
        {
            var sw = Stopwatch.StartNew();

            _db.InventoryItems.Add(item);
            await _db.SaveChangesAsync();

            // bump inventory version to invalidate caches
            int version = 0;
            _cache.TryGetValue("Inventory_Version", out version);
            _cache.Set("Inventory_Version", version + 1);

            sw.Stop();
            return CreatedAtRoute("GetInventoryById", new { id = item.ItemId }, new { data = item, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
        }

		// DELETE /api/inventory/{id}
		[Authorize(Roles = "Manager")]
		[HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var sw = Stopwatch.StartNew();

            var item = await _db.InventoryItems.FindAsync(id);
            if (item is null)
            {
                sw.Stop();
                return NotFound(new { error = $"Inventory Item with Id {id} was not found", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
            }

            _db.InventoryItems.Remove(item);
            await _db.SaveChangesAsync();

            // bump inventory version to invalidate caches
            int version = 0;
            _cache.TryGetValue("Inventory_Version", out version);
            _cache.Set("Inventory_Version", version + 1);

            sw.Stop();
            return Ok(new { message = "Deleted", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
        }
	}
}
