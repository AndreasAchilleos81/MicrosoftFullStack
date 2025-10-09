using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemo;

public class JwtDecoder
{
    public ClaimsPrincipal? DecodeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SharedConstants.Secret);
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = SharedConstants.Issuer,
                ValidAudience = SharedConstants.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };

            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var validatedToken
            );

            Console.WriteLine($"Token is valid. Token: {validatedToken}");
            return principal;
        }
        catch
        {
            // Token validation failed
            return null;
        }
    }
}
