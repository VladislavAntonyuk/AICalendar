using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AICalendar.Client.Application.Auth;

public partial class AuthPageViewModel(IAuthService authService) : ObservableObject
{
	[RelayCommand]
	private async Task Login(CancellationToken cancellationToken)
	{
		var authResult = await authService.SignInInteractively(cancellationToken);
		if (authResult.IsSuccessful)
		{
			await Shell.Current.GoToAsync("//MainPage", new Dictionary<string, object>()
			{
				{"username", authResult.Value.Account.Username}
			});
		}
		else
		{
			await Shell.Current.CurrentPage.DisplayAlertAsync("Error has occured", authResult.Error.Message, "OK");
		}
	}

	public async Task InitializeAsync()
	{
		var authResult = await authService.SignInSilently(CancellationToken.None);
		if (authResult.IsSuccessful)
		{
			await Shell.Current.GoToAsync("//MainPage", new Dictionary<string, object>()
			{
				{"username", authResult.Value.Account.Username}
			});
		}
	}
}