using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using AICalendar.Shared;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Get;

internal sealed class GetEventHandler(AiCalendarDbContext context)
{
	public async Task<Result<GetEventResponse>> Handle(Guid currentUserId,
		Guid eventId,
		CancellationToken cancellationToken = default)
	{
		var calendarEvent = await context.Events
										 .Include(x => x.Organizer)
										 .FirstOrDefaultAsync(x => x.Id == eventId
										                           && (x.OrganizerId == currentUserId || x.Attendees.Any(a => context.Users.Any(u => u.Id == currentUserId && u.Email == a))),
										                      cancellationToken);

		if (calendarEvent is null)
		{
			return Result.Failure<GetEventResponse>(EventsErrors.NotFound(eventId));
		}

		return new GetEventResponse(
			calendarEvent.Id,
			calendarEvent.Start,
			calendarEvent.End,
			calendarEvent.Title,
			calendarEvent.Description,
			new Organizer(calendarEvent.Organizer.Id, calendarEvent.Organizer.Email),
			calendarEvent.Attendees);
	}
}