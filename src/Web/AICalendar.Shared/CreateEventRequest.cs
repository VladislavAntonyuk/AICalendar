namespace AICalendar.Shared;

public record CreateEventRequest(DateTime Start, DateTime End, string Title, ICollection<string> Attendees)
{
	public string? Description { get; init; }
}