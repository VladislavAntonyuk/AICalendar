namespace AICalendar.Shared;

public record GetEventsResponse(Guid Id, DateTime Start, DateTime End, string Title, Guid OrganizerId);