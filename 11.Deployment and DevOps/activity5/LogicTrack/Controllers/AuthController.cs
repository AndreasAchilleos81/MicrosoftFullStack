using LogicTrack.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Diagnostics;
using System.Text;

namespace LogicTrack.Controllers
{
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IMemoryCache _memoryCache;

		public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_configuration = configuration;
		}

		[HttpPost]
		[Route("api/auth/register")]
		public async Task<IActionResult> Register([FromBody] RegistrationUser user)
		{
			var sw = Stopwatch.StartNew();

			if (!ModelState.IsValid)
			{
				sw.Stop();
				return BadRequest(new { errors = ModelState, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
			}

			var identityUser = user.ToIdentityUser();
			var registrationResults = await _userManager.CreateAsync(identityUser, user.Password);

			if (registrationResults.Succeeded)
			{
				// Generate an email confirmation token and return a confirmation link (send via email in production)
				var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
				var encodedToken = WebUtility.UrlEncode(token);
				var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirmemail?userId={identityUser.Id}&token={encodedToken}";

				sw.Stop();
				return Created(string.Empty, new { message = "User registered successfully. Please confirm your email.", confirmationLink = callbackUrl, executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
			}
			// Do not reveal detailed errors in production
			sw.Stop();
			return BadRequest(new { error = "Registration failed. Please try again later or contact support.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
		}

		[HttpGet]
		[Route("api/auth/confirmemail")]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
		{
			var sw = Stopwatch.StartNew();

			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
			{
				sw.Stop();
				return BadRequest(new { error = "Missing userId or token.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				sw.Stop();
				return BadRequest(new { error = "Invalid user.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
			}

			var decodedToken = WebUtility.UrlDecode(token);
			var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
			if (result.Succeeded)
			{
				sw.Stop();
				return Ok(new { message = "Email confirmed successfully.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
			}
			sw.Stop();
			return BadRequest(new { error = "Email confirmation failed.", executionTimeMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2) });
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