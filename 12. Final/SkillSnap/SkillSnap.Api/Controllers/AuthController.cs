using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.DTO;
using SkillSnap.Api.DbContext;

namespace SkillSnap.Api.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpPost]
		[Route("/api/auth/register")]
		public async Task<IActionResult> Register([FromBody] Registration registration)
		{
			var creationResult = await _userManager.CreateAsync(new ApplicationUser { UserName = registration.UserName, Email = registration.Email }, registration.Password);

			if (creationResult.Succeeded)
			{
				return Ok("Registration was successful");
			}
			return BadRequest(creationResult.Errors);
		}

		[HttpPost]
		[Route("/api/auth/login")]
		public async Task<IActionResult> Login([FromBody] Login login)
		{
			var loginResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, false, false);
			if (loginResult.Succeeded)
			{
				return Ok("Logging in was successful");
			}

			return BadRequest("Failed to login");
		}
	}
}
