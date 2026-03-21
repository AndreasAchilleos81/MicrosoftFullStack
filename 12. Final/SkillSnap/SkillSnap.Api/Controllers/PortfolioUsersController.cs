using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.DbContext;

namespace SkillSnap.Api.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class PortfolioUsersController : ControllerBase
	{
		private readonly SkillSnapContext _skillSnapContext;

		public PortfolioUsersController(SkillSnapContext skillSnapContext)
		{
				_skillSnapContext = skillSnapContext;	
		}

		[HttpGet]
		[Route("GetPortfolioUser")]
		public async Task<IActionResult> GetPortfolioUserAsync()
		{
			var portfolioUsers = await _skillSnapContext.PortfolioUsers.ToListAsync();
			return Ok(portfolioUsers);
		}
	}
}
