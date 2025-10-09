using JwtAuthDemo.Models;
using JwtAuthDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly TokenService _tokenService;

    private static readonly List<User> Users = new()
    {
        new User { Email = "testuser", Password = "password123" },
    };

    public UserController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
        // In a real application, you would validate the user's credentials here.
        // For demonstration purposes, we'll assume the credentials are valid.

        var existingUser = Users.FirstOrDefault(u =>
            u.Email == user.Email && u.Password == user.Password
        );
        if (existingUser == null)
        {
            return Unauthorized("Invalid credentials");
        }

        var userId = Guid.NewGuid().ToString(); // This would be fetched from your user store
        var token = _tokenService.CreateToken(userId, user.Email);

        return Ok(new { Token = token });
    }

    [HttpGet("secure-data")]
    [Authorize]
    public IActionResult GetSecureData()
    {
        return Ok("This is protected data accessible only with a valid JWT.");
    }
}
