using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using AICalendar.Client.Auth;
using AICalendar.Shared;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
using Syncfusion.Maui.Scheduler;

namespace AICalendar.Client;

record DateRange(DateTime From, DateTime To);
public partial class MainPageViewModel(IHttpClientFactory httpClientFactory, IAuthService authService, IPopupService popupService) : ObservableObject
{
	private DateRange dateRange = new(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
									  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1));
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

			await popupService.ShowPopupAsync<CalendarEventPopup>(Shell.Current, null, new Dictionary<string, object>()
			{
				{"AiCalendarEvent", appointment}
			});
			await RefreshCalendar();
		}
	}

	[RelayCommand]
	async Task RefreshCalendar(SchedulerViewChangedEventArgs? args = null)
	{
		if (args is not null)
		{
			dateRange = new DateRange(args.NewVisibleDates.First(), args.NewVisibleDates.Last());
		}

		Appointments.Clear();
		var httpClient = httpClientFactory.CreateClient("AuthClient");
		var calendarEvents = await httpClient.GetFromJsonAsync<List<CalendarEvent>>($"events?from={dateRange.From:O}&to={dateRange.To:O}") ?? [];
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

	[RelayCommand]
	async Task CreateEvent()
	{
		var prompt = await Shell.Current.CurrentPage.DisplayPromptAsync("Create event", "Enter event details", initialValue: "Schedule an event to user with email 'test@user.com' on 26/10/2025 from 15:00 till 16:30 with title 'Coffee break'");
		if (string.IsNullOrEmpty(prompt))
		{
			return;
		}

		var response = string.Empty;
		var httpClient = httpClientFactory.CreateClient("AuthClient");
		var result = await httpClient.PostAsJsonAsync("ai", new AiRequest(prompt));
		if (!result.IsSuccessStatusCode)
		{
			var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();
			response = problemDetails?.Title;
			await Shell.Current.CurrentPage.DisplayAlertAsync("Error has occured", response, "OK");
			return;
		}

		await foreach (var update in result.Content.ReadFromJsonAsAsyncEnumerable<ChatResponseUpdate>())
		{
			response += update;
		}

		await Shell.Current.CurrentPage.DisplayAlertAsync("Congratulations!", response, "OK");
		await RefreshCalendar();
	}
}

public class AiCalendarEvent : SchedulerAppointment
{
	public Guid OrganizerId { get; set; }
	public ICollection<string> Attendees { get; set; } = [];
}