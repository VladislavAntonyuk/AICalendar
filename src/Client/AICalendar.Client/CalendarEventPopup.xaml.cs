using CommunityToolkit.Maui.Views;
using System.Windows.Input;
using AICalendar.Shared;

namespace AICalendar.Client;

public partial class CalendarEventPopup : Popup
{
	public CalendarEventPopup(CalendarEventPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}