using Microsoft.Identity.Client;

namespace AICalendar.Client.Auth;

public interface IAuthService
{
	Task<OperationResult<AuthenticationResult>> SignInInteractively(CancellationToken cancellationToken);

	Task<OperationResult<AuthenticationResult>> SignInSilently(CancellationToken cancellationToken);

	Task LogoutAsync(CancellationToken cancellationToken);
}