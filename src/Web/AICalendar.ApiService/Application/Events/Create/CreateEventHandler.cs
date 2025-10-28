using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Database.Entities;
using AICalendar.ApiService.Infrastructure.Results;
using AICalendar.Shared;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Create;

internal sealed class CreateEventHandler(AiCalendarDbContext context)
{
	public async Task<Result<CreateEventResponse>> Handle(Guid currentUserId,
		CreateEventRequest request,
		CancellationToken cancellationToken = default)
	{
		var attendees = request.Attendees.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
		if (await HasOverlaps(attendees, request.Start, request.End))
		{
			return Result.Failure<CreateEventResponse>(EventsErrors.Conflict());
		}

		var model = new CalendarEvent()
		{
			Id = Guid.CreateVersion7(),
			Start = request.Start,
			End = request.End,
			Title = request.Title,
			OrganizerId = currentUserId,
			Attendees = attendees
		};

		context.Events.Add(model);
		await context.SaveChangesAsync(cancellationToken);
		return new CreateEventResponse(model.Id);
	}

	private async Task<bool> HasOverlaps(List<string> attendees, DateTime from, DateTime to)
	{
		return await context.Events.AnyAsync(x => attendees.Any(y => x.Attendees.Contains(y)) &&
												  (x.Start < to) &&
												  (x.End > from));
	}
}