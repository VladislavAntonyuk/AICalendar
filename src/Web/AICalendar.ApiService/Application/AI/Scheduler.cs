using System.ComponentModel;
using AICalendar.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ModelContextProtocol.Server;

namespace AICalendar.ApiService.Application.AI;

[McpServerToolType]
public static class Scheduler
{
	[McpServerTool, Description("Schedules an event to users with emails on specific date and time with title")]
	public static async Task<CalendarEvent?> Schedule(
		HttpClient httpClient,
		IHttpContextAccessor httpContextAccessor,
		string[] emails, DateTime from, DateTime to, string title)
	{
		SetAuth(httpClient, httpContextAccessor);
		var result = await httpClient.PostAsJsonAsync("events", new
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
		HttpClient httpClient,
		IHttpContextAccessor httpContextAccessor,
		DateTime from, DateTime to)
	{
		SetAuth(httpClient, httpContextAccessor);
		var events = await httpClient.GetFromJsonAsync<List<CalendarEvent>>($"events?from={from}&to={to}");
		return events ?? [];
	}

	[McpServerTool, Description("Cancel event by id")]
	public static Task CancelEvent(
		HttpClient httpClient,
		IHttpContextAccessor httpContextAccessor,
		Guid id)
	{
		SetAuth(httpClient, httpContextAccessor);
		return httpClient.DeleteAsync($"events/{id}");
	}


	private static void SetAuth(HttpClient client, IHttpContextAccessor httpContextAccessor)
	{
		client.BaseAddress = new Uri("https://localhost:7118/api/v1/");
		var authHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
		if (!string.IsNullOrEmpty(authHeader))
		{
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authHeader.Split(' ').Last());
		}
	}
}