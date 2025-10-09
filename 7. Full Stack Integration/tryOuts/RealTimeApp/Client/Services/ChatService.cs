using Microsoft.AspNetCore.SignalR.Client;
using Shared.Models;

namespace Client.Services
{
    public class ChatService
    {
        private readonly HubConnection _hubConnection;

        public Action<ChatMessage>? OnMessageReceived { get; set; }

        public ChatService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5059/chathub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<ChatMessage>(
                "ReceiveMessage",
                (chatMessage) =>
                {
                    OnMessageReceived?.Invoke(chatMessage);
                    Console.WriteLine($"{chatMessage}");
                }
            );
        }

        public async Task StartAsync()
        {
            await _hubConnection.StartAsync();
        }

        public async Task SendMessageAsync(ChatMessage chatMessage)
        {
            await _hubConnection.SendAsync("SendMessage", chatMessage);
        }
    }
}
