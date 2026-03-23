using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using SkillSnap.Api.DbContext;
using SkillSnap.Api.Services;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PortfolioUsersController : ControllerBase
    {
        private readonly SkillSnapContext _skillSnapContext;
        private readonly ICacheService _cacheService;

        public PortfolioUsersController(SkillSnapContext skillSnapContext, ICacheService cacheService)
        {
            _skillSnapContext = skillSnapContext;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [Route("GetPortfolioUser")]
        public async Task<List<PortfolioUser>> GetPortfolioUserAsync()
        {
            var key = _cacheService.MakeVersionedKey("PortfolioUsers");
            var portfolioUsers = await  _cacheService.GetOrCreateAsync<List<PortfolioUser>>(key, async () =>
            {
                return await _skillSnapContext.PortfolioUsers.ToListAsync();
            });

            return portfolioUsers;
        }
    }
}
