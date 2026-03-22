using Microsoft.JSInterop;
using Shared.Models.DTO;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services
{
	public class AuthService
	{
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "authToken";
        private readonly HttpClient _httpClient;
		private const string registrationUrl = "/api/auth/register";
		private const string loginUrl = "/api/auth/login";

		public AuthService(IHttpClientFactory factory, IJSRuntime jsRuntime)
		{
            _httpClient = factory.CreateClient("BaseClient");
            _jsRuntime = jsRuntime;
        }

		public async Task<bool> Register(Registration registration)
		{
			var result = await _httpClient.PostAsJsonAsync(registrationUrl, registration);
			return result.IsSuccessStatusCode ? true: false;
		}

        public async Task<bool> SignIn(Login login)
        {
            var result = await _httpClient.PostAsJsonAsync(loginUrl, login);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
                // Check: Are you actually calling the save method here?
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, response.Token);
                return true;
            }
            return false;
        }

        internal async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }
		
        public async Task RemoveTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
    }
}
