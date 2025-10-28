namespace AICalendar.Client.Application.Auth;

public partial class AuthPage : ContentPage
{
	private readonly AuthPageViewModel viewModel;

	public AuthPage(AuthPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = this.viewModel = viewModel;
	}

	/// <inheritdoc />
	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		await viewModel.InitializeAsync();
	}
}