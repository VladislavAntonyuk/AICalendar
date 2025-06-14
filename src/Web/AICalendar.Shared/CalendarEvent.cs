namespace AICalendar.Shared;

public class CalendarEvent
{
	public Guid Id { get; init; }
	public DateTime Start { get; init; }
	public DateTime End { get; init; }
	public string? Title { get; set; }
	public Guid OrganizerId { get; set; }
	public User Organizer { get; set; } = null!;

	public ICollection<string> Attendees { get; set; } = new List<string>();
}