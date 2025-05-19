using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repository;
using Shared.Services;

namespace Shared.Extentions
{
	public static class Extentions
	{
		public static void AddDataService(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<IEventCardDataService, EventCardDataService>();
		}

		public static void AddDataRepositories(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<IGenericRepository<EventCard>, EventCardRepository<EventCard>>();
		}
	}
}
