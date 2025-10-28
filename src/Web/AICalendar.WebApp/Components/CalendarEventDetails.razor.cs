using AICalendar.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AICalendar.WebApp.Components;

public partial class CalendarEventDetails(IHttpClientFactory httpClientFactory) : AiCalendarBaseComponent
{
	[CascadingParameter]
	private IMudDialogInstance? MudDialog { get; set; }

	[Parameter]
	public required GetEventResponse CalendarEvent { get; set; }

	private async Task CancelEvent()
	{
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		await httpClient.DeleteAsync($"events/{CalendarEvent.Id}");
		MudDialog?.Close();
	}

	private void Close()
	{
		MudDialog?.Cancel();
	}
}