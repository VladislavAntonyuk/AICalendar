using AICalendar.Shared;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AICalendar.Client.Application.Calendar.EventDetails;

public partial class CalendarEventPopupViewModel(IHttpClientFactory factory, IPopupService popupService) : ObservableObject, IQueryAttributable
{
	[ObservableProperty]
	public partial GetEventResponse? Event { get; set; }

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue(nameof(Event), out var eventObj) && eventObj is GetEventResponse eventModel)
		{
			Event = eventModel;
		}
	}

	[RelayCommand]
	async Task CancelEvent()
	{
		var client = factory.CreateClient("AuthClient");
		if (Event is not null)
		{
			await client.DeleteAsync($"events/{Event.Id}");
		}

		await popupService.ClosePopupAsync(Shell.Current);
	}

	[RelayCommand]
	Task Close()
	{
		return popupService.ClosePopupAsync(Shell.Current);
	}
}