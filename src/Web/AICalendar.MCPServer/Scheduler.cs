using System.ComponentModel;
using AICalendar.Shared;
using ModelContextProtocol.Server;

namespace AICalendar.MCPServer;

[McpServerToolType]
public static class Scheduler
{
	[McpServerTool, Description("Schedules an event to users with emails on specific date and time with title")]
	public static async Task<CalendarEvent?> Schedule(
		HttpClient httpClient,
		string[] emails, DateTime from, DateTime to, string title)
	{
		var result = await httpClient.PostAsJsonAsync("https://localhost:5002/api/v1/events", new
		{
			Attendees = emails,
			Start = from,
			End = to,
			Title = title
		});
		return await result.Content.ReadFromJsonAsync<CalendarEvent>();
	}

	[McpServerTool, Description("Get list of events on specific time range.")]
	public static async Task<List<CalendarEvent>> GetEvents(
		HttpClient httpClient, DateTime from, DateTime to)
	{
		var events = await httpClient.GetFromJsonAsync<List<CalendarEvent>>($"https://localhost:5002/api/v1/events?from={from}&to={to}");
		return events ?? [];
	}

	[McpServerTool, Description("Cancel event by id")]
	public static Task CancelEvent(HttpClient httpClient, Guid id)
	{
		return httpClient.DeleteAsync($"https://localhost:5002/api/v1/events/{id}");
	}
}