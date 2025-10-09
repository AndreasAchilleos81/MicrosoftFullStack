namespace RoleClaimsApp.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class UsersControllers : ControllerBase
{
    [HttpGet("role-based")]
    public IActionResult GetUsersGyRole()
    {
        // Simulate a logged in user with a role
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, "Alice"),
                    new Claim(ClaimTypes.Role, "Admin"),
                },
                "mock"
            )
        );

        HttpContext.User = user;

        // Perform role-based authorization check
        if (HttpContext.User.IsInRole("Admin"))
        {
            return Ok("Access granted to Admin resources.");
        }
        else
        {
            return Forbid();
        }
    }

    [HttpGet("claim-based")]
    [Authorize(Policy = "RequireITDepartment")]
    public IActionResult GetUsersByClaim()
    {
        // Simulate a logged in user with a claim
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.Name, "Bob"), new Claim("Department", "IT") },
                "mock"
            )
        );

        HttpContext.User = user;

        // Perform claim-based authorization check
        if (HttpContext.User.HasClaim("Department", "IT"))
        {
            return Ok("Access granted to IT Department resources.");
        }
        else
        {
            return Forbid();
        }
    }
}
