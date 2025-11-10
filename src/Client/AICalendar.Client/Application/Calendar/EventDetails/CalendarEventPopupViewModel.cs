using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AICalendar.Client.Application.Calendar.EventDetails;

public partial class CalendarEventPopupViewModel(IHttpClientFactory factory, IPopupService popupService) : ObservableObject, IQueryAttributable
{
	[ObservableProperty]
	public partial EventResponse Event { get; set; } = new();

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue(nameof(Event), out var eventObj) &&
			eventObj is Shared.GetEventResponse eventModel)
		{
			Event = new EventResponse
			{
				Organizer = eventModel.Organizer,
				Id = eventModel.Id,
				Start = eventModel.Start,
				End = eventModel.End,
				Title = eventModel.Title,
				Description = eventModel.Description,
				Attendees = eventModel.Attendees
			};
		}
	}

	[RelayCommand]
	async Task CancelEvent()
	{
		var client = factory.CreateClient("AuthClient");
		await client.DeleteAsync($"events/{Event.Id}");

		await popupService.ClosePopupAsync(Shell.Current);
	}

	[RelayCommand]
	Task Close()
	{
		return popupService.ClosePopupAsync(Shell.Current);
	}
}

public record EventResponse
{
	public Guid Id { get; init; }
	public DateTime Start { get; init; }
	public DateTime End { get; init; }
	public string? Title { get; init; }
	public string? Description { get; init; }
	public Shared.Organizer? Organizer { get; init; }
	public ICollection<string> Attendees { get; init; } = [];
}