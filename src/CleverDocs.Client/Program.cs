using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CleverDocs.Client;
using CleverDocs.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client for API calls
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    // Set API base URL - adjust this to your actual API URL
    if (builder.HostEnvironment.IsProduction())
    {
        httpClient.BaseAddress = new Uri("https://your-api-domain.com/");
    }
    else
    {
        httpClient.BaseAddress = new Uri("https://localhost:7001/"); // Development API URL
    }
    return httpClient;
});

// Register authentication service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Initialize authentication service
var authService = app.Services.GetRequiredService<IAuthenticationService>();
await authService.InitializeAsync();

await app.RunAsync();
