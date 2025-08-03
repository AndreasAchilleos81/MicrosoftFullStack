using DPInjection.interfaces;
using Microsoft.JSInterop;

namespace DPInjection.interfaces
{
	public interface IMyService
	{
		Task LogCreation(string message);
	}
}


namespace DPInjection.Services
{
	public class MyService : IMyService
	{
		private readonly string _serviceId;
		public MyService()
		{
			_serviceId = Guid.NewGuid().ToString();
			LogCreation($"Service ID: {_serviceId}");
		}
		public async Task LogCreation(string message)
		{
			// Implementation of logging the creation of the service
			Console.WriteLine($"{_serviceId} - {message}");

		}
	}
}
