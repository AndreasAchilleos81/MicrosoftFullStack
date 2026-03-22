using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using SkillSnap.Api.DbContext;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProjectsController : ControllerBase
{
	private readonly SkillSnapContext _skillSnapContext;

	public ProjectsController(SkillSnapContext skillSnapContext)
	{
		this._skillSnapContext = skillSnapContext;
	}

	[HttpGet]
    [Authorize(Roles = "Admin, User")]
    [Route("GetProjects")]
	public async Task<List<Project>> GetProjects()
	{
		var projects = await _skillSnapContext.Projects.ToListAsync();
		return projects;
	}

	[HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("AddProject")]
	public async Task<IActionResult> AddProject([FromBody] Project project)
	{
		await _skillSnapContext.Projects.AddAsync(project);
		await _skillSnapContext.SaveChangesAsync();
		return Ok(project);
	}
}
