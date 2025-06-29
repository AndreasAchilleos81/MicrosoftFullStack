﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Shared.Models;
using Shared.Result;

namespace Shared.Communication
{
	public class SignalRService
	{
		private readonly string _hubUrl;
		private HubConnection _hubConnection;
		public HubConnection HubConnection => _hubConnection ??= new HubConnectionBuilder()
	   .WithUrl(_hubUrl)
		.WithAutomaticReconnect()
	   .Build();

		public SignalRService(IConfiguration config)
		{
			_hubUrl = config["SignalR:HubUrl"] ?? throw new InvalidOperationException("Hub URL is not configured.");
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

		public async Task AddToEventGroup(string eventId)
		{
			await HubConnection.InvokeAsync("AddToEventGroup", eventId);
		}

		public async Task RemoveFromEventGroup(string eventId)
		{
			await HubConnection.InvokeAsync("RemoveFromEventGroup", eventId);
		}

		public async Task EventCardTimeApproaching(string eventId, string message)
		{
			await HubConnection.InvokeAsync("EventCardTimeApproaching", eventId, message);
		}

		public async Task<bool> InterestedInEvent(string eventId, string userId)
		{
			return await HubConnection.InvokeAsync<bool>("InterestedInEvent", eventId, userId);
		}

		public async Task<bool> GoingToEvent(string eventId, string userId)
		{
			return await HubConnection.InvokeAsync<bool>("GoingToEvent", eventId, userId);
		}

		public async Task<IEnumerable<EventCard>> GetEvents(IEnumerable<string> eventIds)
		{
			return await HubConnection.InvokeAsync<IEnumerable<EventCard>>("GetEvents", eventIds);
		}

		public async Task<IEnumerable<Attendance>> Attendances(string userId)
		{
			return await HubConnection.InvokeAsync<IEnumerable<Attendance>>("Attendances", userId);
		}
	}
}