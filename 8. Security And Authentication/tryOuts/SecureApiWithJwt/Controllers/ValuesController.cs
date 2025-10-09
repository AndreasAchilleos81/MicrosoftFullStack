namespace SecureApiWithJwt.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet()]
        [Authorize(Roles = "User, Admin")]
        public IActionResult Get()
        {
            var roles = this
                .User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .ToList();
            return Ok(
                roles.Any(r => r.Value == "Admin")
                    ? new[] { "AdminValue1", "AdminValue2", "AdminValue3" }
                    : new[] { "UserValue1", "UserValue2", "UserValue3" }
            );
        }
    }
}
