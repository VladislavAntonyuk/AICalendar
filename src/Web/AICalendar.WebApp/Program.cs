using AICalendar.WebApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddHttpClient("AICalendarAPI", (sp, client) =>
{
	var baseAddress = sp.GetRequiredService<IConfiguration>()["ApiBaseUrl"];
	ArgumentNullException.ThrowIfNull(baseAddress);
	client.BaseAddress = new Uri(baseAddress);
}).AddHttpMessageHandler<GraphApiAuthorizationMessageHandler>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMsalAuthentication(options =>
{
	builder.Configuration.Bind("AzureAd", options.ProviderOptions);
});
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<GraphApiAuthorizationMessageHandler>();

await builder.Build().RunAsync();
