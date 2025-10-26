using AICalendar.Client.Auth;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AICalendar.Client;

public partial class AuthPageViewModel(IAuthService authService) : ObservableObject
{
	[RelayCommand]
	private async Task Login(CancellationToken cancellationToken)
	{
		var result = await authService.SignInInteractively(cancellationToken);
		if (result.IsSuccessful)
		{
			await Shell.Current.GoToAsync("//MainPage");
		}
		else
		{
			await Shell.Current.CurrentPage.DisplayAlertAsync("Error has occured", result.Error.Message, "OK");
		}
	}

	public async Task InitializeAsync()
	{
		var authResult = await authService.SignInSilently(CancellationToken.None);
		if (authResult.IsSuccessful)
		{
			await Shell.Current.GoToAsync("//MainPage");
		}
	}
}