using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.DbContext;
using Shared.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SkillsController : ControllerBase
{
	private readonly SkillSnapContext _skillSnapContext;

	public SkillsController(SkillSnapContext skillSnapContext)
	{
		this._skillSnapContext = skillSnapContext;
	}

	[HttpGet]
	[Route("GetSkills")]
	public async Task<IActionResult> GetSkills()
	{
		// make this a Pre compiled query for better performance
		var skills = await _skillSnapContext.Skills.ToListAsync();
		return Ok(skills);
	}

	[HttpPost]
	[Route("AddSkill")]
	public async Task<IActionResult> AddSkill([FromBody]Skill skill)
	{
		await _skillSnapContext.Skills.AddAsync(skill);
		await _skillSnapContext.SaveChangesAsync();
		return Ok(skill);
	}

}
