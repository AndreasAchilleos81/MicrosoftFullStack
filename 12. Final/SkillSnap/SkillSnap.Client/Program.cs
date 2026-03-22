using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SkillSnap.Client;
using SkillSnap.Client.Middleware;
using SkillSnap.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Plain client for Auth
builder.Services.AddHttpClient("BaseClient", client =>
    client.BaseAddress = new Uri(builder.Configuration["serverUrl"]!));

// 2. Authorized client for Data
builder.Services.AddTransient<AuthorizeApiDefinedMessageHandler>();
builder.Services.AddHttpClient("ServerApi", client =>
    client.BaseAddress = new Uri(builder.Configuration["serverUrl"]!))
    .AddHttpMessageHandler<AuthorizeApiDefinedMessageHandler>();

builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<ProfilesService>();
builder.Services.AddScoped<AuthService>();

// FIX: Instead of 'new HttpClient', use the Factory to create the 'ServerApi' version
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ServerApi");
});

await builder.Build().RunAsync();