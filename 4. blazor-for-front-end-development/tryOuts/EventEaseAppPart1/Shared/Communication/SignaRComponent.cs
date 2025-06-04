using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Shared.Models;
using Shared.Result;

namespace Shared.Communication
{
    public class SignalRService
    {
        public HubConnection HubConnection { get; }

        public SignalRService(NavigationManager navigationManager)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/datahub"))
                .Build();
        }

        public async Task<RegistrationResult> SendUserAsync(User user)
        {
           return await HubConnection.InvokeAsync<RegistrationResult>("SaveUser", user);
        }

        public async Task<LoginResult> LoginUserAsync(LoginModel loginModel)
		{
			return await HubConnection.InvokeAsync<LoginResult>("LoginUser", loginModel);
		}

		public async Task LogoutUserAsync(string userId)
		{
			await HubConnection.InvokeAsync<LoginResult>("LogoutUser", userId);
		}

        public async Task<string> GetUserId(string email)
        {
            return await HubConnection.InvokeAsync<string>("GetUserId", email);
		}
	}
}