using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AICalendar.Shared;

namespace AICalendar.Client;

public partial class CalendarEventView : ContentView
{
	public static readonly BindableProperty CalendarEventCommandProperty =
		BindableProperty.Create(nameof(CalendarEventCommand), typeof(ICommand), typeof(AiCalendarEvent), null);

	public CalendarEventView()
	{
		InitializeComponent();
	}

	public ICommand? CalendarEventCommand
	{
		get => (ICommand)GetValue(CalendarEventCommandProperty);
		set => SetValue(CalendarEventCommandProperty, value);
	}

	void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
	{
		if (BindingContext is AiCalendarEvent eventModel)
		{
			CalendarEventCommand?.Execute(eventModel);
		}
	}
}