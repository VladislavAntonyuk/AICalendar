using System.Net.Http.Json;
using AICalendar.Shared;
using Plugin.Maui.Calendar.Models;

namespace AICalendar.Client;

public class MainPageViewModel(HttpClient httpClient)
{
	public EventCollection Events { get; } = [];

	public DateRange DateRange { get; set; } = new DateRange(new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
	                                                         new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1));

	public async Task InitializeAsync()
	{
		Events.Clear();
		var calendarEvents = await httpClient.GetFromJsonAsync<List<CalendarEvent>>($"events?from={DateRange.Start:O}&to={DateRange.End:O}") ?? [];
		foreach (var events in calendarEvents.GroupBy(x => x.Start))
		{
			Events.Add(events.Key, events.ToList());
		}
	}
}