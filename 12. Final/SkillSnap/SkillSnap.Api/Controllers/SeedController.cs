using Microsoft.AspNetCore.Mvc;
using SkillSnap.Api.DbContext;
using Shared.Models;
using Bogus;
using Microsoft.AspNetCore.Authorization;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SeedController : ControllerBase
{
	private readonly SkillSnapContext _context;

	public SeedController(SkillSnapContext context)
	{
		_context = context;
	}

	[HttpPost]
	[Authorize(Roles ="Admin")]
	public IActionResult Seed()
	{
		if (_context.PortfolioUsers.Any())
		{
			return BadRequest("Sample data already exists.");
		}
		var user = new PortfolioUser
		{
			Name = "Jordan Developer",
			Bio = "Full-stack developer passionate about learning new tech.",
			ProfilePictureUrl = "https://example.com/images/jordan.png",
			Projects = new List<Project>
			{
				new Project { Title = "Task Tracker", Description = "Manage tasks effectively", ImageUrl = "https://example.com/images/task.png" },
				new Project { Title = "Weather App", Description = "Forecast weather using APIs", ImageUrl = "https://example.com/images/weather.png" }
			},
			Skills = new List<Skill>
			{
				new Skill { Name = "C#", Level = "Advanced" },
				new Skill { Name = "Blazor", Level = "Intermediate" }
			}
		};
		_context.PortfolioUsers.Add(user);
		_context.SaveChanges();
		return Ok("Sample data inserted.");
	}


	[HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("PopulateMore")]
	public async Task<IActionResult> PopulateMore()
	{
		var portfolioUsersFaker = new Faker<PortfolioUser>()
			.RuleFor(u => u.Name, f => f.Name.FullName())
			.RuleFor(u => u.Bio, f => f.Lorem.Sentence())
			.RuleFor(u => u.ProfilePictureUrl, f => f.Image.PicsumUrl());

		var skills = new Faker<Skill>()
			.RuleFor(s => s.Name, f => f.Hacker.Abbreviation())
			.RuleFor(s => s.Level, f => f.PickRandom(new[] { "Beginner", "Intermediate", "Advanced" }));

		var projects = new Faker<Project>()
			.RuleFor(p => p.Title, f => f.Company.CatchPhrase())
			.RuleFor(p => p.Description, f => f.Lorem.Paragraph())
			.RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl());

		var portfolioUsers = portfolioUsersFaker.Generate(10);	

		foreach(var portfolioUser in portfolioUsers)
		{
			portfolioUser.Skills.AddRange(skills.Generate(5));
			portfolioUser.Projects.AddRange(projects.Generate(3));
		}

		_context.PortfolioUsers.AddRange(portfolioUsers);
		await _context.SaveChangesAsync();

		return Ok("More Data added");
	}

}
