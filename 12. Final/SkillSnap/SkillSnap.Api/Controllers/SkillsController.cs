using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.DbContext;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using SkillSnap.Api.Services;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SkillsController : ControllerBase
{
	private readonly SkillSnapContext _skillSnapContext;
	private readonly ICacheService _cacheService;
	private string cacheKey;

    public SkillsController(SkillSnapContext skillSnapContext, ICacheService cacheService)
	{
		_skillSnapContext = skillSnapContext;
		_cacheService = cacheService;
		cacheKey = _cacheService.MakeVersionedKey("skills");
	}

	[HttpGet]
	[Authorize(Roles = "Admin, User")]
	[Route("GetSkills")]
	public async Task<IActionResult> GetSkills()
	{
		// make this a Pre compiled query for better performance
		var skills = await _cacheService.GetOrCreateAsync(cacheKey, 
				async () => 
		{ 
			return await _skillSnapContext.Skills.AsNoTracking().Include(p => p.PortfolioUsers).ToListAsync(); 
		});
		return Ok(skills);
	}

	[HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("AddSkill")]
	public async Task<IActionResult> AddSkill([FromBody]Skill skill)
	{
		await _skillSnapContext.Skills.AddAsync(skill);
		await _skillSnapContext.SaveChangesAsync();

		// destroy cache key here to ensure we are getting the latest  
		_cacheService.IncrementVersion(cacheKey);
        return Ok(skill);
	}
}