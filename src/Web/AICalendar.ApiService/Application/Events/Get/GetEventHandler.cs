using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Get;

internal sealed class GetEventHandler(AiCalendarDbContext context)
{
	public async Task<Result<GetEvent.ResponseContent>> Handle(Guid currentUserId,
		Guid eventId,
		CancellationToken cancellationToken = default)
	{
		var calendarEvent = await context.Events.FirstOrDefaultAsync(
			x => x.Id == eventId && (x.OrganizerId == currentUserId || x.Attendees.Any(a => context.Users.Any(u => u.Id == currentUserId && u.Email == a))),
			cancellationToken);
		
		if (calendarEvent is null)
		{
			return Result.Failure<GetEvent.ResponseContent>(EventsErrors.NotFound(eventId));
		}
		
		return new GetEvent.ResponseContent(
			calendarEvent.Id,
			calendarEvent.Start,
			calendarEvent.End,
			calendarEvent.Title,
			calendarEvent.OrganizerId,
			calendarEvent.Attendees);
	}
}