using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthExample.Models;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthExample.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(UserLogin userLogin)
    {
        // Normally, you'd get these values from configuration
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("UserId", userLogin.UserId),
                    new Claim("UserName", userLogin.Username),
                    new Claim(ClaimTypes.Role, userLogin.Role),
                    new Claim(ClaimTypes.Expiration, _configuration["Jwt:DurationInMinutes"]!),
                }
            ),
            Expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:DurationInMinutes"]!)
            ),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(UserLogin userLogin)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:refreshTokenKey"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("UserId", userLogin.UserId),
                    new Claim("UserName", userLogin.Username),
                    new Claim(ClaimTypes.Role, userLogin.Role),
                    new Claim(ClaimTypes.Expiration, _configuration["Jwt:DurationInMinutes"]!),
                }
            ),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
