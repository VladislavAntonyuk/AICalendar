namespace AICalendar.Client.Application.Calendar;

public partial class MainPage : ContentPage
{
	public MainPage(Main.MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}