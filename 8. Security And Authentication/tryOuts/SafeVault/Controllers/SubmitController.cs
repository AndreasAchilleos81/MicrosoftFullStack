using Microsoft.AspNetCore.Mvc;
using SafeVault.Database;
using SafeVault.Models;

namespace SafeVault.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SubmitController : ControllerBase
	{
		private readonly SafeVaultDbContext _safeVaultDbContext;
		public SubmitController(SafeVaultDbContext safeVaultDbContext)
		{
			_safeVaultDbContext = safeVaultDbContext;
		}

		[HttpPost]
		[Route("")]
		public async Task<IActionResult> PostData()
		{

			//string allowedSpecialCharactersForPassword = "!@#$%^&*";
			//string maliciousInput = "<script>alert('XSS')</script>";
			//_safeVaultDbContext.Users.Add(new User
			//{
			//	Username = "testUser",
			//	Password = "Password!",
			//	Email = "testUser@email.com"
			//});
			//_safeVaultDbContext.SaveChanges();

			return Ok("All is good!!!");
		}
	}
}
