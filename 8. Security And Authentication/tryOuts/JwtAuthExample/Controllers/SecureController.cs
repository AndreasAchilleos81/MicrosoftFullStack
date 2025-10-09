using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthExample.Controllers;

[ApiController]
[Route("[controller]")]
public class SecureController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return Ok("This is a public endpoint.");
    }

    [HttpGet("protected")]
    [Authorize(Roles = "Admin")]
    public IActionResult ProtectedEndpoint()
    {
        var userName = User.Identity?.Name ?? "Unknown";
        return Ok($"This is a protected endpoint. Hello, {userName}!");
    }

    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public IActionResult UserEndpoint()
    {
        var userName = User.Identity?.Name ?? "Unknown";
        return Ok($"This is a protected endpoint. Hello, {userName}!");
    }

    [HttpGet("debug")]
    public IActionResult DebugClaims()
    {
        return Ok(
            new
            {
                AccessToken = this.HttpContext.Request.Cookies["accessToken"],
                IsAuthenticated = User.Identity?.IsAuthenticated,
                AuthenticationType = User.Identity?.AuthenticationType,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }),
                Name = User.Identity?.Name,
            }
        );
    }

    [HttpGet("whoami")]
    [Authorize]
    public IActionResult WhoAmI()
    {
        return Ok(User.Identity?.Name ?? "No identity");
    }
}
