using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repository;

namespace Shared.Extentions
{
	public static class Extentions
	{
		public static void AddDataRepositories(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<IGenericRepository<EventCard>, EventCardRepository>();
			services.AddSingleton<IGenericRepository<User>, UserRepository>();
			services.AddSingleton<IGenericRepository<Registration>, RegistrationRepository>();
		}

		public static Registration CreateRegistration(this User user)
		{
			return new Registration
			{
				UserId = user.Id.ToString(),
				RegisteredAt = DateTime.UtcNow,
				TerminatedAt = null
			};
		}
	}
}