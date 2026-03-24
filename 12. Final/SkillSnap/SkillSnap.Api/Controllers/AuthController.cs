using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Shared.Models.DTO;
using SkillSnap.Api.DbContext;
using System.Security.Claims;
using System.Text;

namespace SkillSnap.Api.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IConfiguration _configuration;

		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_signInManager = signInManager;
            _configuration = configuration;
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
			var user = await _userManager.FindByNameAsync(login.UserName);


            if (loginResult.Succeeded)
			{
                // create claims
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                int expiryMinutes = 60;
                if (int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var v)) expiryMinutes = v;

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                return Ok(new { token = tokenString, role = roleClaim, userId = user.Id, expires = token.ValidTo });
			}

			return BadRequest("Failed to login");
		}
	}
}
