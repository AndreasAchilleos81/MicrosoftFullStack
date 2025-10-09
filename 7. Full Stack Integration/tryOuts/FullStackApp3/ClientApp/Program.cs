using ClientApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// Create a WebAssemblyHostBuilder using command-line arguments and default configuration.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register the main App component to the DOM element with id "app".
builder.RootComponents.Add<App>("#app");

// Register the HeadOutlet component for managing the document head.
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient as a scoped service with the base address from configuration.
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["serverUrl"]),
});

// Build the WebAssemblyHost instance.
var app = builder.Build();

// Start the Blazor WebAssembly application asynchronously.
app.RunAsync();

/*
Copilot Contribution:
GitHub Copilot assisted in scaffolding the Blazor WebAssembly startup logic, suggesting component registration,
and generating the HttpClient service setup using configuration. Copilot also provided concise comments for clarity */
