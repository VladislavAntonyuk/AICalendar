using CommunityToolkit.Maui.Views;

namespace AICalendar.Client.Application.Calendar.EventDetails;

public partial class CalendarEventPopup : Popup
{
	public CalendarEventPopup(CalendarEventPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}