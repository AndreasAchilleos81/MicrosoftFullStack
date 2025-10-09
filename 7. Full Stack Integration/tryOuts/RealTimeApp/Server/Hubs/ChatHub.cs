using Microsoft.AspNetCore.SignalR;
using Shared.Models;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub() { }

        public async Task SendMessage(ChatMessage chatMessage)
        {
            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }
    }
}
