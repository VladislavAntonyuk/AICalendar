using AICalendar.ApiService.Hubs;
using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Database.Entities;
using AICalendar.ApiService.Infrastructure.Results;
using AICalendar.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Create;

internal sealed class CreateEventHandler(AiCalendarDbContext context, IHubContext<CalendarEventsHub> hubContext)
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

		var organizer = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
		if (organizer is null)
		{
			return Result.Failure<CreateEventResponse>(EventsErrors.NotFound(currentUserId));
		}

		var model = new CalendarEvent
		{
			Id = Guid.CreateVersion7(),
			Start = request.Start,
			End = request.End,
			Title = request.Title,
			Description = request.Description,
			OrganizerId = currentUserId,
			Organizer = organizer,
			Attendees = attendees
		};

		context.Events.Add(model);
		await context.SaveChangesAsync(cancellationToken);

		await NotifyAffectedUsersAsync(model, cancellationToken);

		return new CreateEventResponse(model.Id);
	}

	private async Task<bool> HasOverlaps(List<string> attendees, DateTime from, DateTime to)
	{
		return await context.Events.AnyAsync(x => attendees.Any(y => x.Attendees.Contains(y)) &&
													(x.Start < to) &&
													(x.End > from));
	}

	public async Task NotifyAffectedUsersAsync(CalendarEvent eventEntity, CancellationToken cancellationToken)
	{
		var affectedUserEmails = eventEntity.Attendees.Append(eventEntity.Organizer.Email).Distinct();
		foreach (var email in affectedUserEmails)
		{
			await hubContext.Clients.Group(email).SendAsync("CalendarEventChanged", cancellationToken: cancellationToken);
		}
	}
}