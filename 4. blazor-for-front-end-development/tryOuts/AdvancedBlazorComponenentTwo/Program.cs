using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AdvancedBlazorComponenentTwo;
using AdvancedBlazorComponenentTwo.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IDataService, Dataservice>();

await builder.Build().RunAsync();


// so one cascading parameter

//do one with parent calling child reference

// do one where list of products disappears DONE