using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using AICalendar.Client.Auth;
using AICalendar.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Scheduler;

namespace AICalendar.Client;

public partial class MainPageViewModel(IHttpClientFactory httpClientFactory, IAuthService authService) : ObservableObject
{
	public ObservableCollection<AiCalendarEvent> Appointments { get; set; } = [];

	[RelayCommand]
	async Task Logout()
	{
		await authService.LogoutAsync(CancellationToken.None);
		await Shell.Current.GoToAsync("//AuthPage");
	}

	[RelayCommand]
	async Task Tapped(SchedulerTappedEventArgs calendarEvent)
	{
		if (calendarEvent.Element == SchedulerElement.Appointment)
		{
			var appointment = calendarEvent.Appointments.OfType<AiCalendarEvent>().FirstOrDefault();
			if (appointment is null)
			{
				return;
			}

			await Shell.Current.CurrentPage.DisplayAlertAsync("Appointment", appointment.Subject, "OK");
		}
	}

	[RelayCommand]
	async Task RefreshCalendar(SchedulerViewChangedEventArgs args)
	{
		List<DateTime> dates =
		[
			new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
			new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1)
		];

		var from = args.NewVisibleDates is not null && args.NewVisibleDates.Count > 0 ? args.NewVisibleDates.First() : dates[0];
		var to = args.NewVisibleDates is not null && args.NewVisibleDates.Count>0? args.NewVisibleDates.Last() : dates[1];
		Appointments.Clear();
		var httpClient = httpClientFactory.CreateClient("AuthClient");
		var calendarEvents = await httpClient.GetFromJsonAsync<List<CalendarEvent>>($"events?from={from:O}&to={to:O}") ?? [];
		foreach (var calendarEvent in calendarEvents)
		{
			Appointments.Add(new AiCalendarEvent
			{
				Id = calendarEvent.Id,
				Subject = calendarEvent.Title,
				StartTime = calendarEvent.Start,
				EndTime = calendarEvent.End,
				OrganizerId = calendarEvent.OrganizerId,
				Attendees = calendarEvent.Attendees
			});
		}
	}
}

public class AiCalendarEvent : SchedulerAppointment
{
	public Guid OrganizerId { get; set; }
	public ICollection<string> Attendees { get; set; } = [];
}