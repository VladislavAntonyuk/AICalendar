using AICalendar.ApiService.Hubs;
using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Delete;

internal sealed class DeleteEventHandler(AiCalendarDbContext context, IHubContext<CalendarEventsHub> hubContext)
{
	public async Task<Result> Handle(Guid id, Guid organizerId, CancellationToken cancellationToken = default)
	{
		var eventEntity = await context.Events.Include(x => x.Organizer).FirstOrDefaultAsync(x => x.Id == id && x.OrganizerId == organizerId, cancellationToken);
		if (eventEntity is null)
		{
			return Result.Success();
		}

		var affectedUserEmails = eventEntity.Attendees.Append(eventEntity.Organizer.Email).Distinct();
		await context.Events.Where(x => x.Id == id && x.OrganizerId == organizerId).ExecuteDeleteAsync(cancellationToken);
		foreach (var email in affectedUserEmails)
		{
			await hubContext.Clients.Group(email).SendAsync("CalendarEventChanged", cancellationToken: cancellationToken);
		}
		return Result.Success();
	}
}