namespace AICalendar.Client.Application.Auth;

public partial class AuthPage : ContentPage
{
	private readonly AuthPageViewModel viewModel;

	public AuthPage(AuthPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = this.viewModel = viewModel;
		Loaded += AuthPage_Loaded;
	}

	private async void AuthPage_Loaded(object? sender, EventArgs e)
	{
		Loaded -= AuthPage_Loaded;
		await viewModel.InitializeAsync();
	}
}