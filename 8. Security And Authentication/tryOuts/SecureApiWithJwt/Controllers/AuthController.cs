namespace SecureApiWithJwt.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SecureApiWithJwt.Models;
    using SecureApiWithJwt.Services;

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // For demonstration purposes, we are using hardcoded credentials.
            // In a real application, you should validate against a user store.
            if (
                (request.Username == "testuser" || request.Username == "adminuser")
                && request.Password == "password"
            )
            {
                var token = _tokenService.GenerateToken(request.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }
}
