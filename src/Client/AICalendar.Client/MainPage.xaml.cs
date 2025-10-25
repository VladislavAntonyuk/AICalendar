using Plugin.Maui.Calendar.Models;

namespace AICalendar.Client;

public partial class MainPage : ContentPage
{
	private readonly MainPageViewModel viewModel;

	public MainPage(MainPageViewModel viewModel)
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

	private async void Calendar_OnMonthChanged(object? sender, MonthChangedEventArgs e)
	{
		viewModel.DateRange = new DateRange(e.OldMonth, e.NewMonth);
		await viewModel.InitializeAsync();
	}
}

public record DateRange(DateOnly Start, DateOnly End);