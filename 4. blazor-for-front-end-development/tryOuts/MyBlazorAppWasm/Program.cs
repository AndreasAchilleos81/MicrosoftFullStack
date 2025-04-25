using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyBlazorAppWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped(sp => new HttpClient 
    { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<ApiService>();

Console.WriteLine(builder.HostEnvironment.Environment);
Console.WriteLine(builder.HostEnvironment.BaseAddress);
Console.WriteLine(builder.HostEnvironment.IsDevelopment() ? "Development" : "Production");
await builder.Build().RunAsync();


