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
		public static void AddDataRepositories(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton(configuration);
			services.AddSingleton<IGenericRepository<EventCard>, EventCardRepository>();
			services.AddSingleton<IGenericRepository<User>, UserRepository>();
			services.AddSingleton<IGenericRepository<Registration>, RegistrationRepository>();
			services.AddSingleton<IGenericRepository<Attendance>, AttendanceRepository>();
			services.AddSingleton<IGenericRepository<Session>, SessionRepository>();
			services.AddSingleton<SessionManagement>();
			services.AddScoped<ApplicationStorage>();
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