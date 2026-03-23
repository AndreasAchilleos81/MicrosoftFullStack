using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using SkillSnap.Api.DbContext;
using SkillSnap.Api.Services;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProjectsController : ControllerBase
{
	private readonly SkillSnapContext _skillSnapContext;
	private readonly ICacheService _cacheService;	
    private string cacheKey;

	public ProjectsController(SkillSnapContext skillSnapContext, ICacheService cacheService)
	{
		_skillSnapContext = skillSnapContext;
		_cacheService = cacheService;
		cacheKey = _cacheService.MakeVersionedKey("projects");
	}

	[HttpGet]
    [Authorize(Roles = "Admin, User")]
    [Route("GetProjects")]
	public async Task<List<Project>> GetProjects()
	{
        var projects = await _cacheService.GetOrCreateAsync(cacheKey, async () => { return await _skillSnapContext.Projects.ToListAsync(); });
        return projects;
	}

	[HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("AddProject")]
	public async Task<IActionResult> AddProject([FromBody] Project project)
	{
		await _skillSnapContext.Projects.AddAsync(project);
		await _skillSnapContext.SaveChangesAsync();

		// destroy cache key here to ensure we are getting the latest  
		_cacheService.IncrementVersion(cacheKey);
		return Ok(project);
	}
}
