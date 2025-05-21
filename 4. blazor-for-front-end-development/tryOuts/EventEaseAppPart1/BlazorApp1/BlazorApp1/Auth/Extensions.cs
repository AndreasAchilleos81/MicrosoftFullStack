using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BlazorApp1.Auth
{
	public static class Extensions
	{
		public static void AddAuthRepositories(this IServiceCollection services, IConfiguration configuration)
		{
			var relativeLocation = configuration["Logging:ConnectionStrings:AuthConnection"].TrimEnd(';');
			var absolutePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeLocation));
			var connectionString = $"Data Source={absolutePath};";

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
			services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
			services.AddAuthorization();
			services.AddAuthentication();
		}
	}
}
