using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AICalendar.WebApp.Components;

public partial class CalendarEventDetails(IHttpClientFactory httpClientFactory) : AiCalendarBaseComponent
{
	[CascadingParameter]
	private IMudDialogInstance? MudDialog { get; set; }

	[Parameter]
	public CalendarEventItem CalendarEvent { get; set; } = new CalendarEventItem();

	private async Task CancelEvent()
	{
		var httpClient = httpClientFactory.CreateClient("AICalendarAPI");
		await httpClient.DeleteAsync($"events/{CalendarEvent.Identifier}");
		MudDialog?.Close();
	}
}