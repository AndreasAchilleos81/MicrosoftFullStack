using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
			services.AddDefaultIdentity<IdentityUser>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.SignIn.RequireConfirmedPhoneNumber = false;
				options.SignIn.RequireConfirmedEmail = false;	
				options.SignIn.RequireConfirmedAccount = false;
			}).AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddAuthorization();
			services.AddAuthentication();
		}
	}
}
