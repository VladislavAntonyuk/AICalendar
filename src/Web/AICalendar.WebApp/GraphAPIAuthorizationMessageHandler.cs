using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Extensions.Options;

namespace AICalendar.WebApp;

public class GraphApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
	public GraphApiAuthorizationMessageHandler(
		IConfiguration configuration,
		IOptions<MsalProviderOptions> options,
		IAccessTokenProvider provider,
		NavigationManager navigationManager)
		: base(provider, navigationManager)
	{
		var baseAddress = configuration["ApiBaseUrl"];
		ArgumentNullException.ThrowIfNull(baseAddress);
		ConfigureHandler(
			authorizedUrls: [baseAddress],
			scopes: options.Value.DefaultAccessTokenScopes);
	}
}