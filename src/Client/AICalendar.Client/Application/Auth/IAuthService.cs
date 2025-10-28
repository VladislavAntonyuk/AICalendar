using AICalendar.Client.Infrastructure.OperationResult;
using Microsoft.Identity.Client;

namespace AICalendar.Client.Application.Auth;

public interface IAuthService
{
	Task<OperationResult<AuthenticationResult>> SignInInteractively(CancellationToken cancellationToken);

	Task<OperationResult<AuthenticationResult>> SignInSilently(CancellationToken cancellationToken);

	Task LogoutAsync(CancellationToken cancellationToken);
}