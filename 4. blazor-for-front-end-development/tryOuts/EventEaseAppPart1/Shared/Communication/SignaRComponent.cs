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

        public async Task EnsureConnectionOpen()
        {
			if (HubConnection.State == HubConnectionState.Disconnected)
			{
				await HubConnection.StartAsync();
			}
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

		public async Task<bool> IsSessionActive(string userId)
        {
			return await HubConnection.InvokeAsync<bool>("IsSessionActive", userId);
		}

		public async Task<string> GetLoggedInUserId()
		{
			return await HubConnection.InvokeAsync<string>("GetLoggedInUserId");
		}

		public async Task<bool> IsAdmin(string userId)
		{
			return await HubConnection.InvokeAsync<bool>("IsAdmin", userId);
		}

		public async Task NotifySessionStatusChanged(string userId, bool isLoggedIn)
		{
            await HubConnection.InvokeAsync("UpdateSessionStatus", userId, isLoggedIn);
		}

		public async Task<bool> AddCard(EventCard card)
		{
			return await HubConnection.InvokeAsync<bool>("AddCard", card);
		}

		public async Task<bool> UpdateCard(EventCard card)
		{
			return await HubConnection.InvokeAsync<bool>("UpdateEventCard", card);
		}

		public async Task<EventCard> GetCard(string cardId)
		{
			return await HubConnection.InvokeAsync<EventCard>("GetCard", cardId);
		}

		public async Task<bool> DeleteCard(EventCard card)
		{
			return await HubConnection.InvokeAsync<bool>("DeleteCard", card);
		}
	}
}