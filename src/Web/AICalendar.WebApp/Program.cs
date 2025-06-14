using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AICalendar.WebApp;
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
});

await builder.Build().RunAsync();
