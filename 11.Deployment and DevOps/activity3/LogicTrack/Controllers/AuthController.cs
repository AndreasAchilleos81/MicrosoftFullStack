using LogicTrack.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using System.Security.Claims;

namespace LogicTrack.Controllers
{
	[ApiController]
	public class AuthController : ControllerBase
	{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }

		
		[HttpPost]
		[Route("api/auth/register")]
		public async Task<IActionResult> Register([FromBody] RegistrationUser user)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var identityUser = user.ToIdentityUser();
			var registrationResults = await _userManager.CreateAsync(identityUser, user.Password);

			if (registrationResults.Succeeded)
			{
				// Generate an email confirmation token and return a confirmation link (send via email in production)
				var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
				var encodedToken = WebUtility.UrlEncode(token);
				var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirmemail?userId={identityUser.Id}&token={encodedToken}";

				return Created(string.Empty, new { message = "User registered successfully. Please confirm your email.", confirmationLink = callbackUrl });
			}
			// Do not reveal detailed errors in production
			return BadRequest("Registration failed. Please try again later or contact support.");
		}

		[HttpGet]
		[Route("api/auth/confirmemail")]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
			{
				return BadRequest("Missing userId or token.");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Invalid user.");
			}

			var decodedToken = WebUtility.UrlDecode(token);
			var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
			if (result.Succeeded)
			{
				return Ok("Email confirmed successfully.");
			}
			return BadRequest("Email confirmation failed.");
		}

		[HttpPost]
		[Route("api/auth/login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var user = await _userManager.FindByNameAsync(request.UserName);
			if (user == null)
				return Unauthorized("Invalid credentials");

			if (!await _userManager.CheckPasswordAsync(user, request.Password))
				return Unauthorized("Invalid credentials");

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

			return Ok(new { token = tokenString, expires = token.ValidTo });
		}
	}
}
