using Microsoft.EntityFrameworkCore;
using SafeVault.Database;
using SafeVault.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, builder.Configuration["SafeValueDatabase"]));
builder.Services.AddDbContext<SafeVaultDbContext>(options => 
{
	options.UseSqlite("Data Source=" + path);
});

builder.Services.AddSingleton<AuthService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<SafeVaultDbContext>();
	dbContext.Database.EnsureCreated();
}
app.UseHttpsRedirection();

app.MapGet("/", () => "This is root!!!");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");	

app.MapControllers();
app.Run();

