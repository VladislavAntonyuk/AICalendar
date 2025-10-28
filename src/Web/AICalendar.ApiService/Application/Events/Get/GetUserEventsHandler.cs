using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using AICalendar.Shared;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Get;

internal sealed class GetUserEventsHandler(AiCalendarDbContext context)
{
	public async Task<Result<List<GetEventsResponse>>> Handle(Guid userId, GetEventsRange range, CancellationToken cancellationToken = default)
	{
		var from = range.From.ToDateTime(TimeOnly.MinValue);
		var to = range.To.ToDateTime(TimeOnly.MaxValue);
		var model = await context.Events
								 .Where(x =>
											x.Start >= from &&
											x.End <= to &&
											(
												x.OrganizerId == userId ||
												x.Attendees.Any(a => context.Users
																			.Any(u => u.Id == userId && u.Email == a))
											)
								 )
								 .Select(calendarEvent =>
									         new GetEventsResponse(
												calendarEvent.Id,
												calendarEvent.Start,
												calendarEvent.End,
												calendarEvent.Title,
												calendarEvent.OrganizerId))
								 .ToListAsync(cancellationToken);

		return Result.Success(model);
	}
}