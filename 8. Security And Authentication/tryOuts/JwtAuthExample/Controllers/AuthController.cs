using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtAuthExample.Models;
using JwtAuthExample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthExample.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    private readonly IConfiguration _configuration;

    public AuthController(TokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin userLogin)
    {
        // Validate the user credentials (this is just a demo, so we'll skip actual validation)
        var token = _tokenService.GenerateJwtToken(userLogin);
        var refreshToken = _tokenService.GenerateRefreshToken(userLogin);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddMinutes(60) //DateTime.UtcNow.AddDays(
            //     double.Parse(_configuration["Jwt:refreshTokenDurationInDays"]!)
            // )
            ,
            Secure = false,
            SameSite = SameSiteMode.Strict,
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        Response.Cookies.Append("accessToken", token, cookieOptions);
        return Ok(new { Token = token });
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("No refresh token provided.");
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:refreshTokenKey"]!);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };

        var principal = tokenHandler.ValidateToken(
            refreshToken,
            validationParameters,
            out var validatedToken
        );
        var userId = principal.FindFirst("UserId")?.Value;
        var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        var userName = principal.FindFirst("UserName")?.Value ?? "DefaultUser";
        if (userId == null || role == null)
        {
            return Unauthorized("Invalid refresh token.");
        }
        var newAccessToken = _tokenService.GenerateJwtToken(
            new UserLogin
            {
                UserId = userId,
                Role = role,
                Username = userName,
            }
        );

        Response.Cookies.Append(
            "accessToken",
            newAccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Secure = false,
                SameSite = SameSiteMode.Strict,
            }
        );

        return Ok(new { Token = newAccessToken });
    }
}
