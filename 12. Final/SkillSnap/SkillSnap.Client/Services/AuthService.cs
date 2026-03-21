using Shared.Models.DTO;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services
{
	public class AuthService
	{
		private readonly HttpClient _httpClient;
		private const string registrationUrl = "/api/auth/register";
		private const string loginUrl = "/api/auth/login";

		public AuthService(HttpClient httpClient)
		{
			_httpClient = httpClient;	
		}

		public async Task<bool> Register(Registration registration)
		{
			var result = await _httpClient.PostAsJsonAsync(registrationUrl, registration);
			return result.IsSuccessStatusCode ? true: false;
		}

		public async Task<bool> SignIn(Login login)
		{
			var result = await _httpClient.PostAsJsonAsync(loginUrl, login);
			return result.IsSuccessStatusCode ? true : false;
		}
	}
}
