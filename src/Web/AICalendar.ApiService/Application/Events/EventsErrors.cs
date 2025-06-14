using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.Events;

public static class EventsErrors
{
	public static Error NotFound(Guid eventId)
	{
		return Error.NotFound("Event.NotFound", $"The event with the identifier {eventId} not found");
	}
	
	public static Error Conflict()
	{
		return Error.Conflict("Event.Conflict", $"The event conflict");
	}
}