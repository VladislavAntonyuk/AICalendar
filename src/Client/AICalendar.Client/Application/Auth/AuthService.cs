using System.Net;
using AICalendar.Client.Infrastructure.OperationResult;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace AICalendar.Client.Application.Auth;

internal class AuthService(
	IOptions<AzureAdConfiguration> azureAdOptions) : IAuthService
{
	private readonly IPublicClientApplication authenticationClient = PublicClientApplicationBuilder.Create(azureAdOptions.Value.ClientId)
		.WithAuthority(azureAdOptions.Value.Authority, validateAuthority: false)
#if WINDOWS
		.WithRedirectUri($"{Uri.UriSchemeHttp}{Uri.SchemeDelimiter}{IPAddress.Loopback.ToString()}")
#else
		.WithRedirectUri($"msal{azureAdOptions.Value.ClientId}://auth")
#endif
#if ANDROID
		.WithParentActivityOrWindow(() => Platform.CurrentActivity)
#endif
		.Build();

	public async Task LogoutAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
#if WINDOWS
			await AttachTokenCache();
#endif
		var accounts = await authenticationClient.GetAccountsAsync();
		foreach (var account in accounts)
		{
			await authenticationClient.RemoveAsync(account);
		}
	}

	public async Task<OperationResult<AuthenticationResult>> SignInInteractively(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		OperationResult<AuthenticationResult>? opResult;

		try
		{
#if WINDOWS
			await AttachTokenCache();
#endif
			var authResult = await authenticationClient.AcquireTokenInteractive(azureAdOptions.Value.Scopes)
				.WithPrompt(Prompt.SelectAccount)
				.ExecuteAsync(cancellationToken);
			opResult = OperationResult<AuthenticationResult>.Success(authResult);
		}
		catch (MsalClientException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
		{
			// cancelled by user (e.g. login window closed),
			// we should not treat it as an error, and bother user with any kind of message
			opResult = OperationResult<AuthenticationResult>.Failed(ex.Message, OperationResultErrorType.AuthenticationCanceled);
		}
		catch (Exception ex) when (ex is MsalException or OperationCanceledException)
		{
			opResult = OperationResult<AuthenticationResult>.Failed(ex.Message, OperationResultErrorType.ConnectivityIssues);
		}
		catch (Exception ex)
		{
			opResult = OperationResult<AuthenticationResult>.Failed(ex.Message);
		}

		return opResult;
	}

	public async Task<OperationResult<AuthenticationResult>> SignInSilently(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		OperationResult<AuthenticationResult>? opResult;

		try
		{
#if WINDOWS
			await AttachTokenCache();
#endif
			IEnumerable<IAccount> accounts = await authenticationClient.GetAccountsAsync();
			IAccount? firstAccount = accounts.FirstOrDefault();
			if (firstAccount is not null)
			{
				var authResult = await authenticationClient
										.AcquireTokenSilent(azureAdOptions.Value.Scopes, firstAccount)
										.ExecuteAsync(cancellationToken);
				opResult = OperationResult<AuthenticationResult>.Success(authResult);
			}
			else
			{
				opResult = OperationResult<AuthenticationResult>.Failed("User not found", OperationResultErrorType.UiInteractiveSignInRequired);
			}
		}
		catch (MsalUiRequiredException ex)
		{
			opResult = OperationResult<AuthenticationResult>.Failed(ex.Message, OperationResultErrorType.UiInteractiveSignInRequired);
		}
		catch (HttpRequestException ex)
		{
			opResult = OperationResult<AuthenticationResult>.Failed(ex.Message, OperationResultErrorType.ConnectivityIssues);
		}
		catch (Exception ex)
		{
			opResult = OperationResult<AuthenticationResult>.Failed(ex.Message);
		}

		return opResult;
	}

#if WINDOWS
	private async Task AttachTokenCache()
	{
		var cacheDir = Path.Join(Path.GetTempPath(), "AICalendar");
		// Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
		var storageProperties = new StorageCreationPropertiesBuilder("AICalendar.cache", cacheDir).Build();
		var msalCacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
		msalCacheHelper.RegisterCache(authenticationClient.UserTokenCache);
	}
#endif
}