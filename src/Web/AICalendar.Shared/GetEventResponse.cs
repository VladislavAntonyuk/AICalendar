namespace AICalendar.Shared;

public record GetEventResponse(
	Guid Id,
	DateTime Start,
	DateTime End,
	string Title,
	string? Description,
	Organizer Organizer,
	ICollection<string> Attendees);