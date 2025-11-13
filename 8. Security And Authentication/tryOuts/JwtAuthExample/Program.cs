using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JwtAuthExample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<TokenService>();

var jwtSection = builder.Configuration.GetSection("Jwt");

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.UseSecurityTokenValidators = true;
        options.IncludeErrorDetails = true;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"Authorization Header: {authHeader}");
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);
                    Console.WriteLine(jwt.Header.Alg); // Should print "HS256"

                    if (!string.IsNullOrWhiteSpace(token) && token.Count(c => c == '.') == 2)
                    {
                        context.Token = token;
                    }
                    else
                    {
                        Console.WriteLine("Malformed token detected.");
                    }
                }
                else
                {
                    // Fallback to cookie
                    if (context.Request.Cookies.Count() != 0)
                    {
                        var token = context.Request.Cookies["accessToken"];
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(token);
                        Console.WriteLine(jwt.Header.Alg); // Should print "HS256"
                        Console.WriteLine($"Cookie Token: {token}");

                        if (!string.IsNullOrEmpty(token))
                        {
                            if (
                                !string.IsNullOrWhiteSpace(token)
                                && token.Count(c => c == '.') == 2
                            )
                            {
                                context.Token = token;
                            }
                            else
                            {
                                Console.WriteLine("Malformed token detected.");
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Error: {context.Exception.Message}");
                Console.WriteLine(context.Exception.StackTrace);
                return Task.CompletedTask;
            },
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"], // 👈 Must match token
            ValidAudience = jwtSection["Audience"], // 👈 Must match token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
            RoleClaimType = "role",
        };
    });

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

var app = builder.Build();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
