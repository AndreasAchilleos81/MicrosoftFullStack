using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Shared.Models;
using Shared.Models.DTO;
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
        var projects = await _cacheService.GetOrCreateAsync(cacheKey,
            async () =>
        {
            return await _skillSnapContext
                            .Projects
                            .AsNoTracking()
                            .Include(s => s.PortfolioUsers)
                            .ToListAsync();
        });

        return projects;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    [Route("GetProject")]
    public async Task<Project> GetProject(int id)
    {
        // we are keeping tracking for getting ONE project to make our lives easier when saving its and its navigatioal properties
        var project = await _skillSnapContext
                            .Projects
                            .Include(p => p.PortfolioUsers).ThenInclude(s => s.Skills)
                            .FirstOrDefaultAsync(p => p.Id == id);
        return project!;
    }

    [HttpPut]
    [Authorize(Roles = "Admin, User")]
    [Route("UpdateProject")]
    public async Task<IActionResult> UpdateProject(ProjectDto dto)
    {
        var project = await _skillSnapContext.Projects
            .Include(p => p.PortfolioUsers)
                .ThenInclude(u => u.Skills)
            .FirstOrDefaultAsync(p => p.Id == dto.value.Id);

        if (project == null)
            return NotFound();

        // Update project fields
        project.Title = dto.value.Title;
        project.Description = dto.value.Description;
        project.ImageUrl = dto.value.ImageUrl;

        // Update users
        foreach (var userDto in dto.value.PortfolioUsers)
        {
            var user = project.PortfolioUsers.FirstOrDefault(u => u.Id == userDto.Id);

            if (user == null)
            {
                // New user
                user = new PortfolioUser
                {
                    Name = userDto.Name,
                    Bio = userDto.Bio,
                    ProfilePictureUrl = userDto.ProfilePictureUrl
                };

                project.PortfolioUsers.Add(user);
            }
            else
            {
                // Update existing user
                user.Name = userDto.Name;
                user.Bio = userDto.Bio;
                user.ProfilePictureUrl = userDto.ProfilePictureUrl;

                // Replace skills
                user.Skills.Clear();

                foreach (var skillDto in userDto.Skills)
                {
                    user.Skills.Add(new Skill
                    {
                        Name = skillDto.Name,
                        Level = skillDto.Level
                    });
                }
            }
        }

        await _skillSnapContext.SaveChangesAsync();
        return Ok(project);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [Route("DeleteProject")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _skillSnapContext.Projects
                .Include(p => p.PortfolioUsers)
                    .ThenInclude(u => u.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        _skillSnapContext.Projects.Remove(project);
        await _skillSnapContext.SaveChangesAsync();

        // destroy cache key here to ensure we are getting the latest  
        _cacheService.IncrementVersion(cacheKey);
        return NoContent();
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
