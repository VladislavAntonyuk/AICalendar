using System.Collections.ObjectModel;
using System.Net.Http.Json;
using AICalendar.Client.Application.Auth;
using AICalendar.Client.Application.Calendar.EventDetails;
using AICalendar.Shared;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.AI;
using Syncfusion.Maui.Scheduler;

namespace AICalendar.Client.Application.Calendar.Main;

public partial class MainPageViewModel(IHttpClientFactory httpClientFactory, IAuthService authService, IPopupService popupService) : ObservableObject, IQueryAttributable, IAsyncDisposable
{
	private DateRange dateRange = new(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
									  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1));
	public ObservableCollection<AiCalendarEvent> Appointments { get; set; } = [];

	[ObservableProperty]
	public partial string? Username { get; set; }

	[RelayCommand]
	async Task Logout()
	{
		await authService.LogoutAsync(CancellationToken.None);
		await Shell.Current.GoToAsync("//AuthPage");
	}

	[RelayCommand]
	async Task Tapped(SchedulerTappedEventArgs eventArgs)
	{
		if (eventArgs.Element == SchedulerElement.Appointment)
		{
			var appointment = eventArgs.Appointments.OfType<AiCalendarEvent>().FirstOrDefault();
			if (appointment is null)
			{
				return;
			}

			var httpClient = httpClientFactory.CreateClient("AuthClient");
			var calendarEvent = await httpClient.GetFromJsonAsync<GetEventResponse>($"events/{appointment.Id}");
			if (calendarEvent is null)
			{
				await RefreshCalendar();
				return;
			}

			await popupService.ShowPopupAsync<CalendarEventPopup>(Shell.Current, null, new Dictionary<string, object>
			{
				{nameof(CalendarEventPopupViewModel.Event), calendarEvent}
			});
		}
	}

	[RelayCommand]
	async Task RefreshCalendar(SchedulerViewChangedEventArgs? args = null)
	{
		if (isRefreshingSignalR) return;
		try
		{
			isRefreshingSignalR = true;
			if (args is not null)
			{
				dateRange = new DateRange(args.NewVisibleDates.First(), args.NewVisibleDates.Last());
			}

			var httpClient = httpClientFactory.CreateClient("AuthClient");
			var calendarEvents = await httpClient.GetFromJsonAsync<List<GetEventsResponse>>($"events?from={dateRange.From:O}&to={dateRange.To:O}") ?? [];
			Appointments.Clear();
			foreach (var calendarEvent in calendarEvents)
			{
				Appointments.Add(new AiCalendarEvent
				{
					Id = calendarEvent.Id,
					Subject = calendarEvent.Title,
					StartTime = calendarEvent.Start,
					EndTime = calendarEvent.End,
					OrganizerId = calendarEvent.OrganizerId
				});
			}
		}
		finally
		{
			isRefreshingSignalR = false;
		}

	}

	[RelayCommand]
	async Task CreateEvent()
	{
		var prompt = await Shell.Current.CurrentPage.DisplayPromptAsync(
			"Create event",
			"Enter event details",
			initialValue: $"Schedule an event to user with email 'test@user.com' on {DateOnly.FromDateTime(DateTime.Now)} from 15:00 till 16:30 with title 'Coffee break'");

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
	}

	public async void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("username", out var usernameObject) && usernameObject is string username)
		{
			Username = username;
			await InitializeSignalRAsync(username);
		}
	}

	private HubConnection? hubConnection;
	private bool isRefreshingSignalR;

	public async Task InitializeSignalRAsync(string userId)
	{
		hubConnection = new HubConnectionBuilder()
			.WithUrl("https://localhost:7118/hubs/calendarEvents")
			.Build();
		hubConnection.On("CalendarEventChanged", async () =>
		{
			await RefreshCalendar();
		});
		await hubConnection.StartAsync();
		await hubConnection.InvokeAsync("JoinUserGroup", userId);
	}

	public async ValueTask DisposeAsync()
	{
		if (hubConnection != null)
		{
			await hubConnection.DisposeAsync();
		}
	}
}