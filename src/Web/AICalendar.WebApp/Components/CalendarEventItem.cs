using Heron.MudCalendar;

namespace AICalendar.WebApp.Components;

public class CalendarEventItem : CalendarItem
{
	public Guid Identifier { get; set; }
	public Guid OrganizerId { get; set; }
	public ICollection<string> Attendees { get; set; } = [];
}