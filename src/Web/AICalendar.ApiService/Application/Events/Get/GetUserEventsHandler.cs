using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using AICalendar.Shared;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Get;

internal sealed class GetUserEventsHandler(AiCalendarDbContext context)
{
	public async Task<Result<List<CalendarEvent>>> Handle(Guid userId, GetUserEvents.GetEventsRange range, CancellationToken cancellationToken = default)
	{
		var model = await context.Events
		                         .Where(x =>
			                                x.Start > range.From &&
			                                x.End < range.To &&
			                                (
				                                x.OrganizerId == userId ||
				                                x.Attendees.Any(a => context.Users
				                                                            .Any(u => u.Id == userId && u.Email == a))
			                                )
		                         )
		                         .ToListAsync(cancellationToken);

		return Result.Success(model);
	}
}