using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Shared.Models;

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

        public async Task SendUserAsync(User user)
        {
            await HubConnection.InvokeAsync("SaveUser", user);
        }
    }
}