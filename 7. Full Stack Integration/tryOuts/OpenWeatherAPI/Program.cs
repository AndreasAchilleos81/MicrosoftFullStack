using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenWeatherAPI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services.AddScoped<OpenWeatherAPI.Services.Caching>();
builder.Services.AddScoped<OpenWeatherAPI.Services.RateLimiter>();
builder.Services.AddScoped<OpenWeatherAPI.Services.WeatherApiService>();

await builder.Build().RunAsync();
