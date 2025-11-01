using AICalendar.Client.Application.Calendar.Main;

namespace AICalendar.Client.Application.Calendar;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}