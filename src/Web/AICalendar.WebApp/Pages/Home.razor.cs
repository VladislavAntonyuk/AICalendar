using System.Net.Http.Json;
using AICalendar.Shared;
using AICalendar.WebApp.Components;
using Heron.MudCalendar;
using Microsoft.Extensions.AI;
using MudBlazor;

namespace AICalendar.WebApp.Pages;

public partial class Home(IHttpClientFactory httpClientFactory, IDialogService dialogService)
	: AiCalendarBaseComponent
{
	private List<CalendarEventItem> events = [];
	private MudCalendar<CalendarEventItem>? calendar;
	private DateRange? currentDateRange;

	public string Prompt { get; set; } = $"Schedule an event to user with email 'test@user.com' on {DateOnly.FromDateTime(DateTime.Now)} from 15:00 till 16:30 with title 'Coffee break'";

	public string? Response { get; set; }

	private async Task DateRangeChanged(DateRange dateRange)
	{
		currentDateRange = dateRange;
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		var calendarEvents = await httpClient.GetFromJsonAsync<List<GetEventsResponse>>($"events?from={dateRange.Start.GetValueOrDefault():yyyy-MM-dd}&to={dateRange.End.GetValueOrDefault():yyyy-MM-dd}") ?? [];
		events = calendarEvents.Select(MapCalendarEventItem).ToList();
	}

	private async Task EventClicked(CalendarEventItem obj)
	{
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		var calendarEvent = await httpClient.GetFromJsonAsync<GetEventResponse>($"events/{obj.Identifier}");
		if (calendarEvent is not null)
		{
			var parameters = new DialogParameters { { nameof(CalendarEventDetails.CalendarEvent), calendarEvent } };
			var dialog = await dialogService.ShowAsync<CalendarEventDetails>(calendarEvent.Title, parameters, new DialogOptions(){CloseButton = false});
			var result = await dialog.Result;
			if (result is not null && !result.Canceled)
			{
				await RefreshCalendar();
			}
		}
	}

	private async Task SendRequest()
	{
		Response = string.Empty;
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		var result = await httpClient.PostAsJsonAsync("ai", new AiRequest(Prompt));
		if (!result.IsSuccessStatusCode)
		{
			var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();
			Response = problemDetails?.Title;
			return;
		}

		await foreach (var update in result.Content.ReadFromJsonAsAsyncEnumerable<ChatResponseUpdate>())
		{
			Response += update;
		}

		await RefreshCalendar();
	}

	private string GetColor(Color color) => $"var(--mud-palette-{color.ToDescriptionString()})";

	private CalendarEventItem MapCalendarEventItem(GetEventsResponse calendarEvent) => new CalendarEventItem
	{
		Identifier = calendarEvent.Id,
		OrganizerId = calendarEvent.OrganizerId,
		Start = calendarEvent.Start,
		End = calendarEvent.End,
		Text = calendarEvent.Title
	};

	private async Task RefreshCalendar()
	{
		if (calendar is not null)
		{
			await calendar.DateRangeChanged.InvokeAsync(currentDateRange);
		}
	}
}
