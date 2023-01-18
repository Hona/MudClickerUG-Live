using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudClicker.WebUI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("profile");

    // API audience
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

var apiBaseUrl = builder.HostEnvironment.IsDevelopment()
    ? "https://localhost:44357/api"
    : "https://agreeable-ocean-001b84200.2.azurestaticapps.net/api";

const string ApiClient = nameof(ApiClient);
builder.Services.AddHttpClient(ApiClient, 
        client => client.BaseAddress = new Uri(apiBaseUrl ?? throw new InvalidOperationException()))
    .AddHttpMessageHandler(sp =>
        sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { apiBaseUrl }
            )
    );

builder.Services.AddMudServices();

builder.Services.AddHttpClient<WeatherForecastClient>(ApiClient);

await builder.Build().RunAsync();