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
        return GenerateToken(userLogin, false);
    }

    public string GenerateRefreshToken(UserLogin userLogin)
    {
        return GenerateToken(userLogin, true);
    }

    private string GenerateToken(UserLogin userLogin, bool refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = null;

        // Or Try this:
        // var base64Key = builder.Configuration["Authentication:Schemes:Bearer:SigningKeys:0:Value"];
        // var key = Convert.FromBase64String(base64Key);

        key = Encoding.UTF8.GetBytes(
            _configuration[refreshToken ? "Jwt:refreshTokenKey" : "Jwt:Key"]!
        );

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
            Expires = refreshToken
                ? DateTime.UtcNow.AddDays(7)
                : DateTime.UtcNow.AddMinutes(
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
}
