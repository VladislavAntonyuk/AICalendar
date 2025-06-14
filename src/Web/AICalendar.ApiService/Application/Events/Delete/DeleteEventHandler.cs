using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.Events.Delete;

internal sealed class DeleteEventHandler(AiCalendarDbContext context)
{
	public async Task<Result> Handle(Guid id, Guid organizerId, CancellationToken cancellationToken = default)
	{
		await context.Events.Where(x => x.Id == id && x.OrganizerId == organizerId).ExecuteDeleteAsync(cancellationToken);
		return Result.Success();
	}
}