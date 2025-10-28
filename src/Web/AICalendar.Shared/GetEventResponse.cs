namespace AICalendar.Shared;

public record GetEventResponse(
	Guid Id,
	DateTime Start,
	DateTime End,
	string Title,
	Organizer Organizer,
	ICollection<string> Attendees);