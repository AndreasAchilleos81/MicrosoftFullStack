using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
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
        [Authorize(Roles = "Admin, User")]
        [Route("GetPortfolioUser")]
		public async Task<List<PortfolioUser>> GetPortfolioUserAsync()
		{
			var portfolioUsers = await _skillSnapContext.PortfolioUsers.ToListAsync();
			return portfolioUsers;
		}
	}
}
