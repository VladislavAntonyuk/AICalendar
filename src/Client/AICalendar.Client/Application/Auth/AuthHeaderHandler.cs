using System.Net.Http.Headers;

namespace AICalendar.Client.Application.Auth;

internal class AuthHeaderHandler(IAuthService authService) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var signInSilently = await authService.SignInSilently(cancellationToken);
		if (signInSilently.IsSuccessful)
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", signInSilently.Value.AccessToken);
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}

		var signInInteractively = await authService.SignInInteractively(cancellationToken);
		if (signInInteractively.IsSuccessful)
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", signInInteractively.Value.AccessToken);
		}

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
