namespace AICalendar.Shared;

public sealed class User
{
	public Guid Id { get; init; }
	public required string Email { get; set; }
	public List<CalendarEvent>? Events { get; init; } = [];
}