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
	private List<CalendarEventItem> events = new();
	private MudCalendar<CalendarEventItem>? calendar;

	public string Prompt { get; set; } = "Schedule an event to user with email 'test@user.com' on 14 June 2025 from 15:00 till 16:30 with title 'Coffee break'";

	public string? Response { get; set; }

	private async Task DateRangeChanged(DateRange dateRange)
	{
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		var calendarEvents = await httpClient.GetFromJsonAsync<List<CalendarEvent>>($"events?from={dateRange.Start.GetValueOrDefault():O}&to={dateRange.End.GetValueOrDefault():O}") ?? [];
		events = calendarEvents.Select(MapCalendarEventItem).ToList();
	}

	private async Task EventClicked(CalendarEventItem obj)
	{
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		var calendarEvent = await httpClient.GetFromJsonAsync<CalendarEvent>($"events/{obj.Identifier}");
		if (calendarEvent is not null)
		{
			var parameters = new DialogParameters { { nameof(CalendarEventDetails.CalendarEvent), MapCalendarEventItem(calendarEvent) } };
			var dialog = await dialogService.ShowAsync<CalendarEventDetails>(calendarEvent.Title, parameters);
			await dialog.Result;
			calendar?.Refresh();
		}
	}

	private async Task SendRequest()
	{
		Response = string.Empty;
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		var result = await httpClient.PostAsJsonAsync("ai", new AiRequest(Prompt));
		await foreach (var update in result.Content.ReadFromJsonAsAsyncEnumerable<ChatResponseUpdate>())
		{
			Response += update;
		}

		calendar?.Refresh();
	}

	private string GetColor(Color color) => $"var(--mud-palette-{color.ToDescriptionString()})";
	
	private CalendarEventItem MapCalendarEventItem(CalendarEvent calendarEvent) => new CalendarEventItem
	{
		Identifier = calendarEvent.Id,
		OrganizerId = calendarEvent.OrganizerId,
		Attendees = calendarEvent.Attendees,
		Start = calendarEvent.Start,
		End = calendarEvent.End,
		Text = calendarEvent.Title ?? string.Empty
	};
}

internal record AiRequest(string Prompt);

